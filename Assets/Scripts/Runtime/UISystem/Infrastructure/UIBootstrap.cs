using Unity.VisualScripting;
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

    public VisualElement AppRoot { get; private set; }

    /// <summary>
    /// Access the UIRouter for screen navigation.
    /// </summary>
    private void Awake()
    {
        // Set the Application default user prefs if first run
        if (AppPrefs.IsFirstRun())
        {
            AppPrefs.SaveVolume(50); //50/100 volume
            AppPrefs.SaveFontPx(100); //100px font size
        }

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
        AppRoot = root.Q<VisualElement>("AppRoot");
        if (AppRoot != null)
        {
            AppRoot.style.fontSize = AppPrefs.LoadFontPx(); // Apply user font size preference
        }
        AudioListener.volume = AppPrefs.LoadVolume() / 100f; // Apply user volume preference

        var baseLayer = AppRoot.Q<VisualElement>("BaseLayer");
        var modalLayer = AppRoot.Q<VisualElement>("ModalLayer");
        var popupLayer = AppRoot.Q<VisualElement>("PopupLayer");
        var menuLayer = AppRoot.Q<VisualElement>("MenuLayer");
        var settingsLayer = AppRoot.Q<VisualElement>("SettingsLayer");

        // Validate layers
        if (
            baseLayer == null
            || modalLayer == null
            || popupLayer == null
            || menuLayer == null
            || settingsLayer == null
        )
        {
            Debug.LogError("[UIBootstrap] Missing layers in BaseLayout UXML.");
            enabled = false;
            return;
        }

        // Disable all layers initially
        modalLayer.style.display = DisplayStyle.None;
        popupLayer.style.display = DisplayStyle.None;
        menuLayer.style.display = DisplayStyle.None;
        settingsLayer.style.display = DisplayStyle.None;

        //Disable picking on layers that should not block input
        baseLayer.pickingMode = PickingMode.Position; // receives input
        modalLayer.pickingMode = PickingMode.Ignore; // pass-through by default
        popupLayer.pickingMode = PickingMode.Ignore;
        menuLayer.pickingMode = PickingMode.Ignore;
        settingsLayer.pickingMode = PickingMode.Ignore;

        ApplyGlobalFontPx(AppPrefs.LoadFontPx());

        // Create View Factory
        var factory = new ViewFactory(uiDocument, atlas);
        // Create UIRouter instance
        Router = new UIRouter(baseLayer, modalLayer, popupLayer, menuLayer, settingsLayer, factory);
        // Decide where to start the UI navigation
        var showOnboarding =
            atlas.OnboardingSet && OnboardingGate.ShouldShow(atlas.OnboardingSet.ShowEveryNDays);
        // Show initial screen
        Router.ShowScreen(showOnboarding ? ScreenState.Onboarding : ScreenState.Home);
    }

    public void ApplyGlobalFontPx(int px)
    {
        var root = uiDocument.rootVisualElement;
        root.RemoveFromClassList("app-font-80");
        root.RemoveFromClassList("app-font-100");
        root.RemoveFromClassList("app-font-120");
        root.AddToClassList(
            px switch
            {
                80 => "app-font-80",
                120 => "app-font-120",
                _ => "app-font-100",
            }
        );
    }
}
