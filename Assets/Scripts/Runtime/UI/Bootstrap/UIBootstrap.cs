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
    [SerializeField]
    private UIAtlas atlas; // has all shared styles and resources

    // Layers from BaseLayout
    private VisualElement baseLayer;
    private VisualElement modalLayer;
    private VisualElement popupLayer;
    private VisualElement menuLayer;
    private VisualElement scrim; // Optional scrim for modals/popups

    private UIRouter router; // Manages screen navigation
    private ViewFactory factory; // Creates views from UXML

    /// <summary>
    /// Access the UIRouter for screen navigation.
    /// </summary>
    private void Awake()
    {
        // Validate references
        if (!uiDocument)
        {
            // If UIDocument is missing, disable this component
            Debug.LogError("[UIBootstrap] UIDocument missing");
            enabled = false;
            return;
        }
        // Get layers from BaseLayout
        var root = uiDocument.rootVisualElement;
        baseLayer = root.Q<VisualElement>("BaseLayer");
        modalLayer = root.Q<VisualElement>("ModalLayer");
        popupLayer = root.Q<VisualElement>("PopupLayer");
        menuLayer = root.Q<VisualElement>("MenuLayer");

        // Validate layers
        if (baseLayer == null || modalLayer == null || popupLayer == null || menuLayer == null)
        {
            // If any layer is missing, disable this component
            Debug.LogError("[UIBootstrap] Some BaseLayout layers missing");
            enabled = false;
            return;
        }

        // Disable layers except base initially
        modalLayer.style.display = DisplayStyle.None;
        popupLayer.style.display = DisplayStyle.None;
        menuLayer.style.display = DisplayStyle.None;

        // Create factory and router
        factory = new ViewFactory(uiDocument, atlas);
        router = new UIRouter(baseLayer, modalLayer, popupLayer, menuLayer, scrim, factory);

        // First screen for this iteration
        router.ShowScreen(ScreenState.Home);
        HookHome();
    }

    /// <summary>
    /// Temporary hooks for Home screen buttons.
    /// </summary>
    private void HookHome()
    {
        if (router.CurrentScreenView is HomeView home)
        {
            // TODO: Replace with proper navigation
            home.OnExpress += () => Debug.Log("Express selected");
            home.OnComplete += () => Debug.Log("Complete selected");
            home.OnMinigames += () =>
            {
                router.ShowScreen(ScreenState.Minigames);
                HookMinigames();
            };
        }
    }

    private void HookMinigames()
    {
        if (router.CurrentScreenView is MinigamesView minigames)
        {
            minigames.OnBreakout += () => Debug.Log("Breakout selected");
            minigames.OnTrivia += () => Debug.Log("Trivia selected");
            minigames.OnFlappy += () => Debug.Log("Flappy selected");
            minigames.OnExit += () =>
            {
                router.ShowScreen(ScreenState.Home);
                HookHome();
            };
        }
    }
}
