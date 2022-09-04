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

namespace SymOntoClay.UnityAsset.Components
{
    public abstract class BaseSymOntoClayGameObject : MonoBehaviour, IPlatformSupport, IGameObjectBehavior
    {
        public SobjFile SobjFile;
        public string Id;

        private string _oldName;
        private string _idForFacts;

        public string IdForFacts => _idForFacts;

        protected virtual void Awake()
        {
            GameObjectsRegistry.AddGameObject(gameObject);
        }

        protected virtual void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                Id = GetIdByName();
            }
            else
            {
                var id = GetIdByName();

                if (Id == GetIdByName(_oldName))
                {
                    Id = id;
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

            _oldName = name;
        }

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
            _worldComponent?.RunInMainThread(function);
        }

        public TResult RunInMainThread<TResult>(Func<TResult> function)
        {
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
            var distance = relativeCoordinate.Distance;
            var angle = relativeCoordinate.HorizontalAngle;

#if DEBUG
            //Debug.Log($"BaseSymOntoClayGameObject ConvertFromRelativeToAbsolute angle = {angle}");
            //Debug.Log($"BaseSymOntoClayGameObject ConvertFromRelativeToAbsolute distance = {distance}");
#endif

            var radAngle = angle * Mathf.Deg2Rad;
            var x = Mathf.Sin(radAngle);
            var y = Mathf.Cos(radAngle);
            var localDirection = new Vector3(x * distance, 0f, y * distance);

#if DEBUG
            //Debug.Log($"BaseSymOntoClayGameObject ConvertFromRelativeToAbsolute localDirection = {localDirection}");
#endif

            var globalDirection = transform.TransformDirection(localDirection);

            var newPosition = globalDirection + transform.position;

            return new System.Numerics.Vector3(newPosition.x, newPosition.y, newPosition.z);
        }

        System.Numerics.Vector3 IPlatformSupport.GetCurrentAbsolutePosition()
        {
            var currPosition = transform.position;

            return new System.Numerics.Vector3(currPosition.x, currPosition.y, currPosition.z);
        }

        float IPlatformSupport.GetDirectionToPosition(System.Numerics.Vector3 position)
        {
#if DEBUG
            //Debug.Log($"BaseSymOntoClayGameObject ConvertFromRelativeToAbsolute position = {position}");
#endif

            var aimPosition = new Vector3(position.X, position.Y, position.Z);

            var myPosition = transform.position;

#if UNITY_EDITOR
            //Debug.Log($"aimPosition = {aimPosition}");
            //Debug.Log($"myPosition = {myPosition}");
#endif

            var heading = aimPosition - myPosition;

#if UNITY_EDITOR
            //Debug.Log($"heading = {heading}");
#endif

            var distance = heading.magnitude;

            var direction = heading / distance;

#if UNITY_EDITOR
            //Debug.Log($"distance = {distance}");
            //Debug.Log($"direction = {direction}");
#endif

            var rotation = Quaternion.FromToRotation(Vector3.forward, direction);

#if UNITY_EDITOR
            //Debug.Log($"rotation = {rotation}");
#endif

            var angle = Quaternion.Angle(transform.rotation, rotation);

#if UNITY_EDITOR
            //Debug.Log($"angle = {angle}");
#endif

            return angle;
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