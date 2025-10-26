using System;
using UnityEngine.UIElements;

public sealed class HomeView : IScreenView
{
    public VisualElement Root { get; }
    public event Action OnExpress;
    public event Action OnComplete;
    public event Action OnMinigames;

    private VisualElement expressBtn;
    private VisualElement completeBtn;
    private VisualElement minigamesBtn;

    public HomeView(VisualElement root)
    {
        Root = root;
    }

    public void Bind(UIDocument doc)
    {
        expressBtn = Root.Q<VisualElement>("ExpressBtn");
        completeBtn = Root.Q<VisualElement>("CompleteBtn");
        minigamesBtn = Root.Q<VisualElement>("MinigamesBtn");

        expressBtn?.RegisterCallback<ClickEvent>(_ => OnExpress?.Invoke());
        completeBtn?.RegisterCallback<ClickEvent>(_ => OnComplete?.Invoke());
        minigamesBtn?.RegisterCallback<ClickEvent>(_ => OnMinigames?.Invoke());
    }

    public void Unbind()
    {
        expressBtn?.UnregisterCallback<ClickEvent>(_ => OnExpress?.Invoke());
        completeBtn?.UnregisterCallback<ClickEvent>(_ => OnComplete?.Invoke());
        minigamesBtn?.UnregisterCallback<ClickEvent>(_ => OnMinigames?.Invoke());
    }
}
