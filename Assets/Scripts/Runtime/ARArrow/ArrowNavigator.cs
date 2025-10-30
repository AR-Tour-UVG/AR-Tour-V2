using UnityEngine;
using UnityEngine.AI;

public class ArrowNavigator : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform arrowModel; // arrow mesh root

    // NEW: AR camera and display params
    [Header("AR")]
    [SerializeField]
    private Transform arCamera; // assign AR camera

    [SerializeField]
    private float arrowDistance = 2f;

    [SerializeField, Range(0f, 360f)]
    private float northOffset = 0f;

    [SerializeField]
    private float turnSmoothing = 12f;

    private PathProvider provider;
    private NavMeshPath currentPath;

    private bool connected;
    private bool paused;
    private TourUIPhase phase;

    void OnEnable()
    {
        AttachProvider(TourSignals.CurrentPathProvider);
        connected = TourSignals.Connected;
        paused = TourSignals.Paused;
        phase = TourSignals.Phase;
        UpdateVisibility();

        TourSignals.PathProviderChanged += AttachProvider;
        TourSignals.ConnectedChanged += OnConnectedChanged;
        TourSignals.PausedChanged += OnPausedChanged;
        TourSignals.PhaseChanged += OnPhaseChanged;

        // NEW: sensors for optional compass trim
        Input.compass.enabled = true;
        Input.location.Start();
    }

    void OnDisable()
    {
        TourSignals.PathProviderChanged -= AttachProvider;
        TourSignals.ConnectedChanged -= OnConnectedChanged;
        TourSignals.PausedChanged -= OnPausedChanged;
        TourSignals.PhaseChanged -= OnPhaseChanged;
        DetachProvider();
    }

    private void AttachProvider(PathProvider pp)
    {
        if (provider == pp)
            return;
        DetachProvider();
        provider = pp;
        if (provider != null)
            provider.OnPathUpdated += HandlePathUpdated;

        currentPath = provider ? provider.CurrentPath : null;
        UpdateVisibility();
    }

    private void DetachProvider()
    {
        if (provider != null)
        {
            provider.OnPathUpdated -= HandlePathUpdated;
            provider = null;
        }
        currentPath = null;
    }

    private void OnConnectedChanged(bool c)
    {
        connected = c;
        UpdateVisibility();
    }

    private void OnPausedChanged(bool p)
    {
        paused = p;
        UpdateVisibility();
    }

    private void OnPhaseChanged(TourUIPhase ph)
    {
        phase = ph;
        UpdateVisibility();
    }

    private void HandlePathUpdated(NavMeshPath path)
    {
        currentPath = path;
        UpdateVisibility();
    }

    void Update()
    {
        if (!IsArrowActive())
            return;

        // NEW: place arrow in front of camera every frame
        if (arCamera == null)
            return;

        Vector3 camPos = arCamera.position;
        Vector3 camFwd = arCamera.forward;
        camFwd.y = 0f;
        if (camFwd.sqrMagnitude < 1e-4f)
            camFwd = Vector3.forward;
        camFwd.Normalize();

        arrowModel.position = camPos + camFwd * arrowDistance;

        // Base rotation = camera yaw
        float camYaw = arCamera.eulerAngles.y;
        Quaternion rot = Quaternion.Euler(0f, camYaw, 0f);

        // Optional compass correction
        float compass = Input.compass.enabled ? Input.compass.trueHeading : 0f;
        compass = (compass + northOffset) % 360f;
        rot *= Quaternion.Euler(0f, -compass, 0f);

        // Path heading: from camera to next corner
        if (currentPath != null && currentPath.corners != null && currentPath.corners.Length >= 2)
        {
            Vector3 nextCorner = currentPath.corners[1];
            Vector3 dir = nextCorner - camPos; // use camera, not corners[0]
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.01f)
            {
                float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                rot *= Quaternion.Euler(0f, targetAngle, 0f);
            }
        }

        // Smooth apply
        arrowModel.rotation = Quaternion.Slerp(
            arrowModel.rotation,
            rot,
            1f - Mathf.Exp(-turnSmoothing * Time.deltaTime)
        );
    }

    private bool IsArrowActive()
    {
        if (arrowModel == null || arCamera == null)
            return false;
        if (provider == null)
            return false;
        if (paused)
            return false;
        if (!connected)
            return false;
        if (phase != TourUIPhase.Navigating)
            return false;
        if (currentPath == null || currentPath.corners == null || currentPath.corners.Length < 2)
            return false;
        // If you want the arrow to stay visible while waiting for a path, relax the two lines above.
        return true;
    }

    private void UpdateVisibility()
    {
        if (arrowModel == null)
            return;
        bool show = IsArrowActive();
        if (arrowModel.gameObject.activeSelf != show)
            arrowModel.gameObject.SetActive(show);
    }
}
