using SymOntoClay;
using SymOntoClay.Core;
using SymOntoClay.UnityAsset.BaseBehaviors;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Interfaces;
using SymOntoClay.UnityAsset.Samles.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Samles.Behavior
{
    public class RapidFireGunController : BaseBehavior, IRifle
    {
        private object _lockObl = new object();

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

        private bool _isFired;

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
            return !_isTaken;
        }

        public bool SetToHandsOfHumanoid(IUBipedHumanoid humanoid)
        {
            var targetParent = humanoid.RightHandWP.transform;

            if (transform.parent == targetParent)
            {
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

            return true;
        }

        public void HideForBackpackInMainThread()
        {
            Task.Run(() => {
                StopFire();
            });

            transform.SetParent(null);
            gameObject.SetActive(false);
        }

        public void HideForBackpackInUsualThread()
        {
            StopFire();

            RunInMainThread(() => {
                transform.SetParent(null);
                gameObject.SetActive(false);
            });
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
            lock (_lockObl)
            {
                if (_isFired)
                {
                    return;
                }

                _isFired = true;
            }

            var timer = 0f;
            var state = InternalStateOfRapidFireGun.TurnedOf;

            //RunInMainThread(() =>
            //{
            //mGunAudio.Play();
            //});            

            StartRepeatingShotSoundInUsualThread();

            while (true)
            {
                if (cancellationToken.IsCancellationRequested || !_isFired)
                {
                    RunInMainThread(() =>
                    {
                        //mGunAudio.Stop();
                        DisableEffects();
                        StopRepeatingShotSoundInMainThread();
                    });

                    break;
                }

                switch (state)
                {
                    case InternalStateOfRapidFireGun.TurnedOf:
                        timer = 0f;
                        RunInMainThread(() => { ProcessShoot(); });
                        state = InternalStateOfRapidFireGun.TurnedOnShot;
                        break;

                    case InternalStateOfRapidFireGun.TurnedOnShot:
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
        }

        private void ProcessShoot()
        {
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
                var targetOfShoot = shootHit.collider.GetComponentInParent<ITargetOfDamage>();

                if (targetOfShoot != null)
                {
                    targetOfShoot.SetHit(shootHit, DamagePerShot);
                }
            }
        }

        private void DisableEffects()
        {
            mGunLight.enabled = false;
            mGunParticles.Stop();
            mGunAudio.Stop();
        }

        public void StopFire()
        {
            lock (_lockObl)
            {
                if (!_isFired)
                {
                    return;
                }

                _isFired = false;
            }

            RunInMainThread(() =>
            {
                DisableEffects();
            });

            StopRepeatingShotSoundInUsualThread();
        }

        private Quaternion? _oldLocalRotation;

        public void LookAt()
        {
            LookAt((Vector3?)null);
        }

        public void LookAt(GameObject target)
        {
            LookAt(target.transform.position);
        }

        public void LookAt(Transform target)
        {
            LookAt(target.position);
        }

        public void LookAt(Vector3? target)
        {
            if(!target.HasValue)
            {
                if(_oldLocalRotation.HasValue)
                {
                    transform.localRotation = _oldLocalRotation.Value;
                    _oldLocalRotation = null;
                }

                return;
            }

            if(!_oldLocalRotation.HasValue)
            {
                _oldLocalRotation = transform.localRotation;
            }            

            transform.LookAt(target.Value);
        }

        public bool ThrowOut()
        {
            if (transform.parent == null)
            {
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
