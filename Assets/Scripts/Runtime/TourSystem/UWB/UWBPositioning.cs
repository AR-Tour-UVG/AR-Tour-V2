using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class UWBPositioning : MonoBehaviour
{
    [Header("Polling")]
    [Tooltip("How often to poll UWBLocator for a new position.")]
    [SerializeField]
    private float pollIntervalSeconds = 0.5f;

    [Header("Filtering")]
    [Tooltip("Minimum movement distance to consider a new position valid.")]
    [SerializeField]
    private float noiseThresholdMeters = 0.10f;

    [Tooltip("Maximum speed (m/s) to consider a new position valid.")]
    [SerializeField]
    private float maxSpeedMetersPerSecond = 3.0f;

    [Tooltip("Tolerance factor for jump filtering (e.g. 1.25 = 25% extra).")]
    [SerializeField]
    private float jumpToleranceFactor = 1.25f;

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
    private int lostConnectionThreshold = 5;

    private Coroutine pollRoutine;
    private Vector3 lastAccepted;
    private bool hasLastAccepted = false;

    private int consecutiveNulls = 0;
    private bool lossDeclared = false;

    private Vector3 currentGoal;
    private bool hasGoal = false;

    public event Action<bool> OnConnectionStatusChanged;
    public bool ApplyTransforms { get; private set; } = true;

    public void SetApplyTransforms(bool apply)
    {
        ApplyTransforms = apply;
    }

    bool connected = false;

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
        var wait = new WaitForSeconds(pollIntervalSeconds <= 0f ? 0.5f : pollIntervalSeconds);
        while (true)
        {
            TryStep();
            yield return wait;
        }
    }

    private void TryStep()
    {
        if (!UWBLocator.TryGetPosition(out var uwbWorld))
        {
            Debug.LogWarning("[UWBPositioning] Failed to get UWB position.");
            HandlePossibleLoss();
            return;
        }
        if (!connected)
        {
            Debug.Log("[UWBPositioning] UWB connected.");
            connected = true;
            OnConnectionStatusChanged?.Invoke(true);
        }

        if (consecutiveNulls > 0)
        {
            if (lossDeclared)
                Debug.Log("[UWBPositioning] UWB reconnected.");
            consecutiveNulls = 0;
            lossDeclared = false;
        }
        if (hasLastAccepted)
        {
            Debug.Log("[UWBPositioning] Using last accepted position.");
            float delta = Vector3.Distance(uwbWorld, lastAccepted);
            if (delta < noiseThresholdMeters)
                return;

            float dt = Mathf.Max(0.01f, pollIntervalSeconds);
            float maxStep = maxSpeedMetersPerSecond * dt * jumpToleranceFactor;
            if (delta > maxStep)
            {
                Debug.LogWarning(
                    $"[UWBPositioning] Rejected jump {delta:F2}m (> {maxStep:F2}m in {dt:F2}s)."
                );
                return;
            }
        }

        Vector3 clamped = ClampToNavmesh(
            uwbWorld,
            navmeshSampleRadius,
            navmeshMaxSampleRadius,
            navmeshRadiusGrowth
        );

        lastAccepted = clamped;
        hasLastAccepted = true;

        if (!ApplyTransforms)
        {
            return;
        }

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

    private void HandlePossibleLoss()
    {
        consecutiveNulls++;

        if (!lossDeclared && consecutiveNulls >= Mathf.Max(1, lostConnectionThreshold))
        {
            lossDeclared = true;
            if (connected)
            {
                connected = false;
                OnConnectionStatusChanged?.Invoke(false);
            }
            Debug.LogWarning("[UWBPositioning] UWB connection lost. Waiting to reconnect…");
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
}
