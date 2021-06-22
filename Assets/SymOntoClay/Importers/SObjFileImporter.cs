using SymOntoClay.Helpers;
using SymOntoClay.Scriptables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.AssetImporters;
#endif
using UnityEngine;

namespace SymOntoClay.Importers
{
#if UNITY_EDITOR
    [ScriptedImporter(0, ".sobj")]
    public class SobjFileImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var fileName = FileHelper.RemoveCommonFragment(ctx.assetPath);
            var obj = ObjectFactory.CreateInstance<SobjFile>();
            obj.name = Path.GetFileNameWithoutExtension(fileName);
            obj.FullName = fileName;
            ctx.AddObjectToAsset("main", obj);
            ctx.SetMainObject(obj);
        }
    }
#endif
}
