using Assets.Scripts;
using ExamplesOfSymOntoClay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewBehaviourScript : MonoBehaviour
{
    private Rigidbody mRigidbody;
    private Animator mAnimator;
    private NavMeshAgent mNavMeshAgent;

    private IPlayerCommonBus _playerCommonBus;

    private InputKeyHelper mInputKeyHelper;

    public GameObject Gun;

    public GameObject RightHandWP;
    public GameObject LeftHandWP;

    // Start is called before the first frame update
    void Start()
    {
        _playerCommonBus = PlayerCommonBus.GetBus();

        mAnimator = GetComponent<Animator>();
        mRigidbody = GetComponent<Rigidbody>();
        mRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        mNavMeshAgent = GetComponent<NavMeshAgent>();

        mInputKeyHelper = new InputKeyHelper(_playerCommonBus);
        mInputKeyHelper.AddPressListener(KeyCode.F, OnFPressAction);
        mInputKeyHelper.AddPressListener(KeyCode.G, OnGPressAction);
        mInputKeyHelper.AddPressListener(KeyCode.H, OnHPressAction);
    }

    // Update is called once per frame
    void Update()
    {
        mInputKeyHelper.Update();
    }

    private void OnFPressAction()
    {
        Debug.Log("OnFPressAction");

        mAnimator.SetBool("hasRifle", true);

        var m4A1 = Gun;

        var gunComponent = m4A1.GetComponent<TstRapidFireGun>();

        gunComponent.SetToHandsOfHumanoid(this);
    }

    private void OnGPressAction()
    {
        Debug.Log("OnGPressAction");

        mAnimator.SetBool("isAim", true);
    }

    private void OnHPressAction()
    {
        Debug.Log("OnHPressAction");

        mAnimator.SetBool("isAim", false);
    }
}
