using UnityEngine;

/// <summary>
/// Enables either keyboard or UWB positioning based on platform.
/// In Editor: keyboard movement.
/// On iOS device: UWB positioning.
/// On other platforms: none.
/// </summary>
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
#if !UNITY_EDITOR
    [Tooltip("Automatically start UWB tracking on enable")]
    [SerializeField] private bool autoStartUWB = true; // calls StartTracking on enable
#endif
    private void Reset()
    {
        editorMover = GetComponent<KeyboardPositioning>();
        uwbMover    = GetComponent<UWBPositioning>();
    }

    private void Awake()
    {
        // Default: disable both until a target is chosen
        SafeEnable(editorMover, false);
        SafeEnable(uwbMover, false);

#if UNITY_EDITOR
        // In Editor: keyboard movement
        SafeEnable(editorMover, true);
        Debug.Log("[MovementAgent] Using keyboard movement (Editor Mode)");
#else
        // On device: prefer iOS+UWB, else none
        if (Application.platform == RuntimePlatform.IPhonePlayer && uwbMover != null)
        {
            SafeEnable(uwbMover, true);
            Debug.Log("[MovementAgent] Using UWB positioning (iOS device)");
            if (autoStartUWB) uwbMover.StartTracking();
        }
        // else: leave both disabled (e.g., Android/Standalone build)
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
