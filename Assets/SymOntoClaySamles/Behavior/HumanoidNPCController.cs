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

            AddStopFact();

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
                NTake(_takeAfterInitialization);

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

        private void PerformStop()
        {
#if UNITY_EDITOR
           UnityEngine.Debug.Log("HumanoidNPCController Begin PerformStop");
#endif

            _navMeshAgent.ResetPath();
            _isWalking = false;
            UpdateAnimator();
            AddStopFact();

#if UNITY_EDITOR
            UnityEngine.Debug.Log("HumanoidNPCController End PerformStop");
#endif
        }

        public void Die()
        {
#if UNITY_EDITOR
            //UnityEngine.Debug.Log("HumanoidNPCController Die");
#endif

            if(_isDead)
            {
                return;
            }

            _isDead = true;

            UpdateAnimator();

            ProcessDeath();
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
        public async Task GoToImpl(CancellationToken cancellationToken,
        [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget target,
        float speed = 12)
        {
            if (_isDead)
            {
                return;
            }

#if UNITY_EDITOR
            //var thread = Thread.CurrentThread;
            //UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl thread.ManagedThreadId = {thread.ManagedThreadId}");

            var methodId = GetMethodId();
            UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] target.Kind = {target.Kind}");
            UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] target.AbcoluteCoordinates = {target.AbcoluteCoordinates}");
            UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] target?.Entity.InstanceId = {target?.Entity.InstanceId}");
            UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] target?.Entity.Position = {target?.Entity.Position}");
            //UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}]  = {}");
            //UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}]  = {}");
