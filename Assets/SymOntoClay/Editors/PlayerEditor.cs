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

using SymOntoClay.UnityAsset.Scriptables;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymOntoClay.UnityAsset.Components;
using SymOntoClay.UnityAsset.Helpers;
using UnityEditor.SceneManagement;
using Assets.SymOntoClay.Editors.CustomEditorGUILayouts;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SymOntoClay.UnityAsset.Editors
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Player))]
    [CanEditMultipleObjects]
    public class PlayerEditor : Editor
    {
        private Player _target;
        private SerializedObject _so;
        private CategoriesCustomEditorGUILayout _categoriesCustomEditorGUILayout;

        private void OnEnable()
        {
            _boldLabelStyle = new GUIStyle() { fontStyle = FontStyle.Bold };

            _target = (Player)target;
            _so = new SerializedObject(target);
            _categoriesCustomEditorGUILayout = new CategoriesCustomEditorGUILayout(_target, _so);
        }

        private GUIStyle _boldLabelStyle;

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();

            MainSymOntoClayInfoCustomEditorGUILayout.DrawGUI(_target);

            _categoriesCustomEditorGUILayout.DrawGUI();

            _so.ApplyModifiedProperties();

            GUILayout.EndVertical();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
        }
    }
#endif
}
