using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;

namespace SymOntoClay.Build
{
#if UNITY_EDITOR
    public class BuildProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        /// <summary>
        /// This method is called after build (publish) game and before finish.
        /// You can write your code for copying target files here.
        /// </summary>
        /// <param name="report"></param>
        public void OnPostprocessBuild(BuildReport report)
        {
            BuildPipeLine.CopyFiles(Application.dataPath, report.summary.outputPath);
        }
    }
#endif
}
