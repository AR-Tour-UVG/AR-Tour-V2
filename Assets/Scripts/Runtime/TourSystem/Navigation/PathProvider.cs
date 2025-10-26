using System;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class PathProvider : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float sampleRadius = 2f;

    [SerializeField]
    private float recomputeThreshold = 0.01f;

    [Header("Optional")]
    [Tooltip("Assign an Area GameObject in the scene to start with (uses its BoxCollider center).")]
    [SerializeField]
    private GameObject initialTarget;

    [SerializeField]
    private Transform playerPosition;

    public bool Paused;
    public NavMeshPath CurrentPath { get; private set; }
    public float CurrentDistance { get; private set; }
    public event Action<NavMeshPath> OnPathUpdated;

    private Vector3 _lastPlayerPos = Vector3.positiveInfinity;
    private Vector3 _lastTargetPos = Vector3.positiveInfinity;
    private bool _hasTargetPoint = false;
    private Vector3 _targetPoint;

    private void Awake()
    {
        CurrentPath = new NavMeshPath();

        if (playerPosition == null)
        {
            var movement = FindFirstObjectByType<MovementAgent>(FindObjectsInactive.Include);
            if (movement != null)
            {
                playerPosition = movement.transform;
            }
            else
            {
                var tagged = GameObject.FindWithTag("Player");
                if (tagged != null)
                    playerPosition = tagged.transform;
            }

            if (playerPosition == null)
                Debug.LogWarning("[PathProvider] No player position found. Distance will stay 0.");
        }
    }

    private void Start()
    {
        if (initialTarget)
            SetTarget(initialTarget);
    }

    private void Update()
    {
        if (Paused || !_hasTargetPoint)
            return;

        Vector3 p = playerPosition ? playerPosition.position : transform.position;
        Vector3 t = _targetPoint;

        float threshSq = recomputeThreshold * recomputeThreshold;
        if (
            (p - _lastPlayerPos).sqrMagnitude < threshSq
            && (t - _lastTargetPos).sqrMagnitude < threshSq
        )
            return;

        _lastPlayerPos = p;
        _lastTargetPos = t;

        if (TryComputePath(p, t, out var path))
        {
            CurrentPath = path;
            CurrentDistance = ComputePathDistance(CurrentPath);
            OnPathUpdated?.Invoke(CurrentPath);
        }
        else
        {
            CurrentPath = null;
            CurrentDistance = 0f;
            OnPathUpdated?.Invoke(null);
        }
    }

    private float ComputePathDistance(NavMeshPath path)
    {
        if (path == null || path.corners.Length < 2)
            return 0f;

        float dist = 0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            dist += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return dist;
    }

    public void SetTarget(GameObject areaGO)
    {
        if (!areaGO)
            return;

        var ai = areaGO.GetComponent<AreaInstance>();
        var box = ai ? ai.NavTarget : areaGO.GetComponent<BoxCollider>();
        if (!box)
        {
            Debug.LogWarning("[PathProvider] Target GameObject has no AreaInstance/BoxCollider.");
            return;
        }

        _targetPoint = box.transform.TransformPoint(box.center);
        _hasTargetPoint = true;
        ForceRecompute();
    }

    public void ClearTarget()
    {
        _hasTargetPoint = false;
        CurrentPath = null;
        CurrentDistance = 0f;
        OnPathUpdated?.Invoke(null);
    }

    public void ForceRecompute()
    {
        _lastPlayerPos = Vector3.positiveInfinity;
        _lastTargetPos = Vector3.positiveInfinity;
    }

    private bool TryComputePath(Vector3 from, Vector3 to, out NavMeshPath path)
    {
        path = new NavMeshPath();
        if (!NavMesh.SamplePosition(from, out var fromHit, sampleRadius, NavMesh.AllAreas))
            return false;
        if (!NavMesh.SamplePosition(to, out var toHit, sampleRadius, NavMesh.AllAreas))
            return false;
        if (!NavMesh.CalculatePath(fromHit.position, toHit.position, NavMesh.AllAreas, path))
            return false;
        return path.status != NavMeshPathStatus.PathInvalid;
    }
}
