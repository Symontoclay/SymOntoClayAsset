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

        public float EffectsDisplayTime = 0.1f;
        public float TimeBetweenBullets = 0.1f;

        public int DamagePerShot = 20;
        public float DamageDistance = 400f;        

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

        //void Update()
        //{

        //}

        private bool _isTaken;

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

        private enum InternalStateOfRapidFireGun
        {
            TurnedOf,
            TurnedOnShot,
            TurnedOnWasShot,
            BeforeOffIfSingle
        }

        public void StartFire(CancellationToken cancellationToken)
        {
#if DEBUG
            var methodId = GetMethodId();

            UnityEngine.Debug.Log($"StartFire Begin {methodId}");
#endif

            var timer = 0f;
            var state = InternalStateOfRapidFireGun.TurnedOf;

            //RunInMainThread(() =>
            //{
                //mGunAudio.Play();
            //});            

            while (true)
            {
#if DEBUG
                //UnityEngine.Debug.Log($"StartFire {methodId} cancellationToken.IsCancellationRequested = {cancellationToken.IsCancellationRequested}");
#endif

                if (cancellationToken.IsCancellationRequested)
                {
                    RunInMainThread(() =>
                    {
                        //mGunAudio.Stop();
                        DisableEffects();
                    });

                    break;
                }

#if DEBUG
                //UnityEngine.Debug.Log($"StartFire {methodId} state = {state}; timer = {timer}");
#endif

                switch (state)
                {
                    case InternalStateOfRapidFireGun.TurnedOf:
                        timer = 0f;
                        RunInMainThread(() => { ProcessShoot(); });
                        state = InternalStateOfRapidFireGun.TurnedOnShot;
                        break;

                    case InternalStateOfRapidFireGun.TurnedOnShot:
#if DEBUG
                        //UnityEngine.Debug.Log($"StartFire {methodId} timer (2) = {timer}");
#endif

                        RunInMainThread(() =>
                        {
                            timer += Time.deltaTime;
                            if (timer >= EffectsDisplayTime)
                            {
                                timer = 0f;
                                DisableEffects();
                                state = InternalStateOfRapidFireGun.TurnedOnWasShot;
                            }
                        });
                        break;

                    case InternalStateOfRapidFireGun.TurnedOnWasShot:
                        RunInMainThread(() => {
                            timer += Time.deltaTime;
                            if (timer >= TimeBetweenBullets)
                            {
                                ProcessShoot();
                                state = InternalStateOfRapidFireGun.TurnedOnShot;
                            }
                        });
                        break;

                    default: 
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }

                //Thread.Sleep(10);
            }

#if DEBUG
            UnityEngine.Debug.Log($"StartFireImpl End {methodId}");
#endif
        }

        private void ProcessShoot()
        {
#if DEBUG
            //UnityEngine.Debug.Log($"Begin ProcessShoot");
#endif

            mGunAudio.Play();

            // Turn on light
            mGunLight.enabled = true;
            //FaceLight.enabled = true;

            mGunParticles.Stop();
            mGunParticles.Play();

            // Fire line
            //mGunLine.enabled = true;
            //mGunLine.SetPosition(0, mGunEndTransform.position);
            //Ray settings

            var shootRay = new Ray();

            shootRay.origin = mGunEndTransform.position;
            shootRay.direction = mGunEndTransform.forward;

            RaycastHit shootHit;

            if (Physics.Raycast(shootRay, out shootHit, DamageDistance))
            {
#if DEBUG
                //UnityEngine.Debug.Log($"ProcessShoot Hit");
#endif

                var targetOfShoot = shootHit.collider.GetComponentInParent<ITargetOfDamage>();

#if UNITY_EDITOR
                //UnityEngine.Debug.Log($"ProcessShoot targetOfShoot == null = {targetOfShoot == null}");
#endif

                if (targetOfShoot != null)
                {
                    targetOfShoot.SetHit(shootHit, DamagePerShot);
                }
            }

#if DEBUG
            //UnityEngine.Debug.Log($"End ProcessShoot");
#endif
        }

        private void DisableEffects()
        {
#if DEBUG
            //UnityEngine.Debug.Log($"Begin DisableEffects");
#endif

            mGunLight.enabled = false;
            mGunParticles.Stop();
            mGunAudio.Stop();

#if DEBUG
            //UnityEngine.Debug.Log($"End DisableEffects");
#endif
        }

        public void StopFire()
        {
            RunInMainThread(() =>
            {
                DisableEffects();
            });
        }

        public void LookAt(Transform target)
        {
            transform.LookAt(target);
        }

        public bool ThrowOut()
        {
            if (transform.parent == null)
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log("transform.parent == null");
#endif

                return true;
            }

            transform.SetParent(null);
            //gameObject.SetActive(false);

            if (mBodyCollider != null)
            {
                mBodyCollider.enabled = true;
            }

            if (mBodyRigidbody != null)
            {
                mBodyRigidbody.isKinematic = false;
                mBodyRigidbody.AddForce(UnityEngine.Random.insideUnitSphere * 200);
            }

            _isTaken = false;

            return true;
        }
    }
}
