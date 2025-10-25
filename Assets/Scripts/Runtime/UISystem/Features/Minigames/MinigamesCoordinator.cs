using UnityEngine;

public sealed class MinigamesCoordinator : ICoordinator<MinigamesView>
{
    private MinigamesView v;
    private readonly UIRouter router;

    public MinigamesCoordinator(UIRouter router)
    {
        this.router = router;
    }

    public void Attach(MinigamesView view)
    {
        v = view;
        v.OnBreakout += OnBreakout;
        v.OnTrivia += OnTrivia;
        v.OnFlappy += OnFlappy;
        v.OnExit += OnExit;
    }

    public void Detach()
    {
        v = null;
    }

    void OnBreakout()
    {
        Debug.Log("Breakout Selected");
    }

    void OnTrivia()
    {
        Debug.Log("Trivia Selected");
    }

    void OnFlappy()
    {
        Debug.Log("Flappy Selected");
    }

    void OnExit()
    {
        Debug.Log("Exit Minigames");
        router.ShowScreen(ScreenState.Home);
    }
}
