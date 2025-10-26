using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(KeyboardPositioning))]
[RequireComponent(typeof(UWBPositioning))]
public class MovementAgent : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Keyboard movement (Editor only)")]
    [SerializeField]
    private KeyboardPositioning editorMover;

    [Tooltip("UWB positioning (iOS device only)")]
    [SerializeField]
    private UWBPositioning uwbMover;

    public bool IsEnabled
    {
        get
        {
#if UNITY_EDITOR
            return editorMover && editorMover.enabled;
#elif UNITY_IOS && !UNITY_EDITOR
            return uwbMover && uwbMover.enabled;
#else
            return false;
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
        SafeEnable(editorMover, false);
        SafeEnable(uwbMover, false);
    }

    private void OnDisable()
    {
#if UNITY_IOS && !UNITY_EDITOR
        if (uwbMover != null)
            uwbMover.StopTracking();
#else
        if (editorMover != null)
            editorMover.enabled = false;
#endif
    }

    public void Enable(bool on)
    {
#if UNITY_EDITOR
        SafeEnable(editorMover, on);
        Debug.Log("[MovementAgent] Keyboard Control ON (Editor)");
#elif UNITY_IOS && !UNITY_EDITOR
        SafeEnable(uwbMover, on);
        Debug.Log("[MovementAgent] UWB Positioning ON (iOS device)");
        if (uwbMover)
        {
            if (on)
                uwbMover.StartTracking();
            else
                uwbMover.StopTracking();
        }
#else
        SafeEnable(uwbMover, false);
        SafeEnable(editorMover, false);
        Debug.Log("[MovementAgent] No movement enabled (unsupported platform)");
#endif
    }

    private static void SafeEnable(Behaviour b, bool on)
    {
        if (b == null)
            return;
        b.enabled = on;
    }
}
