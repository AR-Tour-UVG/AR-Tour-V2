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

    private EventCallback<ClickEvent> onExpressCb;
    private EventCallback<ClickEvent> onCompleteCb;
    private EventCallback<ClickEvent> onMinigamesCb;
    private bool bound;

    public HomeView(VisualElement root)
    {
        Root = root;
    }

    public void Bind(UIDocument doc)
    {
        if (bound)
            return; // prevent double bind

        expressBtn = Root.Q<VisualElement>("ExpressBtn");
        completeBtn = Root.Q<VisualElement>("CompleteBtn");
        minigamesBtn = Root.Q<VisualElement>("MinigamesBtn");

        onExpressCb = _ => OnExpress?.Invoke();
        onCompleteCb = _ => OnComplete?.Invoke();
        onMinigamesCb = _ => OnMinigames?.Invoke();

        expressBtn?.RegisterCallback(onExpressCb);
        completeBtn?.RegisterCallback(onCompleteCb);
        minigamesBtn?.RegisterCallback(onMinigamesCb);

        bound = true;
    }

    public void Unbind()
    {
        if (!bound)
            return;

        expressBtn?.UnregisterCallback(onExpressCb);
        completeBtn?.UnregisterCallback(onCompleteCb);
        minigamesBtn?.UnregisterCallback(onMinigamesCb);

        onExpressCb = null;
        onCompleteCb = null;
        onMinigamesCb = null;
        expressBtn = completeBtn = minigamesBtn = null;
        bound = false;
    }
}
