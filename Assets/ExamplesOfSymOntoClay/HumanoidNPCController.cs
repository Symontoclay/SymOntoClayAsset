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

namespace ExamplesOfSymOntoClay
{
    [RequireComponent(typeof(IUHumanoidNPC))]
    public class HumanoidNPCController : BaseBehavior
    {
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

        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private Rigidbody _rigidbody;

        private bool _hasRifle;
        private bool _isWalking;
        private bool _isAim;
        private bool _isDead;

        private Vector3 _position;

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
        [BipedEndpoint("Take", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void TakeImpl(CancellationToken cancellationToken, IEntity entity)
        {
#if DEBUG
            var name = GetMethodId();

            UnityEngine.Debug.Log($"Begin {name}");
#endif

            entity.Specify(EntityConstraints.CanBeTaken/*, EntityConstraints.OnlyVisible, EntityConstraints.Nearest*/);

#if DEBUG
            UnityEngine.Debug.Log($"{name} entity.InstanceId = {entity.InstanceId}");
            UnityEngine.Debug.Log($"{name} entity.Id = {entity.Id}");
            UnityEngine.Debug.Log($"{name} entity.Position = {entity.Position}");
#endif

#if DEBUG
            UnityEngine.Debug.Log($"End {name}");
#endif
        }
    }
}
