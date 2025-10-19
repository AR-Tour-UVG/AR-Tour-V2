using UnityEditor;
using UnityEngine;

public sealed class BaseHUDCoordinator : ICoordinator<BaseHUDView>
{
    private readonly UIRouter router;
    private readonly TourViewModel vm;
    private readonly MenuCoordinator menu;

    private BaseHUDView v;

    public BaseHUDCoordinator(UIRouter r, TourViewModel model)
    {
        router = r;
        vm = model;
        menu = new MenuCoordinator(r);
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

        // Title
        var title = vm.Phase switch
        {
            TourUIPhase.WaitingForConnection => "Conectando…",
            TourUIPhase.ReadyPrompt => "Listo para empezar",
            TourUIPhase.Navigating => "En ruta",
            TourUIPhase.InAreaInfo => vm.CurrentArea,
            TourUIPhase.FloorTransition => "Cambiando de nivel",
            TourUIPhase.TourComplete => "Tour completado",
            _ => "",
        };
        v.SetTitle(title);

        // Directions card
        var showDir = vm.Phase == TourUIPhase.Navigating && !string.IsNullOrEmpty(vm.NextArea);
        v.SetDirections(showDir ? $"Dirígete a: {vm.NextArea}" : "", showDir);

        // Footer when navigating only
        v.ShowFooter(vm.Phase == TourUIPhase.Navigating);

        // Metrics
        v.SetProgress(vm.ProgressNormalized);
        v.SetDistance(vm.DistanceMeters);
    }

    private void ShowMenu() => menu.Show();
}
