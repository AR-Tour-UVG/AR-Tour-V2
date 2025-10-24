using System;

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

    public void ShowStart()
    {
        var v = router.ShowOverlay(OverlayType.ActionPopup) as ActionPopupView;
        if (v == null)
            return;
        v.Show(atlas.ActionStartData, OnClick);
        void OnClick()
        {
            router.HideOverlay(OverlayType.ActionPopup);
            vm.MarkTourBegan();
            binder.RequestUserReady();
        }
        if (atlas.ActionStartData != null && atlas.ActionStartData.ActionAudioClip)
            AudioDirector.Instance.Play(atlas.ActionStartData.ActionAudioClip, 0.05f, 0.1f);
    }

    public void ShowReadyOnFloor(string descriptionOverride = null)
    {
        var v = router.ShowOverlay(OverlayType.ActionPopup) as ActionPopupView;
        if (v == null)
            return;
        v.Show(atlas.ActionReadyOnFloorData, OnClick);
        if (!string.IsNullOrEmpty(descriptionOverride))
            v.OverrideDescription(descriptionOverride); // add this helper on the view
        void OnClick()
        {
            router.HideOverlay(OverlayType.ActionPopup);
            if (vm.Phase == TourUIPhase.FloorTransition)
                TourRunner.Instance?.ContinueToNextFloor(); // when you add gating
            else
                binder.RequestUserReady();
        }
        if (atlas.ActionReadyOnFloorData != null && atlas.ActionReadyOnFloorData.ActionAudioClip)
            AudioDirector.Instance.Play(atlas.ActionReadyOnFloorData.ActionAudioClip, 0.05f, 0.1f);
    }

    public void Hide() => router.HideOverlay(OverlayType.ActionPopup);
}