#endif
            AddWalkingFact();

            var task = _navHelper.Go(target, cancellationToken);

            RunInMainThread(() =>
            {
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

            if(result.TargetEntity != null)
            {
                RotateToEntityImpl(cancellationToken, result.TargetEntity, speed);
            }

#if UNITY_EDITOR
            UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] Walking has been stoped.");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Stop", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void StopImpl(CancellationToken cancellationToken)
        {
#if UNITY_EDITOR
            var methodId = GetMethodId();

            UnityEngine.Debug.Log($"StopImpl Begin {methodId}");
#endif

            RunInMainThread(() =>
            {
                PerformStop();
            });
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, float direction,
            float speed = 2)
        {
#if UNITY_EDITOR
            //var methodId = GetMethodId();

            //UnityEngine.Debug.Log($"RotateImpl Begin {methodId}; direction = {direction}");
#endif

            var lookRotation = Quaternion.identity;
            
            RunInMainThread(() => {
                var radAngle = direction * Mathf.Deg2Rad;
                var x = Mathf.Sin(radAngle);
                var y = Mathf.Cos(radAngle);
                var localDirection = new Vector3(x, 0f, y);

                var globalDirection = transform.TransformDirection(localDirection);

#if UNITY_EDITOR
                //UnityEngine.Debug.Log($"RotateImpl {methodId} (1) globalDirection = {globalDirection}");
#endif

                lookRotation = Quaternion.LookRotation(globalDirection);
            });

            NRotate(cancellationToken, lookRotation, speed);

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"RotateImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateToEntityImpl(CancellationToken cancellationToken, IEntity entity,
            float speed = 2)
        {
#if UNITY_EDITOR
            //var methodId = GetMethodId();

            //UnityEngine.Debug.Log($"RotateToEntityImpl Begin {methodId}");
#endif

            if (entity.IsEmpty)
            {
                entity.Specify(/*EntityConstraints.OnlyVisible,*/ EntityConstraints.Nearest);

                entity.Resolve();
            }

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"RotateToEntityImpl {methodId} entity.InstanceId = {entity.InstanceId}");
            //UnityEngine.Debug.Log($"RotateToEntityImpl {methodId} entity.Id = {entity.Id}");
            //UnityEngine.Debug.Log($"RotateToEntityImpl {methodId} entity.Position = {entity.Position}");
#endif

            var lookRotation = GetRotationToPositionInUsualThread(entity.Position.Value);

            NRotate(cancellationToken, lookRotation, speed);

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"RotateToEntityImpl End {methodId}");
#endif
        }

        private void NRotate(CancellationToken cancellationToken, Quaternion targetRotation, float speed)
        {
            var initialRotation = Quaternion.identity;

            RunInMainThread(() => {
                initialRotation = transform.rotation;
            });

            var timeCount = 0.0f;

            while (true)
            {
#if UNITY_EDITOR
                //UnityEngine.Debug.Log($"RotateImpl End {methodId} (1) timeCount = {timeCount}");
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
        public void RotateHeadImpl(CancellationToken cancellationToken, float? direction)
        {
#if UNITY_EDITOR
            //var methodId = GetMethodId();

            //UnityEngine.Debug.Log($"RotateHeadImpl Begin {methodId}; direction = {direction}");
#endif

            if(!direction.HasValue || direction == 0)
            {
                NResetHeadRotation();
                return;
            }

            if(Math.Abs(direction.Value) > MaxHeadRotationAngle)
            {
                RotateImpl(cancellationToken, direction.Value);
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

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"RotateHeadImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate head", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateHeadToEntityImpl(CancellationToken cancellationToken, IEntity entity)
        {
#if UNITY_EDITOR
            //var methodId = GetMethodId();

            //UnityEngine.Debug.Log($"RotateHeadToEntityImpl Begin {methodId}");
#endif

            if(entity == null)
            {
                NResetHeadRotation();
                return;
            }

            if (entity.IsEmpty)
            {
                entity.Specify(/*EntityConstraints.OnlyVisible,*/ EntityConstraints.Nearest);

                entity.Resolve();
            }

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"RotateHeadToEntityImpl {methodId} entity.InstanceId = {entity.InstanceId}");
            //UnityEngine.Debug.Log($"RotateHeadToEntityImpl {methodId} entity.Id = {entity.Id}");
            //UnityEngine.Debug.Log($"RotateHeadToEntityImpl {methodId} entity.Position = {entity.Position}");
#endif

            var lookRotation = GetRotationToPositionInUsualThread(entity.Position.Value);

            var anlge = RunInMainThread(() => {
                return Quaternion.Angle(transform.rotation, lookRotation);
            });

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"RotateHeadToEntityImpl {methodId} anlge = {anlge}");
#endif

            if (Math.Abs(anlge) > MaxHeadRotationAngle)
            {
                NRotate(cancellationToken, lookRotation, 2);
            }

            RotateHeadImpl(cancellationToken, anlge);

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"RotateHeadToEntityImpl End {methodId}");
#endif
        }

        private void NResetHeadRotation()
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
        public void TakeImpl(CancellationToken cancellationToken, IEntity entity)
        {
#if UNITY_EDITOR
            //var methodId = GetMethodId();

            //UnityEngine.Debug.Log($"TakeImpl Begin {methodId}");
#endif

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"TakeImpl entity.IsEmpty = {entity.IsEmpty}");
#endif

            if (entity.IsEmpty)
            {
                entity.SpecifyOnce(BackpackStorage);

                entity.Resolve();
            }

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"TakeImpl entity.InstanceId (2) = {entity.InstanceId}");
            //UnityEngine.Debug.Log($"TakeImpl entity.Id (2) = {entity.Id}");
            //UnityEngine.Debug.Log($"TakeImpl entity.Position (2) = {entity.Position}");
            //UnityEngine.Debug.Log($"TakeImpl entity.IsEmpty (2) = {entity.IsEmpty}");
