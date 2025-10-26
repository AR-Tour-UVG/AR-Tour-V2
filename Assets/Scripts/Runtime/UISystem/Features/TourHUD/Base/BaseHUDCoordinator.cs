using UnityEngine;

public sealed class BaseHUDCoordinator : ICoordinator<BaseHUDView>
{
    private readonly UIRouter router;
    private readonly TourViewModel vm;
    private readonly MenuCoordinator menu;
    private readonly Sprite spinnerIcon;
    private readonly Sprite elevatorIcon;
    private readonly Sprite walkingIcon;
    private readonly Sprite doneIcon;
    private readonly Sprite startIcon;

    private BaseHUDView v;

    public BaseHUDCoordinator(
        UIRouter r,
        TourViewModel model,
        UIAtlas uiAtlas,
        AudioAtlas audioAtlas
    )
    {
        router = r;
        vm = model;
        spinnerIcon = uiAtlas.HudSpinnerIcon;
        elevatorIcon = uiAtlas.HudElevatorIcon;
        walkingIcon = uiAtlas.HudWalkingIcon;
        doneIcon = uiAtlas.HudDoneIcon;
        startIcon = uiAtlas.HudStartIcon;
        menu = new MenuCoordinator(r, uiAtlas, audioAtlas);
    }

    public void Attach(BaseHUDView view)
    {
        v = view;
        v.OnMenu += ShowMenu;
        vm.Changed += Apply;
        Apply();
    }

    public void Detach()
    {
        if (v != null)
            v.OnMenu -= ShowMenu;
        vm.Changed -= Apply;
        v = null;
    }

    private void Apply()
    {
        if (v == null)
            return;

        var title = vm.Phase switch
        {
            TourUIPhase.WaitingForConnection => "Conectando…",
            TourUIPhase.ConnectionLost => "Reconectando…",
            TourUIPhase.ReadyPrompt => "Listo para empezar",
            TourUIPhase.Navigating => "En ruta",
            TourUIPhase.InAreaInfo => vm.CurrentArea,
            TourUIPhase.FloorTransition => "Cambiando de nivel",
            TourUIPhase.TourComplete => "Tour completado",
            _ => "",
        };
        v.SetTitle(title);
        Sprite icon = null;
        switch (vm.Phase)
        {
            case TourUIPhase.WaitingForConnection:
            case TourUIPhase.ConnectionLost:
                icon = spinnerIcon;
                break;
            case TourUIPhase.FloorTransition:
                icon = elevatorIcon;
                break;
            case TourUIPhase.Navigating:
                icon = walkingIcon;
                break;
            case TourUIPhase.TourComplete:
                icon = doneIcon;
                break;
            case TourUIPhase.InAreaInfo:
                icon = vm.CurrentAreaDef ? vm.CurrentAreaDef.AreaIcon : null; // per-area if set
                break;
            case TourUIPhase.ReadyPrompt:
                if (!vm.HasBegunTour)
                    icon = startIcon;
                break;
        }

        v.SetTitleIcon(icon);

        var showDir = vm.Phase == TourUIPhase.Navigating && !string.IsNullOrEmpty(vm.NextArea);
        v.SetDirections(showDir ? $"Dirígete a: {vm.NextArea}" : "", showDir);

        v.ShowFooter(vm.Phase == TourUIPhase.Navigating);

        v.SetProgress(vm.ProgressNormalized);
        v.SetDistance(vm.DistanceMeters);
    }

    private void ShowMenu() => menu.Show();
}
