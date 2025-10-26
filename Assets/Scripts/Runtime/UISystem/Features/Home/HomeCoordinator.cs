using UnityEngine;

public sealed class HomeCoordinator : ICoordinator<HomeView>
{
    private HomeView v;
    private readonly UIRouter router;

    public HomeCoordinator(UIRouter router)
    {
        this.router = router;
    }

    public void Attach(HomeView view)
    {
        v = view;
        v.OnExpress += OnExpress;
        v.OnComplete += OnComplete;
        v.OnMinigames += OnMinigames;
    }

    public void Detach()
    {
        if (v == null)
            return;
        v.OnExpress -= OnExpress;
        v.OnComplete -= OnComplete;
        v.OnMinigames -= OnMinigames;
        v = null;
    }

    void OnExpress()
    {
        Debug.Log("[HomeCoordinator] Express Tour Selected");
        var tourRunner = TourRunner.Instance;
        tourRunner.SelectTour(tourRunner.ExpressTour);
        tourRunner.BeginTour();
        router.ShowScreen(ScreenState.TourHUD);
    }

    void OnComplete()
    {
        Debug.Log("Complete Tour Selected");
        var tourRunner = TourRunner.Instance;
        tourRunner.SelectTour(tourRunner.CompleteTour);
        tourRunner.BeginTour();
        router.ShowScreen(ScreenState.TourHUD);
    }

    void OnMinigames()
    {
        Debug.Log("Minigames Selected");
        router.ShowScreen(ScreenState.Minigames);
    }
}
