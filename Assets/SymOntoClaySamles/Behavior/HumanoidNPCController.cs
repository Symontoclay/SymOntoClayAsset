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
            base.Start();

#if UNITY_EDITOR
            UnityEngine.Debug.Log($"HumanoidNPCController Start name = {name}; Logger?.Id = {Logger?.Id}");
#endif

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
            logger.Info("58D66A50-4EBD-4D97-821A-03907FC65273", "HumanoidNPCController Begin PerformStop");
#endif

            _navMeshAgent.ResetPath();
            _isWalking = false;
            UpdateAnimator();
            AddStopFact(logger);

#if DEBUG
            logger.Info("8D45FCDF-28C7-48D1-A622-42BD4EDC5A0B", "HumanoidNPCController End PerformStop");
#endif
        }

        public void Die()
        {
#if DEBUG
            Logger.Info("5A425A9C-CFEC-4F53-ACFE-6F404359EC94", "HumanoidNPCController Die Begin");
#endif

            if (_isDead)
            {
#if DEBUG
                Logger.Info("D194AE2F-4E47-40C5-9184-6C1CE1B30091", "HumanoidNPCController Die End _isDeads");
#endif

                return;
            }

            _isDead = true;

            UpdateAnimator();

            ProcessDeath(Logger);

#if DEBUG
            Logger.Info("DB466FA7-4273-41A8-B6E6-C0CD89D469EA", "HumanoidNPCController Die End");
#endif
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
#if DEBUG
            //svar thread = Thread.CurrentThread;
            //logger.Info($"HumanoidNPCController GoToImpl thread.ManagedThreadId = {thread.ManagedThreadId}");

            var methodId = GetMethodId();
            logger.Info("E2E44073-98A9-4A61-A0CE-C5DC50395F89", $"HumanoidNPCController GoToImpl [{methodId}] target.Kind = {target.Kind}");
            logger.Info("7E79434C-6E7F-4AA0-8C0D-BEA96C42B70F", $"HumanoidNPCController GoToImpl [{methodId}] target.AbcoluteCoordinates = {target.AbcoluteCoordinates}");
            logger.Info("77752A13-AB49-4B34-8E98-2CCF485153B9", $"HumanoidNPCController GoToImpl [{methodId}] target?.Entity.InstanceId = {target?.Entity.InstanceId}");
            logger.Info("F655E84D-F895-419A-9C4E-18C4D975F761", $"HumanoidNPCController GoToImpl [{methodId}] target?.Entity.Position = {target?.Entity.Position}");
            //logger.Info($"HumanoidNPCController GoToImpl [{methodId}]  = {}");
            //logger.Info($"HumanoidNPCController GoToImpl [{methodId}]  = {}");
#endif

            if (_isDead)
            {
#if DEBUG
                logger.Info("D0EF0F50-A81E-4F12-82E3-6F8EB58620AC", $"HumanoidNPCController GoToImpl [{methodId}] _isDead");
#endif

                return;
            }

            AddWalkingFact(logger);

            //var task = _navHelper.GoAsync(logger, target, cancellationToken);

            RunInMainThread(() =>
            {
                _isWalking = true;
                UpdateAnimator();
            });

#if DEBUG
            logger.Info("4014E628-E98D-4C12-979D-D31CD29027FE", $"HumanoidNPCController GoToImpl [{methodId}] Walking has been started.");
#endif

            //var result = await task;
            var result = _navHelper.Go(logger, target, cancellationToken);

#if DEBUG
            logger.Info("22A6E1D0-6E80-4C2A-B242-01DB0D78A2A5", $"HumanoidNPCController GoToImpl [{methodId}] result.GoStatus = {result.GoStatus}");
#endif

            RunInMainThread(() =>
            {
                PerformStop(logger);
            });

            if(result.TargetEntity != null)
            {
#if DEBUG
                logger.Info("CC42BBE6-6265-4DF5-9F13-4255779A5B98", $"HumanoidNPCController GoToImpl [{methodId}] result.TargetEntity != null");
#endif

                RotateToEntityImpl(cancellationToken, logger, result.TargetEntity, speed);
            }

#if DEBUG
            logger.Info("3DBFEB0B-3583-40D1-B7B2-32DC161D786B", $"HumanoidNPCController GoToImpl [{methodId}] Walking has been stoped.");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Stop", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void StopImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info("61826995-BF0B-472F-8E1C-D92319D21B95", $"StopImpl Begin [{methodId}]");
#endif

            RunInMainThread(() =>
            {
                PerformStop(logger);
            });

#if DEBUG
            logger.Info("2E218629-85F8-4CCC-BB4C-9734A438397A", $"StopImpl End [{methodId}]");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, IMonitorLogger logger, float direction,
            float speed = 2)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info("37805A77-4D73-46AB-9AF9-2701DFB6BAD2", $"RotateImpl Begin [{methodId}] direction = {direction}");
