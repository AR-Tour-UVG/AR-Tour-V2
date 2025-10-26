#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor;

public class BundleIdPreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
#if UNITY_IOS
        const string bundleId = "uwb.uvg.edu.gt.gus";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, bundleId);
        UnityEngine.Debug.Log($"[BundleIdPreprocessor] iOS bundle id set to: {bundleId}");
#endif
    }
}
#endif
