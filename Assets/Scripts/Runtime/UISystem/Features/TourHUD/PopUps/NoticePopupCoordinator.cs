using UnityEngine.UIElements;

public sealed class NoticePopupCoordinator
{
    private readonly UIRouter router;
    private readonly UIAtlas atlas;

    public NoticePopupCoordinator(UIRouter r, UIAtlas a)
    {
        router = r;
        atlas = a;
    }

    public void ShowConnecting() => ShowWithData(atlas.NoticeConnectingData);

    public void ShowLostConnection() => ShowWithData(atlas.NoticeLostConnectionData);

    public void ShowTourComplete() => ShowWithData(atlas.NoticeTourCompleteData);

    public void Hide()
    {
        var v = router.GetOverlay<NoticePopupView>(OverlayType.NoticePopup);
        if (v == null)
            return;
        v.Hide();
    }

    void ShowWithData(NoticeData data)
    {
        var action = router.GetOverlay<ActionPopupView>(OverlayType.ActionPopup);
        if (action != null && action.Root.style.display != DisplayStyle.None)
        {
            void AfterActionHidden()
            {
                action.Hidden -= AfterActionHidden;
                ActuallyShow(data);
            }
            action.Hidden -= AfterActionHidden;
            action.Hidden += AfterActionHidden;
            action.Hide();
            return;
        }

        ActuallyShow(data);
    }

    void ActuallyShow(NoticeData data)
    {
        var v = router.ShowOverlay(OverlayType.NoticePopup) as NoticePopupView;
        if (v == null)
            return;

        void OnHidden()
        {
            v.Hidden -= OnHidden;
            router.HideOverlay(OverlayType.NoticePopup);
        }
        v.Hidden -= OnHidden;
        v.Hidden += OnHidden;

        v.Show(data);
        if (data != null && data.NoticeAudioClip)
            AudioDirector.Instance.Play(data.NoticeAudioClip);
    }
}
