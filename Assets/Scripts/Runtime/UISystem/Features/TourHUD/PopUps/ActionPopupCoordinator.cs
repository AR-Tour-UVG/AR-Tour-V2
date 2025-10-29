using System;
using UnityEngine.UIElements;

public sealed class ActionPopupCoordinator
{
    private readonly UIRouter router;
    private readonly UIAtlas atlas;
    private readonly TourBinder binder;
    private readonly TourViewModel vm;

    public ActionPopupCoordinator(UIRouter r, UIAtlas a, TourBinder b, TourViewModel model)
    {
        router = r;
        atlas = a;
        binder = b;
        vm = model;
    }

    public void ShowStart() => ShowWith(atlas.ActionStartData, OnStartClick);

    public void ShowReadyOnFloor(string descriptionOverride = null)
    {
        ShowWith(atlas.ActionReadyOnFloorData, OnReadyClick, descriptionOverride);

        void OnReadyClick(ActionPopupView v)
        {
            v.Hide();
            if (vm.Phase == TourUIPhase.FloorTransition)
                TourRunner.Instance?.ContinueToNextFloor();
            else
                binder.RequestUserReady();
        }
    }

    public void Hide()
    {
        var v = router.GetOverlay<ActionPopupView>(OverlayType.ActionPopup);
        if (v == null)
            return;
        v.Hide(); // removal happens on Hidden
    }

    // Helpers

    void ShowWith(ActionData data, Action<ActionPopupView> onClick, string descOverride = null)
    {
        // If Notice popup is up, hide it first and chain
        var notice = router.GetOverlay<NoticePopupView>(OverlayType.NoticePopup);
        if (notice != null && notice.Root.style.display != DisplayStyle.None)
        {
            void AfterNoticeHidden()
            {
                notice.Hidden -= AfterNoticeHidden;
                ActuallyShow(data, onClick, descOverride);
            }
            notice.Hidden -= AfterNoticeHidden;
            notice.Hidden += AfterNoticeHidden;
            notice.Hide();
            return;
        }

        ActuallyShow(data, onClick, descOverride);
    }

    void ActuallyShow(ActionData data, Action<ActionPopupView> onClick, string descOverride)
    {
        if (router.CurrentScreen != ScreenState.TourHUD)
            return;

        var v = router.ShowOverlay(OverlayType.ActionPopup) as ActionPopupView;
        if (v == null)
            return;

        void OnHidden()
        {
            v.Hidden -= OnHidden;
            router.HideOverlay(OverlayType.ActionPopup);
        }
        v.Hidden -= OnHidden;
        v.Hidden += OnHidden;

        v.Show(data, () => onClick(v));
        if (!string.IsNullOrEmpty(descOverride))
            v.OverrideDescription(descOverride);

        if (data && data.ActionAudioClip)
            AudioDirector.Instance.Play(data.ActionAudioClip, 0.05f, 0.1f);
    }

    void OnStartClick(ActionPopupView v)
    {
        v.Hide();
        vm.MarkTourBegan();
        binder.RequestUserReady();
    }
}
