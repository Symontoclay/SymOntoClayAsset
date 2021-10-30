using ExamplesOfSymOntoClay;
using SymOntoClay;
using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ExamplesOfSymOntoClay
{
    public class RapidFireGunController : BaseBehavior, IRifle
    {
        public KindOfHandThing Kind => KindOfHandThing.Rifle;

        private Collider mBodyCollider;
        private Rigidbody mBodyRigidbody;

        public GameObject MainWP;
        public GameObject AddWP;

        GameObject IRifle.MainWP => MainWP;
        GameObject IRifle.AddWP => AddWP;

        public GameObject GunEnd;

        private GameObject _gunEnd;

        IUSocGameObject IHandThing.USocGameObject => _uSocGameObject;

        private Light mGunLight;
        private ParticleSystem mGunParticles;
        private AudioSource mGunAudio;
        private Transform mGunEndTransform;

        protected override void Start()
        {
            base.Start();

            mBodyCollider = GetComponentInChildren<Collider>();
            mBodyRigidbody = GetComponentInChildren<Rigidbody>();
            mGunLight = GetComponentInChildren<Light>();
            mGunParticles = GetComponentInChildren<ParticleSystem>();
            mGunAudio = GetComponentInChildren<AudioSource>();

            if (GunEnd == null)
            {
                var locator = GetComponentInChildren<GunEndLocator>();
                _gunEnd = locator.gameObject;
            }
            else
            {
                _gunEnd = GunEnd;
            }

            mGunEndTransform = _gunEnd.transform;
        }

        void Update()
        {

        }

        private bool _isTaken;
        private bool _isOn;

        public override bool CanBeTakenBy(IEntity subject)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log("RapidFireGunController CanBeTakenBy");
#endif

            return !_isTaken;
        }

        public bool SetToHandsOfHumanoid(IUBipedHumanoid humanoid)
        {
            var targetParent = humanoid.RightHandWP.transform;

            if (transform.parent == targetParent)
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log("transform.parent == targetParent");
#endif

                return true;
            }

            _isTaken = true;

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
            UnityEngine.Debug.Log("SetToHandsOfHumanoid ^)");
#endif

            return true;
        }

        [DebuggerHidden]
        [BipedEndpoint("Start Fire", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void StartFireImpl(CancellationToken cancellationToken)
        {
#if DEBUG
            var name = GetMethodId();

            UnityEngine.Debug.Log($"StartFireImpl Begin {name}");
#endif

            _isOn = true;

            var timer = 0f;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    RunInMainThread(() =>
                    {
                        DisableEffects();
                    });

                    break;
                }
                


                Thread.Sleep(10);
            }

#if DEBUG
            UnityEngine.Debug.Log($"StartFireImpl End {name}");
#endif
        }

        private void DisableEffects()
        {
            mGunLight.enabled = false;
            mGunParticles.Stop();
            mGunAudio.Stop();
        }

        public void LookAt(Transform target)
        {
            transform.LookAt(target);
        }
    }
}
