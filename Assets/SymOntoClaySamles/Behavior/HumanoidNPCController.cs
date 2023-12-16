using UnityEngine;
using System.Collections;
using SymOntoClay;
using System.Threading;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Helpers;
using UnityEngine.AI;
using System.Diagnostics;
using SymOntoClay.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SymOntoClay.UnityAsset.Interfaces;
using SymOntoClay.UnityAsset.BaseBehaviors;
using SymOntoClay.UnityAsset.Samles.Interfaces;
using SymOntoClay.UnityAsset.Components;
using SymOntoClay.UnityAsset.Helpers;
using SymOntoClay.Monitor.Common;
using NLog;

namespace SymOntoClay.UnityAsset.Samles.Behavior
{
    [AddComponentMenu("SymOntoClay Samles/HumanoidNPCController")]
    [RequireComponent(typeof(IHumanoidNPCBehavior))]
    public class HumanoidNPCController : BaseBehavior, IBipedHumanoidCustomBehavior, IDieCustomBehavior
    {
        public GameObject Head;

        public GameObject RightHandWP;
        public GameObject LeftHandWP;

        private GameObject _rightHandWP;
        private GameObject _leftHandWP;

        GameObject IBipedHumanoidCustomBehavior.RightHandWP => _rightHandWP;
        GameObject IBipedHumanoidCustomBehavior.LeftHandWP => _leftHandWP;

        private Transform _targetHeadTransform;

        public float MaxHeadRotationAngle = 30;
        public float MaxWeaponRotationAngle = 10;

        private bool _isAlreadyStarted;
        private IHandThingCustomBehavior _takeAfterInitialization;

        protected override void Awake()
        {
#if UNITY_EDITOR
            //Debug.Log("HumanoidNPCController Awake");
#endif

            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }

        protected override void Start()
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log("HumanoidNPCController Start");
#endif

            base.Start();

            AddStopFact(Logger);

            if (Head == null)
            {
                var headLocator = GetComponentInChildren<HeadLocator>();
                var head = headLocator.gameObject;
                _targetHeadTransform = head.transform;
            }
            else
            {
                _targetHeadTransform = Head.transform;
            }

            if (RightHandWP == null)
            {
                var locator = GetComponentInChildren<RightHandWPLocator>();
                _rightHandWP = locator?.gameObject;
            }
            else
            {
                _rightHandWP = RightHandWP;
            }

            if(LeftHandWP == null)
            {
                var locator = GetComponentInChildren<LeftHandWPLocator>();
                _leftHandWP = locator?.gameObject;
            }
            else
            {
                _leftHandWP = LeftHandWP;
            }

            _navHelper = new NavHelper(transform, _navMeshAgent, this);

            _isAlreadyStarted = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (_isDead)
            {
                return;
            }

            if(_takeAfterInitialization != null)
            {
                NTake(Logger, _takeAfterInitialization);

                _takeAfterInitialization = null;
            }

            _position = transform.position;
        }

