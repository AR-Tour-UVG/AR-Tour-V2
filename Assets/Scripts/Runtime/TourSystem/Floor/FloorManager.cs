using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[DefaultExecutionOrder(400)]
public class FloorManager : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField]
    private FloorDefinition floor;
    private AreaRegistry registry;
    private PathProvider pathProvider;
    private MovementAgent movementAgent;

    [Tooltip("Seconds the player must remain inside an area to confirm entry.")]
    [SerializeField]
    private float enterConfirmTime = 0.75f;

    private readonly HashSet<AreaDefinition> _visited = new();
    private readonly HashSet<AreaInstance> _inside = new();
    private readonly List<GameObject> _sequence = new();
    private int _currentIndex = -1;
    private AreaInstance _candidate;
    private float _candidateStart;
    private bool _started;

    public System.Action<FloorManager> FloorCompleted;

    public event System.Action<AreaDefinition> AreaConfirmed;
    public event System.Action<AreaDefinition> GuidingToNext;

    public int CurrentIndex => _currentIndex;
    public bool Started => _started;
    public FloorDefinition Floor => floor;

    public int GlobalVisited { get; set; }
    public int GlobalTotal { get; set; }

    private void Start()
    {
        if (!floor)
        {
            Debug.LogError("[FloorManager] Missing FloorDefinition.", this);
            enabled = false;
            return;
        }
        registry = registry
            ? registry
            : FindFirstObjectByType<AreaRegistry>(FindObjectsInactive.Include);
        pathProvider = pathProvider
            ? pathProvider
            : FindFirstObjectByType<PathProvider>(FindObjectsInactive.Include);
        movementAgent = movementAgent
            ? movementAgent
            : FindFirstObjectByType<MovementAgent>(FindObjectsInactive.Include);

        if (!registry || !pathProvider || !movementAgent)
        {
            Debug.LogError("[FloorManager] Missing registry/path/movement references.", this);
            enabled = false;
            return;
        }
        registry.Refresh();

        BuildSequence();
        WireAreaEvents(true);

        foreach (var go in registry.AllObjects)
            if (go)
                go.SetActive(false);

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
        if (!_started)
            return;

        if (_candidate != null)
        {
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
                _candidate = null;
            }
        }
    }

    public void UserReady()
    {
        if (_started)
            return;
        _started = true;

        movementAgent.Enable(true);

        if (_sequence.Count > 0)
        {
            ActivateAreaIndex(0);

            var firstGO = _sequence[0];
            var firstPOI = firstGO.GetComponent<AreaInstance>();
            ConfirmArea(firstPOI);

            Debug.Log("[FloorManager] UserReady → guiding to first area.");
        }
        else
        {
            Debug.LogWarning("[FloorManager] No first area found.", this);
        }
    }

    public void Next()
    {
        if (!_started)
            return;

        int nextIdx = _currentIndex + 1;

        if (nextIdx >= _sequence.Count)
        {
            CompleteFloor();
            return;
        }

        ActivateAreaIndex(nextIdx);

        var nextGO = _sequence[nextIdx];

        if (!nextGO)
        {
            Debug.LogWarning("[FloorManager] Next area GameObject missing.", this);
            return;
        }
        var nextPOI = nextGO.GetComponent<AreaInstance>();
        GuidingToNext?.Invoke(nextPOI ? nextPOI.Definition : null);

        pathProvider.SetTarget(nextGO);
        pathProvider.Paused = false;

        Debug.Log(
            nextIdx < _sequence.Count
                ? $"[FloorManager] Next → guiding to index {nextIdx} ({nextGO.name})."
                : "[FloorManager] Next → no more areas, completing floor."
        );
    }

    private void BuildSequence()
    {
        _sequence.Clear();
        foreach (var go in registry.ForFloor(floor))
            _sequence.Add(go);
        if (_sequence.Count == 0)
            Debug.LogWarning("[FloorManager] Floor has zero resolved areas in this scene.", this);
    }

    private void WireAreaEvents(bool on)
    {
        foreach (var go in registry.AllObjects)
        {
            if (!go)
                continue;
            var ai = go.GetComponent<AreaInstance>();
            if (!ai)
                continue;

            if (on)
            {
                ai.Entered += HandleEntered;
                ai.Exited += HandleExited;
            }
            else
            {
                ai.Entered -= HandleEntered;
                ai.Exited -= HandleExited;
            }
        }
    }

    private void HandleEntered(AreaInstance ai)
    {
        if (!_started)
            return;
        if (!ai || !ai.Definition)
            return;
        if (_visited.Contains(ai.Definition))
            return;

        _inside.Add(ai);
        _candidate = ai;
        _candidateStart = Time.time;
    }

    private void HandleExited(AreaInstance ai)
    {
        if (!ai)
            return;
        _inside.Remove(ai);

        if (_candidate == ai)
            _candidate = null;
    }

    private void ConfirmArea(AreaInstance ai)
    {
        var def = ai.Definition;
        if (def == null)
            return;

        _visited.Add(def);
        _currentIndex = floor.IndexOf(def);

        pathProvider.Paused = true;
        pathProvider.ClearTarget();

        ai.gameObject.SetActive(false);

        AreaConfirmed?.Invoke(def);

        int nextIdx = _currentIndex + 1;
        if (nextIdx < _sequence.Count)
            ActivateAreaIndex(nextIdx);

        Debug.Log($"[FloorManager] ENTERED area '{def.AreaName}'.");
    }

    private void CompleteFloor()
    {
        movementAgent.Enable(false);
        pathProvider.Paused = true;
        pathProvider.ClearTarget();

        Debug.Log("[FloorManager] Floor completed. Waiting for TourRunner to unload scene.");
        FloorCompleted?.Invoke(this);
    }

    private AreaInstance GetAreaInstanceAt(int index)
    {
        if (index < 0 || index >= _sequence.Count)
            return null;
        var go = _sequence[index];
        return go ? go.GetComponent<AreaInstance>() : null;
    }

    private void ActivateAreaIndex(int idx)
    {
        foreach (var go in registry.AllObjects)
            if (go)
                go.SetActive(false);

        var ai = GetAreaInstanceAt(idx);
        if (ai && ai.gameObject)
            ai.gameObject.SetActive(true);
    }
}
