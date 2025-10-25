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
        if (atlas.NoticeConnectingData != null && atlas.NoticeConnectingData.NoticeAudioClip)
        {
            AudioDirector.Instance.Play(atlas.NoticeConnectingData.NoticeAudioClip);
        }
    }

    public void ShowLostConnection()
    {
        var v = router.ShowOverlay(OverlayType.NoticePopup) as NoticePopupView;
        if (v == null)
            return;
        v.Show(atlas.NoticeLostConnectionData);
        if (
            atlas.NoticeLostConnectionData != null
            && atlas.NoticeLostConnectionData.NoticeAudioClip
        )
        {
            AudioDirector.Instance.Play(atlas.NoticeLostConnectionData.NoticeAudioClip);
        }
    }

    public void ShowTourComplete()
    {
        var v = router.ShowOverlay(OverlayType.NoticePopup) as NoticePopupView;
        if (v == null)
            return;
        v.Show(atlas.NoticeTourCompleteData);
        if (atlas.NoticeTourCompleteData != null && atlas.NoticeTourCompleteData.NoticeAudioClip)
        {
            AudioDirector.Instance.Play(atlas.NoticeTourCompleteData.NoticeAudioClip);
        }
    }

    public void Hide() => router.HideOverlay(OverlayType.NoticePopup);
}
