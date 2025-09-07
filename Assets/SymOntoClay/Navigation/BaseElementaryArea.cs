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

using SymOntoClay.UnityAsset.Components;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Scriptables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

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

        private int _mainThreadId;

        public virtual List<string> DefaultCategories => new List<string>();
        public bool EnableCategories = true;

        public List<string> Categories;

        protected virtual void Awake()
        {
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;

            GameObjectsRegistry.AddGameObject(gameObject);

#if DEBUG
            //Debug.Log($"BaseElementaryArea Awake ('{name}') UniqueIdRegistry.ContainsId(Id)({Id}) = {UniqueIdRegistry.ContainsId(Id)}");
            if (UniqueIdRegistry.ContainsId(Id))
            {
                var oldId = Id;

                Id = $"#id{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}";
            }
            UniqueIdRegistry.AddId(Id);
#endif

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

            settings.Categories = Categories;
            settings.EnableCategories = EnableCategories;

#if DEBUG
            //Debug.Log($"BaseElementaryArea Awake ('{name}') GetType().FullName  = {GetType().FullName}");
            //Debug.Log($"BaseElementaryArea Awake ('{name}') gameObject.GetInstanceID() = {gameObject.GetInstanceID()}");
            //Debug.Log($"BaseElementaryArea Awake ('{name}') Id = {Id}");
            //Debug.Log($"BaseElementaryArea Awake ('{name}') settings  = {settings}");
#endif

            _place = WorldFactory.WorldInstance.GetPlace(settings);


            if (string.IsNullOrWhiteSpace(_idForFacts) && !string.IsNullOrWhiteSpace(Id))
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
            //Debug.Log($"({name}) OnValidate");

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

            if(Categories == null)
            {
                Categories = new List<string>(DefaultCategories);
            }

            if(!Categories.Any())
            {
                Categories.AddRange(DefaultCategories);
            }
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

        void OnDestroy()
        {
            _place.Dispose();
        }

        /// <inheritdoc/>
        protected override void RunInMainThread(Action function)
        {
            if (_mainThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                function();
                return;
            }

            _place.RunInMainThread(function);
        }

        /// <inheritdoc/>
        protected override TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            if (_mainThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                return function();
            }

            return _place.RunInMainThread<TResult>(function);
        }
    }
}
