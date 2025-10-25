using UnityEngine;

public sealed class OnboardingCoordinator : ICoordinator<OnboardingView>
{
    private readonly UIRouter router;
    private readonly OnboardingSet set;
    private int index = 0;
    private OnboardingView v;

    public OnboardingCoordinator(UIRouter r, OnboardingSet s)
    {
        router = r;
        set = s;
    }

    public void Attach(OnboardingView view)
    {
        v = view;
        index = 0;
        Apply();
        v.OnNext += Next;
    }

    public void Detach()
    {
        if (v != null)
            v.OnNext -= Next;
        v = null;
    }

    void Apply()
    {
        if (set == null || set.Slides.Length == 0)
        {
            Finish();
            return;
        }
        index = Mathf.Clamp(index, 0, set.Slides.Length - 1);
        bool last = index == set.Slides.Length - 1;
        var slide = set.Slides[index];
        v.SetSlide(slide, last);
    }

    void Next()
    {
        index++;
        if (set == null || index >= set.Slides.Length)
        {
            Finish();
            return;
        }
        Apply();
    }

    void Finish()
    {
        OnboardingGate.MarkSeen();
        router.ShowScreen(ScreenState.Home);
    }
}
