using UnityEngine;
using UnityEngine.AI;

public class ArrowNavigator : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform arrowModel; // arrow mesh root

    private PathProvider provider;
    private NavMeshPath currentPath;

    private bool connected;
    private bool paused;
    private TourUIPhase phase;

    void OnEnable()
    {
        // pick up current state immediately
        AttachProvider(TourSignals.CurrentPathProvider);
        connected = TourSignals.Connected;
        paused = TourSignals.Paused;
        phase = TourSignals.Phase;
        UpdateVisibility(); // reflect initial state

        // react to changes
        TourSignals.PathProviderChanged += AttachProvider;
        TourSignals.ConnectedChanged += OnConnectedChanged;
        TourSignals.PausedChanged += OnPausedChanged;
        TourSignals.PhaseChanged += OnPhaseChanged;
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

        // take current path snapshot if any
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
        // do not force visibility here; Update() will rotate only when valid
        UpdateVisibility();
    }

    private void Update()
    {
        if (!IsArrowActive())
            return;

        if (currentPath == null || currentPath.corners == null || currentPath.corners.Length < 2)
            return;

        // corners[0] should be player/phone, corners[1] is next waypoint
        Vector3 playerPos = currentPath.corners[0];
        Vector3 nextCorner = currentPath.corners[1];

        Vector3 dir = nextCorner - playerPos;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            arrowModel.localRotation = Quaternion.Euler(-90f, 0f, angle + 90f);
        }
    }

    private bool IsArrowActive()
    {
        // show only while navigating with a live provider and app is ready
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
