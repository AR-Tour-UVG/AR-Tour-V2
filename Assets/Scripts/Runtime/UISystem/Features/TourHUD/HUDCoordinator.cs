// HUDCoordinator.cs
using UnityEngine;

public sealed class HUDCoordinator : ICoordinator<BaseHUDView>
{
    private readonly UIRouter router;
    private readonly UIAtlas atlas;
    private readonly TourViewModel vm;
    private readonly TourBinder binder;

    // children
    private BaseHUDCoordinator baseHud;
    private InfoWidgetCoordinator info;
    private ActionPopupCoordinator action;
    private NoticePopupCoordinator notice;

    public HUDCoordinator(UIRouter r, UIAtlas a, TourViewModel model, TourBinder b)
    {
        router = r;
        atlas = a;
        vm = model;
        binder = b;
        info = new InfoWidgetCoordinator(r, vm, b);
        action = new ActionPopupCoordinator(r, a, b, model);
        notice = new NoticePopupCoordinator(r, a);
    }

    private BaseHUDView v;

    public void Attach(BaseHUDView view)
    {
        v = view;
        baseHud = new BaseHUDCoordinator(router, vm);
        baseHud.Attach(view);

        // react to VM signals
        vm.Changed += ApplyPhase;
        vm.EnteredArea += OnEnteredArea;
        vm.GuidingTo += _ =>
        { /* header/directions handled by BaseHUDCoordinator */
        };
        vm.ConnectionLost += OnConnLost;
        vm.ConnectionRestored += OnConnRestored;
        vm.TourCompletedEvent += OnTourCompleted;

        ApplyPhase(); // initial
    }

    public void Detach()
    {
        vm.Changed -= ApplyPhase;
        vm.EnteredArea -= OnEnteredArea;
        vm.ConnectionLost -= OnConnLost;
        vm.ConnectionRestored -= OnConnRestored;
        vm.TourCompletedEvent -= OnTourCompleted;

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
                notice.ShowConnecting();
                action.Hide();
                info.Hide();
                break;

            case TourUIPhase.ReadyPrompt:
                notice.Hide();
                if (!vm.HasBegunTour)
                    action.ShowStart();
                else
                    action.ShowReadyOnFloor();
                info.Hide();
                break;

            case TourUIPhase.Navigating:
                notice.Hide();
                action.Hide();
                info.Hide();
                break;

            case TourUIPhase.InAreaInfo:
                notice.Hide();
                action.Hide();
                // info shown by OnEnteredArea
                break;

            case TourUIPhase.FloorTransition:
                notice.Hide();
                action.ShowReadyOnFloor();
                info.Hide();
                break;

            case TourUIPhase.TourComplete:
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
            // immediately continue to navigating
            binder.RequestNext();
        }
    }

    private void OnConnLost()
    {
        if (!vm.Paused)
            notice.ShowLostConnection();
    }

    private void OnConnRestored()
    {
        notice.Hide();
    }

    private void OnTourCompleted() { /* router back to home is handled elsewhere if desired */
    }
}