        void OnAnimatorIK(int layerIndex)
        {
            if (_enableRifleIK)
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.3f);
                _animator.SetIKPosition(AvatarIKGoal.LeftHand, _rifle.AddWP.transform.position);
            }

            if (_enableRotateHeadIK)
            {
                _animator.SetLookAtWeight(1);
                _animator.SetLookAtPosition(_currentHeadPosition);
                _targetHeadTransform.LookAt(_currentHeadPosition);
            }
        }

        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private Rigidbody _rigidbody;

        private NavHelper _navHelper;

        private bool _hasRifle;
        private bool _isWalking;
        private bool _isAim;
        private bool _isDead;

        private bool _enableRifleIK;
        private bool _enableRotateHeadIK;

        private Vector3 _currentHeadPosition;

        private Vector3 _position;

        private IHandThingCustomBehavior _currentHandThing;

        private IRifleCustomBehavior _rifle;        

        private void UpdateAnimator()
        {
            _animator.SetBool("hasRifle", _hasRifle);
            _animator.SetBool("walk", _isWalking);
            _animator.SetBool("isAim", _isAim);
            _animator.SetBool("isDead", _isDead);
        }

        private void PerformStop(IMonitorLogger logger)
        {
#if DEBUG
            logger.Info("HumanoidNPCController Begin PerformStop");
#endif

            _navMeshAgent.ResetPath();
            _isWalking = false;
            UpdateAnimator();
            AddStopFact(logger);

#if DEBUG
            logger.Info("HumanoidNPCController End PerformStop");
#endif
        }

        public void Die(IMonitorLogger logger)
        {
#if DEBUG
            logger.Info("HumanoidNPCController Die");
#endif

            if (_isDead)
            {
                return;
            }

            _isDead = true;

            UpdateAnimator();

            ProcessDeath(logger);
        }

        /*
                 [DebuggerHidden]
        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl(CancellationToken cancellationToken,
        [EndpointParam("To", KindOfEndpointParam.Position)] Vector3 point,
        float speed = 12)
        {
            if (_isDead)
            {
                return;
            }

#if UNITY_EDITOR
            //var methodId = GetMethodId();
            //UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] point = {point}");
#endif
            AddWalkingFact();

            RunInMainThread(() => {
                _navMeshAgent.SetDestination(point);
                _isWalking = true;
                UpdateAnimator();
            });

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] Walking has been started.");
#endif

            while (true)
            {
                if (point.x == _position.x && point.z == _position.z)
                {
                    RunInMainThread(() =>
                    {
                        PerformStop();
                    });

#if UNITY_EDITOR
                    //UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] Walking has been stoped.");
#endif

                    break;
                }

#if UNITY_EDITOR
                //UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] cancellationToken.IsCancellationRequested = {cancellationToken.IsCancellationRequested}");
#endif

                if (cancellationToken.IsCancellationRequested)
                {
                    RunInMainThread(() =>
                    {
                        PerformStop();
                    });

                    break;
                }

                Thread.Sleep(10);
            }

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] Walking has been stoped.");
#endif
        }
         */

        /*
                 [DebuggerHidden]
        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public async Task GoToImpl(CancellationToken cancellationToken,
        [EndpointParam("To", KindOfEndpointParam.Position)] Vector3 point,
        float speed = 12)
        {
            if (_isDead)
            {
                return;
            }

#if UNITY_EDITOR
            var methodId = GetMethodId();
            UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] point = {point}");
#endif
            AddWalkingFact();

            var task = _navHelper.Go(point, cancellationToken);

            RunInMainThread(() => {
                _isWalking = true;
                UpdateAnimator();
            });

#if UNITY_EDITOR
            UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] Walking has been started.");
#endif

            var result = await task;

#if UNITY_EDITOR
            UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] result.GoStatus = {result.GoStatus}");
#endif

            RunInMainThread(() =>
            {
                PerformStop();
            });

#if UNITY_EDITOR
            UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] Walking has been stoped.");
#endif
        }
         */

        [DebuggerHidden]
        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public async Task GoToImpl(CancellationToken cancellationToken, IMonitorLogger logger,
        [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget target,
        float speed = 12)
        {
            if (_isDead)
            {
                return;
            }

#if DEBUG
            //var thread = Thread.CurrentThread;
            //logger.Info($"HumanoidNPCController GoToImpl thread.ManagedThreadId = {thread.ManagedThreadId}");

            var methodId = GetMethodId();
            logger.Info($"HumanoidNPCController GoToImpl [{methodId}] target.Kind = {target.Kind}");
            logger.Info($"HumanoidNPCController GoToImpl [{methodId}] target.AbcoluteCoordinates = {target.AbcoluteCoordinates}");
            logger.Info($"HumanoidNPCController GoToImpl [{methodId}] target?.Entity.InstanceId = {target?.Entity.InstanceId}");
            logger.Info($"HumanoidNPCController GoToImpl [{methodId}] target?.Entity.Position = {target?.Entity.Position}");
            //logger.Info($"HumanoidNPCController GoToImpl [{methodId}]  = {}");
            //logger.Info($"HumanoidNPCController GoToImpl [{methodId}]  = {}");
#endif
            AddWalkingFact(logger);

            var task = _navHelper.Go(logger, target, cancellationToken);

            RunInMainThread(() =>
            {
                _isWalking = true;
                UpdateAnimator();
            });

#if DEBUG
            logger.Info($"HumanoidNPCController GoToImpl [{methodId}] Walking has been started.");
#endif

            var result = await task;

#if DEBUG
            logger.Info($"HumanoidNPCController GoToImpl [{methodId}] result.GoStatus = {result.GoStatus}");
#endif

            RunInMainThread(() =>
            {
                PerformStop(logger);
            });

            if(result.TargetEntity != null)
            {
                RotateToEntityImpl(cancellationToken, logger, result.TargetEntity, speed);
            }

#if DEBUG
            logger.Info($"HumanoidNPCController GoToImpl [{methodId}] Walking has been stoped.");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Stop", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void StopImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info($"StopImpl Begin {methodId}");
#endif

            RunInMainThread(() =>
            {
                PerformStop(logger);
            });
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, IMonitorLogger logger, float direction,
            float speed = 2)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info($"RotateImpl Begin {methodId}; direction = {direction}");
#endif

            var lookRotation = Quaternion.identity;
            
            RunInMainThread(() => {
                var radAngle = direction * Mathf.Deg2Rad;
                var x = Mathf.Sin(radAngle);
                var y = Mathf.Cos(radAngle);
                var localDirection = new Vector3(x, 0f, y);

                var globalDirection = transform.TransformDirection(localDirection);

#if DEBUG
                logger.Info($"RotateImpl {methodId} (1) globalDirection = {globalDirection}");
#endif

                lookRotation = Quaternion.LookRotation(globalDirection);
            });

            NRotate(cancellationToken, logger, lookRotation, speed);

#if DEBUG
            logger.Info($"RotateImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateToEntityImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity,
            float speed = 2)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info($"RotateToEntityImpl Begin {methodId}");
#endif

            if (entity.IsEmpty)
            {
                entity.Specify(logger, /*EntityConstraints.OnlyVisible,*/ EntityConstraints.Nearest);

                entity.Resolve(logger);
            }

#if DEBUG
            logger.Info($"RotateToEntityImpl {methodId} entity.InstanceId = {entity.InstanceId}");
            logger.Info($"RotateToEntityImpl {methodId} entity.Id = {entity.Id}");
            logger.Info($"RotateToEntityImpl {methodId} entity.Position = {entity.Position}");
#endif

            var lookRotation = GetRotationToPositionInUsualThread(logger, entity.Position.Value);

            NRotate(cancellationToken, logger, lookRotation, speed);

#if DEBUG
            logger.Info($"RotateToEntityImpl End {methodId}");
#endif
        }

        private void NRotate(CancellationToken cancellationToken, IMonitorLogger logger, Quaternion targetRotation, float speed)
        {
            var initialRotation = Quaternion.identity;

            RunInMainThread(() => {
                initialRotation = transform.rotation;
            });

            var timeCount = 0.0f;

            while (true)
            {
#if DEBUG
                logger.Info($"RotateImpl End {methodId} (1) timeCount = {timeCount}");
#endif

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                if (timeCount >= 1)
                {
                    break;
                }

                RunInMainThread(() => {
                    transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, timeCount);
                    timeCount += speed * Time.deltaTime;
                });
            }
        }

        private Quaternion? _oldLocalHeadRotation;

        [DebuggerHidden]
        [BipedEndpoint("Rotate head", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateHeadImpl(CancellationToken cancellationToken, IMonitorLogger logger, float? direction)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info($"RotateHeadImpl Begin {methodId}; direction = {direction}");
#endif

            if (!direction.HasValue || direction == 0)
            {
                NResetHeadRotation(logger);
                return;
            }

            if(Math.Abs(direction.Value) > MaxHeadRotationAngle)
            {
                RotateImpl(cancellationToken, logger, direction.Value);
                return;
            }

            RunInMainThread(() => {
                if(!_oldLocalHeadRotation.HasValue)
                {
                    _oldLocalHeadRotation = _targetHeadTransform.localRotation;
                }

                var radAngle = direction.Value * Mathf.Deg2Rad;
                var x = Mathf.Sin(radAngle);
                var y = Mathf.Cos(radAngle);
                var localDirection = new Vector3(x, 0f, y);
                var globalDirection = transform.TransformDirection(localDirection);
                var oldY = _targetHeadTransform.position.y;
                var newPosition = globalDirection + transform.position;
                _currentHeadPosition = new Vector3(newPosition.x, oldY, newPosition.z);

                _enableRotateHeadIK = true;
            });

#if DEBUG
            logger.Info($"RotateHeadImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate head", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateHeadToEntityImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info($"RotateHeadToEntityImpl Begin {methodId}");
#endif

            if (entity == null)
            {
                NResetHeadRotation(logger);
                return;
            }

            if (entity.IsEmpty)
            {
                entity.Specify(logger, /*EntityConstraints.OnlyVisible,*/ EntityConstraints.Nearest);

                entity.Resolve(logger);
            }

#if DEBUG
            logger.Info($"RotateHeadToEntityImpl {methodId} entity.InstanceId = {entity.InstanceId}");
            logger.Info($"RotateHeadToEntityImpl {methodId} entity.Id = {entity.Id}");
            logger.Info($"RotateHeadToEntityImpl {methodId} entity.Position = {entity.Position}");
#endif

            var lookRotation = GetRotationToPositionInUsualThread(logger, entity.Position.Value);

            var anlge = RunInMainThread(() => {
                return Quaternion.Angle(transform.rotation, lookRotation);
            });

#if DEBUG
            logger.Info($"RotateHeadToEntityImpl {methodId} anlge = {anlge}");
#endif

            if (Math.Abs(anlge) > MaxHeadRotationAngle)
            {
                NRotate(cancellationToken, logger, lookRotation, 2);
            }

            RotateHeadImpl(cancellationToken, logger, anlge);

#if DEBUG
            logger.Info($"RotateHeadToEntityImpl End {methodId}");
#endif
        }

        private void NResetHeadRotation(IMonitorLogger logger)
        {
            _enableRotateHeadIK = false;

            if (_oldLocalHeadRotation.HasValue)
            {
                RunInMainThread(() => {
                    _targetHeadTransform.localRotation = _oldLocalHeadRotation.Value;
                    _oldLocalHeadRotation = null;
                });
            }
        }

        [DebuggerHidden]
        [BipedEndpoint("Take", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void TakeImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity)
        {
#if DEBUG
            //var methodId = GetMethodId();

            logger.Info($"TakeImpl Begin {methodId}");
#endif

#if DEBUG
            //UnityEngine.Debug.Log($"TakeImpl entity.IsEmpty = {entity.IsEmpty}");
#endif

            if (entity.IsEmpty)
            {
                entity.SpecifyOnce(logger, BackpackStorage);

                entity.Resolve(logger);
            }

#if DEBUG
            logger.Info($"TakeImpl entity.InstanceId (2) = {entity.InstanceId}");
            logger.Info($"TakeImpl entity.Id (2) = {entity.Id}");
            logger.Info($"TakeImpl entity.Position (2) = {entity.Position}");
            logger.Info($"TakeImpl entity.IsEmpty (2) = {entity.IsEmpty}");
#endif

            if (entity.IsEmpty)
            {
                entity.Specify(logger, EntityConstraints.CanBeTaken/*, EntityConstraints.OnlyVisible, EntityConstraints.Nearest*/);

                entity.Resolve(logger);

#if DEBUG
                logger.Info($"TakeImpl entity.InstanceId (after) = {entity.InstanceId}");
                logger.Info($"TakeImpl entity.Id (after) = {entity.Id}");
                logger.Info($"TakeImpl entity.Position (after) = {entity.Position}");
                logger.Info($"TakeImpl entity.IsEmpty (after) = {entity.IsEmpty}");
#endif
            }

            NTake(cancellationToken, logger, entity);

#if DEBUG
            logger.Info($"TakeImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Take from surface", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void TakeFromSurfaceImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity)
        {
#if DEBUG
            //var methodId = GetMethodId();

            logger.Info($"TakeFromSurfaceImpl Begin {methodId}");
#endif

            if (entity.IsEmpty)
            {
                entity.Specify(logger, EntityConstraints.CanBeTaken/*, EntityConstraints.OnlyVisible, EntityConstraints.Nearest*/);

                entity.Resolve(logger);
            }

            NTake(cancellationToken, logger, entity);

#if DEBUG
            logger.Info($"TakeFromSurfaceImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Take from backpack", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void TakeFromBackpackImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity)
        {
#if DEBUG
            //var methodId = GetMethodId();

            logger.Info($"TakeFromBackpackImpl Begin {methodId}");
#endif

            if (entity.IsEmpty)
            {
                entity.SpecifyOnce(logger, BackpackStorage);

                entity.Resolve(logger);
            }

#if DEBUG
            logger.Info($"TakeFromBackpackImpl entity.InstanceId = {entity.InstanceId}");
            logger.Info($"TakeFromBackpackImpl entity.Id = {entity.Id}");
            logger.Info($"TakeFromBackpackImpl entity.Position = {entity.Position}");
            logger.Info($"TakeFromBackpackImpl entity.IsEmpty = {entity.IsEmpty}");
#endif

            NTake(cancellationToken, logger, entity);

#if DEBUG
            logger.Info($"TakeFromBackpackImpl End {methodId}");
#endif
        }

        private void NTake(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity)
        {
#if DEBUG
            logger.Info($"NTake entity.InstanceId = {entity.InstanceId}");
            logger.Info($"NTake entity.Id = {entity.Id}");
            logger.Info($"NTake entity.Position = {entity.Position}");
#endif

            var handThing = RunInMainThread(() => { return GameObjectsRegistry.GetComponent<IHandThingCustomBehavior>(entity.InstanceId); });

#if DEBUG
            logger.Info($"NTake (handThing != null) = {handThing != null}");
            logger.Info($"NTake (handThing.USocGameObject != null) = {handThing.USocGameObject != null}");
            logger.Info($"NTake (handThing.USocGameObject.SocGameObject != null) = {handThing.USocGameObject.SocGameObject != null}");
#endif

            NTake(logger, handThing);

#if DEBUG
            logger.Info("NTake End");
#endif
        }

        private void NTake(IMonitorLogger logger, IHandThingCustomBehavior handThing)
        {
            RemoveFromBackpack(logger, handThing.USocGameObject.SocGameObject);

#if DEBUG
            logger.Info($"NTake End of RemoveFromBackpack");
#endif

            RunInMainThread(() => {
                switch (handThing.Kind)
                {
                    case KindOfHandThing.Rifle:
                        TakeRifle(logger, handThing as IRifleCustomBehavior);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(handThing.Kind), handThing.Kind, null);
                }
            });
        }

        public void Take(IMonitorLogger logger, IHandThingCustomBehavior handThing)
        {
#if DEBUG
            logger.Info("Take is not fully implemented");
            logger.Info($"HumanoidNPCController Take _isAlreadyStarted = {_isAlreadyStarted}");
#endif

            if(_isAlreadyStarted)
            {
                NTake(logger, handThing);
                return;
            }

            _takeAfterInitialization = handThing;
        }

        private void TakeRifle(IMonitorLogger logger, IRifleCustomBehavior rifle)
        {
            _rifle = rifle;
            _currentHandThing = rifle;

            _hasRifle = true;
            UpdateAnimator();

            rifle.SetToHandsOfHumanoid(logger, this);

            //I have moved endpoins here from RapidFireGunController
            //AddToManualControl(_rifle.USocGameObject, new List<DeviceOfBiped>() {  DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand });

            AddHoldFact(logger, rifle.IdForFacts);
        }

        [DebuggerHidden]
        [FriendsEndpoints("Aim to")]
        [BipedEndpoint("Start Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void StartShootImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
            if (_rifle == null)
            {
                return;
            }

            AddHeShootsFact(logger);

            _rifle.StartFire(cancellationToken, logger);
        }

        [DebuggerHidden]
        [FriendsEndpoints("Aim to")]
        [BipedEndpoint("Stop Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void StopShootImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info($"StopShootImpl Begin {methodId}");
#endif

            if (_rifle == null)
            {
                return;
            }

            RemoveHeShootsFact(logger);

            _rifle.StopFire(logger);

#if DEBUG
            logger.Info($"StopShootImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Ready For Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void ReadyForShootImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
#if DEBUG
            //var methodId = GetMethodId();

            logger.Info($"ReadyForShootImpl Begin {methodId}");
#endif

            RunInMainThread(() => { 
                _isAim = true;
                UpdateAnimator();
            });

            AddHeIsReadyForShootFact(logger);

#if DEBUG
            logger.Info($"ReadyForShootImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Unready For Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void UnReadyForShootImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
#if DEBUG
            //var methodId = GetMethodId();

            logger.Info($"UnReadyForShootImpl Begin {methodId}");
#endif

            RunInMainThread(() => {
                _enableRifleIK = false;
                _isAim = false;
                UpdateAnimator();
            });

            RemoveHeIsReadyForShootFact(logger);

#if DEBUG
            logger.Info($"UnReadyForShootImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Throw out", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void ThrowOutImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
#if DEBUG
            //var methodId = GetMethodId();

            logger.Info($"ThrowOutImpl Begin {methodId}");
#endif

            if (_rifle != null)
            {
                ThrowOutRifle(cancellationToken, logger);
                _currentHandThing = null;
                _rifle = null;
                return;
            }
        }

        public void ThrowOutRifle(CancellationToken cancellationToken, IMonitorLogger logger)
        {
#if DEBUG
            logger.Info($"ThrowOutRifle");
#endif

            RunInMainThread(() => {
                _enableRifleIK = false;

                _rifle.ThrowOut(logger);

                _isAim = false;
                _hasRifle = false;

                UpdateAnimator();
            });

            RemoveAllShootFacts(logger);
        }

        [DebuggerHidden]
        [BipedEndpoint("Aim to", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void AimToImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity)
        {
#if DEBUG
            //var methodId = GetMethodId();

            logger.Info($"AimToImpl Begin {methodId}");
#endif

            if (entity == null)
            {
#if DEBUG
                logger.Info($"AimToImpl {methodId} entity == null");
#endif

                RunInMainThread(() => {
                    _rifle.LookAt(logger);
                });

#if DEBUG
                logger.Info($"AimToImpl {methodId} entity == null return;");
#endif

                return;
            }

            if (entity.IsEmpty)
            {
                entity.Specify(logger, EntityConstraints.OnlyVisible);

                entity.Resolve(logger);
            }

            if(entity.IsEmpty)
            {
#if DEBUG
                logger.Info($"AimToImpl {methodId} entity.IsEmpty End");
#endif

                return;
            }

#if DEBUG
            logger.Info($"AimToImpl {methodId} entity.InstanceId = {entity.InstanceId}");
            logger.Info($"AimToImpl {methodId} entity.Id = {entity.Id}");
            logger.Info($"AimToImpl {methodId} entity.Position = {entity.Position}");
#endif

            var targetGameObject = RunInMainThread(() => { return GameObjectsRegistry.GetGameObject(entity.InstanceId); });

            var lookRotation = GetRotationToPositionInUsualThread(logger, entity.Position.Value);

            var anlge = RunInMainThread(() => {
                return Quaternion.Angle(transform.rotation, lookRotation);
            });

#if DEBUG
            logger.Info($"AimToImpl {methodId} anlge = {anlge}");
#endif

            if (Math.Abs(anlge) > MaxWeaponRotationAngle)
            {
                NRotate(cancellationToken, logger, lookRotation, 2);
            }

            RunInMainThread(() => {
                _enableRifleIK = true;

                _rifle.LookAt(logger, targetGameObject);
            });

#if DEBUG
            logger.Info($"AimToImpl {methodId} End");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("put in backpack", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void PutInBackpackImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info($"PutInBackpackImpl {methodId} Begin");
#endif

            if (_currentHandThing == null)
            {
                return;
            }

#if DEBUG
            logger.Info($"PutInBackpackImpl {methodId} NEXT");
#endif

            _currentHandThing.HideForBackpackInUsualThread(logger);

            RunInMainThread(() => {
                _enableRifleIK = false;
                _hasRifle = false;
                _isAim = false;
                UpdateAnimator();
            });

            AddToBackpack(logger, _currentHandThing.USocGameObject.SocGameObject);

            RemoveAllShootFacts(logger);

#if DEBUG
            logger.Info($"PutInBackpackImpl {methodId} End");
#endif
        }
    }
}
