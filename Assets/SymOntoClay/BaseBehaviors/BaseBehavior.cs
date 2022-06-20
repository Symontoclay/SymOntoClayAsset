/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Helpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SymOntoClay.UnityAsset.Interfaces;

namespace SymOntoClay.UnityAsset.BaseBehaviors
{
    /// <summary>
    /// Contains base recommended behavior.
    /// </summary>
    public abstract class BaseBehavior : MonoBehaviour, IUHostListener
    {
        protected IUSocGameObject _uSocGameObject;

        private IUHumanoidNPC _uHumanoidNPC;
        private IHumanoidNPC _humanoidNPC;
        private string _idForFacts;
        private object _lockObj = new object();

        public string IdForFacts => _idForFacts;
        public IEntityLogger Logger => _uSocGameObject.Logger;

        public bool DeleteAliveFactsAfterDie = true;

        protected IStorage BackpackStorage => _humanoidNPC.BackpackStorage;

        #region Unity handlers
        protected virtual void Start()
        {
#if UNITY_EDITOR
            //Debug.Log($"BaseBehavior Start");
#endif

            _uSocGameObject = GetComponent<IUSocGameObject>();

#if UNITY_EDITOR
            //Debug.Log($"_uSocGameObject = {_uSocGameObject}");
#endif

            if (_uSocGameObject is IUHumanoidNPC)
            {
                _uHumanoidNPC = _uSocGameObject as IUHumanoidNPC;
                _humanoidNPC = _uHumanoidNPC.NPC;
            }

            _idForFacts = _uSocGameObject.IdForFacts;

            NSetAliveFact();
        }

        protected virtual void Stop()
        {
            NStopStepsSoundRoutine();

            NStopShotSoundRoutine();
        }
        #endregion

        private string _vitalFactId;

        private void NSetAliveFact()
        {
            Task.Run(() => {
                var factStr = $"state({_idForFacts}, alive)";

#if UNITY_EDITOR
                //Debug.Log($"BaseBehavior NSetAliveFact factStr = '{factStr}'");
#endif

                if (!string.IsNullOrWhiteSpace(_vitalFactId))
                {
                    _uSocGameObject.RemovePublicFact(_vitalFactId);
                }

                _vitalFactId = _uSocGameObject.InsertPublicFact(factStr);
            });
        }

        private void NSetDeadFact()
        {
            Task.Run(() => {
                var factStr = $"state({_idForFacts}, dead)";

#if UNITY_EDITOR
                //Debug.Log($"BaseBehavior NSetDeadFact factStr = '{factStr}'");
#endif

                if (!string.IsNullOrWhiteSpace(_vitalFactId))
                {
                    _uSocGameObject.RemovePublicFact(_vitalFactId);
                }

                _vitalFactId = _uSocGameObject.InsertPublicFact(factStr);
            });
        }

        private string _walkingFactId;

        /// <summary>
        /// Adds fact that the NPC has stopped itself.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void AddStopFact()
        {
            Task.Run(() => {
                var factStr = $"act({_idForFacts}, stop)";

#if UNITY_EDITOR
                //Debug.Log($"BaseBehavior NAddStopFact factStr = '{factStr}'");
#endif

                NRemoveCurrWalkingFactId();

                _walkingFactId = _uSocGameObject.InsertPublicFact(factStr);

#if UNITY_EDITOR
                //Debug.Log($"BaseBehavior NAddStopFact _walkingFactId = {_walkingFactId}");
#endif
            });
        }

        private void NRemoveCurrWalkingFactId()
        {
            if (!string.IsNullOrWhiteSpace(_walkingFactId))
            {
                _uSocGameObject.RemovePublicFact(_walkingFactId);
            }
        }

        /// <summary>
        /// Adds fact that the NPC has started walking.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void AddWalkingFact()
        {
            Task.Run(() => {
                var factStr = $"act({_idForFacts}, walk)";

#if UNITY_EDITOR
                //Debug.Log($"BaseBehavior NAddWalkingFact factStr = '{factStr}'");
#endif

                NRemoveCurrWalkingFactId();


                _walkingFactId = _uSocGameObject.InsertPublicFact(factStr);

#if UNITY_EDITOR
                //Debug.Log($"BaseBehavior NAddWalkingFact _walkingFactId = {_walkingFactId}");
#endif
            });
        }

        /// <summary>
        /// Adds fact that the NPC has started running.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void AddRunningFact()
        {
            Task.Run(() => {
                var factStr = $"act({_idForFacts}, run)";

#if UNITY_EDITOR
                //Debug.Log($"BaseBehavior NAddRunningFact factStr = '{factStr}'");
#endif

                NRemoveCurrWalkingFactId();

                _walkingFactId = _uSocGameObject.InsertPublicFact(factStr);

#if UNITY_EDITOR
                //Debug.Log($"BaseBehavior NAddRunningFact _walkingFactId = {_walkingFactId}");
#endif
            });
        }

