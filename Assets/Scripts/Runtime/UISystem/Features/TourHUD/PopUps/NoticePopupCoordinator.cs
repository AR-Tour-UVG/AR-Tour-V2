public sealed class NoticePopupCoordinator
{
    private readonly UIRouter router;
    private readonly UIAtlas atlas;

    public NoticePopupCoordinator(UIRouter r, UIAtlas a)
    {
        router = r;
        atlas = a;
    }

    public void ShowConnecting()
    {
        var v = router.ShowOverlay(OverlayType.NoticePopup) as NoticePopupView;
        if (v == null)
            return;
        v.Show(atlas.NoticeConnectingData);
    }

    public void ShowLostConnection()
    {
        var v = router.ShowOverlay(OverlayType.NoticePopup) as NoticePopupView;
        if (v == null)
            return;
        v.Show(atlas.NoticeLostConnectionData);
    }

    public void ShowTourComplete()
    {
        var v = router.ShowOverlay(OverlayType.NoticePopup) as NoticePopupView;
        if (v == null)
            return;
        v.Show(atlas.NoticeTourCompleteData);
    }

    public void Hide() => router.HideOverlay(OverlayType.NoticePopup);
}
