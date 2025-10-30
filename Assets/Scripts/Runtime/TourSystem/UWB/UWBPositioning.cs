using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class UWBPositioning : MonoBehaviour
{
    [Header("Polling")]
    [Tooltip("How often poll for a new position (0 = every frame. >0 = seconds).")]
    [SerializeField]
    private float pollIntervalSeconds = 0.0f;

    [Header("NavMesh Clamp")]
    [Tooltip("Radius to sample the NavMesh for valid positions.")]
    [SerializeField]
    private float navmeshSampleRadius = 2.0f;

    [Tooltip("Maximum radius to sample the NavMesh.")]
    [SerializeField]
    private float navmeshMaxSampleRadius = 10.0f;

    [Tooltip("Growth factor for NavMesh sampling radius (e.g. 2.0 = double each step).")]
    [SerializeField]
    private float navmeshRadiusGrowth = 2.0f;

    [Header("Movement")]
    [Tooltip("Whether to smoothly move towards the target position.")]
    [SerializeField]
    private bool smoothMove = false;

    [Tooltip("Speed of smoothing (higher = snappier).")]
    [SerializeField]
    private float smoothSpeed = 5f;

    [Header("Target")]
    [Tooltip("The player object to move")]
    [SerializeField]
    private Transform target;

    [Header("Signal Loss")]
    [Tooltip("How many consecutive nulls before declaring connection lost.")]
    [SerializeField]
    private int lostConnectionThreshold = 10;

    [SerializeField]
    [Tooltip("Consecutive identical readings to treat as stale/loss. 0 = disable.")]
    private int staleReadingThreshold = 30;

    private Coroutine pollRoutine;
    private Vector3 currentGoal;
    private bool hasGoal = false;
    public event Action<bool> OnConnectionStatusChanged;
    bool connected = false;
    private int consecutiveNulls = 0;
    private int consecutiveStale = 0;
    private bool lossDeclared = false;
    private Vector2 lastRaw; // x=uwbWorld.x, y=uwbWorld.z
    private bool hasLastRaw = false;

    private void Awake()
    {
#if UNITY_IOS && !UNITY_EDITOR
        if (target == null)
            target = transform;
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

    private void Update()
    {
        if (smoothMove && hasGoal)
        {
            if (smoothSpeed <= 0f)
                return;
            Vector3 next = Vector3.MoveTowards(
                target.position,
                currentGoal,
                smoothSpeed * Time.deltaTime
            );
            target.position = next;
            if ((next - currentGoal).sqrMagnitude < 0.0001f)
                hasGoal = false;
        }
    }

    private void OnEnable()
    {
#if UNITY_IOS && !UNITY_EDITOR
        if (pollRoutine == null)
            StartTracking();
#endif
    }

    public void StartTracking()
    {
        if (pollRoutine != null)
            return;
        Debug.Log("[UWBPositioning] Starting UWB tracking.");
        pollRoutine = StartCoroutine(PollLoop());
    }

    public void StopTracking()
    {
        if (pollRoutine == null)
            return;
        Debug.Log("[UWBPositioning] Stopping UWB tracking.");
        StopCoroutine(pollRoutine);
        pollRoutine = null;
    }

    private IEnumerator PollLoop()
    {
        Debug.Log("[UWBPositioning] Starting PollLoop.");
        if (pollIntervalSeconds <= 0f)
        {
            while (true)
            {
                TryStep();
                yield return null;
            }
        }
        else
        {
            var wait = new WaitForSeconds(pollIntervalSeconds);
            while (true)
            {
                TryStep();
                yield return wait;
            }
        }
    }

    private void TryStep()
    {
        if (!UWBLocator.TryGetPosition(out var uwbWorld))
        {
            Debug.LogWarning("[UWBPositioning] Failed to get UWB position.");
            RegisterNullFailure();
            return;
        }

        // Check for stale reading
        bool isStale = hasLastRaw && (uwbWorld.x == lastRaw.x) && (uwbWorld.z == lastRaw.y);
        if (isStale)
        {
            RegisterStaleFailure(); // handles counting + loss check
            return;
        }

        // Fresh reading
        consecutiveNulls = 0;
        consecutiveStale = 0;
        hasLastRaw = true;
        lastRaw = new Vector2(uwbWorld.x, uwbWorld.z);

        if (!connected || lossDeclared)
        {
            connected = true;
            lossDeclared = false;
            OnConnectionStatusChanged?.Invoke(true);
            Debug.Log("[UWBPositioning] UWB connected.");
        }

        Vector3 clamped = ClampToNavmesh(
            uwbWorld,
            navmeshSampleRadius,
            navmeshMaxSampleRadius,
            navmeshRadiusGrowth
        );

        if (smoothMove)
        {
            currentGoal = clamped;
            hasGoal = true;
        }
        else
        {
            target.position = clamped;
            hasGoal = false;
        }
    }

    private void RegisterNullFailure()
    {
        consecutiveNulls++;
        // stale counter should not accumulate across nulls
        CheckForLoss("null");
    }

    private void RegisterStaleFailure()
    {
        if (staleReadingThreshold <= 0)
            return;
        consecutiveStale++;
        // trace every few counts so you can see progress
        if ((consecutiveStale % 5) == 0)
            Debug.Log($"[UWBPositioning] stale={consecutiveStale}/{staleReadingThreshold}");

        // null counter should not accumulate across stales
        CheckForLoss("stale");
    }

    private void CheckForLoss(string reason)
    {
        // Count-based triggers
        bool hitNulls = consecutiveNulls >= Mathf.Max(1, lostConnectionThreshold);
        bool hitStale = staleReadingThreshold > 0 && consecutiveStale >= staleReadingThreshold;

        if (!lossDeclared && (hitNulls || hitStale))
        {
            lossDeclared = true;
            if (connected)
            {
                connected = false;
                OnConnectionStatusChanged?.Invoke(false);
            }
            Debug.LogWarning(
                $"[UWBPositioning] UWB connection lost ({reason}). Monitoring for recovery."
            );
        }
    }

    private static Vector3 ClampToNavmesh(
        Vector3 desired,
        float startRadius,
        float maxRadius,
        float growth
    )
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

        Debug.LogWarning(
            "[UWBPositioning] No NavMesh found within max radius. Using raw coordinate."
        );
        return desired;
    }

    private void OnDisable()
    {
        StopTracking();
    }

    private void OnDestroy()
    {
        StopTracking();
    }
}
