using System;
using UnityEngine.UIElements;

public sealed class MinigamesView : IScreenView
{
    public VisualElement Root { get; }
    public event Action OnBreakout,
        OnTrivia,
        OnFlappy,
        OnExit;

    VisualElement breakout,
        trivia,
        flappy,
        exit;

    public MinigamesView(VisualElement root)
    {
        Root = root;
    }

    public void Bind(UIDocument doc)
    {
        breakout = Root.Q<VisualElement>("Breakout");
        trivia = Root.Q<VisualElement>("TriviaUVG");
        flappy = Root.Q<VisualElement>("FlappyJack");
        exit = Root.Q<VisualElement>("Exit");

        breakout?.RegisterCallback<ClickEvent>(_ => OnBreakout?.Invoke());
        trivia?.RegisterCallback<ClickEvent>(_ => OnTrivia?.Invoke());
        flappy?.RegisterCallback<ClickEvent>(_ => OnFlappy?.Invoke());
        exit?.RegisterCallback<ClickEvent>(_ => OnExit?.Invoke());
    }

    public void Unbind()
    {
        breakout?.UnregisterCallback<ClickEvent>(_ => OnBreakout?.Invoke());
        trivia?.UnregisterCallback<ClickEvent>(_ => OnTrivia?.Invoke());
        flappy?.UnregisterCallback<ClickEvent>(_ => OnFlappy?.Invoke());
        exit?.UnregisterCallback<ClickEvent>(_ => OnExit?.Invoke());
    }
}
