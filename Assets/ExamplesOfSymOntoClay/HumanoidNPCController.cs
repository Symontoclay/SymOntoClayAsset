using UnityEngine;
using System.Collections;
using SymOntoClay;
using System.Threading;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Helpers;
using UnityEngine.AI;
using System.Diagnostics;
using Assets.SymOntoClay;
using SymOntoClay.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExamplesOfSymOntoClay
{
    [RequireComponent(typeof(IUHumanoidNPC))]
    public class HumanoidNPCController : BaseBehavior, IUBipedHumanoid, IDieProvider
    {

        public GameObject RightHandWP;
        public GameObject LeftHandWP;

        private GameObject _rightHandWP;
        private GameObject _leftHandWP;

        GameObject IUBipedHumanoid.RightHandWP => _rightHandWP;
        GameObject IUBipedHumanoid.LeftHandWP => _leftHandWP;

        void Awake()
        {
#if DEBUG
            //Debug.Log("HumanoidNPCController Awake");
#endif

            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }

        protected override void Start()
        {
#if DEBUG
            //UnityEngine.Debug.Log("HumanoidNPCController OnStart Begin");
#endif

            base.Start();

            AddStopFact();

            if(RightHandWP == null)
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

#if DEBUG
            //UnityEngine.Debug.Log("HumanoidNPCController OnStart End");
#endif
        }

        // Update is called once per frame
        void Update()
        {
            if (_isDead)
            {
                return;
            }

            _position = transform.position;
        }

        void OnAnimatorIK(int layerIndex)
        {
            if (!_enableRifleIK)
            {
                return;
            }

            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.3f);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, _rifle.AddWP.transform.position);
        }

        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private Rigidbody _rigidbody;

        private bool _hasRifle;
        private bool _isWalking;
        private bool _isAim;
        private bool _isDead;

        private bool _enableRifleIK;

        private Vector3 _position;

        private IRifle _rifle; 

        private void UpdateAnimator()
        {
            _animator.SetBool("hasRifle", _hasRifle);
            _animator.SetBool("walk", _isWalking);
            _animator.SetBool("isAim", _isAim);
            _animator.SetBool("isDead", _isDead);
        }

        private void PerformStop()
        {
            _navMeshAgent.ResetPath();
            _isWalking = false;
            UpdateAnimator();
            AddStopFact();
        }

        public void Die()
        {
#if DEBUG
            UnityEngine.Debug.Log("HumanoidNPCController Die");
#endif

            _isDead = true;

            UpdateAnimator();

            Task.Run(() => {
                ProcessDie();
            });            
        }

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

#if DEBUG
            //var methodId = GetMethodId();
            //UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] point = {point}");
#endif
            AddWalkingFact();

            RunInMainThread(() => {
                _navMeshAgent.SetDestination(point);
                _isWalking = true;
                UpdateAnimator();
            });

#if DEBUG
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

#if DEBUG
                    //UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] Walking has been stoped.");
#endif

                    break;
                }

#if DEBUG
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

