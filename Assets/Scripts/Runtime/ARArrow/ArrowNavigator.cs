using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;

public class ArrowNavigator : MonoBehaviour
{
    [Header("Arrow")]
    public Transform arrow3D;
    public float arrowDistance = 2f;

    [Header("Compass")]
    [Tooltip("Place the current facing direction in real world.")]
    public float currentFacing = 0f;

    [Header("Pathfinding")]
    [SerializeField]
    private PathProvider pathfindingObject; // now optional

    [Header("AR Settings")]
    public Transform arCamera;

    private NavMeshPath currentPath;
    private ARSession arSession;
    private ARCameraManager cameraManager;
    private TourRunner tourRunner;
    private float northOffset;

    // === NEW: public binder API ===
    public void SetPathProvider(PathProvider provider)
    {
        if (pathfindingObject == provider)
            return;

        // Unhook old
        if (pathfindingObject != null)
            pathfindingObject.OnPathUpdated -= HandlePathUpdated;

        pathfindingObject = provider;

        // Hook new
        if (pathfindingObject != null)
        {
            pathfindingObject.OnPathUpdated += HandlePathUpdated;

            // Optional: seed current path if provider already has one
            HandlePathUpdated(pathfindingObject.CurrentPath);
        }
        else
        {
            currentPath = null;
            if (arrow3D)
            {
                arrow3D.gameObject.SetActive(false);
                Debug.Log("[AR Nav] Unbound from PathProvider");
            }
        }

        Debug.Log(
            pathfindingObject
                ? $"[AR Nav] Bound to PathProvider #{pathfindingObject.GetInstanceID()} on {pathfindingObject.gameObject.name}"
                : "[AR Nav] Unbound from PathProvider"
        );
    }

    void OnEnable()
    {
        // Hook existing provider if assigned in inspector
        if (pathfindingObject != null)
            pathfindingObject.OnPathUpdated += HandlePathUpdated;

        // === NEW: listen to TourRunner to follow floor changes ===
        tourRunner = FindFirstObjectByType<TourRunner>(FindObjectsInactive.Include);
        if (tourRunner != null)
        {
            tourRunner.FloorLoaded += OnFloorLoaded;
            tourRunner.FloorUnloaded += OnFloorUnloaded;
        }

        ARSession.stateChanged += OnARSessionStateChanged;
    }

    void OnDisable()
    {
        if (pathfindingObject != null)
            pathfindingObject.OnPathUpdated -= HandlePathUpdated;

        if (tourRunner != null)
        {
            tourRunner.FloorLoaded -= OnFloorLoaded;
            tourRunner.FloorUnloaded -= OnFloorUnloaded;
            tourRunner = null;
        }

        ARSession.stateChanged -= OnARSessionStateChanged;
    }

    void Start()
    {
        Debug.Log("[AR Nav] Start called");
        if (arrow3D)
            arrow3D.gameObject.SetActive(false);

        // Find AR Session
        arSession = FindFirstObjectByType<ARSession>();
        if (arSession == null)
            Debug.LogError("[AR] No ARSession found in scene! Add an ARSession GameObject.");
        else
            Debug.Log($"[AR] ARSession state: {ARSession.state}");

        // Find AR Camera Manager
        if (arCamera != null)
        {
            cameraManager = arCamera.GetComponent<ARCameraManager>();
            if (cameraManager == null)
                Debug.LogError("[AR] No ARCameraManager on camera! Add ARCameraManager component.");
            else
                cameraManager.enabled = true;

            var camBg = arCamera.GetComponent<ARCameraBackground>();
            if (camBg == null)
                Debug.LogError("[AR] No ARCameraBackground component on Main Camera!");
            else
                camBg.enabled = true;
        }
        else
        {
            Debug.LogError("[AR] AR Camera not assigned in Inspector!");
        }

        Input.compass.enabled = true;
        Input.location.Start();

        // If no provider yet, try to discover one in the currently loaded floor
        if (pathfindingObject == null)
            TryFindProviderInScene();
    }

    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        Debug.Log($"[AR] Session state changed to: {args.state}");
    }

    // === NEW: rebind when a floor scene loads/unloads ===
    private void OnFloorLoaded(FloorDefinition floor, FloorManager fm)
    {
        // Prefer the MovementAgent’s provider if present
        var ma = fm ? fm.GetComponentInChildren<MovementAgent>(true) : null;
        var pp = ma ? ma.GetComponent<PathProvider>() : null;

        if (pp == null)
        {
            // Fallbacks
            pp = FindFirstObjectByType<PathProvider>(FindObjectsInactive.Include);
            if (pp == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player)
                    pp = player.GetComponent<PathProvider>();
            }
        }

        SetPathProvider(pp);
    }

    private void OnFloorUnloaded(FloorDefinition _)
    {
        SetPathProvider(null);
    }

    private void TryFindProviderInScene()
    {
        // Mirrors TourBinder.BindPathProvider logic
        var ma = FindFirstObjectByType<MovementAgent>(FindObjectsInactive.Include);
        var pp = ma ? ma.GetComponent<PathProvider>() : null;

        if (pp == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player)
                pp = player.GetComponent<PathProvider>();
        }

        if (pp == null)
            pp = FindFirstObjectByType<PathProvider>(FindObjectsInactive.Include);

        if (pp != null)
            SetPathProvider(pp);
        else
            Debug.LogWarning("[AR Nav] No PathProvider found to bind.");
    }

    private void HandlePathUpdated(NavMeshPath path)
    {
        currentPath = path;
        bool hasPath =
            currentPath != null && currentPath.corners != null && currentPath.corners.Length >= 2;
        arrow3D.gameObject.SetActive(hasPath);

        if (!hasPath)
        {
            Debug.Log("[AR Nav] No valid path available. Arrow hidden.");
            return;
        }

        Debug.Log($"[AR Nav] Path updated: {path.corners.Length} corners");
    }

    void Update()
    {
        if (!arrow3D || !arCamera || !arrow3D.gameObject.activeSelf)
            return;

        // Keep it in front of the camera
        arrow3D.position = arCamera.position + arCamera.forward * arrowDistance;

        // Base orientation: camera pitch+yaw, roll removed
        Quaternion camNoRoll = Quaternion.LookRotation(arCamera.forward, Vector3.up);

        // Compass correction (pure yaw)
        northOffset = 360f - currentFacing;
        float compassHeading = (Input.compass.trueHeading + northOffset) % 360f;
        Quaternion compassYaw = Quaternion.AngleAxis(-compassHeading, Vector3.up);

        // Path direction (pure yaw toward next corner)
        Quaternion pathYaw = Quaternion.identity;
        if (currentPath != null && currentPath.corners != null && currentPath.corners.Length >= 2)
        {
            Vector3 playerPos = currentPath.corners[0];
            Vector3 nextCorner = currentPath.corners[1];
            Vector3 flat = new Vector3(nextCorner.x - playerPos.x, 0f, nextCorner.z - playerPos.z);
            if (flat.sqrMagnitude > 0.01f)
            {
                float ang = Mathf.Atan2(flat.x, flat.z) * Mathf.Rad2Deg;
                pathYaw = Quaternion.AngleAxis(ang, Vector3.up);
            }
        }

        // Final rotation
        arrow3D.rotation = camNoRoll * compassYaw * pathYaw;
    }
}