        private string _holdFactId;

        /// <summary>
        /// Adds fact that the NPC holds something in his hands.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        /// <param name="heldId">Id of held thing.</param>
        protected void AddHoldFact(string heldId)
        {
            Task.Run(() => {
                var factStr = $"hold({_idForFacts}, {heldId})";

                NRemoveCurrHoldFactId();

                _holdFactId = _uSocGameObject.InsertPublicFact(factStr);
            });
        }

        /// <summary>
        /// Removes fact that the NPC holds something in his hands.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void RemoveHoldFact()
        {
            Task.Run(() => {
                NRemoveCurrHoldFactId();
            });
        }

        private void NRemoveCurrHoldFactId()
        {
            if (!string.IsNullOrWhiteSpace(_holdFactId))
            {
                _uSocGameObject.RemovePublicFact(_holdFactId);
            }
        }

        private IEnumerator StepsSoundRoutine(float power, string text)
        {
            while (true)
            {
                Task.Run(() => { _uSocGameObject.PushSoundFact(power, text); });

                yield return new WaitForSeconds(0.5f);
            }
        }

        private void NStopStepsSoundRoutine()
        {
            if (_repeatingStepsSoundCoroutine != null)
            {
                StopCoroutine(_repeatingStepsSoundCoroutine);

                _repeatingStepsSoundCoroutine = null;
            }
        }

        private IEnumerator _repeatingStepsSoundCoroutine;

        /// <summary>
        /// Starts pushing sound facts that the NPS is walking.
        /// This method should be called only in main thread.
        /// </summary>
        protected void StartRepeatingWalkingStepsSoundInMainThread()
        {
            NStartRepeatingWalkingStepsSound();
        }

        /// <summary>
        /// Starts pushing sound facts that the NPS is walking.
        /// This method should be called only in usual (not main) thread.
        /// </summary>
        protected void StartRepeatingWalkingStepsSoundInUsualThread()
        {
            RunInMainThread(() =>
            {
                NStartRepeatingWalkingStepsSound();
            });            
        }

        private void NStartRepeatingWalkingStepsSound()
        {
            NStopStepsSoundRoutine();

            _repeatingStepsSoundCoroutine = StepsSoundRoutine(50, "act(someone, walk)");
            StartCoroutine(_repeatingStepsSoundCoroutine);
        }

        /// <summary>
        /// Starts pushing sound facts that the NPS is running.
        /// This method should be called only in main thread.
        /// </summary>
        protected void StartRepeatingRunningStepsSoundInMainThread()
        {
            NStartRepeatingRunningStepsSound();
        }

        /// <summary>
        /// Starts pushing sound facts that the NPS is running.
        /// This method should be called only in usual (not main) thread.
        /// </summary>
        protected void StartRepeatingRunningStepsSoundInUsualThread()
        {
            RunInMainThread(() => {
                NStartRepeatingRunningStepsSound();
            });            
        }

        private void NStartRepeatingRunningStepsSound()
        {
            NStopStepsSoundRoutine();

            _repeatingStepsSoundCoroutine = StepsSoundRoutine(60, "act(someone, run)");
            StartCoroutine(_repeatingStepsSoundCoroutine);
        }

        /// <summary>
        /// Stops pushing sound facts that the NPS is walking or running.
        /// This method should be called only in main thread.
        /// </summary>
        protected void StopRepeatingStepsSoundInMainThread()
        {
            NStopRepeatingStepsSound();
        }

        /// <summary>
        /// Stops pushing sound facts that the NPS is walking or running.
        /// This method should be called only in usual (not main) thread.
        /// </summary>
        protected void StopRepeatingStepsSoundInUsualThread()
        {
            RunInMainThread(() => { 
                NStopRepeatingStepsSound();
            });            
        }

        private void NStopRepeatingStepsSound()
        {
            NStopStepsSoundRoutine();
        }

