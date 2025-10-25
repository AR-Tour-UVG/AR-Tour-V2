#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class ARDevMenu
{
    static TourBinder B() => Object.FindFirstObjectByType<TourBinder>(FindObjectsInactive.Include);

    static FloorManager FM() =>
        Object.FindFirstObjectByType<FloorManager>(FindObjectsInactive.Include);

    [MenuItem("AR Tour Dev Tools/Onboarding/Reset State")]
    public static void ResetOnboarding()
    {
        OnboardingGate.Reset();
        Debug.Log("[Dev Tools] Onboarding progress reset.");
    }

    [MenuItem("AR Tour Dev Tools/AppPrefs/Reset Preferences")]
    public static void ResetPreferences()
    {
        AppPrefs.ClearAll();
        Debug.Log("[Dev Tools] Preferences reset.");
    }

    [MenuItem("AR Tour Dev Tools/Simulation/Connect")]
    public static void Connect()
    {
        if (B() == null)
        {
            Debug.LogWarning("[Dev Tools] No TourBinder found in the scene.");
            return;
        }
        B().SetConnection(true);
        Debug.Log("[Dev Tools] Simulated connection established.");
    }

    [MenuItem("AR Tour Dev Tools/Simulation/Disconnect")]
    public static void Disconnect()
    {
        if (B() == null)
        {
            Debug.LogWarning("[Dev Tools] No TourBinder found in the scene.");
            return;
        }
        B().SetConnection(false);
        Debug.Log("[Dev Tools] Simulated connection disconnected.");
    }

    [MenuItem("AR Tour Dev Tools/Floor/User Ready (R)")]
    public static void Ready()
    {
        if (FM() == null)
        {
            Debug.LogWarning("[Dev Tools] No FloorManager found in the scene.");
            return;
        }
        FM().UserReady();
        Debug.Log("[Dev Tools] User marked as ready for floor transition.");
    }

    [MenuItem("AR Tour Dev Tools/Floor/Next (N)")]
    public static void Next()
    {
        if (FM() == null)
        {
            Debug.LogWarning("[Dev Tools] No FloorManager found in the scene.");
            return;
        }
        FM().Next();
    }
}
#endif
