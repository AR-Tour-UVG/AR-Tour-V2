using UnityEngine;
using UnityEngine.UIElements;

public sealed class ViewFactory : IViewFactory
{
    private readonly UIAtlas atlas;
    private readonly UIDocument doc;

    public ViewFactory(UIDocument doc, UIAtlas atlas)
    {
        this.doc = doc;
        this.atlas = atlas;
    }

    public IScreenView CreateScreen(ScreenState s) =>
        s switch
        {
            ScreenState.Home => new HomeView(Clone(atlas.HomeUXML)),
            ScreenState.Minigames => new MinigamesView(Clone(atlas.MinigamesUXML)),
            ScreenState.Onboarding => new OnboardingView(Clone(atlas.OnboardingUXML)),
            ScreenState.TourHUD => new BaseHUDView(Clone(atlas.BaseHUDUXML)),
            _ => null,
        };

    public IOverlayView CreateOverlay(OverlayType t) =>
        t switch
        {
            OverlayType.InfoModal => new InfoWidgetView(Clone(atlas.InfoWidgetUXML)),
            OverlayType.NoticePopup => new NoticePopupView(Clone(atlas.NoticePopupUXML)),
            OverlayType.ActionPopup => new ActionPopupView(Clone(atlas.ActionPopupUXML)),
            OverlayType.Menu => new MenuView(Clone(atlas.MenuUXML)),
            OverlayType.Settings => new SettingsView(Clone(atlas.SettingsUXML)),
            _ => null,
        };

    private static VisualElement Clone(VisualTreeAsset vta)
    {
        if (!vta)
        {
            Debug.LogError("[ViewFactory] Missing VisualTreeAsset");
            return null;
        }
        var ve = vta.CloneTree();
        ve.style.flexGrow = 1;
        return ve;
    }
}
