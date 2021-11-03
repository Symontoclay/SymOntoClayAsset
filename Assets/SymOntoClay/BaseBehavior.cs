/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using SymOntoClay.Helpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay
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

        protected virtual void Start()
        {
#if UNITY_EDITOR
            //Debug.Log($"BaseBehavior Start");
#endif

            _uSocGameObject = GetComponent<IUSocGameObject>();

#if UNITY_EDITOR
            Debug.Log($"_uSocGameObject = {_uSocGameObject}");
#endif

            if(_uSocGameObject is IUHumanoidNPC)
            {
                _uHumanoidNPC = _uSocGameObject as IUHumanoidNPC;
                _humanoidNPC = _uHumanoidNPC.NPC;
            }

            _idForFacts = _uSocGameObject.IdForFacts;

            SetAliveFact();
        }

        protected virtual void Stop()
        {
            if (_repeatingStepsSoundCoroutine != null)
            {
                StopCoroutine(_repeatingStepsSoundCoroutine);
            }

            if (_repeatingShotSound != null)
            {
                StopCoroutine(_repeatingShotSound);
            }
        }

        private string _walkingFactId;

        private string _vitalFactId;

        protected void SetAliveFact()
        {
            var factStr = $"state({_idForFacts}, alive)";

#if UNITY_EDITOR
            Debug.Log($"BaseBehavior SetAliveFact factStr = '{factStr}'");
#endif

            if (!string.IsNullOrWhiteSpace(_vitalFactId))
            {
                _uSocGameObject.RemovePublicFact(_vitalFactId);
            }

            _vitalFactId = _uSocGameObject.InsertPublicFact(factStr);
        }

        protected void SetDeadFact()
        {
            var factStr = $"state({_idForFacts}, dead)";

#if UNITY_EDITOR
            Debug.Log($"BaseBehavior SetDeadFact factStr = '{factStr}'");
#endif

            if (!string.IsNullOrWhiteSpace(_vitalFactId))
            {
                _uSocGameObject.RemovePublicFact(_vitalFactId);
            }

            _vitalFactId = _uSocGameObject.InsertPublicFact(factStr);
        }

        /// <summary>
        /// Adds fact that the NPC has stopped itself.
        /// </summary>
        protected void AddStopFact()
        {
            var factStr = $"act({_idForFacts}, stop)";

#if UNITY_EDITOR
            Debug.Log($"BaseBehavior AddStopFact factStr = '{factStr}'");
#endif

            if (!string.IsNullOrWhiteSpace(_walkingFactId))
            {
                _uSocGameObject.RemovePublicFact(_walkingFactId);
            }
            
            _walkingFactId = _uSocGameObject.InsertPublicFact(factStr);

#if UNITY_EDITOR
            //Debug.Log($"BaseBehavior AddStopFact _walkingFactId = {_walkingFactId}");
#endif
        }

        /// <summary>
        /// Adds fact that the NPC has started walking.
        /// </summary>
        protected void AddWalkingFact()
        {
            var factStr = $"act({_idForFacts}, walk)";

#if UNITY_EDITOR
            Debug.Log($"BaseBehavior AddWalkingFact factStr = '{factStr}'");
#endif
            if(!string.IsNullOrWhiteSpace(_walkingFactId))
            {
                _uSocGameObject.RemovePublicFact(_walkingFactId);
            }
            
            _walkingFactId = _uSocGameObject.InsertPublicFact(factStr);

#if UNITY_EDITOR
            //Debug.Log($"BaseBehavior AddWalkingFact _walkingFactId = {_walkingFactId}");
#endif
        }

        protected void AddRunningFact()
        {
            var factStr = $"act({_idForFacts}, run)";

#if UNITY_EDITOR
            Debug.Log($"BaseBehavior AddRunningFact factStr = '{factStr}'");
#endif
            if (!string.IsNullOrWhiteSpace(_walkingFactId))
            {
                _uSocGameObject.RemovePublicFact(_walkingFactId);
            }

            _walkingFactId = _uSocGameObject.InsertPublicFact(factStr);

#if UNITY_EDITOR
            //Debug.Log($"BaseBehavior AddRunningFact _walkingFactId = {_walkingFactId}");
#endif
        }

        private string _holdFactId;

        /// <summary>
        /// Adds fact that the NPC holds something in his hands.
        /// </summary>
        /// <param name="heldId">Id of held thing.</param>
        protected void AddHoldFact(string heldId)
        {
            var factStr = $"hold({_idForFacts}, {heldId})";

            if (!string.IsNullOrWhiteSpace(_holdFactId))
            {
                _uSocGameObject.RemovePublicFact(_holdFactId);
            }

            _holdFactId = _uSocGameObject.InsertPublicFact(factStr);
        }

        /// <summary>
        /// Removes fact that the NPC holds something in his hands.
        /// </summary>
        protected void RemoveHoldFact()
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
#if UNITY_EDITOR
                Debug.Log($"BaseBehavior StepsSoundRoutine power = {power}; text = {text}");
