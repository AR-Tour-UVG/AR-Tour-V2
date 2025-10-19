using UnityEngine;

public sealed class TourBinder : MonoBehaviour
{
    private TourRunner tourRunner;

    private PathProvider pathProvider;

    private MovementAgent movementAgent;

    private TourViewModel vm;
    private FloorManager fm;

    public void Init(TourViewModel model)
    {
        vm = model;

        if (!tourRunner)
            tourRunner = FindFirstObjectByType<TourRunner>(FindObjectsInactive.Include);
        if (!pathProvider)
            pathProvider = FindFirstObjectByType<PathProvider>(FindObjectsInactive.Include);
        if (!movementAgent)
            movementAgent = FindFirstObjectByType<MovementAgent>(FindObjectsInactive.Include);

        if (pathProvider)
            pathProvider.OnPathUpdated += _ => vm.SetDistance(pathProvider.CurrentDistance);

        if (tourRunner != null)
        {
            tourRunner.FloorLoaded += OnFloorLoaded;
            tourRunner.FloorUnloaded += OnFloorUnloaded;
            tourRunner.TourCompleted += vm.NotifyTourCompleted;
            vm.SetProgress(tourRunner.Progress);
        }

        // Initial FM hookup if already present (editor play-in-scene)
        HookToActiveFloorManager();

        // Paused flag reflects movement agent; if you later expose an event, set from there.
        if (movementAgent)
            vm.SetPaused(!movementAgent.IsEnabled);
    }

    void OnDestroy()
    {
        if (tourRunner != null)
        {
            tourRunner.FloorLoaded -= OnFloorLoaded;
            tourRunner.FloorUnloaded -= OnFloorUnloaded;
            tourRunner.TourCompleted -= vm.NotifyTourCompleted;
        }
        UnhookFM();
        if (pathProvider)
            pathProvider.OnPathUpdated -= _ => vm.SetDistance(pathProvider.CurrentDistance);
    }

    private void OnFloorLoaded(FloorDefinition floor, FloorManager floorMgr)
    {
        SwapFM(floorMgr);
        vm.SetCurrentFloor(floor);
        vm.SetPhase(TourUIPhase.WaitingForConnection);
        vm.SetProgress(tourRunner.Progress);
        vm.SetDistance(0f);
    }

    private void OnFloorUnloaded(FloorDefinition floor)
    {
        vm.NotifyFloorEnded(floor);
    }

    private void SwapFM(FloorManager floorMgr)
    {
        UnhookFM();
        fm = floorMgr;
        if (fm == null)
            return;
        fm.AreaConfirmed += OnAreaConfirmed;
        fm.GuidingToNext += OnGuidingToNext;
    }

    private void UnhookFM()
    {
        if (fm == null)
            return;
        fm.AreaConfirmed -= OnAreaConfirmed;
        fm.GuidingToNext -= OnGuidingToNext;
        fm = null;
    }

    private void OnAreaConfirmed(AreaDefinition def)
    {
        if (tourRunner != null)
        {
            vm.NotifyEnteredArea(def, tourRunner.VisitedAcrossTour, tourRunner.TotalAcrossTour);
        }
        else
        {
            vm.NotifyEnteredArea(def, 0, 0);
        }
    }

    private void OnGuidingToNext(AreaDefinition def)
    {
        vm.NotifyGuidingTo(def);
        vm.SetPhase(TourUIPhase.Navigating);
    }

    // Call from your UWB positioning bridge
    public void SetConnection(bool connected)
    {
        vm.SetConnected(connected);
        if (connected && !vm.HasBegunTour && vm.Phase == TourUIPhase.WaitingForConnection)
            vm.SetPhase(TourUIPhase.ReadyPrompt);
        else if (!connected && !vm.Paused)
            vm.SetPhase(TourUIPhase.WaitingForConnection);
    }

#if UNITY_EDITOR
    void Update()
    {
        // If a floor scene loads later in editor, rehook
        if (fm == null)
            HookToActiveFloorManager();
    }
#endif

    private void HookToActiveFloorManager()
    {
        var found = FindFirstObjectByType<FloorManager>(FindObjectsInactive.Include);
        if (found != null)
            OnFloorLoaded(found.Floor, found);
    }

    // Optional getters for coordinators
    public FloorManager ActiveFloorManager => fm;
    public MovementAgent Movement => movementAgent;
}