        private IEnumerator ShotSoundRoutine(float power, string text)
        {
            while (true)
            {
                Task.Run(() => { _uSocGameObject.PushSoundFact(power, text); });

                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator _repeatingShotSound;

        /// <summary>
        /// Starts pushing sound facts that something is shooting.
        /// This method should be called only in main thread.
        /// </summary>
        protected void StartRepeatingShotSoundInMainThread()
        {
            NStartRepeatingShotSound();
        }

        /// <summary>
        /// Starts pushing sound facts that something is shooting.
        /// This method should be called only in usual (not main) thread.
        /// </summary>
        protected void StartRepeatingShotSoundInUsualThread()
        {
            RunInMainThread(() => {
                NStartRepeatingShotSound();
            });            
        }

        private void NStartRepeatingShotSound()
        {
            NStopShotSoundRoutine();

            _repeatingShotSound = ShotSoundRoutine(70, $"act({_idForFacts}, shoot)");
            StartCoroutine(_repeatingShotSound);
        }

        /// <summary>
        /// Stops pushing sound facts that something is shooting.
        /// This method should be called only in main thread.
        /// </summary>
        protected void StopRepeatingShotSoundInMainThread()
        {
            NStopRepeatingShotSound();
        }

        /// <summary>
        /// Stops pushing sound facts that something is shooting.
        /// This method should be called only in usual (not main) thread.
        /// </summary>
        protected void StopRepeatingShotSoundInUsualThread()
        {
            RunInMainThread(() => {
                NStopRepeatingShotSound();
            });            
        }

        private void NStopRepeatingShotSound()
        {
            NStopShotSoundRoutine();
        }

        private void NStopShotSoundRoutine()
        {
            if (_repeatingShotSound != null)
            {
                StopCoroutine(_repeatingShotSound);

                _repeatingShotSound = null;
            }
        }

        private string _heShootsFactId;

        /// <summary>
        /// Adds fact that the NPC shoots.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void AddHeShootsFact()
        {
            Task.Run(() => {
                if (!string.IsNullOrWhiteSpace(_heShootsFactId))
                {
                    return;
                }

                var factStr = $"act({_idForFacts}, shoot)";

                _heShootsFactId = _uSocGameObject.InsertPublicFact(factStr);
            });
        }

        /// <summary>
        /// Removes fact that the NPC shoots.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void RemoveHeShootsFact()
        {
            Task.Run(() => {
                NRemoveCurrHeShootsFactId();
            });
        }

        private void NRemoveCurrHeShootsFactId()
        {
            if (!string.IsNullOrWhiteSpace(_heShootsFactId))
            {
                _uSocGameObject.RemovePublicFact(_heShootsFactId);

                _heShootsFactId = string.Empty;
            }
        }

        private string _heIsReadyForShootFactId;

        /// <summary>
        /// Adds fact that the NPC is ready for shooting.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void AddHeIsReadyForShootFact()
        {
            Task.Run(() => {
                if (!string.IsNullOrWhiteSpace(_heIsReadyForShootFactId))
                {
                    return;
                }

                var factStr = $"ready({_idForFacts}, shoot)";

                _heIsReadyForShootFactId = _uSocGameObject.InsertPublicFact(factStr);
            });
        }

        /// <summary>
        /// Removes fact that the NPC is ready for shooting.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void RemoveHeIsReadyForShootFact()
        {
            Task.Run(() => {
                NRemoveCurrHeIsReadyForShootFactId();
            });
        }

        private void NRemoveCurrHeIsReadyForShootFactId()
        {
            if (!string.IsNullOrWhiteSpace(_heIsReadyForShootFactId))
            {
                _uSocGameObject.RemovePublicFact(_heIsReadyForShootFactId);

                _heIsReadyForShootFactId = string.Empty;
            }
        }

        /// <summary>
        /// Removes all facts related to shooting.
        /// </summary>
        protected void RemoveAllShootFacts()
        {
            Task.Run(() => {
                NRemoveCurrHoldFactId();
                NRemoveCurrHeShootsFactId();
                NRemoveCurrHeIsReadyForShootFactId();
            });
        }

        /// <summary>
        /// Proceses death for NPC.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void ProcessDeath()
        {
            NProcessDeath();
        }

        private void NProcessDeath()
        {
            Task.Run(() => {
                NSetDeadFact();

                if(DeleteAliveFactsAfterDie)
                {
                    NRemoveCurrWalkingFactId();
                    NRemoveCurrHoldFactId();
                    NRemoveCurrHeShootsFactId();
                    NRemoveCurrHeIsReadyForShootFactId();

                    RunInMainThread(() => {
                        NStopStepsSoundRoutine();
                        NStopShotSoundRoutine();
                    });

                    _uHumanoidNPC.Die();
                }
            });
        }

        private static int _methodId;

        /// <summary>
        /// Returns integer id which is unique for the component.
        /// It can be helpful for debugging host methods.
        /// </summary>
        /// <returns>Integer id which is unique for the component.</returns>
        protected int GetMethodId()
        {
            lock (_lockObj)
            {
                _methodId++;
                return _methodId;
            }
        }

        /// <summary>
        /// Executes handler in main thread context.
        /// </summary>
        /// <param name="function">Handler which should be executed in main thread context.</param>
        protected void RunInMainThread(Action function)
        {
            _uSocGameObject.RunInMainThread(function);
        }

        /// <summary>
        /// Executes handler in main thread context.
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="function">Handler which should be executed in main thread context.</param>
        /// <returns>Result of the execution.</returns>
        protected TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            return _uSocGameObject.RunInMainThread(function);
        }

        /// <summary>
        /// Checks that It can be taken by the NPC.
        /// </summary>
        /// <param name="subject">The NPC that takes this.</param>
        /// <returns><b>true</b> - if It can be taken, otherwise - <b>false</b>.</returns>
        public virtual bool CanBeTakenBy(IEntity subject)
        {
            return false;
        }

        /// <summary>
        /// Adds a game object into manual controlled area of the NPC.
        /// </summary>
        /// <param name="obj">Instance of the game object.</param>
        /// <param name="device">Describes biped device which will be using the game object.</param>
        protected void AddToManualControl(IUSocGameObject obj, DeviceOfBiped device)
        {
            _humanoidNPC.AddToManualControl(obj.SocGameObject, device);
        }

        /// <summary>
        /// Adds a game object into manual controlled area of the NPC.
        /// </summary>
        /// <param name="obj">Instance of the game object.</param>
        /// <param name="devices">Describes list of biped devices which will be using the game object.</param>
        protected void AddToManualControl(IUSocGameObject obj, IList<DeviceOfBiped> devices)
        {
            _humanoidNPC.AddToManualControl(obj.SocGameObject, devices);
        }

        /// <summary>
        /// Removes a game object from manual controlled area of an NPC.
        /// </summary>
        /// <param name="obj">Instance of the game object.</param>
        protected void RemoveFromManualControl(IUSocGameObject obj)
        {
            _humanoidNPC.RemoveFromManualControl(obj.SocGameObject);
        }

        /// <summary>
        /// Adds a game object into backpack.
        /// </summary>
        /// <param name="obj">Instance of the game object.</param>
        protected void AddToBackpack(IGameObject obj)
        {
            _humanoidNPC.AddToBackpack(obj);
        }

        /// <summary>
        /// Removes game object from backpack.
        /// </summary>
        /// <param name="obj">Instance of the game object.</param>
        protected void RemoveFromBackpack(IGameObject obj)
        {
            _humanoidNPC.RemoveFromBackpack(obj);
        }

        /// <summary>
        /// Creates a rotation with the specified forward and upwards directions.
        /// It is a wrapper on Quaternion.LookRotation.
        /// This method should be called only in usual (not main) thread.
        /// </summary>
        /// <param name="targetPosition">Position that is looked at.</param>
        /// <returns>Rotation to target position.</returns>
        protected Quaternion GetRotationToPositionInUsualThread(System.Numerics.Vector3 targetPosition)
        {
            return GetRotationToPositionInUsualThread(new Vector3(targetPosition.X, targetPosition.Y, targetPosition.Z));
        }

        /// <summary>
        /// Creates a rotation with the specified forward and upwards directions.
        /// It is a wrapper on Quaternion.LookRotation.
        /// This method should be called only in usual (not main) thread.
        /// </summary>
        /// <param name="targetPosition">Position that is looked at.</param>
        /// <returns>Rotation to target position.</returns>
        protected Quaternion GetRotationToPositionInUsualThread(Vector3 targetPosition)
        {
            return RunInMainThread<Quaternion>(() => {
                return GetRotationToPositionInMainThread(targetPosition);
            });
        }

        /// <summary>
        /// Creates a rotation with the specified forward and upwards directions.
        /// It is a wrapper on Quaternion.LookRotation.
        /// This method should be called only in main thread.
        /// </summary>
        /// <param name="targetPosition">Position that is looked at.</param>
        /// <returns>Rotation to target position.</returns>
        protected Quaternion GetRotationToPositionInMainThread(System.Numerics.Vector3 targetPosition)
        {
            return GetRotationToPositionInMainThread(new Vector3(targetPosition.X, targetPosition.Y, targetPosition.Z));
        }

        /// <summary>
        /// Creates a rotation with the specified forward and upwards directions.
        /// It is a wrapper on Quaternion.LookRotation.
        /// This method should be called only in main thread.
        /// </summary>
        /// <param name="targetPosition">Position that is looked at.</param>
        /// <returns>Rotation to target position.</returns>
        protected Quaternion GetRotationToPositionInMainThread(Vector3 targetPosition)
        {
            var heading = targetPosition - transform.position;

            var distance = heading.magnitude;

            var direction = heading / distance;

            return Quaternion.LookRotation(direction);
        }
    }
}