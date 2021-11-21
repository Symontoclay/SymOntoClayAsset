using SymOntoClay.Scriptables;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace SymOntoClay.Editors
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
