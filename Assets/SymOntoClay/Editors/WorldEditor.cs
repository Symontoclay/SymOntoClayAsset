using SymOntoClay.Scriptables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SymOntoClay.Editors
{
#if UNITY_EDITOR
    [CustomEditor(typeof(World))]
    [CanEditMultipleObjects]
    public class WorldEditor : Editor
    {
        private World _target;

        private void OnEnable()
        {
            _target = (World)target;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();
            _target.WorldFile = (WorldFile)EditorGUILayout.ObjectField("World File", _target.WorldFile, typeof(WorldFile), false);
            GUILayout.EndVertical();
        }
    }
#endif
}
