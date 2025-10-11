using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class SimpleARNavigation : MonoBehaviour
{
    [Header("UI References")]
    public RawImage cameraFeed;
    public Transform arrow3D;
    
    [Header("Pathfinding")]
    public PathProvider pathfindingObject;
    
    private WebCamTexture webcamTexture;
    private NavMeshPath currentPath;

    // Store initial rotation
    private Vector3 initialRotation;
    
    void OnEnable()
    {
        // Subscribe to path updates (null-safe)
        if (pathfindingObject != null)
        {
            pathfindingObject.OnPathUpdated += HandlePathUpdated;
        }
    }
    
    void OnDisable()
    {
        // Unsubscribe from path updates (null-safe)
        if (pathfindingObject != null)
        {
            pathfindingObject.OnPathUpdated -= HandlePathUpdated;
        }
    }
    
    void Start()
    {
        initialRotation = arrow3D.rotation.eulerAngles;

        // Start camera
        webcamTexture = new WebCamTexture();
        cameraFeed.texture = webcamTexture;
        webcamTexture.Play();
        
        // Enable compass
        Input.compass.enabled = true;
        
        // Find PathProvider if not assigned
        if (pathfindingObject == null)
        {
            pathfindingObject = FindFirstObjectByType<PathProvider>();
            
            if (pathfindingObject != null)
            {
                Debug.Log("Found PathProvider!");
                // Subscribe now that we found it
                pathfindingObject.OnPathUpdated += HandlePathUpdated;
            }
            else
            {
                Debug.LogError("Could not find PathProvider!");
            }
        }
    }
    
    private void HandlePathUpdated(NavMeshPath path)
    {
        currentPath = path;
        Debug.Log("Path updated! Corners: " + path.corners.Length);
    }
    
    void Update()
    {
        if (currentPath == null || currentPath.corners.Length < 2) return;
        
        Vector3 playerPos = currentPath.corners[0];
        Vector3 nextCorner = currentPath.corners[1];
        
        Vector3 dir = nextCorner - playerPos;
        dir.y = 0;
        
        if (dir.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            float deviceHeading = Input.compass.trueHeading;
            float relativeAngle = targetAngle - deviceHeading;
            
            // Get compass angle from north (0-360 degrees)
            float compassAngle = Input.compass.trueHeading;
            
            // North offset to align real world north with Unity north
            float northOffset = 0f; // Adjust this value to calibrate
            
            // Combine all rotations
            float finalAngle = relativeAngle + compassAngle + northOffset;
            
            // Apply rotation
            arrow3D.rotation = Quaternion.Euler(
                initialRotation.x, 
                initialRotation.y + finalAngle, 
                initialRotation.z
            );
        }
    }
}