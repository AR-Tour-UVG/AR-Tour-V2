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
        if (!bootstrap)
        {
            bootstrap = GetComponent<UIBootstrap>();
        }
        if (!bootstrap)
        {
            bootstrap = FindFirstObjectByType<UIBootstrap>();
        }
        if (!bootstrap)
        {
            Debug.LogError("[UIController] Missing UIBootstrap component.");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        var vm = new TourViewModel();
        var binder = FindFirstObjectByType<TourBinder>(FindObjectsInactive.Include);
        if (binder != null)
        {
            binder.Init(vm);
        }
        else
        {
            Debug.LogError("[UIController] Missing TourBinder in scene.");
        }
        var r = bootstrap.Router;
        if (r == null)
        {
            Debug.LogError("[UIController] Missing UIRouter in UIBootstrap.");
            enabled = false;
            return;
        }
        map[typeof(HomeView)] = new HomeCoordinator(r);
        map[typeof(MinigamesView)] = new MinigamesCoordinator(r);
        map[typeof(OnboardingView)] = new OnboardingCoordinator(r, bootstrap.uiAtlas.OnboardingSet);
        map[typeof(BaseHUDView)] = new HUDCoordinator(
            bootstrap.Router,
            vm,
            binder,
            bootstrap.uiAtlas,
            bootstrap.audioAtlas
        );

        r.ScreenChanged += OnScreenChanged;

        if (r.CurrentScreenView != null)
        {
            OnScreenChanged(r.CurrentScreenView);
        }
    }

    void OnDestroy()
    {
        if (bootstrap && bootstrap.Router != null)
        {
            bootstrap.Router.ScreenChanged -= OnScreenChanged;
        }
        Detach();
    }

    private void OnScreenChanged(IScreenView view)
    {
        Detach();
        currentView = view;

        var t = view.GetType();

        if (map.TryGetValue(t, out var coord))
        {
            currentCoord = coord;
            coord.GetType().GetMethod("Attach")?.Invoke(coord, new object[] { view });
        }
    }

    private void Detach()
    {
        if (currentCoord == null)
            return;
        currentCoord.GetType().GetMethod("Detach")?.Invoke(currentCoord, null);
        currentCoord = null;
        currentView = null;
    }
}
