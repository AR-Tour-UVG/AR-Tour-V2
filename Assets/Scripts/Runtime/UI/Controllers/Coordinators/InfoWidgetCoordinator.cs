using UnityEngine;

public sealed class InfoWidgetCoordinator
{
    private readonly UIRouter router;
    private readonly TourViewModel vm;
    private readonly TourBinder binder;

    public InfoWidgetCoordinator(UIRouter r, TourViewModel model, TourBinder b)
    {
        router = r;
        vm = model;
        binder = b;
    }

    public void Show(AreaDefinition area)
    {
        var w = router.ShowOverlay(OverlayType.InfoModal) as InfoWidgetView;
        if (w == null)
            return;
        w.Show(area);
        w.OnContinue += Continue;

        void Continue()
        {
            w.OnContinue -= Continue;
            router.HideOverlay(OverlayType.InfoModal);
            binder.RequestNext(); // advances to next area
        }
    }

    public void Hide() => router.HideOverlay(OverlayType.InfoModal);
}
