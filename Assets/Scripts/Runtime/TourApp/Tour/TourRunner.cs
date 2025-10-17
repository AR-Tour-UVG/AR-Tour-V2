using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DisallowMultipleComponent]
public class TourRunner : MonoBehaviour
{
    public static TourRunner Instance { get; private set; }

    [SerializeField] private TourDefinition currentTour;
    private int floorIndex = -1;
    private FloorManager activeFM;
    private string loadedScenePath;
    private Scene baseScene; // Reference to the initial scene (main menu)
    private Camera _fallbackCamera;
    private int _visitedAcrossTour;
    private int _totalAcrossTour;
    private double _percentComplete => (_totalAcrossTour > 0) ? (100.0 * _visitedAcrossTour / _totalAcrossTour) : 0.0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
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
        _visitedAcrossTour = 0;
        _totalAcrossTour = (tour != null) ? tour.TotalAreasCount() : 0;
        var name = (tour != null) ? tour.TourName : "null";
        Debug.Log($"[TourRunner] Selected tour: {name} | Total areas: {_totalAcrossTour}");
    }

    // Called by UI right after SelectTour. Loads FIRST floor immediately.
    public void BeginTour()
    {
        if (currentTour == null || currentTour.OrderedFloors == null || currentTour.OrderedFloors.Count == 0)
        { Debug.LogError("[TourRunner] No tour/floors."); return; }

        StartCoroutine(LoadFloorAt(floorIndex));
    }

    private IEnumerator LoadFloorAt(int idx)
    {
        DisableFallbackCamera(); // Floor will provide its own cameras
        var floor = currentTour.OrderedFloors[idx];
        if (!floor) { Debug.LogError("[TourRunner] Null floor asset."); yield break; }
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
            Debug.LogError($"[TourRunner] Floor '{floor.FloorName}' has no valid AnchorMap assigned.");
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
        activeFM.GlobalVisited = _visitedAcrossTour;
        activeFM.GlobalTotal = _totalAcrossTour;

        Debug.Log($"[TourRunner] Floor loaded: {floor.FloorName}. FloorManager will wait for UserReady (R in Editor).");
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
        _visitedAcrossTour++;
        Debug.Log($"[TourRunner] Global progress: ({_percentComplete:F2}%)");
    }

    private IEnumerator UnloadAndAdvance()
    {
        if (!string.IsNullOrEmpty(loadedScenePath))
        {
            Debug.Log($"[TourRunner] Unloading scene: {loadedScenePath}");
            var op = SceneManager.UnloadSceneAsync(loadedScenePath);
            if (op != null) yield return op;
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
            yield break;
        }

        yield return LoadFloorAt(floorIndex);
    }

    private void EnsureFallbackCamera()
    {
        // If any enabled camera exists, do nothing
        foreach (var cam in Camera.allCameras)
            if (cam && cam.enabled) return;

        if (_fallbackCamera == null)
        {
            var go = new GameObject("FallbackClearCamera");
            DontDestroyOnLoad(go);
            _fallbackCamera = go.AddComponent<Camera>();
            _fallbackCamera.clearFlags = CameraClearFlags.SolidColor;
            _fallbackCamera.backgroundColor = Color.black;  // or whatever
            _fallbackCamera.cullingMask = 0;                // Nothing
            _fallbackCamera.depth = -100;
        }
        _fallbackCamera.enabled = true;
    }

    private void DisableFallbackCamera()
    {
        if (_fallbackCamera) _fallbackCamera.enabled = false;
    }
}
