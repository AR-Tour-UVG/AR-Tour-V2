using UnityEngine.UIElements;

public interface IView
{
    VisualElement Root { get; }
    void Bind(UIDocument doc);
    void Unbind();
}

public interface IScreenView : IView { }

public interface IOverlayView : IView { }
