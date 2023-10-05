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
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Components;
using SymOntoClay.UnityAsset.Scriptables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SymOntoClay.UnityAsset.Editors
{
#if UNITY_EDITOR
    [CustomEditor(typeof(World))]
    [CanEditMultipleObjects]
    public class WorldEditor : Editor
    {
        private World _target;

        private bool _showLoggingSection = true;

        private void OnEnable()
        {
            _target = (World)target;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();
            _target.WorldFile = (WorldFile)EditorGUILayout.ObjectField("World File", _target.WorldFile, typeof(WorldFile), false);

            _target.EnableNLP = EditorGUILayout.Toggle("Enable NLP", _target.EnableNLP);

            _showLoggingSection = EditorGUILayout.BeginFoldoutHeaderGroup(_showLoggingSection, "Logging");

            if(_showLoggingSection)
            {
                _target.EnableAddingRemovingFactLoggingInStorages = EditorGUILayout.Toggle("Adding or removing facts", _target.EnableAddingRemovingFactLoggingInStorages);
                _target.KindOfLogicalSearchExplain = (KindOfLogicalSearchExplain)EditorGUILayout.EnumPopup("Logical explain mode", _target.KindOfLogicalSearchExplain);
            }
            
            GUILayout.EndVertical();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
        }
    }
#endif
}
