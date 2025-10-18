using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class SimpleARNavigation : MonoBehaviour
{
    [Header("UI References")]
    public RawImage cameraFeed;
    public Transform arrowParent;  // NEW: The parent for yaw rotation
    public Transform arrow3D;       // CHANGED: Now the child for pitch rotation
    
    [Header("Pathfinding")]
    public PathProvider pathfindingObject;
    
    private WebCamTexture webcamTexture;
    private NavMeshPath currentPath;
    private Vector3 initialRotation;
    
    void OnEnable()
    {
        if (pathfindingObject != null)
            pathfindingObject.OnPathUpdated += HandlePathUpdated;
    }
    
    void OnDisable()
    {
        if (pathfindingObject != null)
            pathfindingObject.OnPathUpdated -= HandlePathUpdated;
            
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
            webcamTexture = null;
        }
    }

    void Start()
    {

        // Make sure arrow is child of arrowParent
        if (arrow3D.parent != arrowParent)
        {
            Debug.LogWarning("[Arrow] Arrow should be child of ArrowParent!");
        }

        // Reset both to zero for clean slate
        arrowParent.rotation = Quaternion.identity;
        arrow3D.localRotation = Quaternion.identity;

        initialRotation = arrow3D.localRotation.eulerAngles;

        // Start front camera
        webcamTexture = new WebCamTexture(
            WebCamTexture.devices[0].name,
            1920,
            1080
        );
        cameraFeed.texture = webcamTexture;
        webcamTexture.Play();

        // Setup camera orientation
        StartCoroutine(FixCameraOrientation());

        // Enable compass and gyroscope
        Input.compass.enabled = true;

        Input.gyro.updateInterval = 0.01f;
        Input.gyro.enabled = true;  // ADD THIS LINE
        Input.location.Start();
        
        Debug.Log($"[Sensors] Gyro enabled: {Input.gyro.enabled}");

        // Find PathProvider if not assigned
        if (pathfindingObject == null)
        {
            pathfindingObject = FindFirstObjectByType<PathProvider>();
            if (pathfindingObject != null)
            {
                pathfindingObject.OnPathUpdated += HandlePathUpdated;
            }
        }
    }

    IEnumerator FixCameraOrientation()
    {
        yield return new WaitForSeconds(0.5f);
        
        RectTransform rtCamFeed = cameraFeed.GetComponent<RectTransform>();
        rtCamFeed.sizeDelta = new Vector2(Screen.height, Screen.width);
        rtCamFeed.localRotation = Quaternion.Euler(0, 0, 90);
        Debug.Log($"RawImage size: {rtCamFeed.sizeDelta}, Rotation: 90°");

        RectTransform rt = cameraFeed.GetComponent<RectTransform>();                                     
            
        rt.localScale = new Vector3(2532.00f / 1170.00f, 2532.00f / 2532.00f, 1);
        // rt.localScale = new Vector3(2532.00f / 1170.00f, 1170.00f / 2532.00f, 1);        
        Debug.Log($"Canvas size changed to: {rt.localScale}");
                         
    }
    
    private void HandlePathUpdated(NavMeshPath path)
    {
        currentPath = path;
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
            // Calculate compass-based direction
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            float compassHeading = Input.compass.trueHeading;
            float yaw = targetAngle - compassHeading;

            // // PARENT: Rotate side-to-side using Quaternion (no gimbal lock!)
            // arrowParent.rotation = Quaternion.Euler(0, yaw, 0);

            // CHILD: Rotate up-down using Euler (simple pitch)
            float pitch = 0f;
            float rawPitch = 0f;
            float adjustedPitch = 0f;

            if (Input.gyro.enabled)
            {
                // Raw gyro data
                Quaternion attitude = Input.gyro.attitude;
                Vector3 eulerAngles = attitude.eulerAngles;
                Vector3 rotationRate = Input.gyro.rotationRate; // Angular velocity
                Vector3 gravity = Input.gyro.gravity; // Gravity vector

                Debug.Log($"[Gyro Raw] Attitude Euler - X:{eulerAngles.x:F1} Y:{eulerAngles.y:F1} Z:{eulerAngles.z:F1}");
                Debug.Log($"[Gyro Raw] Gravity: {gravity}");

                // Try just using one axis directly
                pitch = eulerAngles.y; // Or try x, y
                rawPitch = pitch;
                adjustedPitch = clampAngle(rawPitch); // Clamp between -90 and 90  
                adjustedPitch = (adjustedPitch + 90f) % 360f; // adjust for Arrow model                

                // Normalize
                // if (pitch > 180) pitch -= 360;

                // pitch = Mathf.Clamp(pitch, -90f, 90f);
            }

            // Apply pitch to child (local rotation)
            arrow3D.localRotation = Quaternion.Euler(adjustedPitch, 0f, 0f); ; // Adjust for initial model orientation
            Debug.Log($"Raw: {rawPitch:F1}° → Adjusted: {adjustedPitch:F1}° → Actual X Rotation: {arrow3D.localEulerAngles.x:F1}°");

            // Debug
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"[Arrow] Yaw: {yaw:F1}°, Pitch: {pitch:F1}°");
            }
        }
    }

    float clampAngle(float angle)
    {
        if (angle < 180f)
        {
            return 180f;
        }

        return angle;
    }
}