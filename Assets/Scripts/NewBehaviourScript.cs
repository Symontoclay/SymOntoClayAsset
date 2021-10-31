using ExamplesOfSymOntoClay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewBehaviourScript : MonoBehaviour, IUBipedHumanoid
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

    GameObject IUBipedHumanoid.RightHandWP => RightHandWP;
    GameObject IUBipedHumanoid.LeftHandWP => LeftHandWP;

    private IRifle _twoHandGun;

    // Start is called before the first frame update
    void Start()
    {
        _twoHandGun = Gun?.GetComponent<IRifle>();

        _playerCommonBus = PlayerCommonBus.GetBus();

        mAnimator = GetComponent<Animator>();
        mRigidbody = GetComponent<Rigidbody>();
        mRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        mNavMeshAgent = GetComponent<NavMeshAgent>();

        mInputKeyHelper = new InputKeyHelper(_playerCommonBus);
        //mInputKeyHelper.AddPressListener(KeyCode.F, OnFPressAction);
        //mInputKeyHelper.AddPressListener(KeyCode.G, OnGPressAction);
        //mInputKeyHelper.AddPressListener(KeyCode.H, OnHPressAction);
        //mInputKeyHelper.AddPressListener(KeyCode.J, OnJPressAction);
        //mInputKeyHelper.AddPressListener(KeyCode.E, OnEPressAction);
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

        _twoHandGun.SetToHandsOfHumanoid(this);
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

    private bool _enableTwoHandGunIK;

    private void OnJPressAction()
    {
        Debug.Log("OnJPressAction");

        var m4A1 = Gun;

        _enableTwoHandGunIK = true;

        //var gunForvard = m4A1.transform.forward;

        //var targetPosition = Aim.transform.position;

        //var targetDirection = targetPosition - m4A1.transform.position;

        //var towards = Quaternion.FromToRotation(gunForvard, targetDirection);

        //m4A1.transform.rotation = towards * m4A1.transform.rotation;

        _twoHandGun.LookAt(Aim.transform);
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (!_enableTwoHandGunIK)
        {
            return;
        }

        mAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.3f);
        mAnimator.SetIKPosition(AvatarIKGoal.LeftHand, _twoHandGun.AddWP.transform.position);
    }

    private void OnEPressAction()
    {
        Debug.Log("OnEPressAction");

        var aimPosition = Aim.transform.position;

        var myPosition = transform.position;

#if UNITY_EDITOR
        Debug.Log($"aimPosition = {aimPosition}");
        Debug.Log($"myPosition = {myPosition}");
#endif

        var heading = aimPosition - myPosition;

#if UNITY_EDITOR
        Debug.Log($"heading = {heading}");
#endif

        var distance = heading.magnitude;

        var direction = heading / distance;

#if UNITY_EDITOR
        Debug.Log($"distance = {distance}");
        Debug.Log($"direction = {direction}");
#endif

        var rotation = Quaternion.FromToRotation(Vector3.forward, direction);

#if UNITY_EDITOR
        Debug.Log($"rotation = {rotation}");
#endif

        var agle = Quaternion.Angle(transform.rotation, rotation);

#if UNITY_EDITOR
        Debug.Log($"agle = {agle}");
#endif
    }
}
