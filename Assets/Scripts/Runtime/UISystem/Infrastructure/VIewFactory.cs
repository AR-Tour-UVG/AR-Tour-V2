using UnityEngine;
using UnityEngine.UIElements;

public sealed class ViewFactory : IViewFactory
{
    // dependencies
    private readonly UIAtlas atlas; // Reference to the UI Atlas
    private readonly UIDocument doc; // Base document for UI (base layout)

    public ViewFactory(UIDocument doc, UIAtlas atlas)
    {
        this.doc = doc; // assign the UIDocument
        this.atlas = atlas; // assign the UI Atlas
    }

    public IScreenView CreateScreen(ScreenState s) =>
        s switch
        {
            // Create and return the appropriate screen view based on the screen state
            ScreenState.Home => new HomeView(Clone(atlas.HomeUXML)),
            ScreenState.Minigames => new MinigamesView(Clone(atlas.MinigamesUXML)),
            ScreenState.Onboarding => new OnboardingView(Clone(atlas.OnboardingUXML)),
            // The TourHUD is a special case, it uses the BaseHUDView as the main view
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
            _ => null,
        };

    private static VisualElement Clone(VisualTreeAsset vta)
    {
        // Check if the VisualTreeAsset is valid
        if (!vta)
        {
            // If not, log an error and return null
            Debug.LogError("[ViewFactory] Missing VisualTreeAsset");
            return null;
        }
        // Clone the VisualTreeAsset and set its style to flex-grow
        var ve = vta.CloneTree();
        ve.style.flexGrow = 1;
        return ve;
    }
}
