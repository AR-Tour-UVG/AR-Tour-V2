using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class TourRunner : MonoBehaviour
{
    public static TourRunner Instance { get; private set; }

    // Signals for the tour binder
    public event System.Action<FloorDefinition, FloorManager> FloorLoaded;
    public event System.Action<FloorDefinition> FloorUnloaded;
    public event System.Action TourCompleted;

    // Expose current tour progress
    public int VisitedAcrossTour => visitedAcrossTour;
    public int TotalAcrossTour => totalAcrossTour;
    public float Progress =>
        (totalAcrossTour > 0) ? (visitedAcrossTour / (float)totalAcrossTour) : 0f;
    private TourDefinition currentTour;
    private int floorIndex = -1;
    private FloorManager activeFM;
    private string loadedScenePath;
    private Scene baseScene; // Reference to the initial scene (main menu)
    private Camera fallbackCamera;
    private int visitedAcrossTour;
    private int totalAcrossTour;

    [SerializeField]
    private TourDefinition expressTour;
    public TourDefinition ExpressTour => expressTour;

    [SerializeField]
    private TourDefinition completeTour;
    public TourDefinition CompleteTour => completeTour;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        baseScene = SceneManager.GetActiveScene();
        Debug.Log($"[TourRunner] Awake. Base scene: {baseScene.name}");
    }

    // Called by UI (Ruta Express / Completa button)
    public void SelectTour(TourDefinition tour)
    {
        currentTour = tour;
        // Initialize counters
        floorIndex = 0;
        visitedAcrossTour = 0;
        totalAcrossTour = (tour != null) ? tour.TotalAreasCount() : 0;
        var name = (tour != null) ? tour.TourName : "null";
        Debug.Log($"[TourRunner] Selected tour: {name} | Total areas: {totalAcrossTour}");
    }

    // Called by UI right after SelectTour. Loads FIRST floor immediately.
    public void BeginTour()
    {
        if (
            currentTour == null
            || currentTour.OrderedFloors == null
            || currentTour.OrderedFloors.Count == 0
        )
        {
            Debug.LogError("[TourRunner] No tour/floors.");
            return;
        }

        StartCoroutine(LoadFloorAt(floorIndex));
    }

    private IEnumerator LoadFloorAt(int idx)
    {
        var floor = currentTour.OrderedFloors[idx];
        if (!floor)
        {
            Debug.LogError("[TourRunner] Null floor asset.");
            yield break;
        }
        if (string.IsNullOrEmpty(floor.ScenePath))
        {
            Debug.LogError("[TourRunner] Floor.ScenePath empty.");
            yield break;
        }

        // Set anchor map for the current floor
        if (floor.TryGetAnchorMapText(out var json))
        {
            UWBLocator.SetAnchorMap(json);
            Debug.Log($"[TourRunner] Anchor map applied for floor '{floor.FloorName}'.");
        }
        else
        {
            Debug.LogError(
                $"[TourRunner] Floor '{floor.FloorName}' has no valid AnchorMap assigned."
            );
        }

        // Load scene additively
        var op = SceneManager.LoadSceneAsync(floor.ScenePath, LoadSceneMode.Additive);
        yield return op;

        var scene = SceneManager.GetSceneByPath(floor.ScenePath);
        if (!scene.IsValid() || !scene.isLoaded)
        {
            Debug.LogError("[TourRunner] Scene failed to load.");
            yield break;
        }

        SceneManager.SetActiveScene(scene);

        // Handle cameras
        AdoptSceneCameraOrKeepFallback(scene);

        loadedScenePath = floor.ScenePath;

        // Find FM and inject this floor
        activeFM = FindFirstObjectByType<FloorManager>(FindObjectsInactive.Include);
        if (!activeFM)
        {
            Debug.LogError("[TourRunner] FloorManager not found in scene.");
            yield break;
        }

        activeFM.FloorCompleted += OnFloorCompleted;
        activeFM.AreaConfirmed += OnAreaConfirmed;
        activeFM.GlobalVisited = visitedAcrossTour;
        activeFM.GlobalTotal = totalAcrossTour;

        // Notify listeners that floor is ready
        FloorLoaded?.Invoke(currentTour.OrderedFloors[idx], activeFM);

        Debug.Log(
            $"[TourRunner] Floor loaded: {floor.FloorName}. FloorManager will wait for UserReady (R in Editor)."
        );
    }

    private void OnFloorCompleted(FloorManager _)
    {
        if (activeFM)
        {
            activeFM.FloorCompleted -= OnFloorCompleted;
            activeFM.AreaConfirmed -= OnAreaConfirmed;
        }

        StartCoroutine(UnloadAndAdvance());
    }

    private void OnAreaConfirmed(AreaDefinition _)
    {
        visitedAcrossTour++;
        Debug.Log($"[TourRunner] Global progress: ({Progress * 100:F2}%)");
    }

    private IEnumerator UnloadAndAdvance()
    {
        if (!string.IsNullOrEmpty(loadedScenePath))
        {
            Debug.Log($"[TourRunner] Unloading scene: {loadedScenePath}");
            var prevFloor = currentTour?.OrderedFloors[floorIndex];
            if (prevFloor)
            {
                FloorUnloaded?.Invoke(prevFloor);
            }
            var op = SceneManager.UnloadSceneAsync(loadedScenePath);
            if (op != null)
                yield return op;
            loadedScenePath = null;
            activeFM = null;
        }

        // Switch active scene back to base and force a camera clear
        if (baseScene.IsValid() && baseScene.isLoaded)
        {
            SceneManager.SetActiveScene(baseScene);
        }

        yield return null; // wait a frame
        EnsureFallbackCamera(); // in case the base scene has no active cameras

        floorIndex++;
        if (currentTour == null || floorIndex >= currentTour.OrderedFloors.Count)
        {
            Debug.Log("[TourRunner] Tour complete.");
            TourCompleted?.Invoke();
            yield break;
        }

        yield return LoadFloorAt(floorIndex);
    }

    private void EnsureFallbackCamera()
    {
        // If any enabled camera exists, do nothing
        foreach (var cam in Camera.allCameras)
            if (cam && cam.enabled)
                return;

        if (fallbackCamera == null)
        {
            var go = new GameObject("FallbackClearCamera");
            DontDestroyOnLoad(go);
            fallbackCamera = go.AddComponent<Camera>();
            fallbackCamera.clearFlags = CameraClearFlags.Skybox;
            fallbackCamera.cullingMask = ~0; // Everything
            fallbackCamera.depth = -100;
        }
        fallbackCamera.enabled = true;
    }

    private void DisableFallbackCamera()
    {
        if (fallbackCamera)
            fallbackCamera.enabled = false;
    }

    private void AdoptSceneCameraOrKeepFallback(Scene scene)
    {
        Camera sceneCam = null;
        var cams = FindObjectsByType<Camera>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var c in cams)
        {
            if (!c || !c.enabled)
                continue;
            if (c.gameObject.scene == scene)
            {
                sceneCam = c;
                break;
            }
        }

        if (sceneCam != null)
        {
            DisableFallbackCamera();
            Debug.Log($"[TourRunner] Using scene camera: {sceneCam.name}");
        }
        else
        {
            EnsureFallbackCamera(); // fallback clears screen; change if you want it to render geometry
            Debug.LogWarning(
                "[TourRunner] No enabled camera found in floor scene. Using fallback camera."
            );
        }
    }
}
