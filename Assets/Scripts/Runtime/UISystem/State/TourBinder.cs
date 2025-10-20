using UnityEngine;

public sealed class TourBinder : MonoBehaviour
{
    private TourRunner tourRunner;

    private PathProvider pathProvider;

    private MovementAgent movementAgent;

    private TourViewModel vm;
    private FloorManager fm;

    // Connection Flags
    private bool waitingForFloorStart; // after a floor loads and before UserReady
    private bool waitingForFloorContinue; // after a floor unload and before user continues
    private TourUIPhase lastPhaseBeforeDisconnect = TourUIPhase.Navigating;

    public bool WaitingForFloorStart => waitingForFloorStart;
    public bool WaitingForFloorContinue => waitingForFloorContinue;

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
        waitingForFloorStart = true; // gate set
        waitingForFloorContinue = false;
        vm.SetPhase(TourUIPhase.WaitingForConnection);
        vm.SetProgress(tourRunner.Progress);
        vm.SetDistance(0f);
    }

    private void OnFloorUnloaded(FloorDefinition floor)
    {
        vm.NotifyFloorEnded(floor);
        waitingForFloorStart = false;
        waitingForFloorContinue = true; // gate set during elevator time
        vm.SetPhase(TourUIPhase.FloorTransition);
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

        if (!connected)
        {
            lastPhaseBeforeDisconnect = vm.Phase;
            if (waitingForFloorStart || waitingForFloorContinue)
                vm.SetPhase(TourUIPhase.WaitingForConnection); // expected during elevator
            else
                vm.SetPhase(TourUIPhase.ConnectionLost); // mid-tour loss
            return;
        }

        // connected == true
        if (waitingForFloorContinue)
        {
            // Still in elevator gate → show Ready once.
            vm.SetPhase(TourUIPhase.ReadyPrompt);
            return;
        }

        if (waitingForFloorStart)
        {
            if (!vm.HasBegunTour)
            {
                // First floor only: ask to Start.
                vm.SetPhase(TourUIPhase.ReadyPrompt);
            }
            else
            {
                // Subsequent floors: auto start the floor on connect.
                waitingForFloorStart = false;
                vm.SetPhase(TourUIPhase.Navigating);
                RequestUserReady(); // enable movement + confirm first area
            }
            return;
        }

        // Mid-tour reconnection: resume previous phase.
        vm.SetPhase(lastPhaseBeforeDisconnect);
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

    public void RequestUserReady()
    {
        if (fm == null)
        {
            Debug.LogWarning("[TourBinder] RequestUserReady called with no FloorManager.");
            return;
        }
        waitingForFloorStart = false; // gate clear
        fm.UserReady(); // enables movement and confirms first area per your FM
        vm.SetPaused(false); // reflect movement state in the VM
        // Phase will advance via FM events (GuidingToNext/AreaConfirmed) and SetConnection()
    }

    public void RequestNext()
    {
        if (fm == null)
        {
            Debug.LogWarning("[TourBinder] RequestNext called with no FloorManager.");
            return;
        }
        fm.Next(); // FM will emit GuidingToNext → VM.SetPhase(Navigating) in OnGuidingToNext
    }
}
