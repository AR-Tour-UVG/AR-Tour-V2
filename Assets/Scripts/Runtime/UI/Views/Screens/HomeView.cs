using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Represents the Home screen view in the UI.
/// </summary>
public sealed class HomeView : IScreenView
{
    // properties
    public VisualElement Root { get; }
    public event Action OnExpress;
    public event Action OnComplete;
    public event Action OnMinigames;

    // cached elements
    private VisualElement expressBtn;
    private VisualElement completeBtn;
    private VisualElement minigamesBtn;

    /// <summary>
    /// Constructor for the HomeView.
    /// </summary>
    /// <param name="root">The root VisualElement of the view.</param>
    public HomeView(VisualElement root)
    {
        Root = root;
    }

    /// <summary>
    /// Binds the UI elements and sets up event listeners.
    /// </summary>
    /// <param name="doc">The UIDocument containing the UI elements.</param>
    public void Bind(UIDocument doc)
    {
        // Query UI elements by their names in the UI hierarchy
        expressBtn = Root.Q<VisualElement>("ExpressBtn");
        completeBtn = Root.Q<VisualElement>("CompleteBtn");
        minigamesBtn = Root.Q<VisualElement>("MinigamesBtn");

        // Register click event callbacks
        expressBtn?.RegisterCallback<ClickEvent>(_ => OnExpress?.Invoke());
        completeBtn?.RegisterCallback<ClickEvent>(_ => OnComplete?.Invoke());
        minigamesBtn?.RegisterCallback<ClickEvent>(_ => OnMinigames?.Invoke());
    }

    /// <summary>
    /// Unbinds the UI elements and removes event listeners.
    /// </summary>
    public void Unbind()
    {
        expressBtn?.UnregisterCallback<ClickEvent>(_ => OnExpress?.Invoke());
        completeBtn?.UnregisterCallback<ClickEvent>(_ => OnComplete?.Invoke());
        minigamesBtn?.UnregisterCallback<ClickEvent>(_ => OnMinigames?.Invoke());
    }
}
