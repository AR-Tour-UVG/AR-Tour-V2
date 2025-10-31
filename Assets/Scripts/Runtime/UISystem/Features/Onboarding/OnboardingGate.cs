using System;
using UnityEngine;

public static class OnboardingGate
{
    private const string SeenKey = "onboarding_seen_utcbin";
    private const string ForceKey = "onboarding_force_always";

    public static bool ShouldShow(int days)
    {
        if (GetForceAlways())
            return true;

        if (days <= 0)
            return !PlayerPrefs.HasKey(SeenKey);

        if (!PlayerPrefs.HasKey(SeenKey))
            return true;

        var last = DateTime.FromBinary(long.Parse(PlayerPrefs.GetString(SeenKey)));
        return (DateTime.UtcNow - last).TotalDays >= days;
    }

    public static void MarkSeen()
    {
        PlayerPrefs.SetString(SeenKey, DateTime.UtcNow.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    public static void SetForceAlways(bool enabled)
    {
        PlayerPrefs.SetInt(ForceKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void Reset()
    {
        PlayerPrefs.DeleteKey(SeenKey);
        PlayerPrefs.Save();
    }

    public static bool GetForceAlways()
    {
        return PlayerPrefs.GetInt(ForceKey, 0) == 1;
    }

    public static string DebugInfo()
    {
        return PlayerPrefs.HasKey(SeenKey)
            ? DateTime.FromBinary(long.Parse(PlayerPrefs.GetString(SeenKey))).ToString("u")
            : "never";
    }
}
