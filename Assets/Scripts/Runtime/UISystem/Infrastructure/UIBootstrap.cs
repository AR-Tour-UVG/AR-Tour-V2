using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public sealed class UIBootstrap : MonoBehaviour
{
    [Header("Scene references")]
    [Tooltip("UXML for the Base Layout")]
    [SerializeField]
    private UIDocument uiDocument;

    [Tooltip("UXML for the UI Atlas Asset")]
    public UIAtlas uiAtlas;

    [Tooltip("Audio Atlas for UI sounds")]
    public AudioAtlas audioAtlas;

    public UIRouter Router { get; private set; }

    public VisualElement AppRoot { get; private set; }

    private void Awake()
    {
        if (AppPrefs.IsFirstRun())
        {
            AppPrefs.SaveVolume(50);
            AppPrefs.SaveFontPx(100);
        }

        if (!uiDocument)
        {
            uiDocument = GetComponent<UIDocument>();
        }
        if (!uiDocument || !uiAtlas)
        {
            Debug.LogError("[UIBootstrap] Missing references in UIBootstrap");
            enabled = false;
            return;
        }

        var root = uiDocument.rootVisualElement;
        root.style.display = DisplayStyle.Flex; // Ensure root is visible
        AppRoot = root.Q<VisualElement>("AppRoot");
        AppRoot.style.display = DisplayStyle.Flex; // Ensure AppRoot is visible
        if (AppRoot != null)
        {
            AppRoot.style.fontSize = AppPrefs.LoadFontPx();
        }

        AudioListener.volume = AppPrefs.LoadVolume() / 100f;

        var baseLayer = AppRoot.Q<VisualElement>("BaseLayer");
        var modalLayer = AppRoot.Q<VisualElement>("ModalLayer");
        var popupLayer = AppRoot.Q<VisualElement>("PopupLayer");
        var menuLayer = AppRoot.Q<VisualElement>("MenuLayer");
        var settingsLayer = AppRoot.Q<VisualElement>("SettingsLayer");

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

        baseLayer.style.display = DisplayStyle.Flex;
        modalLayer.style.display = DisplayStyle.None;
        popupLayer.style.display = DisplayStyle.None;
        menuLayer.style.display = DisplayStyle.None;
        settingsLayer.style.display = DisplayStyle.None;

        baseLayer.pickingMode = PickingMode.Position;
        modalLayer.pickingMode = PickingMode.Ignore;
        popupLayer.pickingMode = PickingMode.Ignore;
        menuLayer.pickingMode = PickingMode.Ignore;
        settingsLayer.pickingMode = PickingMode.Ignore;

        ApplyGlobalFontPx(AppPrefs.LoadFontPx());

        var factory = new ViewFactory(uiDocument, uiAtlas);
        Router = new UIRouter(baseLayer, modalLayer, popupLayer, menuLayer, settingsLayer, factory);
        var showOnboarding =
            uiAtlas.OnboardingSet
            && OnboardingGate.ShouldShow(uiAtlas.OnboardingSet.ShowEveryNDays);
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
