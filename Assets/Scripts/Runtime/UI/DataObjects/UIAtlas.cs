using UnityEngine;
using UnityEngine.UIElements;

// Enable creating from Unity menu
[CreateAssetMenu(fileName = "NewAtlas", menuName = "AR GUI/Atlas", order = 1)]
/// <summary>
/// ScriptableObject holding references to UI VisualTreeAssets (UXML).
/// </summary>
public sealed class UIAtlas : ScriptableObject
{
    // References to UXML assets for different UI components
    [Header("UI Screens")]
    [Tooltip("UXML file for the Home/Main Menu screen")]
    [SerializeField]
    private VisualTreeAsset Home;

    [Tooltip("UXML file for the Minigames screen")]
    [SerializeField]
    private VisualTreeAsset Minigames;

    [Tooltip("UXML file for the Onboarding screen")]
    [SerializeField]
    private VisualTreeAsset Onboarding;

    [Header("UI Overlays")]
    [Tooltip("UXML file for the Tour HUD overlay")]
    [SerializeField]
    private VisualTreeAsset TourHUD;

    [Tooltip("UXML file for the Info Widget")]
    [SerializeField]
    private VisualTreeAsset InfoWidget;

    [Tooltip("UXML file for the Notice Popup")]
    [SerializeField]
    private VisualTreeAsset NoticePopup;

    [Tooltip("UXML file for the Action Popup")]
    [SerializeField]
    private VisualTreeAsset ActionPopup;

    [Tooltip("UXML file for the Collapsible Menu")]
    [SerializeField]
    private VisualTreeAsset Menu;

    [Header("Onboarding")]
    [Tooltip("Onboarding Set ScriptableObject containing onboarding slides")]
    [SerializeField]
    private OnboardingSet onboardingSet;

    // Public properties to access private fields

    public VisualTreeAsset HomeUXML => Home;
    public VisualTreeAsset MinigamesUXML => Minigames;
    public VisualTreeAsset OnboardingUXML => Onboarding;
    public VisualTreeAsset TourHUDUXML => TourHUD;
    public VisualTreeAsset InfoWidgetUXML => InfoWidget;
    public VisualTreeAsset NoticePopupUXML => NoticePopup;
    public VisualTreeAsset ActionPopupUXML => ActionPopup;
    public VisualTreeAsset MenuUXML => Menu;
    public OnboardingSet OnboardingSet => onboardingSet;
}
