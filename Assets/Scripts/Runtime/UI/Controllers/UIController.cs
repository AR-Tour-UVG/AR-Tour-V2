using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIBootstrap))]
public sealed class UIController : MonoBehaviour
{
    private UIBootstrap bootstrap = null;
    private readonly Dictionary<Type, object> map = new();
    private IScreenView currentView;
    private object currentCoord;

    void Awake()
    {
        // Get reference to UIBootstrap
        if (!bootstrap)
        {
            bootstrap = GetComponent<UIBootstrap>();
        }
        // Auto resolve bootstrap if missing
        if (!bootstrap)
        {
            bootstrap = FindFirstObjectByType<UIBootstrap>();
        }
        // If no bootstrap, throw error and disable
        if (!bootstrap)
        {
            Debug.LogError("[UIController] Missing UIBootstrap component.");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        // Setup View Model
        var vm = new TourViewModel();
        // Setup Tour Binder
        var binder = FindFirstObjectByType<TourBinder>(FindObjectsInactive.Include);
        if (binder != null)
        {
            binder.Init(vm);
        }
        else
        {
            Debug.LogError("[UIController] Missing TourBinder in scene.");
        }
        // setup router
        var r = bootstrap.Router;
        // Check if router is valid
        if (r == null)
        {
            Debug.LogError("[UIController] Missing UIRouter in UIBootstrap.");
            enabled = false;
            return;
        }
        // Register screen coordinators
        map[typeof(HomeView)] = new HomeCoordinator(r);
        map[typeof(MinigamesView)] = new MinigamesCoordinator(r);
        map[typeof(OnboardingView)] = new OnboardingCoordinator(r, bootstrap.atlas.OnboardingSet);
        map[typeof(BaseHUDView)] = new HUDCoordinator(
            bootstrap.Router,
            bootstrap.atlas,
            vm,
            binder
        );

        // Subscribe to screen changes
        r.ScreenChanged += OnScreenChanged;

        // Attach initial screen
        if (r.CurrentScreenView != null)
        {
            OnScreenChanged(r.CurrentScreenView);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from screen changes
        if (bootstrap && bootstrap.Router != null)
        {
            bootstrap.Router.ScreenChanged -= OnScreenChanged;
        }
        // Detach current coordinator
        Detach();
    }

    private void OnScreenChanged(IScreenView view)
    {
        Detach();
        currentView = view;

        // Find and attach coordinator
        var t = view.GetType();

        if (map.TryGetValue(t, out var coord))
        {
            // Assign current coordinator
            currentCoord = coord;
            // Invoke Attach method
            coord.GetType().GetMethod("Attach")?.Invoke(coord, new object[] { view });
        }
    }

    private void Detach()
    {
        // Detach current coordinator if any
        if (currentCoord == null)
            return;
        // Invoke Detach method
        currentCoord.GetType().GetMethod("Detach")?.Invoke(currentCoord, null);
        // Clear references
        currentCoord = null;
        currentView = null;
    }
}
