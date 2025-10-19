using System;
using UnityEngine;
using UnityEngine.UIElements;

public enum PopupKind
{
    Notice,
    Action,
}

public sealed class PopupView : IOverlayView
{
    public VisualElement Root { get; }

    // shared
    VisualElement container;
    VisualElement img; // RoundImage / RoundedImage
    Label title; // MessageTitle / ActionTitle
    Label body; // Message / ActionDescription

    // action-only
    VisualElement actionBtn; // ActionButton
    Label actionBtnText; // ButtonText
    Action onClick;

    public PopupView(VisualElement root)
    {
        Root = root;
    }

    public void Bind(UIDocument doc)
    {
        container = Root.Q<VisualElement>("PopUP") ?? Root.Q<VisualElement>("PopUp") ?? Root;
        img = Root.Q<VisualElement>("RoundImage") ?? Root.Q<VisualElement>("RoundedImage");
        title = Root.Q<Label>("MessageTitle") ?? Root.Q<Label>("ActionTitle");
        body = Root.Q<Label>("Message") ?? Root.Q<Label>("ActionDescription");

        actionBtn = Root.Q<VisualElement>("ActionButton"); // may be null in notice UXML
        actionBtnText = Root.Q<Label>("ButtonText");

        if (actionBtn != null)
            actionBtn.RegisterCallback<ClickEvent>(_ => onClick?.Invoke());
        Hide();
    }

    public void Unbind()
    {
        if (actionBtn != null)
            actionBtn.UnregisterCallback<ClickEvent>(_ => onClick?.Invoke());
        onClick = null;
    }

    public void ShowNotice(NoticeData d)
    {
        onClick = null;
        if (title != null)
            title.text = d ? d.NoticeTitle : "";
        if (body != null)
            body.text = d ? d.NoticeMessage : "";
        if (img != null)
            img.style.backgroundImage =
                d && d.NoticeJack ? new StyleBackground(d.NoticeJack) : StyleKeyword.Null;

        // hide action bits if present
        if (actionBtn != null)
            actionBtn.style.display = DisplayStyle.None;

        Root.style.display = DisplayStyle.Flex;
    }

    public void ShowAction(ActionData d, Action click)
    {
        onClick = click;
        if (title != null)
            title.text = d ? d.ActionTitle : "";
        if (body != null)
            body.text = d ? d.ActionDescription : "";
        if (img != null)
            img.style.backgroundImage =
                d && d.ActionJack ? new StyleBackground(d.ActionJack) : StyleKeyword.Null;

        if (actionBtn != null)
        {
            actionBtn.style.display = DisplayStyle.Flex;
            if (actionBtnText != null)
                actionBtnText.text = string.IsNullOrEmpty(d?.ActionBtnText)
                    ? "OK"
                    : d.ActionBtnText;
        }

        Root.style.display = DisplayStyle.Flex;
    }

    public void Hide() => Root.style.display = DisplayStyle.None;
}
