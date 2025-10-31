using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class TourRunner : MonoBehaviour
{
    public static TourRunner Instance { get; private set; }

    public event System.Action<FloorDefinition, FloorManager> FloorLoaded;
    public event System.Action<FloorDefinition> FloorUnloaded;
    public event System.Action TourCompleted;

    public int VisitedAcrossTour => visitedAcrossTour;
    public int TotalAcrossTour => totalAcrossTour;
    public float Progress =>
        (totalAcrossTour > 0) ? (visitedAcrossTour / (float)totalAcrossTour) : 0f;
    private TourDefinition currentTour;
    private int floorIndex = -1;
    private FloorManager activeFM;
    private string loadedScenePath;
    private Scene baseScene;

    private int visitedAcrossTour;
    private int totalAcrossTour;

    private bool isStopping;
    public bool IsStopping => isStopping;

    [SerializeField]
    private TourDefinition expressTour;
    public TourDefinition ExpressTour => expressTour;

    [SerializeField]
    private TourDefinition completeTour;
    public TourDefinition CompleteTour => completeTour;

    [SerializeField]
    private ArrowSceneDefinition arrowSceneDef;

    [SerializeField]
    [Tooltip("If true, use AR mode for the tour.")]
    private bool useAR = false;

    [SerializeField, Tooltip("Disable floor scene cameras when AR is on.")]
    private bool disableFloorCamerasInAR = true;

    private bool arOverlayLoaded = false;

    private bool waitingForUserToContinue;
    public bool WaitingForUserToContinue => waitingForUserToContinue;

    public TourDefinition CurrentTour => currentTour;

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

    public void SelectTour(TourDefinition tour)
    {
        currentTour = tour;
        floorIndex = 0;
        visitedAcrossTour = 0;
        totalAcrossTour = (tour != null) ? tour.TotalAreasCount() : 0;
        var name = (tour != null) ? tour.TourName : "null";
        Debug.Log($"[TourRunner] Selected tour: {name} | Total areas: {totalAcrossTour}");
    }

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
#if UNITY_EDITOR
        Debug.Log("[TourRunner] Running in Editor mode. AR features disabled.");
        useAR = false;
#endif
        if (useAR)
        {
            if (arrowSceneDef != null && !string.IsNullOrEmpty(arrowSceneDef.ScenePath))
            {
                var sc = SceneManager.GetSceneByPath(arrowSceneDef.ScenePath);
                if (!sc.IsValid() || !sc.isLoaded)
                {
                    Debug.Log("[TourRunner] Loading AR Arrow scene additively...");
                    SceneManager.LoadSceneAsync(arrowSceneDef.ScenePath, LoadSceneMode.Additive);
                    arOverlayLoaded = true;
                }
            }
            else
            {
                Debug.LogWarning("[TourRunner] ArrowSceneDefinition not assigned or empty path.");
            }
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

        if (floor.TryGetAnchorMapText(out var json))
        {
#if UNITY_IOS && !UNITY_EDITOR
            UWBLocator.SetAnchorMap(json);
            UWBLocator.Start();
#endif
            Debug.Log($"[TourRunner] Anchor map applied for floor '{floor.FloorName}'.");
        }
        else
        {
            Debug.LogError(
                $"[TourRunner] Floor '{floor.FloorName}' has no valid AnchorMap assigned."
            );
        }

        var op = SceneManager.LoadSceneAsync(floor.ScenePath, LoadSceneMode.Additive);
        yield return op;

        var scene = SceneManager.GetSceneByPath(floor.ScenePath);
        if (!scene.IsValid() || !scene.isLoaded)
        {
            Debug.LogError("[TourRunner] Scene failed to load.");
            yield break;
        }

        loadedScenePath = floor.ScenePath;
        ApplyCameraPolicy(scene);

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

        yield return null;

        floorIndex++;
        if (currentTour == null || floorIndex >= currentTour.OrderedFloors.Count)
        {
            Debug.Log("[TourRunner] Tour complete.");
            TourCompleted?.Invoke();
            yield break;
        }

        waitingForUserToContinue = true;
        Debug.Log("[TourRunner] Waiting for user to continue to next floor.");
    }

    public void ContinueToNextFloor()
    {
        if (!waitingForUserToContinue)
            return;
        waitingForUserToContinue = false;
        StartCoroutine(LoadFloorAt(floorIndex));
    }

    public void StopTour(bool returnToHome)
    {
        isStopping = true;
        waitingForUserToContinue = false;

        // proactively unhook before unload to stop late events
        if (activeFM)
        {
            activeFM.FloorCompleted -= OnFloorCompleted;
            activeFM.AreaConfirmed -= OnAreaConfirmed;
        }

        if (!string.IsNullOrEmpty(loadedScenePath))
        {
            var prevFloor = currentTour?.OrderedFloors[floorIndex];
            if (prevFloor)
                FloorUnloaded?.Invoke(prevFloor);

            Debug.Log($"[TourRunner] Stopping tour. Unloading active scene: {loadedScenePath}");
            SceneManager.UnloadSceneAsync(loadedScenePath); // once
            loadedScenePath = null;
            activeFM = null;
        }

        if (
            arOverlayLoaded
            && arrowSceneDef != null
            && !string.IsNullOrEmpty(arrowSceneDef.ScenePath)
        )
        {
            var sc = SceneManager.GetSceneByPath(arrowSceneDef.ScenePath);
            if (sc.IsValid() && sc.isLoaded)
                SceneManager.UnloadSceneAsync(arrowSceneDef.ScenePath);
            arOverlayLoaded = false;
        }

        currentTour = null;
        floorIndex = -1;
        visitedAcrossTour = 0;
        totalAcrossTour = 0;

        isStopping = false;

        if (returnToHome)
            Debug.Log("[TourRunner] Tour stopped. Returning to home state.");
    }

    private void ApplyCameraPolicy(Scene floorScene)
    {
        // When AR is off, just ensure floor cameras are enabled.
        if (!useAR)
        {
            EnableCamerasInScene(floorScene, enable: true);
            EnsureSingleAudioListener();
            return;
        }

        // AR is on: disable floor cameras if configured
        if (disableFloorCamerasInAR)
            EnableCamerasInScene(floorScene, enable: false);

        // Ensure only one AudioListener remains globally
        EnsureSingleAudioListener();
    }

    private void EnableCamerasInScene(Scene scene, bool enable)
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var cam in root.GetComponentsInChildren<Camera>(true))
                cam.enabled = enable;
        }
    }

    private void EnsureSingleAudioListener()
    {
        // Collect listeners in base scene
        AudioListener baseSceneListener = null;
        foreach (var root in baseScene.GetRootGameObjects())
        {
            baseSceneListener = root.GetComponentInChildren<AudioListener>(true);
            if (baseSceneListener)
                break;
        }

        if (baseSceneListener == null)
        {
            Debug.LogWarning("[TourRunner] No AudioListener found in base scene.");
            return;
        }

        // Disable all listeners not in base scene
        var all = FindObjectsByType<AudioListener>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
        foreach (var al in all)
        {
            if (al == baseSceneListener)
                continue;
            if (al.enabled)
            {
                al.enabled = false;
                Debug.Log($"[TourRunner] Disabled extra AudioListener on {al.gameObject.name}");
            }
        }
    }
}
