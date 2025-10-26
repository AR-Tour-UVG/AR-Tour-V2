using System;
using UnityEngine.UIElements;

public sealed class InfoWidgetView : IOverlayView
{
    public VisualElement Root { get; }
    public event Action OnContinue;
    public event Action Hidden;
    VisualElement widget,
        areaImage,
        continueBtn;
    Label description;
    bool isOpen;
    EventCallback<ClickEvent> continueClick;

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

        UIPickingUtils.ConfigureTreePickingMode(Root, PickingMode.Ignore);

        UIPickingUtils.ConfigureTreePickingMode(widget, PickingMode.Position);

        continueClick = _ => OnContinue?.Invoke();
        continueBtn?.RegisterCallback(continueClick);

        widget.RegisterCallback<TransitionEndEvent>(OnAnyTransitionEnd);

        widget.RemoveFromClassList("is-open");
        Root.style.display = DisplayStyle.None;
        isOpen = false;
    }

    public void Unbind()
    {
        if (continueBtn != null && continueClick != null)
            continueBtn.UnregisterCallback(continueClick);
        widget?.UnregisterCallback<TransitionEndEvent>(OnAnyTransitionEnd);
    }

    public void Show(AreaDefinition area)
    {
        if (isOpen)
            return;

        if (description != null)
            description.text = area ? area.AreaText : "";

        if (areaImage != null)
            areaImage.style.backgroundImage =
                area && area.AreaImage ? new StyleBackground(area.AreaImage) : StyleKeyword.Null;

        Root.style.display = DisplayStyle.Flex;

        widget.RemoveFromClassList("is-open");

        Root.schedule.Execute(() =>
            {
                widget.AddToClassList("is-open");
                isOpen = true;
            })
            .StartingIn(1);
    }

    public void Hide()
    {
        if (!isOpen && Root.style.display == DisplayStyle.None)
        {
            Hidden?.Invoke();
            return;
        }
        widget.RemoveFromClassList("is-open");
        isOpen = false;
    }

    void OnAnyTransitionEnd(TransitionEndEvent e)
    {
        if (e.target != widget)
            return;

        bool relevant = false;
        foreach (var name in e.stylePropertyNames)
        {
            var prop = name.ToString();
            if (prop == "opacity" || prop == "translate")
            {
                relevant = true;
                break;
            }
        }
        if (!relevant)
            return;

        if (!isOpen)
            Root.style.display = DisplayStyle.None;
    }
}
