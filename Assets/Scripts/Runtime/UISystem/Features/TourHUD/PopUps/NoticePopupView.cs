using UnityEngine;
using UnityEngine.UIElements;

public sealed class NoticePopupView : IOverlayView
{
    public VisualElement Root { get; }

    VisualElement roundImage,
        container;
    Label title,
        message;

    public NoticePopupView(VisualElement root)
    {
        Root = root;
    }

    public void Bind(UIDocument doc)
    {
        container = Root.Q<VisualElement>("PopUP") ?? Root.Q<VisualElement>("PopUp");
        roundImage = Root.Q<VisualElement>("RoundImage") ?? Root.Q<VisualElement>("RoundedImage");
        title = Root.Q<Label>("MessageTitle");
        message = Root.Q<Label>("Message");
        Hide();
    }

    public void Unbind() { }

    public void Show(NoticeData data)
    {
        if (title != null)
            title.text = data ? data.NoticeTitle : "";
        if (message != null)
            message.text = data ? data.NoticeMessage : "";
        if (roundImage != null)
            roundImage.style.backgroundImage =
                data && data.NoticeJack ? new StyleBackground(data.NoticeJack) : StyleKeyword.Null;
        Root.style.display = DisplayStyle.Flex;
    }

    public void Hide() => Root.style.display = DisplayStyle.None;
}
