using SymOntoClay;
using SymOntoClay.Scriptables;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace SymOntoClay.Editors
{
#if UNITY_EDITOR
    [CustomEditor(typeof(HumanoidNPC))]
    [CanEditMultipleObjects]
    public class HumanoidNPCEditor : Editor
    {
        private HumanoidNPC _target;

        private void OnEnable()
        {
            _boldLabelStyle = new GUIStyle() { fontStyle = FontStyle.Bold };

            _target = (HumanoidNPC)target;
        }

        //protected override void OnHeaderGUI()
        //{
        //}

        private bool _showVisionPosition = true;
        private bool _showHealthPosition = true;
        private GUIStyle _boldLabelStyle;

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

            _showHealthPosition = EditorGUILayout.BeginFoldoutHeaderGroup(_showHealthPosition, "Life");

            if(_showHealthPosition)
            {
                _target.IsImmortal = EditorGUILayout.Toggle("Is Immortal", _target.IsImmortal);
                _target.Health = EditorGUILayout.IntField("Health", _target.Health);
                _target.IsResurrected = EditorGUILayout.Toggle("Is Resurrected", _target.IsResurrected);
                _target.IsInitiallyDead = EditorGUILayout.Toggle("Is Initially Dead", _target.IsInitiallyDead);
            }

            GUILayout.EndVertical();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
        }

        private bool IsValidId(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            if(!id.StartsWith("#"))
            {
                return false;
            }

            if(UniqueIdRegistry.ContainsId(id))
            {
                return false;
            }

            return true;
        }
    }
#endif
}
