using UnityEngine;

public sealed class MenuCoordinator
{
    private readonly UIRouter router;
    private readonly AudioAtlas audioAtlas;
    private readonly UIAtlas uiAtlas;

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
            Unhook();
            router.HideOverlay(OverlayType.Menu);
        }
        void ReturnHome()
        {
            Close();
            var tr = TourRunner.Instance;
            if (tr == null)
                return;
            tr.StopTour(true);
            router.ShowScreen(ScreenState.Home);
        }
        void Restart()
        {
            var tr = TourRunner.Instance;
            if (tr == null)
                return;
            var currentTour = tr.CurrentTour;
            tr.StopTour(false);
            tr.SelectTour(currentTour);
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
        }

        m.OnClose += Close;
        m.OnReturnHome += ReturnHome;
        m.OnRestart += Restart;
        m.OnHelp += Help;
        m.OnSettings += Settings;

        m.Show();
    }
}
