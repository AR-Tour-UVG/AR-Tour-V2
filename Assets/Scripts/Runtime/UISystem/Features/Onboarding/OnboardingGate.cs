using System;
using UnityEngine;

public static class OnboardingGate
{
    private const string SeenKey = "onboarding_seen_utcbin";

    public static bool ShouldShow(int days)
    {
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

    // Dev helpers
    public static void Reset()
    {
        PlayerPrefs.DeleteKey(SeenKey);
        PlayerPrefs.Save();
    }

    public static string DebugInfo()
    {
        return PlayerPrefs.HasKey(SeenKey)
            ? DateTime.FromBinary(long.Parse(PlayerPrefs.GetString(SeenKey))).ToString("u")
            : "never";
    }
}
