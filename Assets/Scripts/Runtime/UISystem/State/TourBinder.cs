using UnityEngine;
using UnityEngine.AI;

public sealed class TourBinder : MonoBehaviour
{
    private TourRunner tourRunner;

    private PathProvider pathProvider;

    private MovementAgent movementAgent;

    private TourViewModel vm;
    private FloorManager fm;

    private bool waitingForFloorStart;
    private bool waitingForFloorContinue;
    private TourUIPhase lastPhaseBeforeDisconnect = TourUIPhase.Navigating;

    public bool WaitingForFloorStart => waitingForFloorStart;
    public bool WaitingForFloorContinue => waitingForFloorContinue;

    public void Init(TourViewModel model)
    {
        vm = model;

        if (!tourRunner)
            tourRunner = FindFirstObjectByType<TourRunner>(FindObjectsInactive.Include);

        if (tourRunner != null)
        {
            tourRunner.FloorLoaded += OnFloorLoaded;
            tourRunner.FloorUnloaded += OnFloorUnloaded;
            tourRunner.TourCompleted += vm.NotifyTourCompleted;
            vm.SetProgress(tourRunner.Progress);
        }

        HookToActiveFloorManager();
    }

    private void BindPathProvider()
    {
        if (pathProvider)
            return;

        if (!movementAgent)
            movementAgent = FindFirstObjectByType<MovementAgent>(FindObjectsInactive.Include);
        if (movementAgent)
            pathProvider = movementAgent.GetComponent<PathProvider>();

        if (!pathProvider)
        {
            var player = GameObject.FindWithTag("Player");
            if (player)
                pathProvider = player.GetComponent<PathProvider>();
        }

        if (pathProvider)
        {
            pathProvider.OnPathUpdated += OnPathUpdated;
            Debug.Log(
                $"[Binder] Subscribed to PathProvider #{pathProvider.GetInstanceID()} on {pathProvider.gameObject.name}"
            );
        }
        else
        {
            Debug.LogWarning("[Binder] No PathProvider found after floor load.");
        }
    }

    private void UnbindPathProvider()
    {
        if (pathProvider)
        {
            pathProvider.OnPathUpdated -= OnPathUpdated;
            Debug.Log("[Binder] Unsubscribed from PathProvider");
            pathProvider = null;
        }
    }

    private void OnPathUpdated(NavMeshPath path)
    {
        if (vm != null && pathProvider != null)
            vm.SetDistance(pathProvider.CurrentDistance);
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
            pathProvider.OnPathUpdated -= OnPathUpdated;
    }

    private void OnFloorLoaded(FloorDefinition floor, FloorManager floorMgr)
    {
        SwapFM(floorMgr);

        movementAgent = FindFirstObjectByType<MovementAgent>(FindObjectsInactive.Include);
        vm.SetPaused(movementAgent ? !movementAgent.IsEnabled : true);

        BindPathProvider();

        vm.SetCurrentFloor(floor);
        waitingForFloorStart = true;
        waitingForFloorContinue = false;
        vm.SetPhase(TourUIPhase.WaitingForConnection);
        vm.SetProgress(tourRunner.Progress);
        vm.SetDistance(0f);
    }

    private void OnFloorUnloaded(FloorDefinition floor)
    {
        UnbindPathProvider();
        movementAgent = null;
        vm.SetPaused(true);
        waitingForFloorStart = false;
        waitingForFloorContinue = true;
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
            if (def != null && def.AudioClips != null && def.AudioClips.Count > 0)
                AudioDirector.Instance.PlaySequence(def.AudioClips, 0.1f);
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

    public void SetConnection(bool connected)
    {
        vm.SetConnected(connected);

        if (!connected)
        {
            lastPhaseBeforeDisconnect = vm.Phase;
            if (waitingForFloorStart || waitingForFloorContinue)
                vm.SetPhase(TourUIPhase.WaitingForConnection);
            else
                vm.SetPhase(TourUIPhase.ConnectionLost);
            return;
        }

        if (waitingForFloorContinue)
        {
            vm.SetPhase(TourUIPhase.ReadyPrompt);
            return;
        }

        if (waitingForFloorStart)
        {
            if (!vm.HasBegunTour)
            {
                vm.SetPhase(TourUIPhase.ReadyPrompt);
            }
            else
            {
                waitingForFloorStart = false;
                vm.SetPhase(TourUIPhase.Navigating);
                RequestUserReady();
            }
            return;
        }

        vm.SetPhase(lastPhaseBeforeDisconnect);
    }

#if UNITY_EDITOR
    void Update()
    {
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

    public FloorManager ActiveFloorManager => fm;
    public MovementAgent Movement => movementAgent;

    public void RequestUserReady()
    {
        if (fm == null)
        {
            Debug.LogWarning("[TourBinder] RequestUserReady called with no FloorManager.");
            return;
        }
        waitingForFloorStart = false;
        fm.UserReady();
        vm.SetPaused(false);
    }

    public void RequestNext()
    {
        if (fm == null)
        {
            Debug.LogWarning("[TourBinder] RequestNext called with no FloorManager.");
            return;
        }
        fm.Next();
    }
}
