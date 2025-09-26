// PathProvider.cs (minimal, no agent, no snap)
using System;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class PathProvider : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform player;   // defaults to this.transform
    [SerializeField] private Transform target;   // optional; you can ignore if using BoxCollider API

    [Header("Settings")]
    [SerializeField] private float sampleRadius = 2f;
    [SerializeField] private float recomputeThreshold = 0.01f;

    public bool Paused;

    public NavMeshPath CurrentPath { get; private set; }
    public event Action<NavMeshPath> OnPathUpdated;

    private Vector3 _lastPlayerPos = Vector3.positiveInfinity;
    private Vector3 _lastTargetPos = Vector3.positiveInfinity;
    private bool _usePointTarget = false;
    private Vector3 _pointTarget;

    private void Reset() { player = transform; }

    private void Awake()
    {
        if (player == null) player = transform;
        CurrentPath = new NavMeshPath();
    }

    private void Update()
    {
        if (Paused || player == null) return;

        Vector3 p = player.position;
        Vector3 t = _usePointTarget ? _pointTarget : (target == null ? p : target.position);
        if (target == null && !_usePointTarget) return;

        float threshSq = recomputeThreshold * recomputeThreshold;
        if ((p - _lastPlayerPos).sqrMagnitude < threshSq &&
            (t - _lastTargetPos).sqrMagnitude < threshSq) return;

        _lastPlayerPos = p;
        _lastTargetPos = t;

        if (TryComputePath(p, t, out var path))
        {
            CurrentPath = path;
            OnPathUpdated?.Invoke(CurrentPath);
        }
    }

    // Public API
    public void SetTarget(BoxCollider areaBox)
    {
        if (areaBox == null) return;
        _usePointTarget = true;
        _pointTarget = areaBox.transform.TransformPoint(areaBox.center);
        target = null;
        ForceRecompute();
    }

    public void SetTarget(Transform newTarget)
    {
        _usePointTarget = false;
        target = newTarget;
        ForceRecompute();
    }

    public void ForceRecompute()
    {
        _lastPlayerPos = Vector3.positiveInfinity;
        _lastTargetPos = Vector3.positiveInfinity;
    }

    public bool TryGetCorners(out Vector3[] corners)
    {
        if (CurrentPath == null || CurrentPath.corners == null || CurrentPath.corners.Length == 0)
        { corners = Array.Empty<Vector3>(); return false; }
        corners = CurrentPath.corners; return true;
    }

    // Core: static calc; never moves the player
    private bool TryComputePath(Vector3 from, Vector3 to, out NavMeshPath path)
    {
        path = new NavMeshPath();

        if (!NavMesh.SamplePosition(from, out var fromHit, sampleRadius, NavMesh.AllAreas)) return false;
        if (!NavMesh.SamplePosition(to,   out var toHit,   sampleRadius, NavMesh.AllAreas)) return false;

        bool ok = NavMesh.CalculatePath(fromHit.position, toHit.position, NavMesh.AllAreas, path);
        if (!ok || path.status == NavMeshPathStatus.PathInvalid) return false;

        return true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (CurrentPath == null || CurrentPath.corners == null || CurrentPath.corners.Length < 2) return;
        Gizmos.color = Color.yellow;
        var c = CurrentPath.corners;
        for (int i = 0; i < c.Length - 1; i++) Gizmos.DrawLine(c[i], c[i + 1]);
    }
#endif
}
