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

        protected virtual void Awake()
        {
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;

            GameObjectsRegistry.AddGameObject(gameObject);
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
        
        public IEntityLogger Logger => _worldComponent?.Logger;

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

        public string InsertPublicFact(string text)
        {
            return _worldComponent.InsertPublicFact(text);
        }

        public string InsertPublicFact(RuleInstance fact)
        {
            return _worldComponent.InsertPublicFact(fact);
        }

        public void RemovePublicFact(string id)
        {
            _worldComponent.RemovePublicFact(id);
        }

        public void PushSoundFact(float power, string text)
        {
            Task.Run(() => { _worldComponent.PushSoundFact(power, text); });            
        }

        public void PushSoundFact(float power, RuleInstance fact)
        {
            Task.Run(() => { _worldComponent.PushSoundFact(power, fact); });
        }

        System.Numerics.Vector3 IPlatformSupport.ConvertFromRelativeToAbsolute(SymOntoClay.Core.RelativeCoordinate relativeCoordinate)
        {
            return PlatformSupportHelper.ConvertFromRelativeToAbsolute(transform, relativeCoordinate);
        }

        System.Numerics.Vector3 IPlatformSupport.GetCurrentAbsolutePosition()
        {
            return PlatformSupportHelper.GetCurrentAbsolutePosition(transform);
        }

        float IPlatformSupport.GetDirectionToPosition(System.Numerics.Vector3 position)
        {
            return PlatformSupportHelper.GetDirectionToPosition(transform, position);
        }

        bool IPlatformSupport.CanBeTakenBy(IEntity subject)
        {
            return CanBeTakenBy(subject);
        }
        
        protected virtual bool CanBeTakenBy(IEntity subject)
        {
            return false;
        }
    }
}