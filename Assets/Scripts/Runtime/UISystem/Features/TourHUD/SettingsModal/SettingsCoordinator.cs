using UnityEngine;

public sealed class SettingsCoordinator
{
    private readonly UIRouter router;

    public SettingsCoordinator(UIRouter r)
    {
        router = r;
    }

    public void Show()
    {
        if (router.ShowOverlay(OverlayType.Settings) is not SettingsView v)
            return;

        // load persisted or defaults
        int volume = Mathf.Clamp(AppPrefs.LoadVolume(), 0, 100);
        int fontPx = NormalizeFontPx(AppPrefs.LoadFontPx()); // 80/100/120

        // push into UI
        v.SetSlider(volume);
        v.SetSelectedFontPx(fontPx);

        // apply to systems once on open
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
        }

        void OnFont(int px)
        {
            px = NormalizeFontPx(px);
            AppPrefs.SaveFontPx(px);
            v.SetSelectedFontPx(px); // visuals only
            ApplyFontPx(px); // effect
        }

        void Unhook()
        {
            v.CloseRequested -= OnClose;
            v.VolumeChanged -= OnVol;
            v.FontPxPicked -= OnFont;
        }

        v.CloseRequested += OnClose;
        v.VolumeChanged += OnVol;
        v.FontPxPicked += OnFont;
    }

    static int NormalizeFontPx(int px) => (px == 80 || px == 120) ? px : 100;

    static void ApplyVolume(int v)
    {
        // example mapping
        AudioListener.volume = v / 100f;
        // or hand off to your media player
    }

    static void ApplyFontPx(int px)
    {
        var boot = Object.FindFirstObjectByType<UIBootstrap>();
        boot?.ApplyGlobalFontPx(px);
    }
}
