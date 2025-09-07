using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Samples.Environment;
using SymOntoClay.UnityAsset.Samples.Interfaces;
using SymOntoClay.UnityAsset.Samples.Internal;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewBehaviourScript : MonoBehaviour, IBipedHumanoidCustomBehavior
{
    private Rigidbody mRigidbody;
    private Animator mAnimator;
    private NavMeshAgent mNavMeshAgent;

    private IPlayerCommonBus _playerCommonBus;

    private InputKeyHelper mInputKeyHelper;

    public GameObject Gun;
    public GameObject Aim;

    public GameObject Head;

    public GameObject RightHandWP;
    public GameObject LeftHandWP;

    GameObject IBipedHumanoidCustomBehavior.RightHandWP => RightHandWP;
    GameObject IBipedHumanoidCustomBehavior.LeftHandWP => LeftHandWP;

    private IRifleCustomBehavior _twoHandGun;

    public List<GameObject> Backpack;

    // Start is called before the first frame update
    void Start()
    {
        _twoHandGun = Gun?.GetComponent<IRifleCustomBehavior>();

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
        //mInputKeyHelper.AddPressListener(KeyCode.L, OnLPressAction);
        //mInputKeyHelper.AddPressListener(KeyCode.K, OnKPressAction);
    }

    // Update is called once per frame
    void Update()
    {
        mInputKeyHelper.Update();
    }

    public void Take(IMonitorLogger logger, IHandThingCustomBehavior handThing)
    {
    }

    private void OnFPressAction()
    {
        Debug.Log("OnFPressAction");

        mAnimator.SetBool("hasRifle", true);

        _twoHandGun.SetToHandsOfHumanoid(null, this);
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
    private bool _enableRotateHeadIK;

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

        _twoHandGun.LookAt(null, Aim);
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (_enableTwoHandGunIK)
        {
            mAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.3f);
            mAnimator.SetIKPosition(AvatarIKGoal.LeftHand, _twoHandGun.AddWP.transform.position);
        }

        if(_enableRotateHeadIK)
        {
            mAnimator.SetLookAtWeight(1);
            mAnimator.SetLookAtPosition(mCurrentHeadPosition);
            Head.transform.LookAt(mCurrentHeadPosition);
        }
    }

    private float mCurrentHeadAngle = 30f;
    private Vector3 mCurrentHeadPosition;

    private void OnLPressAction()
    {
        Debug.Log("OnLPressAction");

        var radAngle = mCurrentHeadAngle * Mathf.Deg2Rad;
        var x = Mathf.Sin(radAngle);
        var y = Mathf.Cos(radAngle);
        var localDirection = new Vector3(x, 0f, y);
        var globalDirection = transform.TransformDirection(localDirection);
        var oldY = Head.transform.position.y;

        var newPosition = globalDirection + transform.position;
        mCurrentHeadPosition = new Vector3(newPosition.x, oldY, newPosition.z);

        _enableRotateHeadIK = true;
    }

    private void OnKPressAction()
    {
        Debug.Log("OnKPressAction");
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
