using UnityEngine.UIElements;

public enum ScreenState
{
    Onboarding,
    Home,
    Minigames,
    TourHUD,
}

public enum OverlayType
{
    InfoModal,
    Popup,
    Menu,
}

public interface IView
{
    VisualElement Root { get; }
    void Bind(UIDocument doc);
    void Unbind();
}

public interface IScreenView : IView { }

public interface IOverlayView : IView { }

public interface IViewFactory
{
    IScreenView CreateScreen(ScreenState s);
    IOverlayView CreateOverlay(OverlayType t);
}
