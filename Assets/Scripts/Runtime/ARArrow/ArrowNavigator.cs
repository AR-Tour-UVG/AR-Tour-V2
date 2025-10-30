using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;

public class ArrowNavigator : MonoBehaviour
{
    [Header("Arrow")]
    public Transform arrow3D;
    public float arrowDistance = 2f;

    [Range(0f, 360f)]
    public float northOffset = 0f;

    [Header("Pathfinding")]
    [SerializeField]
    private PathProvider pathfindingObject; // now optional

    [Header("AR Settings")]
    public Transform arCamera;

    private NavMeshPath currentPath;
    private ARSession arSession;
    private ARCameraManager cameraManager;
    private TourRunner tourRunner;

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
        if (!arrow3D || !arCamera)
            return;

        if (!arrow3D.gameObject.activeSelf)
            return;

        // Place arrow in front of camera
        arrow3D.position = arCamera.position + arCamera.forward * arrowDistance;

        // Step 1: camera yaw
        var cameraEuler = arCamera.rotation.eulerAngles;
        arrow3D.rotation = Quaternion.Euler(0, cameraEuler.y, 0);

        // Step 2: compass correction
        var compassHeading = (Input.compass.trueHeading + northOffset) % 360f;
        arrow3D.rotation = arrow3D.rotation * Quaternion.Euler(0, -compassHeading, 0);

        // Step 3: path direction
        if (currentPath != null && currentPath.corners != null && currentPath.corners.Length >= 2)
        {
            var playerPos = currentPath.corners[0];
            var nextCorner = currentPath.corners[1];
            var dir = nextCorner - playerPos;
            var flat = new Vector3(dir.x, 0, dir.z);
            if (flat.sqrMagnitude > 0.01f)
            {
                var ang = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                arrow3D.rotation = arrow3D.rotation * Quaternion.Euler(0, ang, 0);
            }
        }
    }
}
