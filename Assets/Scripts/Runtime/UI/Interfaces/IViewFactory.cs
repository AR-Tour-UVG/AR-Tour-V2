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

public interface IViewFactory
{
    IScreenView CreateScreen(ScreenState s);
    IOverlayView CreateOverlay(OverlayType t);
}
