using SymOntoClay.UnityAsset.Samles.Internal;
using SymOntoClay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SymOntoClay.UnityAsset.BaseBehaviors;
using SymOntoClay.UnityAsset.Samles.Environment;
using SymOntoClay.UnityAsset.Samles.Interfaces;

namespace SymOntoClay.UnityAsset.Samles.Behavior
{
    [AddComponentMenu("SymOntoClaySamles/FirstPersonController")]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class FirstPersonController : BaseBehavior
    {
        public Camera cam;
        public MyMovementSettings movementSettings = new MyMovementSettings();
        public MyMouseLook mouseLook = new MyMouseLook();
        public MyAdvancedSettings advancedSettings = new MyAdvancedSettings();

        private Rigidbody mRigidBody;
        private CapsuleCollider mCapsule;
        private float mYRotation;
        private Vector3 mGroundContactNormal { get; set; }
        private bool mJump, mPreviouslyGrounded, mJumping, mIsGrounded;
        public Vector3 Velocity
        {
            get { return mRigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return mIsGrounded; }
        }

        public bool Jumping
        {
            get { return mJumping; }
        }

        public bool Running
        {
            get
            {
                return movementSettings.Running;
            }
        }

        protected override void Start()
        {
            base.Start();

            _playerCommonBus = PlayerCommonBus.GetBus();
            mRigidBody = GetComponent<Rigidbody>();
            mCapsule = GetComponent<CapsuleCollider>();
            mouseLook.Init(transform, cam.transform, _playerCommonBus);
        }

        private IPlayerCommonBus _playerCommonBus;

        private void Update()
        {
            RotateView();

            if (_playerCommonBus.GetButtonDown("Jump") && !mJump)
            {
                mJump = true;
            }
        }

        private void FixedUpdate()
        {
            GroundCheck();
            var input = GetInput();

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || mIsGrounded))
            {
                // always move along the camera forward as it is the direction that it being aimed at
                var desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, mGroundContactNormal).normalized;

                desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed;
                desiredMove.z = desiredMove.z * movementSettings.CurrentTargetSpeed;
                desiredMove.y = desiredMove.y * movementSettings.CurrentTargetSpeed;
                if (mRigidBody.velocity.sqrMagnitude < (movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed))
                {
                    mRigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                }
            }

            if (mIsGrounded)
            {
                mRigidBody.drag = 5f;

                if (mJump)
                {
                    mRigidBody.drag = 0f;
                    mRigidBody.velocity = new Vector3(mRigidBody.velocity.x, 0f, mRigidBody.velocity.z);
                    mRigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    mJumping = true;
                }

                if (!mJumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && mRigidBody.velocity.magnitude < 1f)
                {
                    mRigidBody.Sleep();
                }
            }
            else
            {
                mRigidBody.drag = 0f;
                if (mPreviouslyGrounded && !mJumping)
                {
                    StickToGroundHelper();
                }
            }
            mJump = false;
        }

        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(mGroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }

        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, mCapsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((mCapsule.height / 2f) - mCapsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    mRigidBody.velocity = Vector3.ProjectOnPlane(mRigidBody.velocity, hitInfo.normal);
                }
            }
        }

        private enum MovementState
        {
            Unknown,
            Stops,
            Walks,
            Runs
        }

        private MovementState _movementState;

        private Vector2 GetInput()
        {
            var input = new Vector2
            {
                x = _playerCommonBus.GetAxis("Horizontal"),
                y = _playerCommonBus.GetAxis("Vertical")
            };
            movementSettings.UpdateDesiredTargetSpeed(input);

            if(input == Vector2.zero)
            {
                if(_movementState != MovementState.Stops)
                {
                    _movementState = MovementState.Stops;

                    AddStopFact();
                    StopRepeatingStepsSoundInMainThread();
                }
            }
            else
            {
                if(movementSettings.Running)
                {
                    if(_movementState != MovementState.Runs)
                    {
                        _movementState = MovementState.Runs;

                        AddRunningFact();
                        StartRepeatingRunningStepsSoundInMainThread();
                    }
                }
                else
                {
                    if(_movementState != MovementState.Walks)
                    {
                        _movementState = MovementState.Walks;

                        AddWalkingFact();
                        StartRepeatingWalkingStepsSoundInMainThread();
                    }
                }
            }

            return input;//if (0.0, 0.0) player stands otherwise walks.
        }

        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon)
            {
                return;
            }

            // get the rotation before it's changed
            var oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation(transform, cam.transform);

            if (mIsGrounded || advancedSettings.airControl)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                var velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                mRigidBody.velocity = velRotation * mRigidBody.velocity;
            }
        }

        private void GroundCheck()
        {
            mPreviouslyGrounded = mIsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, mCapsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((mCapsule.height / 2f) - mCapsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                mIsGrounded = true;
                mGroundContactNormal = hitInfo.normal;
            }
            else
            {
                mIsGrounded = false;
                mGroundContactNormal = Vector3.up;
            }
            if (!mPreviouslyGrounded && mIsGrounded && mJumping)
            {
                mJumping = false;
            }
        }
    }
}
