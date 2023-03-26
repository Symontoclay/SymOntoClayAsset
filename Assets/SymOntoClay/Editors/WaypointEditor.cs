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

using Assets.SymOntoClay.Editors.CustomEditorGUILayouts;
using SymOntoClay.UnityAsset.Components;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Helpers;
using SymOntoClay.UnityAsset.Navigation;
using SymOntoClay.UnityAsset.Scriptables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace SymOntoClay.UnityAsset.Editors
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Waypoint))]
    [CanEditMultipleObjects]
    public class WaypointEditor : Editor
    {
        private Waypoint _target;
        private SerializedObject _so;
        private SerializedProperty _categoriesProperty;
        private CategoriesCustomEditorGUILayout _categoriesCustomEditorGUILayout;

        private void OnEnable()
        {
            _target = (Waypoint)target;
            _so = new SerializedObject(target);
            //_categoriesProperty = _so.FindProperty("Categories");
            _categoriesCustomEditorGUILayout = new CategoriesCustomEditorGUILayout(_target, _so);
        }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();

            _target.SobjFile = (SobjFile)EditorGUILayout.ObjectField("App File", _target.SobjFile, typeof(SobjFile), false);

            var isInstance = PrefabStageUtility.GetCurrentPrefabStage() == null;

            if (isInstance)
            {
                var newIdValue = EditorGUILayout.TextField("Id", _target.Id);

                if (_target.Id != newIdValue && EditorHelper.IsValidId(newIdValue))
                {
                    UniqueIdRegistry.RemoveId(_target.Id);
                    UniqueIdRegistry.AddId(newIdValue);

                    _target.Id = newIdValue;
                }
            }
            else
            {
                _target.Id = string.Empty;
            }

            _categoriesCustomEditorGUILayout.DrawGUI();

            //_target.EnableCategories = EditorGUILayout.Toggle("Enable Categories", _target.EnableCategories);

            //EditorGUILayout.PropertyField(_categoriesProperty, true);

            //if(_target.Categories == null)
            //{
            //    _target.Categories = new List<string>(_target.DefaultCategories);
            //}

            //if(!_target.Categories.Any())
            //{
            //    _target.Categories.AddRange(_target.DefaultCategories);
            //}

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
