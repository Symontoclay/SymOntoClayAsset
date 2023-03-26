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

using Assets.SymOntoClay.Components.Helpers;
using Assets.SymOntoClay.Interfaces;
using SymOntoClay.UnityAsset.Components;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Scriptables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace SymOntoClay.UnityAsset.Navigation
{
    public abstract class BaseElementaryArea: AbstractArea, IŅategorized, IMainSymOntoClayInfo
    {
        public SobjFile SobjFile;
        public string Id;

        private string _oldName;
        private string _idForFacts;

        public string IdForFacts => _idForFacts;

        private IPlace _place;

        /// <inheritdoc/>
        SobjFile IMainSymOntoClayInfo.SobjFile { get => SobjFile; set => SobjFile = value; }

        /// <inheritdoc/>
        string IMainSymOntoClayInfo.Id { get => Id; set => Id = value; }

        /// <inheritdoc/>
        string IMainSymOntoClayInfo.IdForFacts { get => _idForFacts; set => _idForFacts = value; }

        /// <inheritdoc/>
        string IMainSymOntoClayInfo.Name => name;

        /// <inheritdoc/>
        string IMainSymOntoClayInfo.OldName { get => _oldName; set => _oldName = value; }

        public virtual List<string> DefaultCategories => new List<string>();
        public bool EnableCategories = true;

        public List<string> Categories;

        /// <inheritdoc/>
        List<string> IŅategorized.Categories { get => Categories; set => Categories = value; }

        /// <inheritdoc/>
        bool IŅategorized.EnableCategories { get => EnableCategories; set => EnableCategories = value; }

        protected virtual void Awake()
        {
            GameObjectsRegistry.AddGameObject(gameObject);

            MainSymOntoClayInfoComponentHelper.CheckAndFixId(this);

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

#if UNITY_EDITOR
            //Debug.Log($"({name}) Id = {Id}");
            //Debug.Log($"({name}) _idForFacts = {_idForFacts}");
#endif
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            //Debug.Log($"({name}) OnValidate");

            MainSymOntoClayInfoComponentHelper.Validate(this);
            ŅategorizedComponentHelper.Validate(this);
        }
#endif

        void Stop()
        {
            _place.Dispose();
        }

        /// <inheritdoc/>
        protected override void RunInMainThread(Action function)
        {
            if (Thread.CurrentThread.ManagedThreadId == 1)
            {
                function();
                return;
            }

            _place.RunInMainThread(function);
        }

        /// <inheritdoc/>
        protected override TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            if (Thread.CurrentThread.ManagedThreadId == 1)
            {
                return function();
            }

            return _place.RunInMainThread(function);
        }
    }
}
