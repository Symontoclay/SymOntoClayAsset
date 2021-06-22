using SymOntoClay.Importers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AssetImporters;
#endif

namespace SymOntoClay.Editors
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SobjFileImporter))]
    [CanEditMultipleObjects]
    public class SobjFileImporterEditor : ScriptedImporterEditor
    {
        protected override bool needsApplyRevert => false;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Describes behaviour and information of NPC or Thing.", EditorStyles.boldLabel);
        }
    }
#endif
}
