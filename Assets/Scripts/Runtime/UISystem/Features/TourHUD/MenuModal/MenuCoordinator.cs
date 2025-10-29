using UnityEngine;

public sealed class MenuCoordinator
{
    private readonly UIRouter router;
    private readonly AudioAtlas audioAtlas;

    public MenuCoordinator(UIRouter r, UIAtlas ua, AudioAtlas aa)
    {
        router = r;
        audioAtlas = aa;
    }

    public void Show()
    {
        var m = router.ShowOverlay(OverlayType.Menu) as MenuView;
        if (m == null)
            return;

        void Close()
        {
            m.Hide();
        }

        void OnHidden()
        {
            Unhook();
            router.HideOverlay(OverlayType.Menu);
        }

        void ReturnHome()
        {
            Close();
            router.HideAllOverlays();
            router.ShowScreen(ScreenState.Home);
            AudioDirector.Instance.Stop(0.12f);

            var tr = TourRunner.Instance;
            if (tr == null)
                return;
            tr.StopTour(true);
        }

        void Restart()
        {
            var tr = TourRunner.Instance;
            if (tr == null)
                return;
            var current = tr.CurrentTour;
            tr.StopTour(false);
            tr.SelectTour(current);
            tr.BeginTour();
            Close();
        }

        void Help()
        {
            Debug.Log("[MenuCoordinator] Help pressed.");
        }

        void Settings()
        {
            var settings = new SettingsCoordinator(router, audioAtlas);
            settings.Show();
        }

        void Unhook()
        {
            m.OnClose -= Close;
            m.OnReturnHome -= ReturnHome;
            m.OnRestart -= Restart;
            m.OnHelp -= Help;
            m.OnSettings -= Settings;
            m.Hidden -= OnHidden;
        }

        m.OnClose += Close;
        m.OnReturnHome += ReturnHome;
        m.OnRestart += Restart;
        m.OnHelp += Help;
        m.OnSettings += Settings;
        m.Hidden += OnHidden;

        m.Show();
    }
}
