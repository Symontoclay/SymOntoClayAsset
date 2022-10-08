using SymOntoClay.UnityAsset.Components;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Scriptables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace SymOntoClay.UnityAsset.Navigation
{
    public abstract class BaseElementaryArea: AbstractArea
    {
        public SobjFile SobjFile;
        public string Id;

        private string _oldName;
        private string _idForFacts;

        public string IdForFacts => _idForFacts;

        private IPlace _place;

        protected virtual void Awake()
        {
            GameObjectsRegistry.AddGameObject(gameObject);

            var settings = new PlaceSettings();
            settings.Id = Id;
            settings.InstanceId = gameObject.GetInstanceID();
            settings.AllowPublicPosition = true;
            settings.UseStaticPosition = new System.Numerics.Vector3(transform.position.x, transform.position.y, transform.position.z);

            if(SobjFile != null)
            {
                var fullFileName = Path.Combine(Application.dataPath, SobjFile.FullName);
                settings.HostFile = fullFileName;
            }
            
            settings.PlatformSupport = this;

            _place = WorldFactory.WorldInstance.GetPlace(settings);
        }

        protected virtual void OnValidate()
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

        void Stop()
        {
            _place.Dispose();
        }

        /// <inheritdoc/>
        protected override void RunInMainThread(Action function)
        {
            _place.RunInMainThread(function);
        }

        /// <inheritdoc/>
        protected override TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            return _place.RunInMainThread<TResult>(function);
        }
    }
}