#if DEBUG
            //UnityEngine.Debug.Log($"HumanoidNPCController GoToImpl [{methodId}] Walking has been stoped.");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, float direction)
        {
#if DEBUG
            var methodId = GetMethodId();

            UnityEngine.Debug.Log($"RotateImpl Begin {methodId}; direction = {direction}");
#endif



#if DEBUG
            UnityEngine.Debug.Log($"RotateImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Take", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void TakeImpl(CancellationToken cancellationToken, IEntity entity)
        {
#if DEBUG
            var methodId = GetMethodId();

            UnityEngine.Debug.Log($"TakeImpl Begin {methodId}");
#endif

            entity.Specify(EntityConstraints.CanBeTaken/*, EntityConstraints.OnlyVisible, EntityConstraints.Nearest*/);

            entity.Resolve();

#if DEBUG
            UnityEngine.Debug.Log($"TakeImpl {methodId} entity.InstanceId = {entity.InstanceId}");
            UnityEngine.Debug.Log($"TakeImpl {methodId} entity.Id = {entity.Id}");
            UnityEngine.Debug.Log($"TakeImpl {methodId} entity.Position = {entity.Position}");
#endif

            RunInMainThread(() => {
                var handThing = GameObjectsRegistry.GetComponent<IHandThing>(entity.InstanceId);

#if DEBUG
                UnityEngine.Debug.Log($"TakeImpl {methodId} (handThing != null) = {handThing != null}");
#endif

                switch(handThing.Kind)
                {
                    case KindOfHandThing.Rifle:
                        TakeRifle(cancellationToken, handThing as IRifle);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(handThing.Kind), handThing.Kind, null);
                }
            });

#if DEBUG
            UnityEngine.Debug.Log($"TakeImpl End {methodId}");
#endif
        }

        private void TakeRifle(CancellationToken cancellationToken, IRifle rifle)
        {
            _rifle = rifle;

            _hasRifle = true;
            UpdateAnimator();

            rifle.SetToHandsOfHumanoid(this);

            AddToManualControl(_rifle.USocGameObject, new List<DeviceOfBiped>() {  DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand });

            AddHoldFact(rifle.IdForFacts);
        }

        [DebuggerHidden]
        [BipedEndpoint("Start Fire", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void StartFireImpl(CancellationToken cancellationToken)
        {
            if (_rifle == null)
            {
                return;
            }

            _rifle.StartFire(cancellationToken);
        }

        [DebuggerHidden]
        [BipedEndpoint("Stop Fire", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void StopFireImpl(CancellationToken cancellationToken)
        {
            if(_rifle == null)
            {
                return;
            }

            _rifle.StopFire();
        }

        [DebuggerHidden]
        [BipedEndpoint("Ready For Fire", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void ReadyForFireImpl(CancellationToken cancellationToken)
        {
#if DEBUG
            var methodId = GetMethodId();

            UnityEngine.Debug.Log($"ReadyForFireImpl Begin {methodId}");
#endif

            RunInMainThread(() => { 
                _isAim = true;
                UpdateAnimator();
            });

#if DEBUG
            UnityEngine.Debug.Log($"ReadyForFireImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Unready For Fire", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void UnReadyForFireImpl(CancellationToken cancellationToken)
        {
#if DEBUG
            var methodId = GetMethodId();

            UnityEngine.Debug.Log($"UnReadyForFireImpl Begin {methodId}");
#endif

            RunInMainThread(() => {
                _enableRifleIK = false;
                _isAim = false;
                UpdateAnimator();
            });

#if DEBUG
            UnityEngine.Debug.Log($"UnReadyForFireImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Throw out", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void ThrowOutImpl(CancellationToken cancellationToken)
        {
#if DEBUG
            var methodId = GetMethodId();

            UnityEngine.Debug.Log($"ThrowOutImpl Begin {methodId}");
#endif

            if (_rifle != null)
            {
                ThrowOutRifle(cancellationToken);
                return;
            }
        }

        public void ThrowOutRifle(CancellationToken cancellationToken)
        {
#if DEBUG
            UnityEngine.Debug.Log($"ThrowOutRifle");
#endif

            RunInMainThread(() => {
                _enableRifleIK = false;

                _rifle.ThrowOut();

                _isAim = false;
                _hasRifle = false;

                UpdateAnimator();
            });

            RemoveFromManualControl(_rifle.USocGameObject);

            RemoveHoldFact();
        }

        [DebuggerHidden]
        [BipedEndpoint("Aim to", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void AimToImpl(CancellationToken cancellationToken, IEntity entity)
        {
#if DEBUG
            var methodId = GetMethodId();

            UnityEngine.Debug.Log($"AimToImpl Begin {methodId}");
#endif

            entity.Specify(EntityConstraints.OnlyVisible);

            entity.Resolve();

#if DEBUG
            UnityEngine.Debug.Log($"AimToImpl {methodId} entity.InstanceId = {entity.InstanceId}");
            UnityEngine.Debug.Log($"AimToImpl {methodId} entity.Id = {entity.Id}");
            UnityEngine.Debug.Log($"AimToImpl {methodId} entity.Position = {entity.Position}");
#endif

            RunInMainThread(() => {
                var targetGameObject = GameObjectsRegistry.GetGameObject(entity.InstanceId);

                _enableRifleIK = true;

                _rifle.LookAt(targetGameObject.transform);
            });                
        }
    }
}
