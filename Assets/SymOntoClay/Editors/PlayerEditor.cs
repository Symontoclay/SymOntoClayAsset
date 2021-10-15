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
    [CustomEditor(typeof(Player))]
    [CanEditMultipleObjects]
    public class PlayerEditor : Editor
    {
        private Player _target;

        private void OnEnable()
        {
            _boldLabelStyle = new GUIStyle() { fontStyle = FontStyle.Bold };

            _target = (Player)target;
        }

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
