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

    // cached
    VisualElement container,
        closeBtn,
        restart,
        help,
        settings,
        returnBtn;

    public MenuView(VisualElement root)
    {
        Root = root;
    }

    public void Bind(UIDocument doc)
    {
        container = Root.Q<VisualElement>("Menu") ?? Root; // menu container
        closeBtn = Root.Q<VisualElement>("CloseButton");
        restart = Root.Q<VisualElement>("Restart");
        help = Root.Q<VisualElement>("Help");
        settings = Root.Q<VisualElement>("Settings");
        returnBtn = Root.Q<VisualElement>("Return");

        // Disable raycast blocking for the menu modal
        UIPickingUtils.ConfigureTreePickingMode(Root, PickingMode.Ignore);
        // Re-enable raycast blocking for the container
        UIPickingUtils.ConfigureTreePickingMode(container, PickingMode.Position);

        closeBtn?.RegisterCallback<ClickEvent>(_ => OnClose?.Invoke());
        restart?.RegisterCallback<ClickEvent>(_ => OnRestart?.Invoke());
        help?.RegisterCallback<ClickEvent>(_ => OnHelp?.Invoke());
        settings?.RegisterCallback<ClickEvent>(_ => OnSettings?.Invoke());
        returnBtn?.RegisterCallback<ClickEvent>(_ => OnReturnHome?.Invoke());

        Hide(); // start hidden
    }

    public void Unbind()
    {
        closeBtn?.UnregisterCallback<ClickEvent>(_ => OnClose?.Invoke());
        restart?.UnregisterCallback<ClickEvent>(_ => OnRestart?.Invoke());
        help?.UnregisterCallback<ClickEvent>(_ => OnHelp?.Invoke());
        settings?.UnregisterCallback<ClickEvent>(_ => OnSettings?.Invoke());
        returnBtn?.UnregisterCallback<ClickEvent>(_ => OnReturnHome?.Invoke());
    }

    public void Show() => Root.style.display = DisplayStyle.Flex;

    public void Hide() => Root.style.display = DisplayStyle.None;
}
