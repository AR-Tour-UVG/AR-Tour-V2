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
    NoticePopup,
    ActionPopup,
    Menu,
    Settings,
}

public interface IViewFactory
{
    IScreenView CreateScreen(ScreenState s);
    IOverlayView CreateOverlay(OverlayType t);
}
