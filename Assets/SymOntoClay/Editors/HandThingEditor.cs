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

using SymOntoClay.UnityAsset.Scriptables;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymOntoClay.UnityAsset.Components;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace SymOntoClay.UnityAsset.Editors
{
#if UNITY_EDITOR
    [CustomEditor(typeof(HandThing))]
    [CanEditMultipleObjects]
    public class HandThingEditor : Editor
    {
        private HandThing _target;

        private void OnEnable()
        {
            _target = (HandThing)target;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();

            _target.SobjFile = (SobjFile)EditorGUILayout.ObjectField("App File", _target.SobjFile, typeof(SobjFile), false);

            var newIdValue = EditorGUILayout.TextField("Id", _target.Id);

            if (_target.Id != newIdValue && IsValidId(newIdValue))
            {
                UniqueIdRegistry.RemoveId(_target.Id);
                UniqueIdRegistry.AddId(newIdValue);

                _target.Id = newIdValue;
            }

            _target.TakingPolicy = (TakingPolicy)EditorGUILayout.EnumPopup("Taking Policy", _target.TakingPolicy);

            if (_target.TakingPolicy == TakingPolicy.ByDistance)
            {
                _target.TakingDistance = EditorGUILayout.FloatField("Taking Distance", _target.TakingDistance);
            }

            GUILayout.EndVertical();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
        }

        private bool IsValidId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            if (!id.StartsWith("#"))
            {
                return false;
            }

            if (UniqueIdRegistry.ContainsId(id))
            {
                return false;
            }

            return true;
        }
    }
#endif
}
