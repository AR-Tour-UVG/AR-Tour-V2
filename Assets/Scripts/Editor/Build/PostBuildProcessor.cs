#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public static class PostBuildProcessor
{

    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS)
            return;

        string plistPath = Path.Combine(path, "Info.plist"); 
        var plist = new PlistDocument(); 
        plist.ReadFromFile(plistPath); 
        var root = plist.root; 

        root.SetString(
            "NSNearbyInteractionUsageDescription",
            "Used to perform precise ranging with nearby devices/beacons."
        );
        root.SetString(
            "NSNearbyInteractionAllowOnceUsageDescription",
            "Used to perform precise ranging with nearby devices/beacons."
        );
        root.SetString(
            "NSBluetoothAlwaysUsageDescription",
            "Bluetooth is required to communicate with nearby accessories."
        );
        root.SetString(
            "NSCameraUsageDescription",
            "Camera is used by ARKit for spatial understanding."
        );

        File.WriteAllText(plistPath, plist.WriteToString());
    }
}
#endif
