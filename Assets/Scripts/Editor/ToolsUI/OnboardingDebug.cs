using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public static class OnboardingDebug
{
    [MenuItem("AR Tour Dev Tools/Onboarding/Reset")]
    public static void ResetOnboarding()
    {
        OnboardingGate.Reset();
        Debug.Log("Onboarding progress reset.");
    }
}
#endif
