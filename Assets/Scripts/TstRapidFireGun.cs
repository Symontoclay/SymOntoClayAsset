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

            transform.rotation = Quaternion.Euler(0, 0, 0);

            transform.SetParent(targetParent, false);

            transform.localPosition = new Vector3(0, 0, 0);

            gameObject.SetActive(true);

#if UNITY_EDITOR
            Debug.Log("SetToHandsOfHumanoid ^)");
#endif

            return true;
        }
    }
}
