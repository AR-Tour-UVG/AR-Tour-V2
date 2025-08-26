#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BundleIdPreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        // Use all-lowercase if possible; otherwise match EXACTLY what you registered.
        const string bundleId = "gt.edu.uvg.uwb"; // or "Uwb.uvg.edu.gt" if you must match existing

        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, bundleId);

        UnityEngine.Debug.Log($"[BundleIdPreprocessor] iOS bundle id set to: {bundleId}");
    }
}
#endif
