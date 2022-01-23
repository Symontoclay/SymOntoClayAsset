using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExamplesOfSymOntoClay.Internal
{
    [Serializable]
    public class MyMouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;

        private Quaternion mCharacterTargetRot;
        private Quaternion mCameraTargetRot;
        private IPlayerCommonBus _playerCommonBus;

        public void Init(Transform character, Transform camera, IPlayerCommonBus playerCommonBus)
        {
            mCharacterTargetRot = character.localRotation;
            mCameraTargetRot = camera.localRotation;
            _playerCommonBus = playerCommonBus;
            _playerCommonBus.SetCharacterMode();
        }

        public void LookRotation(Transform character, Transform camera)
        {
            var yRot = _playerCommonBus.GetAxis("Mouse X") * XSensitivity;
            var xRot = _playerCommonBus.GetAxis("Mouse Y") * YSensitivity;

            mCharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            mCameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
            {
                mCameraTargetRot = ClampRotationAroundXAxis(mCameraTargetRot);
            }

            if (smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, mCharacterTargetRot, smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, mCameraTargetRot, smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = mCharacterTargetRot;
                camera.localRotation = mCameraTargetRot;
            }
        }

        private Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            var angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
    }
}
