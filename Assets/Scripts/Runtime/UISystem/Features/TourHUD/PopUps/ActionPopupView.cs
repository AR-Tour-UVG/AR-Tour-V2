using System;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class ActionPopupView : IOverlayView
{
    public VisualElement Root { get; }

    VisualElement roundedImage,
        button;
    Label title,
        description,
        buttonText;
    Action click;

    public ActionPopupView(VisualElement root)
    {
        Root = root;
    }

    public void Bind(UIDocument doc)
    {
        roundedImage = Root.Q<VisualElement>("RoundedImage");
        title = Root.Q<Label>("ActionTitle");
        description = Root.Q<Label>("ActionDescription");
        button = Root.Q<VisualElement>("ActionButton");
        buttonText = Root.Q<Label>("ButtonText");
        button?.RegisterCallback<ClickEvent>(_ => click?.Invoke());
        Hide();
    }

    public void Unbind()
    {
        button?.UnregisterCallback<ClickEvent>(_ => click?.Invoke());
        click = null;
    }

    public void Show(ActionData data, Action onClick)
    {
        click = onClick;
        if (title != null)
            title.text = data ? data.ActionTitle : "";
        if (description != null)
            description.text = data ? data.ActionDescription : "";
        if (buttonText != null)
            buttonText.text = string.IsNullOrEmpty(data?.ActionBtnText) ? "OK" : data.ActionBtnText;
        if (roundedImage != null)
            roundedImage.style.backgroundImage =
                data && data.ActionJack ? new StyleBackground(data.ActionJack) : StyleKeyword.Null;
        Root.style.display = DisplayStyle.Flex;
    }

    public void OverrideDescription(string text)
    {
        if (description != null)
            description.text = text ?? "";
    }

    public void Hide() => Root.style.display = DisplayStyle.None;
}
