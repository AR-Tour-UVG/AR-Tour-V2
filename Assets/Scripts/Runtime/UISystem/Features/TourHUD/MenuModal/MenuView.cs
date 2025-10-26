using System;
using UnityEngine.UIElements;

public sealed class MenuView : IOverlayView
{
    public VisualElement Root { get; }

    public event Action OnClose;
    public event Action OnRestart;
    public event Action OnHelp;
    public event Action OnSettings;
    public event Action OnReturnHome;

    public event Action Hidden;
    VisualElement container,
        closeBtn,
        restart,
        help,
        settings,
        returnBtn;

    bool isOpen;
    EventCallback<ClickEvent> cbClose,
        cbRestart,
        cbHelp,
        cbSettings,
        cbReturn;

    public MenuView(VisualElement root)
    {
        Root = root;
    }

    public void Bind(UIDocument doc)
    {
        container = Root.Q<VisualElement>("Menu") ?? Root;
        closeBtn = Root.Q<VisualElement>("CloseButton");
        restart = Root.Q<VisualElement>("Restart");
        help = Root.Q<VisualElement>("Help");
        settings = Root.Q<VisualElement>("Settings");
        returnBtn = Root.Q<VisualElement>("Return");

        UIPickingUtils.ConfigureTreePickingMode(Root, PickingMode.Ignore);
        UIPickingUtils.ConfigureTreePickingMode(container, PickingMode.Position);

        cbClose = _ => OnClose?.Invoke();
        cbRestart = _ => OnRestart?.Invoke();
        cbHelp = _ => OnHelp?.Invoke();
        cbSettings = _ => OnSettings?.Invoke();
        cbReturn = _ => OnReturnHome?.Invoke();

        closeBtn?.RegisterCallback(cbClose);
        restart?.RegisterCallback(cbRestart);
        help?.RegisterCallback(cbHelp);
        settings?.RegisterCallback(cbSettings);
        returnBtn?.RegisterCallback(cbReturn);

        container.RegisterCallback<TransitionEndEvent>(OnTransitionEnd);

        container.RemoveFromClassList("is-open");
        Root.style.display = DisplayStyle.None;
        isOpen = false;
    }

    public void Unbind()
    {
        closeBtn?.UnregisterCallback(cbClose);
        restart?.UnregisterCallback(cbRestart);
        help?.UnregisterCallback(cbHelp);
        settings?.UnregisterCallback(cbSettings);
        returnBtn?.UnregisterCallback(cbReturn);
        container?.UnregisterCallback<TransitionEndEvent>(OnTransitionEnd);
    }

    public void Show()
    {
        if (isOpen)
            return;

        Root.style.display = DisplayStyle.Flex;
        container.RemoveFromClassList("is-open");

        void AfterLayout(GeometryChangedEvent _)
        {
            container.UnregisterCallback<GeometryChangedEvent>(AfterLayout);
            container
                .schedule.Execute(() =>
                {
                    container.AddToClassList("is-open");
                    isOpen = true;
                })
                .StartingIn(0);
        }
        container.RegisterCallback<GeometryChangedEvent>(AfterLayout);
    }

    public void Hide()
    {
        if (!isOpen && Root.style.display == DisplayStyle.None)
        {
            Hidden?.Invoke();
            return;
        }
        container.RemoveFromClassList("is-open"); // slide-out to right
        isOpen = false;
    }

    void OnTransitionEnd(TransitionEndEvent e)
    {
        if (e.target != container)
            return;

        bool relevant = false;
        foreach (var n in e.stylePropertyNames)
        {
            var prop = n.ToString();
            if (prop == "translate" || prop == "opacity")
            {
                relevant = true;
                break;
            }
        }
        if (!relevant)
            return;

        if (!isOpen)
        {
            Root.style.display = DisplayStyle.None;
            Hidden?.Invoke();
        }
    }
}
