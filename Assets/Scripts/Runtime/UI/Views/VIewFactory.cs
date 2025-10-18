using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Factory class responsible for creating screen and overlay views.
/// </summary>
public sealed class ViewFactory : IViewFactory
{
    // dependencies
    private readonly UIAtlas atlas; // Reference to the UI Atlas
    private readonly UIDocument doc; // Base document for UI (base layout)

    /// <summary>
    /// Constructor for the ViewFactory.
    /// </summary>
    /// <param name="doc">The base UIDocument for the UI.</param>
    /// <param name="atlas">The UI Atlas containing all UXML references.</param>
    public ViewFactory(UIDocument doc, UIAtlas atlas)
    {
        this.doc = doc; // assign the UIDocument
        this.atlas = atlas; // assign the UI Atlas
    }

    /// <summary>
    /// Creates a screen view based on the specified screen state.
    /// </summary>
    /// <param name="s">The screen state for which to create the view.</param>
    /// <returns>The created screen view or null if the state is unrecognized.</returns>
    public IScreenView CreateScreen(ScreenState s) =>
        s switch
        {
            // Create and return the appropriate screen view based on the screen state
            ScreenState.Home => new HomeView(Clone(atlas.HomeUXML)),
            ScreenState.Minigames => new MinigamesView(Clone(atlas.MinigamesUXML)),
            ScreenState.Onboarding => new OnboardingView(Clone(atlas.OnboardingUXML)),
            _ => null,
        };

    public IOverlayView CreateOverlay(OverlayType t) => null; // TODO: implement overlays

    /// <summary>
    /// Clones a VisualTreeAsset to create a new VisualElement.
    /// </summary>
    /// <param name="vta">The VisualTreeAsset to clone.</param>
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
