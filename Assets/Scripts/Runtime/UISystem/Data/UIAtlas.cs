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
    [Tooltip("UXML file for the Base HUD overlay")]
    [SerializeField]
    private VisualTreeAsset BaseHUD;

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

    [Header("Notice Pop Ups")]
    [SerializeField]
    private NoticeData NoticeConnecting;

    [SerializeField]
    private NoticeData NoticeLostConnection;

    [SerializeField]
    private NoticeData NoticeTourComplete;

    [Header("Action Pop Ups")]
    [SerializeField]
    private ActionData ActionStart;

    [SerializeField]
    private ActionData ActionReadyOnFloor;

    // Public properties to access private fields

    public VisualTreeAsset HomeUXML => Home;
    public VisualTreeAsset MinigamesUXML => Minigames;
    public VisualTreeAsset OnboardingUXML => Onboarding;
    public VisualTreeAsset BaseHUDUXML => BaseHUD;
    public VisualTreeAsset InfoWidgetUXML => InfoWidget;
    public VisualTreeAsset NoticePopupUXML => NoticePopup;
    public VisualTreeAsset ActionPopupUXML => ActionPopup;
    public VisualTreeAsset MenuUXML => Menu;
    public OnboardingSet OnboardingSet => onboardingSet;
    public NoticeData NoticeConnectingData => NoticeConnecting;
    public NoticeData NoticeLostConnectionData => NoticeLostConnection;
    public NoticeData NoticeTourCompleteData => NoticeTourComplete;
    public ActionData ActionStartData => ActionStart;
    public ActionData ActionReadyOnFloorData => ActionReadyOnFloor;
}
