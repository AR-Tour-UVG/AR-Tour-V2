using UnityEngine;
using UnityEngine.UIElements;

// Enable creating from Unity menu
[CreateAssetMenu(fileName = "Atlas", menuName = "AR-GUI/Atlas", order = 1)]

/// <summary>
/// ScriptableObject holding references to UI VisualTreeAssets (UXML).
/// </summary>
public sealed class UIAtlas : ScriptableObject
{
    // References to UXML assets for different UI components
    [Header("UI Screens")]
    [Tooltip("UXML file for the Home/Main Menu screen")]
    [SerializeField] public VisualTreeAsset Home;
    [Tooltip("UXML file for the Minigames screen")]
    [SerializeField] public VisualTreeAsset Minigames;
    [Tooltip("UXML file for the Onboarding screen")]
    [SerializeField] public VisualTreeAsset Onboarding;

    [Header("UI Overlays")]
    [Tooltip("UXML file for the Tour HUD overlay")]
    [SerializeField] public VisualTreeAsset TourHUD;
    [Tooltip("UXML file for the Info Widget")]
    [SerializeField] public VisualTreeAsset InfoWidget;
    [Tooltip("UXML file for the Notice Popup")]
    [SerializeField] public VisualTreeAsset NoticePopup;
    [Tooltip("UXML file for the Action Popup")]
    [SerializeField] public VisualTreeAsset ActionPopup;
    [Tooltip("UXML file for the Collapsible Menu")]
    [SerializeField] public VisualTreeAsset Menu;
}
