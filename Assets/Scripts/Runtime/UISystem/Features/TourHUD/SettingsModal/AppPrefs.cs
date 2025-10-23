using UnityEngine;

public static class AppPrefs
{
    const string VolumeKey = "app.volume";
    const string FontPxKey = "app.fontpx";
    const string FirstRunKey = "app.firstRun";

    public static bool IsFirstRun()
    {
        if (PlayerPrefs.GetInt(FirstRunKey, 1) == 1)
        {
            PlayerPrefs.SetInt(FirstRunKey, 0);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    public static int LoadVolume() => PlayerPrefs.GetInt(VolumeKey, 50);

    public static int LoadFontPx() => PlayerPrefs.GetInt(FontPxKey, 100);

    public static void SaveVolume(int v)
    {
        PlayerPrefs.SetInt(VolumeKey, v);
        PlayerPrefs.Save();
    }

    public static void SaveFontPx(int px)
    {
        PlayerPrefs.SetInt(FontPxKey, px);
        PlayerPrefs.Save();
    }
}
