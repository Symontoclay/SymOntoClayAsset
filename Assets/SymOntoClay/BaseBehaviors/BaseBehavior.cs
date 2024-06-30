/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.CodeModel;
using System.Threading;
using SymOntoClay.Monitor.Common;
using NLog;

namespace SymOntoClay.UnityAsset.BaseBehaviors
{
    /// <summary>
    /// Contains base recommended behavior.
    /// </summary>
    public abstract class BaseBehavior : MonoBehaviour, IHostListenerBehavior, IExecutorInMainThread
    {
        protected IGameObjectBehavior _uSocGameObject;

        private IHumanoidNPCBehavior _uHumanoidNPC;
        private IHumanoidNPC _humanoidNPC;
        private string _idForFacts;
        private IStandardFactsBuilder _standardFactsBuilder;
        private object _lockObj = new object();

        public string IdForFacts => _idForFacts;
        public IMonitorLogger Logger => _uSocGameObject.Logger;

        public bool DeleteAliveFactsAfterDeath = true;

        protected IStorage BackpackStorage => _humanoidNPC.BackpackStorage;

        private int _mainThreadId;

        #region Unity handlers

        protected virtual void Awake()
        {
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        protected virtual void Start()
        {
#if UNITY_EDITOR
            //Debug.Log($"BaseBehavior Start");
#endif

            _uSocGameObject = GetComponent<IGameObjectBehavior>();

#if UNITY_EDITOR
            //Debug.Log($"_uSocGameObject = {_uSocGameObject}");
#endif

            if (_uSocGameObject is IHumanoidNPCBehavior)
            {
                _uHumanoidNPC = _uSocGameObject as IHumanoidNPCBehavior;
                _humanoidNPC = _uHumanoidNPC.NPC;
            }

            _idForFacts = _uSocGameObject.IdForFacts;

#if UNITY_EDITOR
            //Debug.Log($"({name}) _uSocGameObject == null = {_uSocGameObject == null}");
            //Debug.Log($"({name}) _idForFacts = {_idForFacts}");
#endif

            _standardFactsBuilder = _uSocGameObject.StandardFactsBuilder;

            NSetAliveFact(Logger);
        }

        protected virtual void Stop()
        {
            NStopStepsSoundRoutine();

            NStopShotSoundRoutine();
        }
        #endregion

        private string _vitalFactId;

        private void NSetAliveFact(IMonitorLogger logger)
        {
#if UNITY_EDITOR
            Debug.Log($"({name}) NSetAliveFact _idForFacts = {_idForFacts}");
            Debug.Log($"({name}) NSetAliveFact logger == null = {logger == null}; logger?.Id = {logger?.Id}");
#endif

            Task.Run(() => {
                var taskId = logger.StartTask("72F737AD-F4A2-4080-BFD7-3BE50630584C");

#if UNITY_EDITOR
                Debug.Log($"({name}) NEXT NSetAliveFact _idForFacts = {_idForFacts}");
#endif

                try
                {
                    var fact = _standardFactsBuilder.BuildAliveFactInstance(_idForFacts);

#if UNITY_EDITOR
                    Debug.Log($"({name}) NSetAliveFact fact {fact.ToHumanizedLabel()}");
#endif

#if DEBUG
                    //logger?.Info("D55496E9-7B4D-4B62-B95D-FB44B5A4EC4B", $"BaseBehavior NSetAliveFact factStr = '{factStr}'");
#endif

                    if (!string.IsNullOrWhiteSpace(_vitalFactId))
                    {
                        _uSocGameObject.RemovePublicFact(logger, _vitalFactId);
                    }

                    _vitalFactId = _uSocGameObject.InsertPublicFact(logger, fact);

#if UNITY_EDITOR
                    Debug.Log($"({name}) NSetAliveFact _vitalFactId = {_vitalFactId}");
#endif
                }
                catch (Exception e)
                {
                    try
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) e = {e}");
#endif

                        logger.Error("5F0C0C29-1AF3-4B37-9BA3-D29102E727CB", e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) ex = {ex}");
#endif
                    }
                }

                logger.StopTask("1914801B-3AE2-4437-A352-23CAB90CBB73", taskId);
            });
        }

