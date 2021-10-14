using Assets.Scripts;
using ExamplesOfSymOntoClay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewBehaviourScript : MonoBehaviour
{
    private Rigidbody mRigidbody;
    public Animator mAnimator;
    private NavMeshAgent mNavMeshAgent;

    private IPlayerCommonBus _playerCommonBus;

    private InputKeyHelper mInputKeyHelper;

    public GameObject Gun;
    public GameObject Aim;

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
        mInputKeyHelper.AddPressListener(KeyCode.R, OnRPressAction);
        mInputKeyHelper.AddPressListener(KeyCode.G, OnGPressAction);
        mInputKeyHelper.AddPressListener(KeyCode.H, OnHPressAction);
        mInputKeyHelper.AddPressListener(KeyCode.J, OnJPressAction);
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

    private void OnRPressAction()
    {
        Debug.Log("OnRPressAction");

        mAnimator.SetBool("hasRifle", true);

        var m4A1 = Gun;

        var gunComponent = m4A1.GetComponent<TstRapidFireGun>();

        gunComponent.SetToHandsOfHumanoid_2(this);
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

    private bool _enableIK;

    private void OnJPressAction()
    {
        Debug.Log("OnJPressAction");

        var m4A1 = Gun;

        _enableIK = true;

        //var gunForvard = m4A1.transform.forward;

        //var targetPosition = Aim.transform.position;

        //var targetDirection = targetPosition - m4A1.transform.position;

        //var towards = Quaternion.FromToRotation(gunForvard, targetDirection);

        //m4A1.transform.rotation = towards * m4A1.transform.rotation;

        m4A1.transform.LookAt(Aim.transform);
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (!_enableIK)
        {
            return;
        }

        var m4A1 = Gun;

        var gunComponent = m4A1.GetComponent<TstRapidFireGun>();

        mAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.3f);
        mAnimator.SetIKPosition(AvatarIKGoal.LeftHand, gunComponent.AddWP.transform.position);
    }
}