#endif

            if (entity.IsEmpty)
            {
                entity.Specify(EntityConstraints.CanBeTaken/*, EntityConstraints.OnlyVisible, EntityConstraints.Nearest*/);

                entity.Resolve();

#if UNITY_EDITOR
                //UnityEngine.Debug.Log($"TakeImpl entity.InstanceId (after) = {entity.InstanceId}");
                //UnityEngine.Debug.Log($"TakeImpl entity.Id (after) = {entity.Id}");
                //UnityEngine.Debug.Log($"TakeImpl entity.Position (after) = {entity.Position}");
                //UnityEngine.Debug.Log($"TakeImpl entity.IsEmpty (after) = {entity.IsEmpty}");
#endif
            }

            NTake(cancellationToken, entity);

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"TakeImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Take from surface", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void TakeFromSurfaceImpl(CancellationToken cancellationToken, IEntity entity)
        {
#if UNITY_EDITOR
            //var methodId = GetMethodId();

            //UnityEngine.Debug.Log($"TakeFromSurfaceImpl Begin {methodId}");
#endif

            if (entity.IsEmpty)
            {
                entity.Specify(EntityConstraints.CanBeTaken/*, EntityConstraints.OnlyVisible, EntityConstraints.Nearest*/);

                entity.Resolve();
            }

            NTake(cancellationToken, entity);

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"TakeFromSurfaceImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Take from backpack", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void TakeFromBackpackImpl(CancellationToken cancellationToken, IEntity entity)
        {
#if UNITY_EDITOR
            //var methodId = GetMethodId();

            //UnityEngine.Debug.Log($"TakeFromBackpackImpl Begin {methodId}");
#endif

            if (entity.IsEmpty)
            {
                entity.SpecifyOnce(BackpackStorage);

                entity.Resolve();
            }

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"TakeFromBackpackImpl entity.InstanceId = {entity.InstanceId}");
            //UnityEngine.Debug.Log($"TakeFromBackpackImpl entity.Id = {entity.Id}");
            //UnityEngine.Debug.Log($"TakeFromBackpackImpl entity.Position = {entity.Position}");
            //UnityEngine.Debug.Log($"TakeFromBackpackImpl entity.IsEmpty = {entity.IsEmpty}");
#endif

            NTake(cancellationToken, entity);

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"TakeFromBackpackImpl End {methodId}");
#endif
        }

        private void NTake(CancellationToken cancellationToken, IEntity entity)
        {
#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"NTake entity.InstanceId = {entity.InstanceId}");
            //UnityEngine.Debug.Log($"NTake entity.Id = {entity.Id}");
            //UnityEngine.Debug.Log($"NTake entity.Position = {entity.Position}");
#endif

            var handThing = RunInMainThread(() => { return GameObjectsRegistry.GetComponent<IHandThingCustomBehavior>(entity.InstanceId); });

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"NTake (handThing != null) = {handThing != null}");
            //UnityEngine.Debug.Log($"NTake (handThing.USocGameObject != null) = {handThing.USocGameObject != null}");
            //UnityEngine.Debug.Log($"NTake (handThing.USocGameObject.SocGameObject != null) = {handThing.USocGameObject.SocGameObject != null}");
#endif

            NTake(handThing);

#if UNITY_EDITOR
            //UnityEngine.Debug.Log("NTake End");
#endif
        }

        private void NTake(IHandThingCustomBehavior handThing)
        {
            RemoveFromBackpack(handThing.USocGameObject.SocGameObject);

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"NTake End of RemoveFromBackpack");
#endif

            RunInMainThread(() => {
                switch (handThing.Kind)
                {
                    case KindOfHandThing.Rifle:
                        TakeRifle(handThing as IRifleCustomBehavior);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(handThing.Kind), handThing.Kind, null);
                }
            });
        }

        public void Take(IHandThingCustomBehavior handThing)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log("Take is not fully implemented");
            UnityEngine.Debug.Log($"HumanoidNPCController Take _isAlreadyStarted = {_isAlreadyStarted}");
