/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.UnityAsset.Scriptables;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using SymOntoClay.UnityAsset.Interfaces;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.UnityAsset.Helpers;
using UnityEditor.SceneManagement;
using System.Threading;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.Monitor.Common;
using NLog;

namespace SymOntoClay.UnityAsset.Components
{
    public abstract class BaseSymOntoClayGameObject : MonoBehaviour, IPlatformSupport, IGameObjectBehavior, IExecutorInMainThread
    {
        public SobjFile SobjFile;
        public string Id;

        private string _oldName;
        private string _idForFacts;

        public string IdForFacts => _idForFacts;

        private int _mainThreadId;

        private InvokerInMainThread _invokerInMainThread;

        protected virtual void Awake()
        {
            _invokerInMainThread = new InvokerInMainThread();

            _mainThreadId = Thread.CurrentThread.ManagedThreadId;

            GameObjectsRegistry.AddGameObject(gameObject);

            if(string.IsNullOrWhiteSpace(_idForFacts) && !string.IsNullOrWhiteSpace(Id))
            {
                if (Id.StartsWith("#`"))
                {
                    _idForFacts = Id;
                }
                else
                {
                    _idForFacts = $"{Id.Insert(1, "`")}`";
                }
            }

#if UNITY_EDITOR
            //Debug.Log($"({name}) Id = {Id}");
            //Debug.Log($"({name}) _idForFacts = {_idForFacts}");
#endif
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            var isInstance = PrefabStageUtility.GetCurrentPrefabStage() == null;

            if (isInstance)
            {
                if (string.IsNullOrWhiteSpace(Id))
                {
                    Id = GetIdByName();
                }
                else
                {
                    if (Id == GetIdByName(_oldName))
                    {
                        Id = GetIdByName();
                    }
                }

                if (Id.StartsWith("#`"))
                {
                    _idForFacts = Id;
                }
                else
                {
                    _idForFacts = $"{Id.Insert(1, "`")}`";
                }
            }

            _oldName = name;
        }
#endif

        protected virtual void Update()
        {
            _currentAbsolutePosition = PlatformSupportHelper.GetCurrentAbsolutePosition(Logger, transform);

            _invokerInMainThread.Update();
        }

        private System.Numerics.Vector3 _currentAbsolutePosition;

        private string GetIdByName()
        {
            return GetIdByName(name);
        }

        private string GetIdByName(string nameStr)
        {
            return $"#{nameStr}";
        }

        protected void SetSelfWorldComponent(IWorldComponent worldComponent)
        {
            _worldComponent = worldComponent;

            _socGameObject = _worldComponent as IGameObject;
        }

        private IWorldComponent _worldComponent;
        private IGameObject _socGameObject;

        public IGameObject SocGameObject => _socGameObject;

        public IStandardFactsBuilder StandardFactsBuilder => _worldComponent.StandardFactsBuilder;
        
        public IMonitorLogger Logger => _worldComponent?.Logger;

        public void RunInMainThread(Action function)
        {
            if(_mainThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                function();
                return;
            }

            _worldComponent?.RunInMainThread(function);
        }

        public TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            if (_mainThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                return function();
            }

            return _worldComponent.RunInMainThread(function);
        }

        public string InsertPublicFact(IMonitorLogger logger, string text)
        {
            return _worldComponent.InsertPublicFact(logger, text);
        }

        public string InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _worldComponent.InsertPublicFact(logger, fact);
        }

        public void RemovePublicFact(IMonitorLogger logger, string id)
        {
            _worldComponent.RemovePublicFact(logger, id);
        }

        public void PushSoundFact(IMonitorLogger logger, float power, string text)
        {
            _worldComponent.PushSoundFact(power, text);
        }

        public void PushSoundFactAsync(IMonitorLogger logger, float power, string text)
        {
            Task.Run(() => {//logged
                var taskId = logger.StartTask("3F95AB51-CD23-4744-88B0-1151048E7046");

                try
                {
                    _worldComponent.PushSoundFact(power, text);
                }
                catch (Exception e)
                {
                    try
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) e = {e}");
#endif

                        logger.Error("178D22F7-1863-4173-9984-0241072120B1", e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) ex = {ex}");
#endif
                    }
                }

                logger.StopTask("6C96A3C9-AF9F-444B-9B42-F3BBDE89F67F", taskId);
            });            
        }

        public void PushSoundFact(IMonitorLogger logger, float power, RuleInstance fact)
        {
            _worldComponent.PushSoundFact(power, fact);
        }

        public void PushSoundFactAsync(IMonitorLogger logger, float power, RuleInstance fact)
        {
            Task.Run(() => {//logged
                var taskId = logger.StartTask("83A11806-08F7-4ED7-9DFD-76DA762AA0A2");

                try
                {
                    _worldComponent.PushSoundFact(power, fact);
                }
                catch (Exception e)
                {
                    try
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) e = {e}");
#endif

                        logger.Error("81F37759-00DC-4739-9CAB-7224DBB7B76B", e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"({name}) ex = {ex}");
#endif
                    }
                }

                logger.StopTask("1F452966-D7F3-4485-8B63-312B5C2FEA4F", taskId);
            });
        }

        System.Numerics.Vector3 IPlatformSupport.ConvertFromRelativeToAbsolute(IMonitorLogger logger, SymOntoClay.Core.RelativeCoordinate relativeCoordinate)
        {
            return _invokerInMainThread.RunInMainThread(() => { return PlatformSupportHelper.ConvertFromRelativeToAbsolute(logger, transform, relativeCoordinate); });
        }

        System.Numerics.Vector3 IPlatformSupport.GetCurrentAbsolutePosition(IMonitorLogger logger)
        {
            return _currentAbsolutePosition;
        }

        float IPlatformSupport.GetDirectionToPosition(IMonitorLogger logger, System.Numerics.Vector3 position)
        {
            return _invokerInMainThread.RunInMainThread(() => { return PlatformSupportHelper.GetDirectionToPosition(logger, transform, position); });  
        }
        
        bool IPlatformSupport.CanBeTakenBy(IMonitorLogger logger, IEntity subject)
        {
            return CanBeTakenBy(logger, subject);
        }
        
        protected virtual bool CanBeTakenBy(IMonitorLogger logger, IEntity subject)
        {
            return false;
        }
    }
}