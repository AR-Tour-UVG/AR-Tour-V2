using System;
using UnityEngine.UIElements;

public sealed class OnboardingView : IScreenView
{
    public VisualElement Root { get; }
    public event Action OnNext;

    VisualElement art,
        btn;

    Label title,
        desc,
        btnText;

    public OnboardingView(VisualElement root)
    {
        Root = root;
    }

    public void Bind(UIDocument doc)
    {
        art = Root.Q<VisualElement>("Art");
        title = Root.Q<Label>("Title");
        desc = Root.Q<Label>("Description");
        btn = Root.Q<VisualElement>("Button");
        btnText = Root.Q<Label>("Text");

        btn?.RegisterCallback<ClickEvent>(_ => OnNext?.Invoke());
    }

    public void Unbind()
    {
        btn?.UnregisterCallback<ClickEvent>(_ => OnNext?.Invoke());
    }

    public void SetSlide(OnboardingSlide s, bool isLast)
    {
        title.text = s != null ? s.Title : null ?? "";
        desc.text = s != null ? s.Description : null ?? "";
        if (btnText != null)
            btnText.text = !string.IsNullOrEmpty(s?.ButtonText ?? s?.ButtonText)
                ? (s.ButtonText ?? s.ButtonText)
                : (isLast ? "Empezar" : "Siguiente");
        if (s != null ? s.Art : null)
            art.style.backgroundImage = new StyleBackground(s.Art);
        else
            art.style.backgroundImage = StyleKeyword.Null;
    }
}
