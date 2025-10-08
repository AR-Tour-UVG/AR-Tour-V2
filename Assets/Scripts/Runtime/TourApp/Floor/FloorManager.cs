using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Per-floor orchestrator. Enables the floor's areas, debounces entry,
/// pauses/resumes pathing, and signals completion. User-paced flow:
/// - Waits for UserReady()
/// - Confirms first area immediately (simulate being inside)
/// - On "Next", sets target to next area and unpauses pathing
/// - On entering an area (debounced), pauses pathing and shows content (TODO)
/// - Disables visited areas to avoid retriggers
/// </summary>
[DisallowMultipleComponent]
[DefaultExecutionOrder(400)]
public class FloorManager : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private FloorDefinition floor;
    private AreaRegistry registry;
    private PathProvider pathProvider;
    private MovementAgent movementAgent;

    [Header("Options")]
    [Tooltip("Disable areas not listed in this floor's OrderedAreas.")]
    [SerializeField] private bool disableNonFloorAreas = true;
    [Tooltip("Seconds the player must remain inside an area to confirm entry.")]
    [SerializeField] private float enterConfirmTime = 0.75f;
    
    // State
    private readonly HashSet<AreaDefinition> _visited = new();
    private readonly HashSet<AreaInstance> _inside = new();
    private readonly List<GameObject> _sequence = new();     // ordered area GOs for this floor
    private int _currentIndex = -1;                           // index of last confirmed area
    private AreaInstance _candidate;                          // area we're debouncing
    private float _candidateStart;                            // time we started debouncing
    private bool _started;                                    // after UserReady()

    public System.Action<FloorManager> FloorCompleted;

    // Events
    public event System.Action<AreaDefinition> AreaConfirmed; // fired when an area is confirmed
    public event System.Action<AreaDefinition> GuidingToNext; // fired when Next() selects a target

    // Optional helpers (read-only)
    public int CurrentIndex => _currentIndex;                 // -1 before first confirm
    public bool Started => _started;
    public FloorDefinition Floor => floor;

    // Global tour progress
    public int GlobalVisited { get; set; }
    public int GlobalTotal   { get; set; }

    private void Start()
    {
        if (!floor)
        {
            Debug.LogError("[FloorManager] Missing FloorDefinition.", this);
            enabled = false;
            return;
        }
        // Here the Area registry call?
        registry = registry ? registry : FindFirstObjectByType<AreaRegistry>(FindObjectsInactive.Include);
        pathProvider = pathProvider ? pathProvider : FindFirstObjectByType<PathProvider>(FindObjectsInactive.Include);
        movementAgent = movementAgent ? movementAgent : FindFirstObjectByType<MovementAgent>(FindObjectsInactive.Include);

        if (!registry || !pathProvider || !movementAgent)
        {
            Debug.LogError("[FloorManager] Missing registry/path/movement references.", this);
            enabled = false;
            return;
        }
        // Ensure all AreaInstances are indexed (after scene load)
        registry.Refresh();

        BuildSequence();
        WireAreaEvents(true);
        ApplyAreaVisibility();

        movementAgent.Enable(false);
        pathProvider.Paused = true;
        pathProvider.ClearTarget();
        Debug.Log($"[FloorManager] Floor Initialization Complete. Waiting for UserReady().");
    }

    private void OnDestroy()
    {
        WireAreaEvents(false);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            Next();
        }
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            UserReady();
        }
