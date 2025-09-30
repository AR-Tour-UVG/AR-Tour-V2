using UnityEngine;

/// <summary>
/// Enables either keyboard or UWB positioning based on platform.
/// In Editor: keyboard movement.
/// On iOS device: UWB positioning.
/// On other platforms: none.
/// </summary>
/// <remarks>Attach to the player object or an empty GameObject.</remarks>

[DisallowMultipleComponent]
[RequireComponent(typeof(KeyboardPositioning))]
[RequireComponent(typeof(UWBPositioning))]
public class MovementAgent : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Keyboard movement (Editor only)")]
    [SerializeField] private KeyboardPositioning editorMover;
    [Tooltip("UWB positioning (iOS device only)")]
    [SerializeField] private UWBPositioning uwbMover;


    public bool IsEnabled
    {
        get
        {
#if UNITY_EDITOR
            return editorMover && editorMover.enabled;
#else
            return uwbMover && uwbMover.enabled;
#endif
        }
    }

    private void Reset()
    {
        editorMover = GetComponent<KeyboardPositioning>();
        uwbMover = GetComponent<UWBPositioning>();
    }

    private void Awake()
    {
        // Default: disable both until a target is chosen
        SafeEnable(editorMover, false);
        SafeEnable(uwbMover, false);
    }

    private void OnDisable()
    {
#if UNITY_IOS && !UNITY_EDITOR
        // Stop UWB if active
        if (uwbMover != null) uwbMover.StopTracking();
#else
        // Stop keyboard if active
        if (editorMover != null ) editorMover.enabled = false;
#endif
    }


    public void Enable(bool on)
    {
#if UNITY_EDITOR
        // In Editor: keyboard movement
        SafeEnable(editorMover, on);
        Debug.Log("[MovementAgent] Keyboard Control ON (Editor)");
#elif UNITY_IOS && !UNITY_EDITOR
        // On device: prefer iOS+UWB, else none
        SafeEnable(uwbMover, on);
        Debug.Log("[MovementAgent] UWB Positioning ON (iOS device)");
        if (uwbMover)
        {
            if (on) uwbMover.StartTracking();
            else    uwbMover.StopTracking();
        }
#else
        // else: leave both disabled (e.g., Android/Standalone build)
        SafeEnable(uwbMover, false);
        SafeEnable(editorMover, false);
        Debug.Log("[MovementAgent] No movement enabled (unsupported platform)");
#endif
    }

    /// <summary>
    /// Safely enable/disable a Behaviour (if not null).
    /// </summary>
    private static void SafeEnable(Behaviour b, bool on)
    {
        if (b == null) return;
        b.enabled = on;
    }
}
