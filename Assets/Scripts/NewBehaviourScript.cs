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
    }

    // Update is called once per frame
    void Update()
    {
        mInputKeyHelper.Update();
    }

    private void OnFPressAction()
    {
        Debug.Log("OnFPressAction");
    }
}