#endif
        // Debounce confirmation only after tour started
        if (!_started) return;

        if (_candidate != null)
        {
            // Still inside candidate?
            if (_inside.Contains(_candidate))
            {
                if (Time.time - _candidateStart >= enterConfirmTime)
                {
                    ConfirmArea(_candidate);
                    _candidate = null;
                }
            }
            else
            {
                // Left before confirm → drop candidate
                _candidate = null;
            }
        }
    }

    // ---------- Public control ----------

    /// <summary>Called by UI/TourRunner when the user is ready to start this floor.</summary>
    public void UserReady()
    {
        if (_started) return;
        _started = true;
        

        // Enable tracking/movement
        movementAgent.Enable(true);

        // Assume we start inside the first area; confirm it immediately.
        var first = GetAreaInstanceAt(0);
        if (first != null)
        {
            ConfirmArea(first);
            Debug.Log("[FloorManager] UserReady → movement ON, confirming first area.");
        }
        else
        {
            Debug.LogWarning("[FloorManager] No first area found to confirm.", this);
        }
    }

    /// <summary>Advance to the next area (user-paced).</summary>
    public void Next()
    {
        if (!_started) return;

        int nextIdx = _currentIndex + 1;

        if (nextIdx >= _sequence.Count)
        {
            // End of floor
            CompleteFloor();
            return;
        }

        var nextGO = _sequence[nextIdx];

        Debug.Log(nextIdx < _sequence.Count
        ? $"[FloorManager] Next → guiding to index {nextIdx} ({nextGO.name})."
        : "[FloorManager] Next → no more areas, completing floor.");

        if (!nextGO)
        {
            Debug.LogWarning("[FloorManager] Next area GameObject missing.", this);
            return;
        }
        var nextPOI = nextGO.GetComponent<AreaInstance>();
        GuidingToNext?.Invoke(nextPOI ? nextPOI.Definition : null);

        // Set navigation target and unpause pathing to show directions
        pathProvider.SetTarget(nextGO);
        pathProvider.Paused = false;
    }

    // ---------- Internal ----------

    private void BuildSequence()
    {
        _sequence.Clear();
        foreach (var go in registry.ForFloor(floor))
            _sequence.Add(go);
        if (_sequence.Count == 0) Debug.LogWarning("[FloorManager] Floor has zero resolved areas in this scene.", this);
    }

    private void ApplyAreaVisibility()
    {
        if (!disableNonFloorAreas)
        {
            Debug.Log("[FloorManager] Not disabling non-floor areas (option off).", this);
            return;
        }

        var allowed = new HashSet<GameObject>(_sequence);
        foreach (var go in registry.AllObjects)
        {
            if (!go) continue;
            Debug.Log($"[FloorManager] Setting area '{go.name}' active={allowed.Contains(go)}");
            go.SetActive(allowed.Contains(go));
        }
        Debug.Log($"[FloorManager] Completed area visibility pass. Enabled {_sequence.Count} areas, disabled {registry.AllObjects.Count - _sequence.Count} non-floor areas.", this);
    }

    private void WireAreaEvents(bool on)
    {
        foreach (var go in registry.AllObjects)
        {
            if (!go) continue;
            var ai = go.GetComponent<AreaInstance>();
            if (!ai) continue;

            if (on)
            {
                ai.Entered += HandleEntered;
                ai.Exited  += HandleExited;
            }
            else
            {
                ai.Entered -= HandleEntered;
                ai.Exited  -= HandleExited;
            }
        }
    }

    private void HandleEntered(AreaInstance ai)
    {
        if (!_started) return;
        if (!ai || !ai.Definition) return;
        if (_visited.Contains(ai.Definition)) return; // already visited and disabled

        _inside.Add(ai);

        // Start/refresh debounce for this candidate
        _candidate = ai;
        _candidateStart = Time.time;
        // Path pauses once we confirm; no change here to keep guidance until confirmation.
    }

    private void HandleExited(AreaInstance ai)
    {
        if (!ai) return;
        _inside.Remove(ai);

        // If the current candidate was exited before confirm, drop it
        if (_candidate == ai) _candidate = null;
    }

    private void ConfirmArea(AreaInstance ai)
    {
        var def = ai.Definition;
        if (def == null) return;

        // Mark and index
        _visited.Add(def);
        _currentIndex = floor.IndexOf(def);

        // Pause pathing and clear line while content plays
        pathProvider.Paused = true;
        pathProvider.ClearTarget();

        // Disable the area GO to avoid lingering re-triggers
        ai.gameObject.SetActive(false);

        // Fire event
        AreaConfirmed?.Invoke(def);

        // Content handling
        Debug.Log($"[FloorManager] ENTERED area '{def.AreaName}'. TODO: show text and play audio.");
        // TODO: ContentPlayer.Play(def)

        // If this was the last area, wait for user to choose Next (which will finish),
        // or they can press a UI "Finish floor" that calls CompleteFloor().
    }

    private void CompleteFloor()
    {
        // Stop movement and pathing
        movementAgent.Enable(false);
        pathProvider.Paused = true;
        pathProvider.ClearTarget();

        Debug.Log("[FloorManager] Floor completed. Waiting for TourRunner to unload scene.");
        FloorCompleted?.Invoke(this);
    }

    private AreaInstance GetAreaInstanceAt(int index)
    {
        if (index < 0 || index >= _sequence.Count) return null;
        var go = _sequence[index];
        return go ? go.GetComponent<AreaInstance>() : null;
    }
}
