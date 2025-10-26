public sealed class InfoWidgetCoordinator
{
    private readonly UIRouter router;
    private readonly TourViewModel vm;
    private readonly TourBinder binder;

    private bool showing;

    public InfoWidgetCoordinator(UIRouter r, TourViewModel model, TourBinder b)
    {
        router = r;
        vm = model;
        binder = b;
    }

    public void Show(AreaDefinition area)
    {
        if (showing)
        {
            Hide();
            return;
        }

        var w = router.ShowOverlay(OverlayType.InfoModal) as InfoWidgetView;

        if (w == null)
            return;

        showing = true;
        w.Hidden -= OnHidden;
        w.Hidden += OnHidden;

        w.OnContinue += Continue;

        w.Show(area);

        void Continue()
        {
            w.OnContinue -= Continue;
            Hide();
        }
    }

    public void Hide()
    {
        var w = router.GetOverlay<InfoWidgetView>(OverlayType.InfoModal);
        if (w == null)
        {
            showing = false;
            return;
        }
        w.Hide();
    }

    void OnHidden()
    {
        var w = router.GetOverlay<InfoWidgetView>(OverlayType.InfoModal);
        if (w != null)
            w.Hidden -= OnHidden;

        showing = false;
        router.HideOverlay(OverlayType.InfoModal);
    }
}