#endif

            var lookRotation = Quaternion.identity;
            
            RunInMainThread(() => {
                var radAngle = direction * Mathf.Deg2Rad;
                var x = Mathf.Sin(radAngle);
                var y = Mathf.Cos(radAngle);
                var localDirection = new Vector3(x, 0f, y);

                var globalDirection = transform.TransformDirection(localDirection);

#if DEBUG
                logger.Info("66395D97-A49B-4745-9E48-FEF361BCB8A8", $"RotateImpl [{methodId}] (1) globalDirection = {globalDirection}");
#endif

                lookRotation = Quaternion.LookRotation(globalDirection);
            });

            NRotate(cancellationToken, logger, lookRotation, speed);

#if DEBUG
            logger.Info("B8C3FEBA-985E-4FE7-B4C4-E35B64814621", $"RotateImpl End [{methodId}]");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateToEntityImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity,
            float speed = 2)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info("2EFA75FA-14F2-4E8B-8E4E-AFFCD7E13FA0", $"RotateToEntityImpl Begin [{methodId}]");
#endif

            if (entity.IsEmpty)
            {
#if DEBUG
                logger.Info("7A1FC945-7C11-495B-9769-9E7D4C8F20CB", $"RotateToEntityImpl [{methodId}] entity.IsEmpty");
#endif

                entity.Specify(logger, /*EntityConstraints.OnlyVisible,*/ EntityConstraints.Nearest);

                entity.Resolve(logger);
            }

#if DEBUG
            logger.Info("AAC3C955-2263-4A6A-828F-20CEA61E3DED", $"RotateToEntityImpl [{methodId}] entity.InstanceId = {entity.InstanceId}");
            logger.Info("D65B6DDC-53E7-4FBA-8B27-556D352F529E", $"RotateToEntityImpl [{methodId}] entity.Id = {entity.Id}");
            logger.Info("AF90CF05-BC53-49C5-9E73-D906E8CD1045", $"RotateToEntityImpl [{methodId}] entity.Position = {entity.Position}");
#endif

            var lookRotation = GetRotationToPositionInUsualThread(logger, entity.Position.Value);

            NRotate(cancellationToken, logger, lookRotation, speed);

#if DEBUG
            logger.Info("B4F4FF43-E215-4980-B95D-59FED975742D", $"RotateToEntityImpl End [{methodId}]");
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
                logger.Info("FDC38F30-1442-4EE0-9655-D51CB7FCCA5F", $"RotateImpl timeCount = {timeCount}");
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

            logger.Info("5FEFBFAB-9B7B-4A4F-A421-925242F82206", $"RotateHeadImpl Begin [{methodId}] direction = {direction}");
