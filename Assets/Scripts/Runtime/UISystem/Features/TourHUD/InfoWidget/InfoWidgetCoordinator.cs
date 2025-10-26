using UnityEngine;

public sealed class InfoWidgetCoordinator
{
    private readonly UIRouter router;
    private readonly TourBinder binder;

    private bool showing;

    public InfoWidgetCoordinator(UIRouter r, TourViewModel model, TourBinder b)
    {
        router = r;
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

        void Continue()
        {
            w.OnContinue -= Continue;

            AudioDirector.Instance.Stop(0.2f);
            binder.RequestNext();

            w.Hide();
        }

        w.OnContinue += Continue;
        w.Show(area);
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

    private void OnHidden()
    {
        var w = router.GetOverlay<InfoWidgetView>(OverlayType.InfoModal);
        if (w != null)
            w.Hidden -= OnHidden;

        showing = false;
        router.HideOverlay(OverlayType.InfoModal);
    }
}
