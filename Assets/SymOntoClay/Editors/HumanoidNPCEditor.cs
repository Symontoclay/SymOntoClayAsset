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

using SymOntoClay;
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
    [CustomEditor(typeof(HumanoidNPC))]
    [CanEditMultipleObjects]
    public class HumanoidNPCEditor : Editor
    {
        private HumanoidNPC _target;
        private CategoriesCustomEditorGUILayout _categoriesCustomEditorGUILayout;

        private void OnEnable()
        {
            _boldLabelStyle = new GUIStyle() { fontStyle = FontStyle.Bold };

            _target = (HumanoidNPC)target;

            _so = new SerializedObject(target);
            _categoriesCustomEditorGUILayout = new CategoriesCustomEditorGUILayout(_target, _so);

            _backPackProperty = _so.FindProperty("Backpack");
        }

        //protected override void OnHeaderGUI()
        //{
        //}

        private bool _showVisionPosition = true;
        //private bool _showHealthPosition = true;
        private bool _showGizmosPosition = true;
        private GUIStyle _boldLabelStyle;

        private SerializedObject _so;
        private SerializedProperty _backPackProperty;

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();

            MainSymOntoClayInfoCustomEditorGUILayout.DrawGUI(_target);

            _categoriesCustomEditorGUILayout.DrawGUI();

            EditorGUILayout.PropertyField(_backPackProperty, true);

            _so.ApplyModifiedProperties();

            _showVisionPosition = EditorGUILayout.BeginFoldoutHeaderGroup(_showVisionPosition, "Vision");

            if(_showVisionPosition)
            {
                _target.Head = (GameObject)EditorGUILayout.ObjectField("Head", _target.Head, typeof(GameObject), true);
                _target.RaysDistance = EditorGUILayout.IntField("Distance", _target.RaysDistance);
                GUILayout.Label("Field of view", _boldLabelStyle);
                _target.TotalRaysAngle = EditorGUILayout.IntField("Angle", _target.TotalRaysAngle);
                _target.TotalRaysInterval = EditorGUILayout.IntField("Angle between rays", _target.TotalRaysInterval);
                GUILayout.Label("Field of focus", _boldLabelStyle);
                _target.FocusRaysAngle = EditorGUILayout.IntField("Angle", _target.FocusRaysAngle);
                _target.FocusRaysInterval = EditorGUILayout.IntField("Angle between rays", _target.FocusRaysInterval);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            //_showHealthPosition = EditorGUILayout.BeginFoldoutHeaderGroup(_showHealthPosition, "Life");

            //if(_showHealthPosition)
            //{
            //    _target.IsImmortal = EditorGUILayout.Toggle("Is Immortal", _target.IsImmortal);
            //    _target.Health = EditorGUILayout.IntField("Health", _target.Health);
            //    _target.IsResurrected = EditorGUILayout.Toggle("Is Resurrected", _target.IsResurrected);
            //    _target.IsInitiallyDead = EditorGUILayout.Toggle("Is Initially Dead", _target.IsInitiallyDead);
            //}

            _showGizmosPosition = EditorGUILayout.BeginFoldoutHeaderGroup(_showGizmosPosition, "Gizmos");

            if(_showGizmosPosition)
            {
                _target.ShowRayCastGizmo = EditorGUILayout.Toggle("Show RayCast Gizmo", _target.ShowRayCastGizmo);
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
