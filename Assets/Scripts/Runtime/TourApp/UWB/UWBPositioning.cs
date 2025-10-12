using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Moves a target object based on UWBLocator positions, with filtering and NavMesh clamping.
/// </summary>
/// <remarks>Attach to the player object or an empty GameObject.</remarks>
[RequireComponent(typeof(Rigidbody))]
public class UWBPositioning : MonoBehaviour
{
    [Header("Polling")]
    [Tooltip("How often to poll UWBLocator for a new position.")]
    [SerializeField] private float pollIntervalSeconds = 0.5f;

    [Header("Filtering")]
    [Tooltip("Minimum movement distance to consider a new position valid.")]
    [SerializeField] private float noiseThresholdMeters = 0.10f;
    [Tooltip("Maximum speed (m/s) to consider a new position valid.")]
    [SerializeField] private float maxSpeedMetersPerSecond = 3.0f;
    [Tooltip("Tolerance factor for jump filtering (e.g. 1.25 = 25% extra).")]
    [SerializeField] private float jumpToleranceFactor = 1.25f;

    [Header("NavMesh Clamp")]
    [Tooltip("Radius to sample the NavMesh for valid positions.")]
    [SerializeField] private float navmeshSampleRadius = 2.0f;
    [Tooltip("Maximum radius to sample the NavMesh.")]
    [SerializeField] private float navmeshMaxSampleRadius = 10.0f;
    [Tooltip("Growth factor for NavMesh sampling radius (e.g. 2.0 = double each step).")]
    [SerializeField] private float navmeshRadiusGrowth = 2.0f;

    [Header("Movement")]
    [Tooltip("Whether to smoothly move towards the target position.")]
    [SerializeField] private bool smoothMove = false;
    [Tooltip("Speed of smoothing (higher = snappier).")]
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Target")]
    [Tooltip("The player object to move")]
    [SerializeField] private Transform target;

    [Header("Signal Loss")]
    [Tooltip("How many consecutive nulls before declaring connection lost.")]
    [SerializeField] private int lostConnectionThreshold = 5;

    private Coroutine pollRoutine; // null when not polling
    private Vector3 lastAccepted; // last accepted position
    private bool hasLastAccepted = false; // whether we have a valid last accepted position

    // null handling
    private int consecutiveNulls = 0;  // how many nulls in a row
    private bool lossDeclared = false; // whether loss has been logged

    // smoothing
    private Vector3 currentGoal; // current target position when smoothing
    private bool hasGoal = false; // whether we have a current goal

    /// <summary>
    /// Set target to self if not assigned.
    /// </summary>
    private void Awake()
    {
#if UNITY_IOS && !UNITY_EDITOR
        if (target == null) target = transform;
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        gameObject.tag = "Player";
#else
        Debug.Log("[UWBPositioning] Disabled in non-iOS build.");
        enabled = false;
#endif
    }

    /// <summary> 
    /// If smoothing, move towards goal each frame.
    /// </summary>
    private void Update()
    {
        if (smoothMove && hasGoal)
        {
            if (smoothSpeed <= 0f) return;
            Vector3 next = Vector3.MoveTowards(target.position, currentGoal, smoothSpeed * Time.deltaTime);
            target.position = next;
            if ((next - currentGoal).sqrMagnitude < 0.0001f) hasGoal = false;
        }
    }

    /// <summary>
    /// Start polling UWBLocator for positions.
    /// </summary>
    public void StartTracking()
    {
        if (pollRoutine != null) return;
        pollRoutine = StartCoroutine(PollLoop());
    }

    /// <summary>
    /// Stop polling UWBLocator for positions.
    /// </summary>
    public void StopTracking()
    {
        if (pollRoutine == null) return;
        StopCoroutine(pollRoutine);
        pollRoutine = null;
    }

    /// <summary>
    /// Toggle tracking state.
    /// </summary>
    public void ToggleTracking()
    {
        if (pollRoutine == null) StartTracking(); else StopTracking();
    }

    /// <summary>
    /// Coroutine for polling UWBLocator positions.
    /// </summary>
    private IEnumerator PollLoop()
    {
        var wait = new WaitForSeconds(pollIntervalSeconds <= 0f ? 0.5f : pollIntervalSeconds);
        while (true)
        {
            TryStep();
            yield return wait;
        }
    }

    /// <summary>
    /// Attempt to get a new position from UWBLocator and apply filtering and clamping.
    /// </summary>
    private void TryStep()
    {
        if (!UWBLocator.TryGetPosition(out var uwbWorld))
        {
            // Let UWBLocator log per-null warnings.
            HandlePossibleLoss();
            return;
        }

        // Recovered from a null streak
        if (consecutiveNulls > 0)
        {
            if (lossDeclared) Debug.Log("[UWBPositioning] UWB reconnected.");
            consecutiveNulls = 0;
            lossDeclared = false;
        }

        // Filters
        if (hasLastAccepted)
        {
            float delta = Vector3.Distance(uwbWorld, lastAccepted);
            if (delta < noiseThresholdMeters) return;

            float dt = Mathf.Max(0.01f, pollIntervalSeconds);
            float maxStep = maxSpeedMetersPerSecond * dt * jumpToleranceFactor;
            if (delta > maxStep)
            {
                Debug.LogWarning($"[UWBPositioning] Rejected jump {delta:F2}m (> {maxStep:F2}m in {dt:F2}s).");
                return;
            }
        }

        // Clamp to nearest NavMesh (any area)
        Vector3 clamped = ClampToNavmesh(uwbWorld, navmeshSampleRadius, navmeshMaxSampleRadius, navmeshRadiusGrowth);

        lastAccepted = clamped;
        hasLastAccepted = true;

        if (smoothMove) { currentGoal = clamped; hasGoal = true; }
        else { target.position = clamped; hasGoal = false; }
    }

    /// <summary>
    /// Handle a possible loss of UWB signal.
    /// </summary>
    private void HandlePossibleLoss()
    {
        consecutiveNulls++;

        // Only declare loss once per streak, after threshold
        if (!lossDeclared && consecutiveNulls >= Mathf.Max(1, lostConnectionThreshold))
        {
            Debug.LogWarning("[UWBPositioning] UWB connection lost. Waiting to reconnect…");
            lossDeclared = true;
        }
    }

    /// <summary>
    /// Attempt to clamp a position to the NavMesh within a max radius.
    /// If no NavMesh is found, returns the original position.
    /// </summary>
    private static Vector3 ClampToNavmesh(Vector3 desired, float startRadius, float maxRadius, float growth)
    {
        float r = Mathf.Max(0.01f, startRadius);
        float cap = Mathf.Max(r, maxRadius);
        float g = Mathf.Max(1.01f, growth);

        while (r <= cap)
        {
            if (NavMesh.SamplePosition(desired, out var hit, r, NavMesh.AllAreas))
                return hit.position;
            r *= g;
        }

        Debug.LogWarning("[UWBPositioning] No NavMesh found within max radius. Using raw coordinate.");
        return desired;
    }
}

