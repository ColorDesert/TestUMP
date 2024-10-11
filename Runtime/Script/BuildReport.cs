#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEngine;

namespace Script
{
    public class BuildReport : IPostprocessBuildWithReport,IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }
        public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            Debug.Log("build 前："+report.packedAssets[0].name);
        }
        public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            Debug.Log("build 后："+report.packedAssets[0].name);
        }
    }
}
#endif
