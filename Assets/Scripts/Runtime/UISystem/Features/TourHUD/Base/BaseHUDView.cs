using System;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class BaseHUDView : IScreenView
{
    public VisualElement Root { get; }
    public event Action OnMenu;

    VisualElement directionsCard,
        footer,
        menuBtn,
        titleIcon;
    Label titleLabel,
        directionsLabel,
        progressValue,
        distanceValue;

    public BaseHUDView(VisualElement root)
    {
        Root = root;
    }

    public void Bind(UIDocument doc)
    {
        directionsCard = Root.Q<VisualElement>("DirectionsCard");
        footer = Root.Q<VisualElement>("Footer");
        menuBtn = Root.Q<VisualElement>("MenuButton");
        titleLabel = Root.Q<Label>("Title");
        directionsLabel = Root.Q<Label>("Directions");
        progressValue = Root.Q<Label>("ProgressValue");
        distanceValue = Root.Q<Label>("DistanceValue");
        titleIcon = Root.Q<VisualElement>("TitleIcon");

        UIPickingUtils.ConfigureTreePickingMode(Root, PickingMode.Ignore);

        UIPickingUtils.SetPickable(menuBtn);

        menuBtn?.RegisterCallback<ClickEvent>(_ => OnMenu?.Invoke());
    }

    public void Unbind()
    {
        menuBtn?.UnregisterCallback<ClickEvent>(_ => OnMenu?.Invoke());
    }

    public void SetTitle(string text, Sprite icon = null)
    {
        if (titleLabel != null)
            titleLabel.text = text ?? "";
        if (titleIcon != null)
            titleIcon.style.backgroundImage = icon ? new StyleBackground(icon) : StyleKeyword.Null;
    }

    public void SetDirections(string text, bool visible)
    {
        if (directionsLabel != null)
            directionsLabel.text = text ?? "";
        if (directionsCard != null)
            directionsCard.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void SetProgress(float normalized01)
    {
        if (progressValue == null)
            return;
        var pct = Mathf.Round(normalized01 * 100f);
        progressValue.text = $"{pct:0}%";
    }

    public void SetDistance(float meters)
    {
        if (distanceValue != null)
            distanceValue.text = $"{meters:0.00}m";
    }

    public void ShowFooter(bool on)
    {
        if (footer != null)
            footer.style.display = on ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void SetTitleIcon(Sprite s)
    {
        if (titleIcon == null)
            return;
        titleIcon.style.backgroundImage = s != null ? new StyleBackground(s) : StyleKeyword.Null; // falls back to USS default
    }
}
