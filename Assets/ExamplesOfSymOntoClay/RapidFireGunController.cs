using ExamplesOfSymOntoClay;
using SymOntoClay;
using SymOntoClay.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExamplesOfSymOntoClay
{
    public class RapidFireGunController : BaseBehavior, IUTwoHandGun
    {
        private Collider mBodyCollider;
        private Rigidbody mBodyRigidbody;

        public GameObject MainWP;
        public GameObject AddWP;

        GameObject IUTwoHandGun.MainWP => MainWP;
        GameObject IUTwoHandGun.AddWP => AddWP;

        void Start()
        {
            mBodyCollider = GetComponentInChildren<Collider>();
            mBodyRigidbody = GetComponentInChildren<Rigidbody>();
        }

        void Update()
        {

        }

        public override bool CanBeTakenBy(IEntity subject)
        {
#if UNITY_EDITOR
            Debug.Log("RapidFireGunController CanBeTakenBy");
#endif

            return true;//tmp
        }

        public bool SetToHandsOfHumanoid(IUBipedHumanoid humanoid)
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

        public void LookAt(Transform target)
        {
            transform.LookAt(target);
        }
    }
}
