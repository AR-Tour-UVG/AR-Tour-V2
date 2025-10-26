using UnityEngine;

public sealed class SettingsCoordinator
{
    private readonly UIRouter router;
    private readonly AudioAtlas audioAtlas;

    public SettingsCoordinator(UIRouter r, AudioAtlas aa)
    {
        router = r;
        audioAtlas = aa;
    }

    public void Show()
    {
        if (router.ShowOverlay(OverlayType.Settings) is not SettingsView v)
            return;

        int volume = Mathf.Clamp(AppPrefs.LoadVolume(), 0, 100);
        int fontPx = NormalizeFontPx(AppPrefs.LoadFontPx());

        v.SetSlider(volume);
        v.SetSelectedFontPx(fontPx);
        v.SetVolumeIcon(LevelFor(volume));

        ApplyVolume(volume);
        ApplyFontPx(fontPx);

        void OnClose()
        {
            Unhook();
            router.HideOverlay(OverlayType.Settings);
        }

        void OnVol(int val)
        {
            val = Mathf.Clamp(val, 0, 100);
            AppPrefs.SaveVolume(val);
            ApplyVolume(val);
            v.SetVolumeIcon(LevelFor(val));
        }

        void OnVolCommit()
        {
            if (audioAtlas && audioAtlas.settingsPreview)
            {
                AudioDirector.Instance.Play(audioAtlas.settingsPreview, 0f, 0.1f);
            }
        }

        void OnFont(int px)
        {
            px = NormalizeFontPx(px);
            AppPrefs.SaveFontPx(px);
            v.SetSelectedFontPx(px);
            ApplyFontPx(px);
        }

        void Unhook()
        {
            v.CloseRequested -= OnClose;
            v.VolumeChanged -= OnVol;
            v.FontPxPicked -= OnFont;
            v.VolumeChangeCommitted -= OnVolCommit;
        }

        v.CloseRequested += OnClose;
        v.VolumeChanged += OnVol;
        v.FontPxPicked += OnFont;
        v.VolumeChangeCommitted += OnVolCommit;
    }

    static VolumeLevel LevelFor(int v)
    {
        int d = Mathf.RoundToInt(v);
        if (d == 0)
            return VolumeLevel.Mute;
        if (d <= 35)
            return VolumeLevel.Low;
        if (d <= 70)
            return VolumeLevel.Med;
        return VolumeLevel.High;
    }

    static int NormalizeFontPx(int px) => (px == 80 || px == 120) ? px : 100;

    static void ApplyVolume(int v)
    {
        AppPrefs.SaveVolume(v);
        AudioDirector.ApplyVolumeFromPrefs();
    }

    static void ApplyFontPx(int px)
    {
        var boot = Object.FindFirstObjectByType<UIBootstrap>();
        if (boot != null)
        {
            boot.ApplyGlobalFontPx(px);
        }
    }
}
