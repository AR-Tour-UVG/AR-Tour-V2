using UnityEngine;

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
            // Reset if already showing
            Hide();
            return;
        }

        var w = router.ShowOverlay(OverlayType.InfoModal) as InfoWidgetView;

        if (w == null)
            return;

        w.Show(area);

        w.OnContinue += Continue;

        void Continue()
        {
            w.OnContinue -= Continue;
            showing = false;
            router.HideOverlay(OverlayType.InfoModal);
            AudioDirector.Instance.Stop(0.2f); // fade out any audio associated with the widget
            binder.RequestNext(); // advances to next area
        }
    }

    public void Hide()
    {
        showing = false;
        router.HideOverlay(OverlayType.InfoModal);
    }
}
