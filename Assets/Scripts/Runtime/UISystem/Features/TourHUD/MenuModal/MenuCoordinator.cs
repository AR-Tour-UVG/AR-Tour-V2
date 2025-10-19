public sealed class MenuCoordinator
{
    private readonly UIRouter router;

    public MenuCoordinator(UIRouter r)
    {
        router = r;
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
            router.ShowScreen(ScreenState.Home);
        }
        void Restart()
        {
            Close(); /* TODO: trigger tour restart */
        }
        void Help()
        { /* TODO */
        }
        void Settings()
        { /* TODO */
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
