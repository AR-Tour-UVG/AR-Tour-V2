using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls the navigation agent's movement and pathfinding.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NavigationAgent : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Destination transform that the agent will attempt to reach.")]
    public Transform target;

    [Tooltip("Speed at which the agent moves toward the most recent UWB position.")]
    [Range(0.5f, 10f)] public float followSpeed = 5f;

    [Tooltip("Maximum search radius for snapping to the nearest valid NavMesh position.")]
    [Range(0.05f, 1f)] public float sampleRadius = 0.3f;

    [Header("Path Recompute")]
    [Tooltip("Maximum frequency (in Hz) for recalculating the navigation path.")]
    [Range(1f, 10f)] public float pathRefreshHz = 5f;

    [Tooltip("Minimum movement distance (in meters) before triggering a path recalculation.")]
    [Range(0.01f, 1f)] public float minMoveToRepath = 0.1f;

    [Header("Diagnostics")]

    [Tooltip("Timeout waiting for NavMesh (seconds). 0 = no timeout.")]
    [Range(0f, 30f)] public float navmeshWaitTimeout = 8f;

    [Tooltip("Timeout waiting for UWB init (seconds). 0 = no timeout.")]
    [Range(0f, 30f)] public float uwbWaitTimeout = 8f;

    // NavMesh components and path state
    private NavMeshAgent agent;        // NavMeshAgent responsible for path calculation
    private NavMeshPath navPath;       // Current computed path
    private Vector3 lastKnownPosition; // Most recent valid NavMesh position from UWB data

    // Internal state for path recalculation
    private float nextPathTime;
    private Vector3 lastPathFrom;

    // Debugging Helpers
    private bool _everGotUWB;

    /// <summary>
    /// Initializes the agent and sets up the NavMesh.
    /// </summary>
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component

        // Disable automatic position and rotation updates
        agent.updatePosition = false;
        agent.updateRotation = false;

        // Initialize the NavMeshPath and last known position
        navPath = new NavMeshPath();
        lastKnownPosition = transform.position;
        lastPathFrom = transform.position;
    }

    /// <summary>
    /// Initializes the NavMeshAgent and sets up the pathfinding.
    /// </summary>
    /// <returns>
    /// An enumerator for the coroutine.
    /// </returns>
    private IEnumerator Start()
    {
        // Gate 1: wait until NavMesh is ready before enabling the agent
        agent.enabled = false; // Disable the NavMeshAgent to prevent movement before NavMesh is ready
        bool firstMeshCheck = true; // Flag to log NavMesh readiness only once
        float t0 = Time.realtimeSinceStartup;

        while (NavMesh.CalculateTriangulation().vertices.Length == 0)
        {
            if (firstMeshCheck)
            {
                Debug.Log("[NavigationAgent] Waiting for NavMesh to be ready...");
                firstMeshCheck = false;
            }
            if (navmeshWaitTimeout > 0f && Time.realtimeSinceStartup - t0 > navmeshWaitTimeout)
            {
                Debug.LogError($"[NavigationAgent] NavMesh never became available after {navmeshWaitTimeout}s. " +
                               $"Did you bake/include it or build at runtime?");
                yield break;
            }
            yield return null;
        }
        Debug.Log("[NavigationAgent] NavMesh is ready.");

        // Gate 2: Wait until the UWB Locator is initialized
#if UNITY_IOS && !UNITY_EDITOR
        float t1 = Time.realtimeSinceStartup;
        while (!UWBLocator.IsInitialized)
        {
            Debug.Log("[NavigationAgent] Waiting for UWBLocator to initialize...");
            
            if (uwbWaitTimeout > 0f && Time.realtimeSinceStartup - t1 > uwbWaitTimeout)
            {
                Debug.LogError($"[NavigationAgent] UWBLocator never initialized after {uwbWaitTimeout}s. " +
                               $"Is the UWB bridge set up correctly?");
                yield break;
            }
            yield return null;
        }
        Debug.Log("[NavigationAgent] UWBLocator is initialized.");
#endif
        // Snap starting transform to nearest on-Mesh position
        if (NavMesh.SamplePosition(transform.position, out var hit, 1f, NavMesh.AllAreas))
        {
            transform.position = hit.position; // Snap to nearest NavMesh position
            lastKnownPosition = hit.position; // Update last known position
            lastPathFrom = hit.position;      // Update last path start position
            Debug.Log($"[NavigationAgent] Snapped to nearest NavMesh position at {hit.position}");
        }

        agent.enabled = true; // Enable the NavMeshAgent after confirming NavMesh is ready
        Debug.Log("[NavigationAgent] NavMeshAgent enabled and ready.");
    }

    /// <summary>
    /// Updates the agent's position and pathfinding.
    /// </summary>
    void Update()
    {
        bool gotUWB = UWBLocator.TryGetPosition(out Vector3 uwbPosition);

        // Attempt to get UWB position from the bridge
        if (gotUWB)
        {
            if (!_everGotUWB)
            {
                Debug.Log($"[NavigationAgent] Successfully received first UWB pos: {uwbPosition}");
                _everGotUWB = true;
            }

            // Preserve current height (Y)
            uwbPosition.y = transform.position.y;

            // Snap to nearest valid point inside the NavMesh if possible
            if (NavMesh.SamplePosition(uwbPosition, out var hit, sampleRadius, NavMesh.AllAreas))
            {
                lastKnownPosition = hit.position; // Update last known position
            }

            // Smoothly move toward the last known position
            transform.position = Vector3.Lerp(
                transform.position,
                lastKnownPosition,
                Time.deltaTime * followSpeed
            );

            // Check if there's a target and the agent is enabled
            if (target != null && agent.enabled)
            {
                // Determine if the agent has moved enough
                bool movedEnough =
                    (transform.position - lastPathFrom).sqrMagnitude >
                    (minMoveToRepath * minMoveToRepath);
                bool timeout = Time.time >= nextPathTime;

                // Check if the agent has moved enough or if the path has timed out
                if (movedEnough || timeout)
                {
                    // Calculate the path to the target
                    NavMesh.CalculatePath(
                        transform.position,
                        target.position,
                        NavMesh.AllAreas,
                        navPath
                    );

                    lastPathFrom = transform.position; // Update last path start position
                    nextPathTime = Time.time + 1f / Mathf.Max(1f, pathRefreshHz); // Update next path time
                }
            }
            else
            {
                // No target to follow -> clear the path
                navPath.ClearCorners();
            }

        }
#if UNITY_EDITOR
        // else: No valid UWB position found this frame -> keep the last known position
        // but get calculated path to target just for debugging purposes
        if (target != null)
        {
            NavMesh.CalculatePath(
                lastKnownPosition,
                target.position,
                NavMesh.AllAreas,
                navPath
            );
        }
#endif
    }

    /// <summary>
    /// Gets the current NavMesh path and exposes it for external use.
    /// </summary>
    /// <returns>The current NavMesh path.</returns>
    public NavMeshPath GetCurrentPath() => navPath;


    /// <summary>
    /// Draws gizmos for the current NavMesh path.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (navPath == null || navPath.corners == null || navPath.corners.Length < 2) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < navPath.corners.Length - 1; i++)
            Gizmos.DrawLine(navPath.corners[i], navPath.corners[i + 1]);
    }
}