#endif

                Task.Run(() => { _uSocGameObject.PushSoundFact(power, text); });

                //yield return null;
                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator _repeatingStepsSoundCoroutine;

        protected void StartRepeatingWalkingStepsSound()
        {
#if UNITY_EDITOR
            Debug.Log("BaseBehavior StartRepeatingWalkingStepsSound");
#endif

            if(_repeatingStepsSoundCoroutine != null)
            {
                StopCoroutine(_repeatingStepsSoundCoroutine);
            }

            _repeatingStepsSoundCoroutine = StepsSoundRoutine(50, "act(someone, walk)");
            StartCoroutine(_repeatingStepsSoundCoroutine);
        }

        protected void StartRepeatingRunningStepsSound()
        {
#if UNITY_EDITOR
            Debug.Log("BaseBehavior StartRepeatingRunningStepsSound");
#endif

            if (_repeatingStepsSoundCoroutine != null)
            {
                StopCoroutine(_repeatingStepsSoundCoroutine);
            }

            _repeatingStepsSoundCoroutine = StepsSoundRoutine(60, "act(someone, run)");
            StartCoroutine(_repeatingStepsSoundCoroutine);
        }

        protected void StopRepeatingStepsSound()
        {
#if UNITY_EDITOR
            Debug.Log("BaseBehavior StopRepeatingStepsSound");
#endif

            if (_repeatingStepsSoundCoroutine != null)
            {
                StopCoroutine(_repeatingStepsSoundCoroutine);
            }
        }

        private IEnumerator ShotSoundRoutine(float power, string text)
        {
            while (true)
            {
#if UNITY_EDITOR
                Debug.Log($"BaseBehavior ShotSoundRoutine power = {power}; text = {text}");
#endif

                Task.Run(() => { _uSocGameObject.PushSoundFact(power, text); });

                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator _repeatingShotSound;

        protected void StartRepeatingShotSound()
        {
#if UNITY_EDITOR
            Debug.Log("BaseBehavior StartRepeatingShotSound");
#endif

            if (_repeatingShotSound != null)
            {
                StopCoroutine(_repeatingShotSound);
            }

            _repeatingShotSound = ShotSoundRoutine(70, $"act({_idForFacts}, shoot)");
            StartCoroutine(_repeatingShotSound);
        }

        protected void StopRepeatingShotSound()
        {
#if UNITY_EDITOR
            Debug.Log("BaseBehavior StopRepeatingShotSound");
#endif

            if(_repeatingShotSound != null)
            {
                StopCoroutine(_repeatingShotSound);
            }
        }

        protected void ProcessDie()
        {
            SetDeadFact();

            _uHumanoidNPC.Die();
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
        /// Checks that It can be taken by the subject.
        /// </summary>
        /// <param name="subject">The subject that takes this.</param>
        /// <returns>true - if It can be taken, otherwise - false.</returns>
        public virtual bool CanBeTakenBy(IEntity subject)
        {
            return false;
        }

        protected void AddToManualControl(IUSocGameObject obj, DeviceOfBiped device)
        {
            _humanoidNPC.AddToManualControl(obj.SocGameObject, device);
        }

        protected void AddToManualControl(IUSocGameObject obj, IList<DeviceOfBiped> devices)
        {
            _humanoidNPC.AddToManualControl(obj.SocGameObject, devices);
        }

        protected void RemoveFromManualControl(IUSocGameObject obj)
        {
            _humanoidNPC.RemoveFromManualControl(obj.SocGameObject);
        }
    }
}