#endif

            if (!direction.HasValue || direction == 0)
            {
#if DEBUG
                logger.Info("30874E82-4731-4B60-BA2C-40AFDAE9F48D", $"RotateHeadImpl [{methodId}] !direction.HasValue || direction == 0");
#endif

                NResetHeadRotation(logger);
                return;
            }

            if(Math.Abs(direction.Value) > MaxHeadRotationAngle)
            {
#if DEBUG
                logger.Info("E9D160B5-DC0C-490E-A89D-67303522B2B5", $"RotateHeadImpl [{methodId}] ");
#endif

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
            logger.Info("5A7F123E-EDEB-476D-BC85-3837CC27EECA", $"RotateHeadImpl End [{methodId}]");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate head", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateHeadToEntityImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info("AEB2DFA3-B691-4F09-986D-5DC194CC812A", $"RotateHeadToEntityImpl Begin [{methodId}]");
#endif

            if (entity == null)
            {
#if DEBUG
                logger.Info("877A0A72-3F55-47F5-9E9D-AD644EFACD8B", $"RotateHeadToEntityImpl [{methodId}] entity == null");
#endif

                NResetHeadRotation(logger);
                return;
            }

            if (entity.IsEmpty)
            {
#if DEBUG
                logger.Info("CFABCF34-96E7-407F-B170-F8742ABED585", $"RotateHeadToEntityImpl [{methodId}] entity.IsEmpty");
#endif

                entity.Specify(logger, /*EntityConstraints.OnlyVisible,*/ EntityConstraints.Nearest);

                entity.Resolve(logger);
            }

#if DEBUG
            logger.Info("CAE084B9-2072-4F66-A8D5-FF00AEA2FE73", $"RotateHeadToEntityImpl [{methodId}] entity.InstanceId = {entity.InstanceId}");
            logger.Info("D7316F8A-4A49-4D04-BA31-677A84B12317", $"RotateHeadToEntityImpl [{methodId}] entity.Id = {entity.Id}");
            logger.Info("7FE0E2FB-173A-403C-B756-EFE8941536A2", $"RotateHeadToEntityImpl [{methodId}] entity.Position = {entity.Position}");
#endif

            var lookRotation = GetRotationToPositionInUsualThread(logger, entity.Position.Value);

            var anlge = RunInMainThread(() => {
                return Quaternion.Angle(transform.rotation, lookRotation);
            });

#if DEBUG
            logger.Info("ECB09534-6349-4E6F-A22E-D23E08821B33", $"RotateHeadToEntityImpl [{methodId}] anlge = {anlge}");
#endif

            if (Math.Abs(anlge) > MaxHeadRotationAngle)
            {
#if DEBUG
                logger.Info("D6015C30-612B-47B1-AEF8-A5223AB5EFDA", $"RotateHeadToEntityImpl [{methodId}] Math.Abs(anlge) > MaxHeadRotationAngle");
#endif

                NRotate(cancellationToken, logger, lookRotation, 2);
            }

            RotateHeadImpl(cancellationToken, logger, anlge);

#if DEBUG
            logger.Info("AADF50E6-1975-4B73-A99F-D0A5D32F3E95", $"RotateHeadToEntityImpl End [{methodId}]");
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
            var methodId = GetMethodId();

            logger.Info("5DCBC769-74F8-4FE4-8AC9-6FF0358AF79D", $"TakeImpl Begin [{methodId}]");
#endif

#if DEBUG
            logger.Info("085D8A50-53B2-4971-9A0B-F8CAE1A58E0D", $"TakeImpl [{methodId}] entity.IsEmpty = {entity.IsEmpty}");
#endif

            if (entity.IsEmpty)
            {
                entity.SpecifyOnce(logger, BackpackStorage);

                entity.Resolve(logger);
            }

#if DEBUG
            logger.Info("16A4857E-B3EC-411C-8133-264F33B9F86D", $"TakeImpl [{methodId}] entity.InstanceId (2) = {entity.InstanceId}");
            logger.Info("5F75A369-62E4-4BEE-B2E2-93BBA3D1D55D", $"TakeImpl [{methodId}] entity.Id (2) = {entity.Id}");
            logger.Info("CC070752-F87E-4B9E-BC37-B5B4D1A14274", $"TakeImpl [{methodId}] entity.Position (2) = {entity.Position}");
            logger.Info("8BE5EB48-2DA7-48CF-95DC-7FDC8B02C7A5", $"TakeImpl [{methodId}] entity.IsEmpty (2) = {entity.IsEmpty}");
#endif

            if (entity.IsEmpty)
            {
                entity.Specify(logger, EntityConstraints.CanBeTaken/*, EntityConstraints.OnlyVisible, EntityConstraints.Nearest*/);

                entity.Resolve(logger);

#if DEBUG
                logger.Info("839FFB7E-EB90-44F6-9083-51CB88A9469E", $"TakeImpl [{methodId}] entity.InstanceId (after) = {entity.InstanceId}");
                logger.Info("77836249-5A57-4230-A115-4B1A725FB498", $"TakeImpl [{methodId}] entity.Id (after) = {entity.Id}");
                logger.Info("DDA2BB5F-1A5B-4B3D-9875-A6A79BBA43D4", $"TakeImpl [{methodId}] entity.Position (after) = {entity.Position}");
                logger.Info("4D2D1CBA-9E91-484D-A268-5E0DC5E6AF1E", $"TakeImpl [{methodId}] entity.IsEmpty (after) = {entity.IsEmpty}");
#endif
            }

            NTake(cancellationToken, logger, entity);

#if DEBUG
            logger.Info("8D2FEB78-75C3-44EC-84BE-9DC71D205631", $"TakeImpl End [{methodId}]");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Take from surface", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void TakeFromSurfaceImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info("DCB1DAAD-D2D0-4057-9EC8-8ABC91B3FD83", $"TakeFromSurfaceImpl Begin [{methodId}]");
#endif

            if (entity.IsEmpty)
            {
#if DEBUG
                logger.Info("7BCAC122-9CB0-437F-BCF9-644EF24FF221", $"TakeFromSurfaceImpl [{methodId}] entity.IsEmpty");
#endif

                entity.Specify(logger, EntityConstraints.CanBeTaken/*, EntityConstraints.OnlyVisible, EntityConstraints.Nearest*/);

                entity.Resolve(logger);
            }

            NTake(cancellationToken, logger, entity);

#if DEBUG
            logger.Info("1062BAC1-F886-4E37-BA6E-512F6041C169", $"TakeFromSurfaceImpl End [{methodId}]");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Take from backpack", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void TakeFromBackpackImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info("D49F7BDA-C287-46CA-882E-2DB90CCD22A6", $"TakeFromBackpackImpl Begin [{methodId}]");
#endif

            if (entity.IsEmpty)
            {
#if DEBUG
                logger.Info("B1AEC41C-5B3C-4764-B30E-5DE66A43089E", $"TakeFromBackpackImpl [{methodId}] entity.IsEmpty");
#endif

                entity.SpecifyOnce(logger, BackpackStorage);

                entity.Resolve(logger);
            }

#if DEBUG
            logger.Info("2667B804-DC30-48E7-8745-E59147C5F2CB", $"TakeFromBackpackImpl [{methodId}] entity.InstanceId = {entity.InstanceId}");
            logger.Info("7DC4DCEF-B7F0-4388-8623-9E47B101D515", $"TakeFromBackpackImpl [{methodId}] entity.Id = {entity.Id}");
            logger.Info("3F521AAD-0AD4-4A50-9712-9224EDA2A3D5", $"TakeFromBackpackImpl [{methodId}] entity.Position = {entity.Position}");
            logger.Info("C291B422-6133-45F9-8161-6B34FF1113D3", $"TakeFromBackpackImpl [{methodId}] entity.IsEmpty = {entity.IsEmpty}");
#endif

            NTake(cancellationToken, logger, entity);

#if DEBUG
            logger.Info("EE04368B-5464-4745-BE83-1221B8030A0B", $"TakeFromBackpackImpl End [{methodId}]");
#endif
        }

        private void NTake(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity)
        {
#if DEBUG
            logger.Info("3474934A-7962-4C82-995A-C3FC3EC011B8", $"NTake entity.InstanceId = {entity.InstanceId}");
            logger.Info("B9B9DF1C-69F5-4B89-94CB-59E841772B0B", $"NTake entity.Id = {entity.Id}");
            logger.Info("CBF0DCE4-99E5-4A5A-8FF2-973C4F8F318F", $"NTake entity.Position = {entity.Position}");
#endif

            var handThing = RunInMainThread(() => { return GameObjectsRegistry.GetComponent<IHandThingCustomBehavior>(entity.InstanceId); });

#if DEBUG
            logger.Info("86A71A7D-FDF8-4EB2-8A80-534C5C7C832F", $"NTake (handThing != null) = {handThing != null}");
            logger.Info("B671BD12-66BD-4828-81E7-D71C27304D8F", $"NTake (handThing.USocGameObject != null) = {handThing.USocGameObject != null}");
            logger.Info("DDCC1E18-8BFE-41B9-94B0-DA20524AC27E", $"NTake (handThing.USocGameObject.SocGameObject != null) = {handThing.USocGameObject.SocGameObject != null}");
#endif

            NTake(logger, handThing);

#if DEBUG
            logger.Info("39C494A8-171A-4746-A6AE-E605DB088E63", "NTake End");
#endif
        }

        private void NTake(IMonitorLogger logger, IHandThingCustomBehavior handThing)
        {
            RemoveFromBackpack(logger, handThing.USocGameObject.SocGameObject);

#if DEBUG
            logger?.Info("968DC76B-7216-470B-9965-ACEC1ECA70F3", $"NTake End of RemoveFromBackpack");
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
            logger?.Info("91E033D0-F450-467B-994E-D214C5D566A6", "Take is not fully implemented");
            logger?.Info("3067B94A-DCBD-461C-AFAC-360AD0217E4C", $"HumanoidNPCController Take _isAlreadyStarted = {_isAlreadyStarted}");
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
#if DEBUG
            var methodId = GetMethodId();

            logger.Info("28F7B1C1-6F83-4AA0-B83B-7AA679CF0443", $"StartShootImpl Begin [{methodId}]");
#endif

            if (_rifle == null)
            {
#if DEBUG
                logger.Info("63D01449-9554-4CF8-9DF4-CEE292296F16", $"StartShootImpl End [{methodId}] _rifle == null");
#endif

                return;
            }

            AddHeShootsFact(logger);

            _rifle.StartFire(cancellationToken, logger);

#if DEBUG
            logger.Info("C2E3110B-F6EA-46EF-9FC1-B509C30275EE", $"StartShootImpl End [{methodId}]");
#endif
        }

        [DebuggerHidden]
        [FriendsEndpoints("Aim to")]
        [BipedEndpoint("Stop Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void StopShootImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info("FB36F499-6314-4034-B3CF-4DB912A2BC6C", $"StopShootImpl Begin [{methodId}]");
#endif

            if (_rifle == null)
            {
#if DEBUG
                logger.Info("E5A226ED-B70F-4ACC-B6D9-AB70CB6D2BF9", $"StopShootImpl End [{methodId}] _rifle == null");
#endif

                return;
            }

            RemoveHeShootsFact(logger);

            _rifle.StopFire(logger);

#if DEBUG
            logger.Info("9404BBEB-9726-4441-A76D-C9FC0866B4E5", $"StopShootImpl End [{methodId}]");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Ready For Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void ReadyForShootImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info("942B7068-CB59-4998-A487-E72861587E75", $"ReadyForShootImpl Begin [{methodId}]");
#endif

            RunInMainThread(() => { 
                _isAim = true;
                UpdateAnimator();
            });

            AddHeIsReadyForShootFact(logger);

#if DEBUG
            logger.Info("71D1999C-1322-4122-8BEA-9B20D05E6952", $"ReadyForShootImpl End [{methodId}]");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Unready For Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void UnReadyForShootImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info("1DA34AE5-93CD-48E7-8085-4CBFFE4D64FD", $"UnReadyForShootImpl Begin [{methodId}]");
#endif

            RunInMainThread(() => {
                _enableRifleIK = false;
                _isAim = false;
                UpdateAnimator();
            });

            RemoveHeIsReadyForShootFact(logger);

#if DEBUG
            logger.Info("B7FC0923-D35F-417C-92A7-77B0DC99791C", $"UnReadyForShootImpl End [{methodId}]");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Throw out", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void ThrowOutImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info("83EA15FF-2B4A-4F5C-AADA-EF21E51A77AA", $"ThrowOutImpl Begin [{methodId}]");
#endif

            if (_rifle != null)
            {
#if DEBUG
                logger.Info("58E0731B-CF72-4D6B-B6F3-8E4D86B6C625", $"ThrowOutImpl [{methodId}] _rifle != null");
#endif

                ThrowOutRifle(cancellationToken, logger);
                _currentHandThing = null;
                _rifle = null;

#if DEBUG
                logger.Info("23E288C5-63E4-4482-A063-B63AEEE7F922", $"ThrowOutImpl End [{methodId}]");
#endif

                return;
            }

#if DEBUG
            logger.Info("F06177E6-5ACE-4D37-B2C1-1E02F1BDB1DE", $"ThrowOutImpl End [{methodId}[");
#endif
        }

        public void ThrowOutRifle(CancellationToken cancellationToken, IMonitorLogger logger)
        {
#if DEBUG
            logger.Info("00DFB8FB-59EE-4697-B792-5D8520731E83", $"ThrowOutRifle Begin");
#endif

            RunInMainThread(() => {
                _enableRifleIK = false;

                _rifle.ThrowOut(logger);

                _isAim = false;
                _hasRifle = false;

                UpdateAnimator();
            });

            RemoveAllShootFacts(logger);

#if DEBUG
            logger.Info("5353C623-96D6-4EEA-94B3-566FFF33C234", $"ThrowOutRifle End");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Aim to", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void AimToImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info("0679D3D8-5A5D-4F57-86D6-FA89CDB267F3", $"AimToImpl Begin [{methodId}]");
#endif

            if (entity == null)
            {
#if DEBUG
                logger.Info("BCDAE495-7F4D-4742-9A76-1F3556672EA4", $"AimToImpl [{methodId}] entity == null");
#endif

                RunInMainThread(() => {
                    _rifle.LookAt(logger);
                });

#if DEBUG
                logger.Info("312A62E9-DC27-44E8-BCC5-E0622D5BDEC5", $"AimToImpl [{methodId}] entity == null return;");
#endif

                return;
            }

            if (entity.IsEmpty)
            {
#if DEBUG
                logger.Info("58465AB3-2A2F-4789-8066-280DBAF04866", $"AimToImpl [{methodId}] entity.IsEmpty (1)");
#endif

                entity.Specify(logger, EntityConstraints.OnlyVisible);

                entity.Resolve(logger);
            }

            if(entity.IsEmpty)
            {
#if DEBUG
                logger.Info("47735032-3535-439A-BEA2-EB773FDACF1E", $"AimToImpl [{methodId}] entity.IsEmpty End");
#endif

                return;
            }

#if DEBUG
            logger.Info("08AC9168-005D-49A8-9BE8-D8C606DD8A6F", $"AimToImpl [{methodId}] entity.InstanceId = {entity.InstanceId}");
            logger.Info("58398B81-7565-44E0-8E32-F3F318DAD732", $"AimToImpl [{methodId}] entity.Id = {entity.Id}");
            logger.Info("25EEADC2-48C0-4DE5-910C-F337CF29428B", $"AimToImpl [{methodId}] entity.Position = {entity.Position}");
#endif

            var targetGameObject = RunInMainThread(() => { return GameObjectsRegistry.GetGameObject(entity.InstanceId); });

            var lookRotation = GetRotationToPositionInUsualThread(logger, entity.Position.Value);

            var anlge = RunInMainThread(() => {
                return Quaternion.Angle(transform.rotation, lookRotation);
            });

#if DEBUG
            logger.Info("4D809EB4-E7D8-4447-95B6-6A823B015B10", $"AimToImpl [{methodId}] anlge = {anlge}");
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
            logger.Info("7853AE68-DA2A-4F29-AE55-A4C758560867", $"AimToImpl [{methodId}] End");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("put in backpack", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void PutInBackpackImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info("225181C0-5656-49FA-9F3D-7DA5B515E58A", $"PutInBackpackImpl [{methodId}] Begin");
#endif

            if (_currentHandThing == null)
            {
#if DEBUG
                logger.Info("B9733273-898A-402A-A6C7-29AAED79F4A2", $"PutInBackpackImpl [{methodId}] _currentHandThing == null = {_currentHandThing == null}");
#endif

                return;
            }

#if DEBUG
            logger.Info("FDFC045B-57D1-4F60-831A-B8A7013EDD76", $"PutInBackpackImpl [{methodId}] NEXT");
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
            logger.Info("E4748301-470F-49B2-A33E-CAE2249B851A", $"PutInBackpackImpl [{methodId}] End");
#endif
        }
    }
}