#endif

            if(_isAlreadyStarted)
            {
                NTake(handThing);
                return;
            }

            _takeAfterInitialization = handThing;
        }

        private void TakeRifle(IRifleCustomBehavior rifle)
        {
            _rifle = rifle;
            _currentHandThing = rifle;

            _hasRifle = true;
            UpdateAnimator();

            rifle.SetToHandsOfHumanoid(this);

            //I have moved endpoins here from RapidFireGunController
            //AddToManualControl(_rifle.USocGameObject, new List<DeviceOfBiped>() {  DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand });

            AddHoldFact(rifle.IdForFacts);
        }

        [DebuggerHidden]
        [FriendsEndpoints("Aim to")]
        [BipedEndpoint("Start Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void StartShootImpl(CancellationToken cancellationToken)
        {
            if (_rifle == null)
            {
                return;
            }

            AddHeShootsFact();

            _rifle.StartFire(cancellationToken);
        }

        [DebuggerHidden]
        [FriendsEndpoints("Aim to")]
        [BipedEndpoint("Stop Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void StopShootImpl(CancellationToken cancellationToken)
        {
#if UNITY_EDITOR
            var methodId = GetMethodId();

            //UnityEngine.Debug.Log($"StopShootImpl Begin {methodId}");
#endif

            if (_rifle == null)
            {
                return;
            }

            RemoveHeShootsFact();

            _rifle.StopFire();

#if UNITY_EDITOR
            UnityEngine.Debug.Log($"StopShootImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Ready For Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void ReadyForShootImpl(CancellationToken cancellationToken)
        {
#if UNITY_EDITOR
            //var methodId = GetMethodId();

            //UnityEngine.Debug.Log($"ReadyForShootImpl Begin {methodId}");
#endif

            RunInMainThread(() => { 
                _isAim = true;
                UpdateAnimator();
            });

            AddHeIsReadyForShootFact();

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"ReadyForShootImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Unready For Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void UnReadyForShootImpl(CancellationToken cancellationToken)
        {
#if UNITY_EDITOR
            //var methodId = GetMethodId();

            //UnityEngine.Debug.Log($"UnReadyForShootImpl Begin {methodId}");
#endif

            RunInMainThread(() => {
                _enableRifleIK = false;
                _isAim = false;
                UpdateAnimator();
            });

            RemoveHeIsReadyForShootFact();

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"UnReadyForShootImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Throw out", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void ThrowOutImpl(CancellationToken cancellationToken)
        {
#if UNITY_EDITOR
            //var methodId = GetMethodId();

            //UnityEngine.Debug.Log($"ThrowOutImpl Begin {methodId}");
#endif

            if (_rifle != null)
            {
                ThrowOutRifle(cancellationToken);
                _currentHandThing = null;
                _rifle = null;
                return;
            }
        }

        public void ThrowOutRifle(CancellationToken cancellationToken)
        {
#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"ThrowOutRifle");
#endif

            RunInMainThread(() => {
                _enableRifleIK = false;

                _rifle.ThrowOut();

                _isAim = false;
                _hasRifle = false;

                UpdateAnimator();
            });

            RemoveAllShootFacts();
        }

        [DebuggerHidden]
        [BipedEndpoint("Aim to", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void AimToImpl(CancellationToken cancellationToken, IEntity entity)
        {
#if UNITY_EDITOR
            //var methodId = GetMethodId();

            //UnityEngine.Debug.Log($"AimToImpl Begin {methodId}");
#endif

            if(entity == null)
            {
#if UNITY_EDITOR
                //UnityEngine.Debug.Log($"AimToImpl {methodId} entity == null");
#endif

                RunInMainThread(() => {
                    _rifle.LookAt();
                });

#if UNITY_EDITOR
                //UnityEngine.Debug.Log($"AimToImpl {methodId} entity == null return;");
#endif

                return;
            }

            if (entity.IsEmpty)
            {
                entity.Specify(EntityConstraints.OnlyVisible);

                entity.Resolve();
            }

            if(entity.IsEmpty)
            {
#if UNITY_EDITOR
                //UnityEngine.Debug.Log($"AimToImpl {methodId} entity.IsEmpty End");
#endif

                return;
            }

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"AimToImpl {methodId} entity.InstanceId = {entity.InstanceId}");
            //UnityEngine.Debug.Log($"AimToImpl {methodId} entity.Id = {entity.Id}");
            //UnityEngine.Debug.Log($"AimToImpl {methodId} entity.Position = {entity.Position}");
#endif

            var targetGameObject = RunInMainThread(() => { return GameObjectsRegistry.GetGameObject(entity.InstanceId); });

            var lookRotation = GetRotationToPositionInUsualThread(entity.Position.Value);

            var anlge = RunInMainThread(() => {
                return Quaternion.Angle(transform.rotation, lookRotation);
            });

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"AimToImpl {methodId} anlge = {anlge}");
#endif

            if(Math.Abs(anlge) > MaxWeaponRotationAngle)
            {
                NRotate(cancellationToken, lookRotation, 2);
            }

            RunInMainThread(() => {
                _enableRifleIK = true;

                _rifle.LookAt(targetGameObject);
            });

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"AimToImpl {methodId} End");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("put in backpack", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void PutInBackpackImpl(CancellationToken cancellationToken)
        {
#if UNITY_EDITOR
            //var methodId = GetMethodId();

            //UnityEngine.Debug.Log($"PutInBackpackImpl {methodId} Begin");
#endif

            if(_currentHandThing == null)
            {
                return;
            }

#if UNITY_EDITOR
            //UnityEngine.Debug.Log($"PutInBackpackImpl {methodId} NEXT");
#endif

            _currentHandThing.HideForBackpackInUsualThread();

            RunInMainThread(() => {
                _enableRifleIK = false;
                _hasRifle = false;
                _isAim = false;
                UpdateAnimator();
            });

            AddToBackpack(_currentHandThing.USocGameObject.SocGameObject);

            RemoveAllShootFacts();
        }
    }
}
