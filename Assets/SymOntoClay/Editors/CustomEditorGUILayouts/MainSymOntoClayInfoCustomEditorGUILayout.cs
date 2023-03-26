using Assets.SymOntoClay.Interfaces;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Helpers;
using SymOntoClay.UnityAsset.Scriptables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEditor;

namespace Assets.SymOntoClay.Editors.CustomEditorGUILayouts
{
    public static class MainSymOntoClayInfoCustomEditorGUILayout
    {
        public static void DrawGUI(IMainSymOntoClayInfo target)
        {
            target.SobjFile = (SobjFile)EditorGUILayout.ObjectField("App File", target.SobjFile, typeof(SobjFile), false);

            var isInstance = PrefabStageUtility.GetCurrentPrefabStage() == null;

            if (isInstance)
            {
                var newIdValue = EditorGUILayout.TextField("Id", target.Id);

                if (target.Id != newIdValue && EditorHelper.IsValidId(newIdValue))
                {
                    UniqueIdRegistry.RemoveId(target.Id);
                    UniqueIdRegistry.AddId(newIdValue);

                    target.Id = newIdValue;
                }
            }
            else
            {
                target.Id = string.Empty;
            }
        }
    }
}
