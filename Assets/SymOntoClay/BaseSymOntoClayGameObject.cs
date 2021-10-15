using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Scriptables;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections;
using UnityEngine;

namespace SymOntoClay
{
    public abstract class BaseSymOntoClayGameObject : MonoBehaviour, IPlatformSupport, IUSocGameObject
    {
        public SobjFile SobjFile;
        public string Id;

        private string _oldName;
        private string _idForFacts;

        public string IdForFacts => _idForFacts;

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
        }

        private IWorldComponent _worldComponent;

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

        public void RemovePublicFact(string id)
        {
            _worldComponent.RemovePublicFact(id);
        }

        public void PushSoundFact(float power, string text)
        {
            _worldComponent.PushSoundFact(power, text);
        }

        System.Numerics.Vector3 IPlatformSupport.ConvertFromRelativeToAbsolute(SymOntoClay.Core.RelativeCoordinate relativeCoordinate)
        {
            var distance = relativeCoordinate.Distance;
            var angle = relativeCoordinate.HorizontalAngle;

#if DEBUG
            //Debug.Log($"HumanoidNPC ConvertFromRelativeToAbsolute angle = {angle}");
            //Debug.Log($"HumanoidNPC ConvertFromRelativeToAbsolute distance = {distance}");
#endif

            var radAngle = angle * Mathf.Deg2Rad;
            var x = Mathf.Sin(radAngle);
            var y = Mathf.Cos(radAngle);
            var localDirection = new Vector3(x * distance, 0f, y * distance);

#if DEBUG
            //Debug.Log($"HumanoidNPC ConvertFromRelativeToAbsolute localDirection = {localDirection}");
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
            throw new NotImplementedException();
        }
    }
}