        private void NSetDeadFact(IMonitorLogger logger)
        {
            Task.Run(() => {
                var taskId = logger.StartTask("3F66428E-C833-4F22-9EB2-1AB6C01DC24A");

                try
                {
                    var fact = _standardFactsBuilder.BuildDeadFactInstance(_idForFacts);

#if DEBUG
                    //logger?.Info("F431ECFC-3188-4157-8171-2A2F2C353A49", $"BaseBehavior NSetDeadFact factStr = '{factStr}'");
#endif

#if UNITY_EDITOR
                    Debug.Log($"({name}) NSetDeadFact fact {fact.ToHumanizedLabel()}");
#endif

                    if (!string.IsNullOrWhiteSpace(_vitalFactId))
                    {
                        _uSocGameObject.RemovePublicFact(logger, _vitalFactId);
                    }

                    _vitalFactId = _uSocGameObject.InsertPublicFact(logger, fact);

#if UNITY_EDITOR
                    Debug.Log($"({name}) NSetDeadFact _vitalFactId = {_vitalFactId}");
#endif
                }
                catch (Exception e)
                {
                    try
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) e = {e}");
#endif

                        logger.Error("470979E8-2996-4B74-A6FD-E6EF0659EC3F", e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) ex = {ex}");
#endif
                    }
                }

                logger.StopTask("6BCC156F-2837-4C7D-B8C9-0685C8748349", taskId);
            });
        }

        /*
    try
    {

    }
    catch (Exception e)
    {
        try
        {

        }
        catch (Exception ex)
        {

        }
    }
 */

        /*
            try
            {

            }
            catch (Exception e)
            {
                try
                {
#if UNITY_EDITOR
                    Debug.LogError($"({name}) e = {e}");
#endif

                    logger.Error(, e);
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR
                    Debug.LogError($"({name}) ex = {ex}");
#endif
                }
            }
         */

        private string _walkingFactId;

        /// <summary>
        /// Adds fact that the NPC has stopped itself.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void AddStopFact(IMonitorLogger logger)
        {
            Task.Run(() => {
                var taskId = logger.StartTask("4E872860-246A-43F0-844F-7EE3E15055AB");

                try
                {
                    var fact = _standardFactsBuilder.BuildStopFactInstance(_idForFacts);

#if DEBUG
                    //logger?.Info("8FD485BE-A814-4CC3-8B98-F9ABB5DCB5C8", $"BaseBehavior NAddStopFact factStr = '{factStr}'");
#endif

                    NRemoveCurrWalkingFactId(logger);

                    _walkingFactId = _uSocGameObject.InsertPublicFact(logger, fact);

#if DEBUG
                    //logger?.Info("193723A8-0481-46CA-A6BF-C7477F2838CE", $"BaseBehavior NAddStopFact _walkingFactId = {_walkingFactId}");
#endif
                }
                catch (Exception e)
                {
                    try
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) e = {e}");
#endif

                        logger.Error("7EB12A8F-62BC-4A75-A6B3-EC22516BA767", e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) ex = {ex}");
#endif
                    }
                }

                logger.StopTask("71B2AC6C-73CD-440D-8FAC-6E204F948469", taskId);
            });
        }

        private void NRemoveCurrWalkingFactId(IMonitorLogger logger)
        {
            if (!string.IsNullOrWhiteSpace(_walkingFactId))
            {
                _uSocGameObject.RemovePublicFact(logger, _walkingFactId);
            }
        }

        /// <summary>
        /// Adds fact that the NPC has started walking.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void AddWalkingFact(IMonitorLogger logger)
        {
#if DEBUG
            logger?.Info("83385CD2-A3E4-4543-9DCE-EDDAC7BE8AB1", $"BaseBehavior NAddWalkingFact _idForFacts = '{_idForFacts}'");
#endif
            
            Task.Run(() => {
                var taskId = logger.StartTask("05C3A625-E42D-4007-926C-4FE929AEF1FF");

                try
                {
                    var fact = _standardFactsBuilder.BuildWalkFactInstance(_idForFacts);

#if DEBUG
                    logger?.Info("1AC92372-0C01-4EB1-A8EF-C1959D7727CA", $"BaseBehavior NAddWalkingFact fact = '{fact.ToHumanizedString()}'");
#endif

                    NRemoveCurrWalkingFactId(logger);


                    _walkingFactId = _uSocGameObject.InsertPublicFact(logger, fact);

#if DEBUG
                    //logger?.Info("30FC2DC9-E067-4832-94BE-60672F90F7F8", $"BaseBehavior NAddWalkingFact _walkingFactId = {_walkingFactId}");
#endif
                }
                catch (Exception e)
                {
                    try
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) e = {e}");
#endif

                        logger.Error("FF8CB845-1FB5-4457-BCAB-FA08DA0C0224", e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) ex = {ex}");
#endif
                    }
                }

                logger.StopTask("0777E8A1-7528-443D-A3B1-37E76C5CCA5D", taskId);
            });
        }

        /// <summary>
        /// Adds fact that the NPC has started running.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void AddRunningFact(IMonitorLogger logger)
        {
            Task.Run(() => {
                var taskId = logger.StartTask("2EB9B259-DCD1-4A1B-870F-C81024D763AF");

                try
                {
                    var fact = _standardFactsBuilder.BuildRunFactInstance(_idForFacts);

#if DEBUG
                    //logger?.Info("3D9360A2-466F-4657-AAC7-F6D5F66EC516", $"BaseBehavior NAddRunningFact factStr = '{factStr}'");
#endif

                    NRemoveCurrWalkingFactId(logger);

                    _walkingFactId = _uSocGameObject.InsertPublicFact(logger, fact);

#if DEBUG
                    //logger?.Info("05AE7BDF-C5B6-4861-92B7-5B3C88E42E42", $"BaseBehavior NAddRunningFact _walkingFactId = {_walkingFactId}");
#endif
                }
                catch (Exception e)
                {
                    try
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) e = {e}");
#endif

                        logger.Error("A1CC6ED0-20FC-4E03-A7BF-85C404C47C3B", e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) ex = {ex}");
#endif
                    }
                }

                logger.StopTask("9970DB2B-0FFA-4EC1-8F9B-E04C77C9F0A4", taskId);
            });
        }

        private string _holdFactId;

        /// <summary>
        /// Adds fact that the NPC holds something in his hands.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        /// <param name="logger">Logger for method call.</param>
        /// <param name="heldId">Id of held thing.</param>
        protected void AddHoldFact(IMonitorLogger logger, string heldId)
        {
            Task.Run(() => {
                var taskId = logger.StartTask("914B9BAF-5159-412F-966F-A57B41207822");

                try
                {
                    var fact = _standardFactsBuilder.BuildHoldFactInstance(_idForFacts, heldId);

                    NRemoveCurrHoldFactId(logger);

                    _holdFactId = _uSocGameObject.InsertPublicFact(logger, fact);
                }
                catch (Exception e)
                {
                    try
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) e = {e}");
#endif

                        logger.Error("432EE236-4DEE-4AB9-BD6E-FA2181C0A99F", e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) ex = {ex}");
#endif
                    }
                }

                logger.StopTask("015B0445-DC37-4261-9148-99EFB0AB1317", taskId);
            });
        }

        /// <summary>
        /// Removes fact that the NPC holds something in his hands.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void RemoveHoldFact(IMonitorLogger logger)
        {
            Task.Run(() => {
                var taskId = logger.StartTask("C84246AB-E797-4C88-BD09-6BFB62E71995");

                try
                {
                    NRemoveCurrHoldFactId(logger);
                }
                catch (Exception e)
                {
                    try
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) e = {e}");
#endif

                        logger.Error("8F950667-4802-4B49-BC7A-A3A7AFB56FCB", e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) ex = {ex}");
#endif
                    }
                }

                logger.StopTask("973F8F71-9B3D-4124-9BFA-9AB47385E336", taskId);
            });
        }

        private void NRemoveCurrHoldFactId(IMonitorLogger logger)
        {
            if (!string.IsNullOrWhiteSpace(_holdFactId))
            {
                _uSocGameObject.RemovePublicFact(logger, _holdFactId);
            }
        }

        private IEnumerator StepsSoundRoutine(float power, string text, IMonitorLogger logger)
        {
            while (true)
            {
                Task.Run(() => {
                    var taskId = logger.StartTask("8E46E18C-5283-40AF-A760-1CB0A1466934");

                    try
                    {
                        _uSocGameObject.PushSoundFact(logger, power, text);
                    }
                    catch (Exception e)
                    {
                        try
                        {
#if UNITY_EDITOR
                            Debug.LogError($"({name}) e = {e}");
#endif

                            logger.Error("F9E9635D-30A6-4152-81AA-2BFC622C01BD", e);
                        }
                        catch (Exception ex)
                        {
#if UNITY_EDITOR
                            Debug.LogError($"({name}) ex = {ex}");
#endif
                        }
                    }

                    logger.StopTask("5D37496E-A190-44F5-8F59-5E8A3A3770F5", taskId);
                });

                yield return new WaitForSeconds(REPEAT_FACT_INTERVAL);
            }
        }

        private IEnumerator StepsSoundRoutine(float power, RuleInstance fact, IMonitorLogger logger)
        {
            while (true)
            {
                Task.Run(() => {
                    var taskId = logger.StartTask("8A87FBFF-4F88-47DA-913D-5B9329181BDE");

                    try
                    {
                        _uSocGameObject.PushSoundFact(logger, power, fact);
                    }
                    catch (Exception e)
                    {
                        try
                        {
#if UNITY_EDITOR
                            Debug.LogError($"({name}) e = {e}");
#endif

                            logger.Error("31B7F3A6-56A3-477B-927F-81924840F57E", e);
                        }
                        catch (Exception ex)
                        {
#if UNITY_EDITOR
                            Debug.LogError($"({name}) ex = {ex}");
#endif
                        }
                    }

                    logger.StopTask("B5C6FCA5-35CB-486C-B96E-AE21E9D41B77", taskId);
                });

                yield return new WaitForSeconds(REPEAT_FACT_INTERVAL);
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
        protected void StartRepeatingWalkingStepsSoundInMainThread(IMonitorLogger logger)
        {
            NStartRepeatingWalkingStepsSound(logger);
        }

        /// <summary>
        /// Starts pushing sound facts that the NPS is walking.
        /// This method should be called only in usual (not main) thread.
        /// </summary>
        protected void StartRepeatingWalkingStepsSoundInUsualThread(IMonitorLogger logger)
        {
            RunInMainThread(() =>
            {
                NStartRepeatingWalkingStepsSound(logger);
            });            
        }

        private void NStartRepeatingWalkingStepsSound(IMonitorLogger logger)
        {
            NStopStepsSoundRoutine();

            _repeatingStepsSoundCoroutine = StepsSoundRoutine(50, _standardFactsBuilder.BuildWalkSoundFactInstance(), logger);
            StartCoroutine(_repeatingStepsSoundCoroutine);
        }

        /// <summary>
        /// Starts pushing sound facts that the NPS is running.
        /// This method should be called only in main thread.
        /// </summary>
        protected void StartRepeatingRunningStepsSoundInMainThread(IMonitorLogger logger)
        {
            NStartRepeatingRunningStepsSound(logger);
        }

        /// <summary>
        /// Starts pushing sound facts that the NPS is running.
        /// This method should be called only in usual (not main) thread.
        /// </summary>
        protected void StartRepeatingRunningStepsSoundInUsualThread(IMonitorLogger logger)
        {
            RunInMainThread(() => {
                NStartRepeatingRunningStepsSound(logger);
            });
        }

        private void NStartRepeatingRunningStepsSound(IMonitorLogger logger)
        {
            NStopStepsSoundRoutine();

            _repeatingStepsSoundCoroutine = StepsSoundRoutine(60, _standardFactsBuilder.BuildRunSoundFactInstance(), logger);
            StartCoroutine(_repeatingStepsSoundCoroutine);
        }

        /// <summary>
        /// Stops pushing sound facts that the NPS is walking or running.
        /// This method should be called only in main thread.
        /// </summary>
        protected void StopRepeatingStepsSoundInMainThread(IMonitorLogger logger)
        {
            NStopRepeatingStepsSound(logger);
        }

        /// <summary>
        /// Stops pushing sound facts that the NPS is walking or running.
        /// This method should be called only in usual (not main) thread.
        /// </summary>
        protected void StopRepeatingStepsSoundInUsualThread(IMonitorLogger logger)
        {
            RunInMainThread(() => { 
                NStopRepeatingStepsSound(logger);
            });            
        }

        private void NStopRepeatingStepsSound(IMonitorLogger logger)
        {
            NStopStepsSoundRoutine();
        }

        private float REPEAT_FACT_INTERVAL = 30f;

        private IEnumerator ShotSoundRoutine(float power, string text, IMonitorLogger logger)
        {
            while (true)
            {
                Task.Run(() => {
                    var taskId = logger.StartTask("26C48A2B-2F3E-44A8-A1EC-DB782F432F1C");

                    try
                    {
                        _uSocGameObject.PushSoundFact(logger, power, text);
                    }
                    catch (Exception e)
                    {
                        try
                        {
#if UNITY_EDITOR
                            Debug.LogError($"({name}) e = {e}");
#endif

                            logger.Error("07FFDBD0-47A4-45B5-9937-45CC1CAA2503", e);
                        }
                        catch (Exception ex)
                        {
#if UNITY_EDITOR
                            Debug.LogError($"({name}) ex = {ex}");
#endif
                        }
                    }

                    logger.StopTask("8B912625-EBE1-4FD0-8361-B57FA68ABFF3", taskId);
                });

                yield return new WaitForSeconds(REPEAT_FACT_INTERVAL);
            }
        }

        private IEnumerator ShotSoundRoutine(float power, RuleInstance fact, IMonitorLogger logger)
        {
            while (true)
            {
                Task.Run(() => {
                    var taskId = logger.StartTask("6CB4B374-FDEF-4A6A-BA37-C67F7222BF00");

                    try
                    {
                        _uSocGameObject.PushSoundFact(logger, power, fact);
                    }
                    catch (Exception e)
                    {
                        try
                        {
#if UNITY_EDITOR
                            Debug.LogError($"({name}) e = {e}");
#endif

                            logger.Error("3036C8D2-04CF-4076-8A4B-73A4755278C6", e);
                        }
                        catch (Exception ex)
                        {
#if UNITY_EDITOR
                            Debug.LogError($"({name}) ex = {ex}");
#endif
                        }
                    }

                    logger.StopTask("DC05A8CC-39D4-418F-A652-3758EDD64EB9", taskId);
                });

                yield return new WaitForSeconds(REPEAT_FACT_INTERVAL);
            }
        }

        private IEnumerator _repeatingShotSound;

        /// <summary>
        /// Starts pushing sound facts that something is shooting.
        /// This method should be called only in main thread.
        /// </summary>
        protected void StartRepeatingShotSoundInMainThread(IMonitorLogger logger)
        {
            NStartRepeatingShotSound(logger);
        }

        /// <summary>
        /// Starts pushing sound facts that something is shooting.
        /// This method should be called only in usual (not main) thread.
        /// </summary>
        protected void StartRepeatingShotSoundInUsualThread(IMonitorLogger logger)
        {
            RunInMainThread(() => {
                NStartRepeatingShotSound(logger);
            });
        }

        private void NStartRepeatingShotSound(IMonitorLogger logger)
        {
            NStopShotSoundRoutine();

            _repeatingShotSound = ShotSoundRoutine(70, _standardFactsBuilder.BuildShootSoundFactInstance(), logger);
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
        protected void AddHeShootsFact(IMonitorLogger logger)
        {
            Task.Run(() => {
                var taskId = logger.StartTask("CE05EB0E-2149-42C4-9BC8-F0812983B4BF");

                try
                {
                    if (!string.IsNullOrWhiteSpace(_heShootsFactId))
                    {
                        logger.StopTask("D6EB95B3-588A-4A58-B73C-557B2B9CC13F", taskId);

                        return;
                    }

                    var fact = _standardFactsBuilder.BuildShootFactInstance(_idForFacts);

                    _heShootsFactId = _uSocGameObject.InsertPublicFact(logger, fact);
                }
                catch (Exception e)
                {
                    try
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) e = {e}");
#endif

                        logger.Error("7004EBA4-ABC3-48CD-A6EA-01B8700D6D07", e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) ex = {ex}");
#endif
                    }
                }

                logger.StopTask("55B39F53-FAC0-4E6B-95AD-4C1C184958A2", taskId);
            });
        }

        /// <summary>
        /// Removes fact that the NPC shoots.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void RemoveHeShootsFact(IMonitorLogger logger)
        {
            Task.Run(() => {
                var taskId = logger.StartTask("F91100C3-198B-4BC8-B0C4-500E357797A7");

                try
                {
                    NRemoveCurrHeShootsFactId(logger);
                }
                catch (Exception e)
                {
                    try
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) e = {e}");
#endif

                        logger.Error("B4F4BFEF-2E77-4D5A-A56A-AB3DB998C9CB", e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) ex = {ex}");
#endif
                    }
                }

                logger.StopTask("39CDF78A-2F37-43C5-BC4E-724E1A3CC67E", taskId);
            });
        }

        private void NRemoveCurrHeShootsFactId(IMonitorLogger logger)
        {
            if (!string.IsNullOrWhiteSpace(_heShootsFactId))
            {
                _uSocGameObject.RemovePublicFact(logger, _heShootsFactId);

                _heShootsFactId = string.Empty;
            }
        }

        private string _heIsReadyForShootFactId;

        /// <summary>
        /// Adds fact that the NPC is ready for shooting.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void AddHeIsReadyForShootFact(IMonitorLogger logger)
        {
            Task.Run(() => {
                var taskId = logger.StartTask("E21D86B1-817A-4AF4-9661-D13380035270");

                try
                {
                    if (!string.IsNullOrWhiteSpace(_heIsReadyForShootFactId))
                    {
                        logger.StopTask("615A26D4-AA84-4530-A5ED-5CB2EBC2E288", taskId);

                        return;
                    }

                    var fact = _standardFactsBuilder.BuildReadyForShootFactInstance(_idForFacts);

                    _heIsReadyForShootFactId = _uSocGameObject.InsertPublicFact(logger, fact);
                }
                catch (Exception e)
                {
                    try
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) e = {e}");
#endif

                        logger.Error("08580DB6-94C2-4A40-88E7-FA34676F591C", e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) ex = {ex}");
#endif
                    }
                }

                logger.StopTask("A06CB5F0-13D6-498D-A487-6531DBA200A2", taskId);
            });
        }

        /// <summary>
        /// Removes fact that the NPC is ready for shooting.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void RemoveHeIsReadyForShootFact(IMonitorLogger logger)
        {
            Task.Run(() => {
                var taskId = logger.StartTask("7C7AE14F-BCD1-4669-8E8B-2B3A8B180AAA");

                try
                {
                    NRemoveCurrHeIsReadyForShootFactId(logger);
                }
                catch (Exception e)
                {
                    try
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) e = {e}");
#endif

                        logger.Error("1CBD20CB-15F8-4E10-92E6-40193131DC9D", e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) ex = {ex}");
#endif
                    }
                }

                logger.StopTask("E6428BA4-8E79-495A-8CE5-F974AB34FC32", taskId);
            });
        }

        private void NRemoveCurrHeIsReadyForShootFactId(IMonitorLogger logger)
        {
            if (!string.IsNullOrWhiteSpace(_heIsReadyForShootFactId))
            {
                _uSocGameObject.RemovePublicFact(logger, _heIsReadyForShootFactId);

                _heIsReadyForShootFactId = string.Empty;
            }
        }

        /// <summary>
        /// Removes all facts related to shooting.
        /// </summary>
        protected void RemoveAllShootFacts(IMonitorLogger logger)
        {
            Task.Run(() => {
                var taskId = logger.StartTask("C8DE57D9-922B-4D1C-ADE7-89E0E20DABE6");

                try
                {
                    NRemoveCurrHoldFactId(logger);
                    NRemoveCurrHeShootsFactId(logger);
                    NRemoveCurrHeIsReadyForShootFactId(logger);
                }
                catch (Exception e)
                {
                    try
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) e = {e}");
#endif

                        logger.Error("53B8ED2A-5FEE-40E9-9748-19444F1E922A", e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) ex = {ex}");
#endif
                    }
                }

                logger.StopTask("3710E0E9-5C54-4CE8-8C56-D9554BE927DA", taskId);
            });
        }

        /// <summary>
        /// Proceses death for NPC.
        /// This method can be called both in main and in usual (not main) thread.
        /// </summary>
        protected void ProcessDeath(IMonitorLogger logger)
        {
            NProcessDeath(logger);
        }

        private void NProcessDeath(IMonitorLogger logger)
        {
            Task.Run(() => {
                var taskId = logger.StartTask("4459C9CC-DFFE-48DA-A3DB-4C1CE8E7559F");

                try
                {
                    NSetDeadFact(logger);

                    if (DeleteAliveFactsAfterDeath)
                    {
                        NRemoveCurrWalkingFactId(logger);
                        NRemoveCurrHoldFactId(logger);
                        NRemoveCurrHeShootsFactId(logger);
                        NRemoveCurrHeIsReadyForShootFactId(logger);

                        RunInMainThread(() => {
                            NStopStepsSoundRoutine();
                            NStopShotSoundRoutine();
                        });

                        _uHumanoidNPC.Die();
                    }
                }
                catch (Exception e)
                {
                    try
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) e = {e}");
#endif

                        logger.Error("4C865122-233B-48E4-A2E3-100A2FC55742", e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) ex = {ex}");
#endif
                    }
                }

                logger.StopTask("A952FA28-AF06-45E0-B7A3-4BAF1F3B6F34", taskId);
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

        /// <inheritdoc/>
        void IExecutorInMainThread.RunInMainThread(Action function)
        {
            RunInMainThread(function);
        }

        /// <inheritdoc/>
        TResult IExecutorInMainThread.RunInMainThread<TResult>(Func<TResult> function)
        {
            return RunInMainThread(function);
        }

        /// <summary>
        /// Executes handler in main thread context.
        /// </summary>
        /// <param name="function">Handler which should be executed in main thread context.</param>
        protected void RunInMainThread(Action function)
        {
            if (_mainThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                function();
                return;
            }

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
            if (_mainThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                return function();
            }

            return _uSocGameObject.RunInMainThread(function);
        }

        /// <summary>
        /// Checks that It can be taken by the NPC.
        /// </summary>
        /// <param name="logger">Logger for method call.</param>
        /// <param name="subject">The NPC that takes this.</param>
        /// <returns><b>true</b> - if It can be taken, otherwise - <b>false</b>.</returns>
        public virtual bool CanBeTakenBy(IMonitorLogger logger, IEntity subject)
        {
            return false;
        }

        /// <summary>
        /// Adds a game object into manual controlled area of the NPC.
        /// </summary>
        /// <param name="logger">Logger for method call.</param>
        /// <param name="obj">Instance of the game object.</param>
        /// <param name="device">Describes biped device which will be using the game object.</param>
        protected void AddToManualControl(IMonitorLogger logger, IGameObjectBehavior obj, DeviceOfBiped device)
        {
            _humanoidNPC.AddToManualControl(obj.SocGameObject, device);
        }

        /// <summary>
        /// Adds a game object into manual controlled area of the NPC.
        /// </summary>
        /// <param name="logger">Logger for method call.</param>
        /// <param name="obj">Instance of the game object.</param>
        /// <param name="devices">Describes list of biped devices which will be using the game object.</param>
        protected void AddToManualControl(IMonitorLogger logger, IGameObjectBehavior obj, IList<DeviceOfBiped> devices)
        {
            _humanoidNPC.AddToManualControl(obj.SocGameObject, devices);
        }

        /// <summary>
        /// Removes a game object from manual controlled area of an NPC.
        /// </summary>
        /// <param name="logger">Logger for method call.</param>
        /// <param name="obj">Instance of the game object.</param>
        protected void RemoveFromManualControl(IMonitorLogger logger, IGameObjectBehavior obj)
        {
            _humanoidNPC.RemoveFromManualControl(obj.SocGameObject);
        }

        /// <summary>
        /// Adds a game object into backpack.
        /// </summary>
        /// <param name="logger">Logger for method call.</param>
        /// <param name="obj">Instance of the game object.</param>
        protected void AddToBackpack(IMonitorLogger logger, IGameObject obj)
        {
            _humanoidNPC.AddToBackpack(logger, obj);
        }

        /// <summary>
        /// Removes game object from backpack.
        /// </summary>
        /// <param name="logger">Logger for method call.</param>
        /// <param name="obj">Instance of the game object.</param>
        protected void RemoveFromBackpack(IMonitorLogger logger, IGameObject obj)
        {
            _humanoidNPC.RemoveFromBackpack(logger, obj);
        }

        /// <summary>
        /// Creates a rotation with the specified forward and upwards directions.
        /// It is a wrapper on Quaternion.LookRotation.
        /// This method should be called only in usual (not main) thread.
        /// </summary>
        /// <param name="logger">Logger for method call.</param>
        /// <param name="targetPosition">Position that is looked at.</param>
        /// <returns>Rotation to target position.</returns>
        protected Quaternion GetRotationToPositionInUsualThread(IMonitorLogger logger, System.Numerics.Vector3 targetPosition)
        {
            return GetRotationToPositionInUsualThread(logger, new Vector3(targetPosition.X, targetPosition.Y, targetPosition.Z));
        }

        /// <summary>
        /// Creates a rotation with the specified forward and upwards directions.
        /// It is a wrapper on Quaternion.LookRotation.
        /// This method should be called only in usual (not main) thread.
        /// </summary>
        /// <param name="logger">Logger for method call.</param>
        /// <param name="targetPosition">Position that is looked at.</param>
        /// <returns>Rotation to target position.</returns>
        protected Quaternion GetRotationToPositionInUsualThread(IMonitorLogger logger, Vector3 targetPosition)
        {
            return RunInMainThread<Quaternion>(() => {
                return GetRotationToPositionInMainThread(logger, targetPosition);
            });
        }

        /// <summary>
        /// Creates a rotation with the specified forward and upwards directions.
        /// It is a wrapper on Quaternion.LookRotation.
        /// This method should be called only in main thread.
        /// </summary>
        /// <param name="logger">Logger for method call.</param>
        /// <param name="targetPosition">Position that is looked at.</param>
        /// <returns>Rotation to target position.</returns>
        protected Quaternion GetRotationToPositionInMainThread(IMonitorLogger logger, System.Numerics.Vector3 targetPosition)
        {
            return GetRotationToPositionInMainThread(logger, new Vector3(targetPosition.X, targetPosition.Y, targetPosition.Z));
        }

        /// <summary>
        /// Creates a rotation with the specified forward and upwards directions.
        /// It is a wrapper on Quaternion.LookRotation.
        /// This method should be called only in main thread.
        /// </summary>
        /// <param name="logger">Logger for method call.</param>
        /// <param name="targetPosition">Position that is looked at.</param>
        /// <returns>Rotation to target position.</returns>
        protected Quaternion GetRotationToPositionInMainThread(IMonitorLogger logger, Vector3 targetPosition)
        {
            var heading = targetPosition - transform.position;

            var distance = heading.magnitude;

            var direction = heading / distance;

            return Quaternion.LookRotation(direction);
        }
    }
}