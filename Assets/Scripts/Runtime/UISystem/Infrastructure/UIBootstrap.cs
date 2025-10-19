using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Bootstrapper for UI system. Sets up UIRouter and ViewFactory.
/// </summary>
[RequireComponent(typeof(UIDocument))]
public sealed class UIBootstrap : MonoBehaviour
{
    [Header("Scene references")]
    [Tooltip("UXML for the Base Layout")]
    [SerializeField]
    private UIDocument uiDocument; // has BaseLayout.uxml

    [Tooltip("UXML for the UI Atlas Asset")]
    public UIAtlas atlas; // has all shared styles and resources

    public UIRouter Router { get; private set; }

    /// <summary>
    /// Access the UIRouter for screen navigation.
    /// </summary>
    private void Awake()
    {
        // Get Base UI Document if not assigned
        if (!uiDocument)
        {
            uiDocument = GetComponent<UIDocument>();
        }
        // Validate references
        if (!uiDocument || !atlas)
        {
            // If UIDocument is missing, disable this component
            Debug.LogError("[UIBootstrap] Missing references in UIBootstrap");
            enabled = false;
            return;
        }

        // Setup layers
        var root = uiDocument.rootVisualElement;
        var baseLayer = root.Q<VisualElement>("BaseLayer");
        var modalLayer = root.Q<VisualElement>("ModalLayer");
        var popupLayer = root.Q<VisualElement>("PopupLayer");
        var menuLayer = root.Q<VisualElement>("MenuLayer");

        // Validate layers
        if (baseLayer == null || modalLayer == null || popupLayer == null || menuLayer == null)
        {
            Debug.LogError("[UIBootstrap] Missing layers in BaseLayout UXML.");
            enabled = false;
            return;
        }

        // Disable all layers initially
        modalLayer.style.display = DisplayStyle.None;
        popupLayer.style.display = DisplayStyle.None;
        menuLayer.style.display = DisplayStyle.None;

        // Create View Factory
        var factory = new ViewFactory(uiDocument, atlas);
        // Create UIRouter instance
        Router = new UIRouter(baseLayer, modalLayer, popupLayer, menuLayer, null, factory);
        // Decide where to start the UI navigation
        var showOnboarding =
            atlas.OnboardingSet && OnboardingGate.ShouldShow(atlas.OnboardingSet.ShowEveryNDays);
        // Show initial screen
        Router.ShowScreen(showOnboarding ? ScreenState.Onboarding : ScreenState.Home);
    }
}
