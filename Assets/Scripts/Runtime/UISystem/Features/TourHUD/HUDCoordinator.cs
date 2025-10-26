public sealed class HUDCoordinator : ICoordinator<BaseHUDView>
{
    private readonly UIRouter router;
    private readonly UIAtlas uiAtlas;
    private readonly TourViewModel vm;
    private readonly TourBinder binder;

    private readonly AudioAtlas audioAtlas;

    private bool sawConnectionThisFloor;

    private BaseHUDCoordinator baseHud;
    private InfoWidgetCoordinator info;
    private ActionPopupCoordinator action;
    private NoticePopupCoordinator notice;
    private SettingsCoordinator settings;

    public HUDCoordinator(UIRouter r, TourViewModel model, TourBinder b, UIAtlas ua, AudioAtlas aa)
    {
        router = r;
        uiAtlas = ua;
        vm = model;
        binder = b;
        audioAtlas = aa;
        info = new InfoWidgetCoordinator(r, vm, b);
        action = new ActionPopupCoordinator(r, ua, b, model);
        notice = new NoticePopupCoordinator(r, ua);
        settings = new SettingsCoordinator(r, audioAtlas);
        baseHud = new BaseHUDCoordinator(r, vm, ua, aa);
    }

    private BaseHUDView v;

    public void Attach(BaseHUDView view)
    {
        v = view;
        baseHud.Attach(view);

        vm.Changed += ApplyPhase;
        vm.EnteredArea += OnEnteredArea;
        vm.GuidingTo += _ => { };
        vm.FloorBegan += _ => sawConnectionThisFloor = false;
        vm.ConnectionLost += OnConnLost;
        vm.ConnectionRestored += OnConnRestored;
        vm.TourCompletedEvent += OnTourCompleted;
        vm.GuidingTo += OnGuidingTo;
        ApplyPhase();
    }

    public void Detach()
    {
        vm.Changed -= ApplyPhase;
        vm.EnteredArea -= OnEnteredArea;
        vm.ConnectionLost -= OnConnLost;
        vm.ConnectionRestored -= OnConnRestored;
        vm.TourCompletedEvent -= OnTourCompleted;
        vm.GuidingTo -= OnGuidingTo;

        baseHud?.Detach();
        info?.Hide();
        action?.Hide();
        notice?.Hide();
        v = null;
    }

    private void ApplyPhase()
    {
        switch (vm.Phase)
        {
            case TourUIPhase.WaitingForConnection:
                AudioDirector.Instance.Stop(0.12f);
                action.Hide();
                info.Hide();
                notice.ShowConnecting();
                break;

            case TourUIPhase.ConnectionLost:
                AudioDirector.Instance.Stop(0.12f);
                action.Hide();
                info.Hide();
                notice.ShowLostConnection();
                break;

            case TourUIPhase.ReadyPrompt:
                AudioDirector.Instance.Stop(0.12f);
                notice.Hide();
                info.Hide();
                if (!vm.HasBegunTour)
                {
                    action.ShowStart();
                }
                else if (binder.WaitingForFloorContinue)
                {
                    var desc = vm.CurrentFloor ? vm.CurrentFloor.TransitionText : null;
                    action.ShowReadyOnFloor(desc);
                }
                else
                {
                    action.ShowReadyOnFloor();
                }
                break;

            case TourUIPhase.Navigating:
                notice.Hide();
                action.Hide();
                info.Hide();
                break;

            case TourUIPhase.InAreaInfo:
                notice.Hide();
                action.Hide();
                if (vm.CurrentAreaDef != null && vm.CurrentAreaDef.ShowInfo)
                    info.Show(vm.CurrentAreaDef);
                break;

            case TourUIPhase.FloorTransition:
                AudioDirector.Instance.Stop(0.12f);
                notice.Hide();
                action.ShowReadyOnFloor();
                info.Hide();
                break;

            case TourUIPhase.TourComplete:
                AudioDirector.Instance.Stop(0.12f);
                action.Hide();
                info.Hide();
                notice.ShowTourComplete();
                break;
        }
    }

    private void OnEnteredArea(AreaDefinition def)
    {
        if (def && def.ShowInfo)
        {
            vm.SetPhase(TourUIPhase.InAreaInfo);
            info.Show(def);
        }
        else
        {
            binder.RequestNext();
        }
    }

    private void OnConnLost()
    {
        if (sawConnectionThisFloor)
            notice.ShowLostConnection();
        else
            notice.ShowConnecting();
    }

    private void OnConnRestored()
    {
        sawConnectionThisFloor = true;

        if (vm.Phase == TourUIPhase.ConnectionLost)
        {
            if (vm.CurrentAreaDef != null && vm.CurrentAreaDef.ShowInfo)
                vm.SetPhase(TourUIPhase.InAreaInfo);
            else
                vm.SetPhase(TourUIPhase.Navigating);
        }

        notice.Hide();
    }

    private void OnGuidingTo(AreaDefinition _)
    {
        if (audioAtlas && audioAtlas.navigating)
            AudioDirector.Instance.Play(audioAtlas.navigating, 0f, 0.1f);
        else
            AudioDirector.Instance.Stop(0.1f);
    }

    public void OpenSettings() => settings.Show();

    private void OnTourCompleted() { }
}
