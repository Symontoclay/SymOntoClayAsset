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
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace SymOntoClay.UnityAsset.Editors
{
#if UNITY_EDITOR
    [CustomEditor(typeof(LinkingWaypoint))]
    [CanEditMultipleObjects]
    public class LinkingWaypointEditor : Editor
    {
        private LinkingWaypoint _target;

        private void OnEnable()
        {
            _target = (LinkingWaypoint)target;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();

            _target.SobjFile = (SobjFile)EditorGUILayout.ObjectField("App File", _target.SobjFile, typeof(SobjFile), false);

            var newIdValue = EditorGUILayout.TextField("Id", _target.Id);

            if (_target.Id != newIdValue && EditorHelper.IsValidId(newIdValue))
            {
                UniqueIdRegistry.RemoveId(_target.Id);
                UniqueIdRegistry.AddId(newIdValue);

                _target.Id = newIdValue;
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
