using System;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class InfoWidgetView : IOverlayView
{
    public VisualElement Root { get; }
    public event Action OnContinue;

    VisualElement widget,
        areaImage,
        continueBtn;
    Label description;

    public InfoWidgetView(VisualElement root)
    {
        Root = root;
    }

    public void Bind(UIDocument doc)
    {
        widget = Root.Q<VisualElement>("InfoWidget");
        areaImage = Root.Q<VisualElement>("AreaImage");
        description = Root.Q<Label>("Description");
        continueBtn = Root.Q<VisualElement>("ContinueButton");

        // Disable raycast blocking for the info widget
        UIPickingUtils.ConfigureTreePickingMode(Root, PickingMode.Ignore);

        // Re-enable picking for the widget and its children
        UIPickingUtils.ConfigureTreePickingMode(widget, PickingMode.Position);

        continueBtn?.RegisterCallback<ClickEvent>(_ => OnContinue?.Invoke());
        Hide();
    }

    public void Unbind()
    {
        continueBtn?.UnregisterCallback<ClickEvent>(_ => OnContinue?.Invoke());
    }

    public void Show(AreaDefinition area)
    {
        if (description != null)
            description.text = area ? area.AreaText : "";
        if (areaImage != null)
            areaImage.style.backgroundImage =
                area && area.AreaImage ? new StyleBackground(area.AreaImage) : StyleKeyword.Null;
        Root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        Root.style.display = DisplayStyle.None;
    }
}
