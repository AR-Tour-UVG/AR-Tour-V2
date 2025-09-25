using UnityEngine;

public class ControlModeBootstrap : MonoBehaviour
{
    public enum Mode { Auto, Simulate, UWB }
    public Mode mode = Mode.Auto;

    KeyboardAgent keyboard;
    NavigationAgent navigation;

    void Awake()
    {
        Debug.Log("[ControlModeBootstrap] Awakened");
        keyboard   = GetComponent<KeyboardAgent>();
        navigation = GetComponent<NavigationAgent>();
        Apply(mode);
    }

    public void Apply(Mode m)
    {
        // Resolve Auto -> platform
        if (m == Mode.Auto)
        {
#if UNITY_EDITOR
            m = Mode.Simulate;
#elif UNITY_IOS
            m = Mode.UWB;   // iOS builds run UWB by default
#else
            m = Mode.Simulate;   // non-iOS builds run simulate by default
#endif
        }

        Debug.Log($"[ControlModeBootstrap] Applying mode: {m}");
        bool useSim = (m == Mode.Simulate);
        if (keyboard)   keyboard.enabled   = useSim;
        if (navigation) navigation.enabled = !useSim;
    }

    // Optional: call from UI “Simular” button
    public void ForceSimulate()  => Apply(Mode.Simulate);
    // Optional: call when UWB control needs to be forced
    public void ForceUWB()       => Apply(Mode.UWB);
}
