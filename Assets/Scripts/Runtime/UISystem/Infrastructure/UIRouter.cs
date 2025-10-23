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
    private readonly VisualElement settingsLayer;

    // Factory to create views
    private readonly IViewFactory factory;

    // Current states
    public ScreenState CurrentScreen { get; private set; }
    public IScreenView CurrentScreenView { get; private set; }

    // Notify when the screen changes to coordinators
    public event System.Action<IScreenView> ScreenChanged;

    // Track active overlays
    private readonly Dictionary<OverlayType, IOverlayView> overlays = new();

    // per-layer counts to toggle layer visibility
    private int modalCount,
        popupCount,
        menuCount,
        settingsCount;

    public UIRouter(
        VisualElement baseLayer,
        VisualElement modalLayer,
        VisualElement popupLayer,
        VisualElement menuLayer,
        VisualElement settingsLayer,
        IViewFactory factory
    )
    {
        this.baseLayer = baseLayer;
        this.modalLayer = modalLayer;
        this.popupLayer = popupLayer;
        this.menuLayer = menuLayer;
        this.settingsLayer = settingsLayer;
        this.factory = factory;
    }

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

    // -------- Overlays --------
    public IOverlayView ShowOverlay(OverlayType t)
    {
        // already visible → return
        if (overlays.TryGetValue(t, out var existing))
            return existing;

        var v = factory.CreateOverlay(t);
        if (v == null || v.Root == null)
        {
            Debug.LogError($"[UIRouter] Failed to create overlay {t}");
            return null;
        }

        var layer = ResolveLayer(t);
        if (layer == null)
        {
            Debug.LogError($"[UIRouter] No layer for overlay {t}");
            return null;
        }

        // ensure layer visible
        layer.style.display = DisplayStyle.Flex;
        IncrementLayerCount(t);

        layer.Add(v.Root);
        v.Bind(GetDoc(layer));
        overlays[t] = v;
        return v;
    }

    public void HideOverlay(OverlayType t)
    {
        if (!overlays.TryGetValue(t, out var v))
            return;
        v.Unbind();
        v.Root.RemoveFromHierarchy();
        overlays.Remove(t);
        DecrementLayerCount(t);
    }

    public T GetOverlay<T>(OverlayType t)
        where T : class, IOverlayView
    {
        return overlays.TryGetValue(t, out var v) ? v as T : null;
    }

    private VisualElement ResolveLayer(OverlayType t) =>
        t switch
        {
            OverlayType.InfoModal => modalLayer,
            OverlayType.NoticePopup => popupLayer,
            OverlayType.ActionPopup => popupLayer,
            OverlayType.Menu => menuLayer,
            OverlayType.Settings => settingsLayer,
            _ => null,
        };

    private void IncrementLayerCount(OverlayType t)
    {
        switch (t)
        {
            case OverlayType.InfoModal:
                modalCount++;
                break;
            case OverlayType.Settings:
                settingsCount++;
                break;
            case OverlayType.NoticePopup:
            case OverlayType.ActionPopup:
                popupCount++;
                break;
            case OverlayType.Menu:
                menuCount++;
                break;
        }
    }

    private void DecrementLayerCount(OverlayType t)
    {
        switch (t)
        {
            case OverlayType.InfoModal:
                modalCount = Mathf.Max(0, modalCount - 1);
                if (modalCount == 0)
                    modalLayer.style.display = DisplayStyle.None;
                break;
            case OverlayType.Settings:
                settingsCount = Mathf.Max(0, settingsCount - 1);
                if (settingsCount == 0)
                    settingsLayer.style.display = DisplayStyle.None;
                break;

            case OverlayType.NoticePopup:
            case OverlayType.ActionPopup:
                popupCount = Mathf.Max(0, popupCount - 1);
                if (popupCount == 0)
                    popupLayer.style.display = DisplayStyle.None;
                break;

            case OverlayType.Menu:
                menuCount = Mathf.Max(0, menuCount - 1);
                if (menuCount == 0)
                    menuLayer.style.display = DisplayStyle.None;
                break;
        }
    }

    private static UIDocument GetDoc(VisualElement any)
    {
        // In practice we pass the UIDocument into the factory
        // this is a fallback
        return Object.FindFirstObjectByType<UIDocument>();
    }
}
