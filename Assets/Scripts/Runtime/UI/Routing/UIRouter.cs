using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class UIRouter
{
    // Layers
    private readonly VisualElement baseLayer;
    private readonly VisualElement modalLayer;
    private readonly VisualElement popupLayer;
    private readonly VisualElement menuLayer;

    // Optional scrim for modals/popups
    private readonly VisualElement scrim;

    // Factory to create views
    private readonly IViewFactory factory;

    // Current states
    public ScreenState CurrentScreen { get; private set; }
    public IScreenView CurrentScreenView { get; private set; }

    // Notify when the screen changes to coordinators
    public event System.Action<IScreenView> ScreenChanged;

    /// <summary>
    /// Constructor for UIRouter.
    /// </summary>
    public UIRouter(
        VisualElement baseLayer,
        VisualElement modalLayer,
        VisualElement popupLayer,
        VisualElement menuLayer,
        VisualElement scrim,
        IViewFactory factory
    )
    {
        this.baseLayer = baseLayer;
        this.modalLayer = modalLayer;
        this.popupLayer = popupLayer;
        this.menuLayer = menuLayer;
        this.scrim = scrim;
        this.factory = factory;
    }

    /// <summary>
    /// Show a screen in the base layer.
    /// </summary>
    public void ShowScreen(ScreenState s)
    {
        // Unmount and Unbind previous
        CurrentScreenView?.Unbind();

        // Clear base layer
        baseLayer.Clear();

        // Create view
        var view = factory.CreateScreen(s);
        if (view == null || view.Root == null)
        {
            Debug.LogError($"[UIRouter] Failed to create screen {s}");
            return;
        }

        // Mount and bind new
        baseLayer.Add(view.Root);
        view.Bind(GetDoc(baseLayer));
        // Update state
        CurrentScreen = s;
        CurrentScreenView = view;
        // Notify coordinators
        ScreenChanged?.Invoke(view);
    }

    /// <summary>
    /// Show the UIDocument owning the given VisualElement.
    /// </summary>
    private static UIDocument GetDoc(VisualElement any)
    {
        // In practice we pass the UIDocument into the factory
        // this is a fallback
        return Object.FindFirstObjectByType<UIDocument>();
    }
}
