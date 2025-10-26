using UnityEngine;
using UnityEngine.AI;

public sealed class TourBinder : MonoBehaviour
{
    private TourRunner tourRunner;

    private PathProvider pathProvider;

    private MovementAgent movementAgent;
    private UWBPositioning uwb;

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

        if (movementAgent == null)
            movementAgent = FindFirstObjectByType<MovementAgent>(FindObjectsInactive.Include);

        if (pathProvider == null)
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
                $"[TourBinder] Subscribed to PathProvider #{pathProvider.GetInstanceID()} on {pathProvider.gameObject.name}"
            );
        }
        else
        {
            Debug.LogWarning("[TourBinder] No PathProvider found after floor load.");
        }
    }

    private void UnbindPathProvider()
    {
        if (pathProvider)
        {
            pathProvider.OnPathUpdated -= OnPathUpdated;
            Debug.Log("[TourBinder] Unsubscribed from PathProvider");
            pathProvider = null;
        }
    }

    private void BindUWB()
    {
        if (uwb)
            return;

        if (movementAgent == null)
        {
            movementAgent = FindFirstObjectByType<MovementAgent>(FindObjectsInactive.Include);
            Debug.Log("[UWBPositioning] Found MovementAgent");
        }

        if (movementAgent)
        {
            uwb = movementAgent.GetComponent<UWBPositioning>();
            Debug.Log("[UWBPositioning] Found UWBPositioning component");
        }
        if (uwb == null)
        {
            uwb = FindFirstObjectByType<UWBPositioning>(FindObjectsInactive.Include);
            Debug.Log("[UWBPositioning] Found UWBPositioning in scene");
        }
        if (uwb)
        {
            uwb.OnConnectionStatusChanged += HandleUWBConnectionChanged;
            Debug.Log("[TourBinder] Subscribed to UWBPositioning");
        }
    }

    private void UnbindUWB()
    {
        if (uwb == null)
            return;
        uwb.OnConnectionStatusChanged -= HandleUWBConnectionChanged;
        uwb = null;
    }

    private void HandleUWBConnectionChanged(bool connected)
    {
        Debug.Log($"[TourBinder] UWB connection changed: {connected}");
        SetConnection(connected);
        if (connected && waitingForFloorStart)
        {
            Debug.Log("[TourBinder] UWB reconnected, prompting user to start tour.");
            vm.SetPhase(TourUIPhase.ReadyPrompt);
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
        BindUWB();
#if UNITY_IOS
        var u = movementAgent ? movementAgent.GetComponent<UWBPositioning>() : null;
        if (u)
        {
            u.enabled = true;
            Debug.Log("[UWBPositioning] Enabled UWBPositioning component.");
            u.SetApplyTransforms(false);
            u.StartTracking();
        }
        else
        {
            Debug.LogWarning("[TourBinder] No UWBPositioning component found on MovementAgent.");
        }
#endif

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
        UnbindUWB();
# if UNITY_IOS
        var u = movementAgent ? movementAgent.GetComponent<UWBPositioning>() : null;
        if (u)
        {
            u.StopTracking();
            u.enabled = false;
        }
#endif
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
        Debug.Log(
            $"[TourBinder] SetConnection({connected}), waitingForFloorStart={waitingForFloorStart}, waitingForFloorContinue={waitingForFloorContinue}, lastPhase={lastPhaseBeforeDisconnect}"
        );
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
#if UNITY_IOS
        var u = movementAgent ? movementAgent.GetComponent<UWBPositioning>() : null;
        if (u)
        {
            u.SetApplyTransforms(true);
        }
#endif
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
