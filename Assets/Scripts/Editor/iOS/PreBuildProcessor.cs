#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BundleIdPreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
#if UNITY_IOS
        const string bundleId = "Uwb.uvg.edu.gt";

        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, bundleId);

        UnityEngine.Debug.Log($"[BundleIdPreprocessor] iOS bundle id set to: {bundleId}");
#endif
    }
}
#endif