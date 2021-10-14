using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class TstRapidFireGun : MonoBehaviour
    {
        private Collider mBodyCollider;
        private Rigidbody mBodyRigidbody;

        public GameObject MainWP;
        public GameObject AddWP;

        void Start()
        {
            mBodyCollider = GetComponentInChildren<Collider>();
            mBodyRigidbody = GetComponentInChildren<Rigidbody>();
        }

        void Update()
        {

        }

        public bool SetToHandsOfHumanoid(NewBehaviourScript humanoid)
        {
            var targetParent = humanoid.RightHandWP.transform;

            if (transform.parent == targetParent)
            {
#if UNITY_EDITOR
                Debug.Log("transform.parent == targetParent");
#endif

                return true;
            }

            if (mBodyCollider != null)
            {
                mBodyCollider.enabled = false;
            }

            if (mBodyRigidbody != null)
            {
                mBodyRigidbody.isKinematic = true;
            }

            var heading = targetParent.position - humanoid.LeftHandWP.transform.position;

#if UNITY_EDITOR
            Debug.Log($"targetParent.position = {targetParent.position}");
            Debug.Log($"humanoid.LeftHandWP.transform.position = {humanoid.LeftHandWP.transform.position}");
            Debug.Log($"heading = {heading}");
#endif

            var distance = heading.magnitude;

            var direction = heading / distance;

#if UNITY_EDITOR
            Debug.Log($"distance = {distance}");
            Debug.Log($"direction = {direction}");
#endif

            transform.rotation = Quaternion.Euler(0, 0, 0);

            transform.SetParent(targetParent, false);

            //transform.localRotation = Quaternion.Euler(-90, -183.234f, 0);
            //transform.localRotation = Quaternion.Euler(0, 180.234f, 0);
            //transform.rotation = targetParent.rotation;
            //transform.localPosition = new Vector3(0, 0, 0.2f);
            //transform.localPosition = new Vector3(0, 0.06f, 0.08f);
            transform.localPosition = new Vector3(0, 0, 0);

            //transform.LookAt(humanoid.LeftHandWP.transform);

            gameObject.SetActive(true);

#if UNITY_EDITOR
            Debug.Log("SetToHandsOfHumanoid ^)");
#endif

            return true;
        }

        public bool SetToHandsOfHumanoid_2(NewBehaviourScript humanoid)
        {
            var targetParent = humanoid.RightHandWP.transform;

            if (transform.parent == targetParent)
            {
#if UNITY_EDITOR
                Debug.Log("transform.parent == targetParent");
#endif

                return true;
            }

            if (mBodyCollider != null)
            {
                mBodyCollider.enabled = false;
            }

            if (mBodyRigidbody != null)
            {
                mBodyRigidbody.isKinematic = true;
            }



#if UNITY_EDITOR
            Debug.Log("SetToHandsOfHumanoid_2 ^)");
#endif

            return true;
        }
    }
}
