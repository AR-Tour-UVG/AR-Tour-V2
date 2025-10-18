using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;
using System.Collections;

public class ARNavigationArrow : MonoBehaviour
{
    [Header("Arrow")]
    public Transform arrow3D;  // Drag your arrow here
    public float arrowDistance = 2f; // Distance in front of camera
    [Range(0f, 360f)]
    public float northOffset = 0f; // Degrees to offset compass north

    [Header("Pathfinding")]
    public PathProvider pathfindingObject;
    
    [Header("AR Settings")]
    public Transform arCamera; 
    
    private NavMeshPath currentPath;
    private ARSession arSession;
    private ARCameraManager cameraManager;
    
    void OnEnable()
    {
        if (pathfindingObject != null)
            pathfindingObject.OnPathUpdated += HandlePathUpdated;
            
        ARSession.stateChanged += OnARSessionStateChanged;
    }
    
    void OnDisable()
    {
        if (pathfindingObject != null)
            pathfindingObject.OnPathUpdated -= HandlePathUpdated;
            
        ARSession.stateChanged -= OnARSessionStateChanged;
    }
    
    void Start()
    {
        Debug.Log("[AR Nav] Start called");
        
        // Find AR Session
        arSession = FindFirstObjectByType<ARSession>();
        if (arSession == null)
        {
            Debug.LogError("[AR] No ARSession found in scene! Add an ARSession GameObject.");
        }
        else
        {
            Debug.Log($"[AR] ARSession state: {ARSession.state}");
        }

        // Find AR Camera Manager
        if (arCamera != null)
        {
            cameraManager = arCamera.GetComponent<ARCameraManager>();
            if (cameraManager == null)
            {
                Debug.LogError("[AR] No ARCameraManager on camera! Add ARCameraManager component.");
            }
            else
            {
                Debug.Log("[AR] ARCameraManager found");
                Debug.Log($"[AR] ARCameraManager enabled: {cameraManager.enabled}");
                
                // Make sure camera manager is enabled
                cameraManager.enabled = true;
            }
            
            ARCameraBackground camBg = arCamera.GetComponent<ARCameraBackground>();
            if (camBg == null)
            {
                Debug.LogError("[AR] No ARCameraBackground component on Main Camera!");
            }
            else
            {
                Debug.Log("[AR] ARCameraBackground found");
                Debug.Log($"[AR] ARCameraBackground enabled: {camBg.enabled}");
                
                // Make sure background component is enabled
                camBg.enabled = true;
                
                Debug.Log($"[AR] Background rendering mode: {camBg.backgroundRenderingEnabled}");
            }
        }
        else
        {
            Debug.LogError("[AR] AR Camera not assigned in Inspector!");
        }
        
        Input.compass.enabled = true;
        Input.location.Start();
        
        if (pathfindingObject == null)
        {
            pathfindingObject = FindFirstObjectByType<PathProvider>();
            if (pathfindingObject != null)
            {
                pathfindingObject.OnPathUpdated += HandlePathUpdated;
            }
        }
        
        StartCoroutine(CheckARStatus());
    }
    
    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        Debug.Log($"[AR] Session state changed to: {args.state}");
        
        switch (args.state)
        {
            case ARSessionState.Ready:
                Debug.Log("[AR] ✓ AR Session is READY! Camera should be working now.");
                break;
            case ARSessionState.SessionInitializing:
                Debug.Log("[AR] AR Session initializing...");
                break;
            case ARSessionState.Unsupported:
                Debug.LogError("[AR] AR is not supported on this device!");
                break;
            case ARSessionState.NeedsInstall:
                Debug.LogError("[AR] ARCore/ARKit needs to be installed!");
                break;
        }
    }
    
    IEnumerator CheckARStatus()
    {
        // Check every second for 10 seconds
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log($"[AR] Status check {i + 1}: State = {ARSession.state}");
            
            if (ARSession.state == ARSessionState.Ready)
            {
                Debug.Log("[AR] ✓ Camera feed should be visible now!");
                yield break;
            }
        }
        
        Debug.LogWarning("[AR] Session took too long to initialize. Check device permissions!");
    }
    
    private void HandlePathUpdated(NavMeshPath path)
    {
        currentPath = path;
        Debug.Log($"[Nav] Path updated: {path.corners.Length} corners");
    }
    
    void Update()
    {
        // Position arrow in front of camera
        if (arCamera != null)
        {
            arrow3D.position = arCamera.position + arCamera.forward * arrowDistance;
        }
        
        // ===== STEP 1: Lock arrow Y rotation to match camera Y rotation =====
        // Extract only the Y rotation from camera
        Vector3 cameraEuler = arCamera.rotation.eulerAngles;
        arrow3D.rotation = Quaternion.Euler(0, cameraEuler.y, 0);
        // STOP HERE and comment out steps 2 & 3 to test ONLY camera lock

        // ===== STEP 2: Apply compass correction =====
        float compassHeading = Input.compass.trueHeading;
        compassHeading = (compassHeading + northOffset) % 360f;
        arrow3D.rotation = arrow3D.rotation * Quaternion.Euler(0, -compassHeading, 0);
        // STOP HERE and comment out step 3 to test camera + compass
        
        // ===== STEP 3: Apply target direction =====
        if (currentPath != null && currentPath.corners.Length >= 2)
        {
            Vector3 playerPos = currentPath.corners[0];
            Vector3 nextCorner = currentPath.corners[1];
            Vector3 targetDirection = nextCorner - playerPos;
            
            Vector3 targetFlat = new Vector3(targetDirection.x, 0, targetDirection.z);
            
            if (targetFlat.sqrMagnitude > 0.01f)
            {
                float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
                arrow3D.rotation = arrow3D.rotation * Quaternion.Euler(0, targetAngle, 0);
            }
        }
    }
}