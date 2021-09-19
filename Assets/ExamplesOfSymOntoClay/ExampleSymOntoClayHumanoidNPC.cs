using UnityEngine;
using System.Collections;
using SymOntoClay;
using System.Threading;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Helpers;
using UnityEngine.AI;
using System.Diagnostics;

[RequireComponent(typeof(IUHumanoidNPC))]
public class ExampleSymOntoClayHumanoidNPC : MonoBehaviour, IUHostListener
{
    private IUHumanoidNPC _uHumanoidNPC;
    private IHumanoidNPC _npc;

    void Awake()
    {
#if DEBUG
        //Debug.Log("ExampleSymOntoClayHumanoidNPC Awake");
#endif

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    // Use this for initialization
    void Start()
    {
#if DEBUG
        //Debug.Log("ExampleSymOntoClayHumanoidNPC Start Begin");
#endif

        _uHumanoidNPC = GetComponent<IUHumanoidNPC>();
        _npc = _uHumanoidNPC.NPC;
        _idForFacts = _uHumanoidNPC.IdForFacts;

        AddStopFact();

#if DEBUG
        //Debug.Log("ExampleSymOntoClayHumanoidNPC Start End");
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead)
        {
            return;
        }

        if (_isWalking)
        {
            var nextPosition = _navMeshAgent.nextPosition;
            if (_targetPosition.x == nextPosition.x && _targetPosition.z == nextPosition.z)
            {
                PerformStop();

                lock (_lockObj)
                {
                    lock (_walkingRepresentative)
                    {
                        _walkingRepresentative.IsFinished = true;
                    }

                    _walkingRepresentative = null;
                }

#if DEBUG
                //Debug.Log("HumanoidNPC Update Walking has been stoped.");
#endif
            }
        }
    }

    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private Rigidbody _rigidbody;

    private object _lockObj = new object();

    private string _idForFacts;

    private bool _hasRifle;
    private bool _isWalking;
    private bool _isAim;
    private bool _isDead;

    private string _walkingFactId;

    private ActionRepresentative _walkingRepresentative;

    private Vector3 _targetPosition;

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

    private void AddStopFact()
    {
#if DEBUG
        var factStr = $"act({_idForFacts}, stop)";
        //Debug.Log($"ExampleSymOntoClayHumanoidNPC AddStopFact factStr = '{factStr}'");
#endif

        _npc.RemovePublicFact(_walkingFactId);
        _walkingFactId = _npc.InsertPublicFact($"act({_idForFacts}, stop)");

#if DEBUG
        //Debug.Log($"ExampleSymOntoClayHumanoidNPC AddStopFact _walkingFactId = {_walkingFactId}");
#endif
    }

    private void AddWalkingFact()
    {
#if DEBUG
        var factStr = $"act({_idForFacts}, walk)";
        //Debug.Log($"ExampleSymOntoClayHumanoidNPC AddWalkingFact factStr = '{factStr}'");
#endif

        _npc.RemovePublicFact(_walkingFactId);
        _walkingFactId = _npc.InsertPublicFact($"act({_idForFacts}, walk)");

#if DEBUG
        //Debug.Log($"ExampleSymOntoClayHumanoidNPC AddWalkingFact _walkingFactId = {_walkingFactId}");
#endif
    }

    private static int _methodId;

    private int GetMethodId()
    {
        lock (_lockObj)
        {
            _methodId++;
            return _methodId;
        }
    }

    [DebuggerHidden]
    [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
    public void GoToImpl(CancellationToken cancellationToken,
    [EndpointParam("To", KindOfEndpointParam.Position)] Vector3 point,
    float speed = 12)
    {
        var methodId = GetMethodId();

#if DEBUG
        UnityEngine.Debug.Log($"ExampleSymOntoClayHumanoidNPC GoToImpl [{methodId}] point = {point}");
#endif
        AddWalkingFact();

        var representative = new ActionRepresentative();

        _npc.RunInMainThread(() => {
            _targetPosition = point;
            _navMeshAgent.SetDestination(point);
            _isWalking = true;
            UpdateAnimator();

            lock (_lockObj)
            {
                _walkingRepresentative = representative;
            }
        });

#if DEBUG
        UnityEngine.Debug.Log($"ExampleSymOntoClayHumanoidNPC GoToImpl [{methodId}] Walking has been started.");
#endif

        while (true)
        {
            lock (representative)
            {
                if (representative.IsFinished)
                {
                    break;
                }

#if DEBUG
                UnityEngine.Debug.Log($"ExampleSymOntoClayHumanoidNPC GoToImpl [{methodId}] cancellationToken.IsCancellationRequested = {cancellationToken.IsCancellationRequested}");
#endif

                if(cancellationToken.IsCancellationRequested)
                {
                    _npc.RunInMainThread(() => {
                        PerformStop();

                        //lock (_lockObj)//this block frozes Unity! Fix It!!!
                        //{
                        //    lock (_walkingRepresentative)
                        //    {
                        //        _walkingRepresentative.IsFinished = true;
                        //    }

                        //    _walkingRepresentative = null;
                        //}
                    });
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            Thread.Sleep(100);
        }

#if DEBUG
        UnityEngine.Debug.Log($"ExampleSymOntoClayHumanoidNPC GoToImpl [{methodId}] Walking has been stoped.");
#endif
    }
}
