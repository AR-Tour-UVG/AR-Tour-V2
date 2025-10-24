using System;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class SettingsView : IOverlayView
{
    public VisualElement Root { get; }
    public event Action CloseRequested;
    public event Action<int> VolumeChanged; // 0..100
    public event Action VolumeChangeCommitted; // after debounce
    public event Action<int> FontPxPicked; // 80/100/120
    Slider slider;
    VisualElement smallOpt,
        normalOpt,
        largeOpt,
        volumeIcon;
    UIDocument baseDoc;

    // debounce scheduler
    IVisualElementScheduledItem previewSched;

    public SettingsView(VisualElement root) => Root = root;

    public void Bind(UIDocument doc)
    {
        baseDoc = doc;
        slider = Root.Q<Slider>("Slider");
        smallOpt = Root.Q<VisualElement>("Small");
        normalOpt = Root.Q<VisualElement>("Normal");
        largeOpt = Root.Q<VisualElement>("Large");
        volumeIcon = Root.Q<VisualElement>("VolumeIcon");

        Root.Q<VisualElement>("CloseIcon")
            ?.RegisterCallback<ClickEvent>(_ => CloseRequested?.Invoke());

        slider.lowValue = 0;
        slider.highValue = 100;

        // Live volume update
        slider.RegisterValueChangedCallback(e =>
        {
            VolumeChanged?.Invoke(Mathf.RoundToInt(e.newValue));

            // Debounce commit (fires 250 ms after last change)
            previewSched?.Pause();
            previewSched = slider
                .schedule.Execute(() => VolumeChangeCommitted?.Invoke())
                .StartingIn(250);
        });

        // Touch release / cancel on the dragger
        var dragger = slider.Q<VisualElement>("unity-dragger");
        if (dragger != null)
        {
            dragger.RegisterCallback<PointerUpEvent>(_ => VolumeChangeCommitted?.Invoke());
            dragger.RegisterCallback<PointerCaptureOutEvent>(_ => VolumeChangeCommitted?.Invoke());
            dragger.RegisterCallback<PointerCancelEvent>(_ => VolumeChangeCommitted?.Invoke());
        }

        smallOpt?.RegisterCallback<ClickEvent>(_ => FontPxPicked?.Invoke(80));
        normalOpt?.RegisterCallback<ClickEvent>(_ => FontPxPicked?.Invoke(100));
        largeOpt?.RegisterCallback<ClickEvent>(_ => FontPxPicked?.Invoke(120));

        slider.RegisterCallback<GeometryChangedEvent>(_ => ApplySliderStyles());
        slider.RegisterValueChangedCallback(_ => ApplySliderStyles());

        Show();
    }

    public void ApplySliderStyles()
    {
        var tracker = slider.Q<VisualElement>("unity-tracker");
        var dragger = slider.Q<VisualElement>("unity-dragger");
        var fill = tracker.Q<VisualElement>("unity-fill");

        if (tracker != null)
        {
            tracker.style.backgroundImage = null;
            tracker.style.unityBackgroundImageTintColor = StyleKeyword.None;
            tracker.style.backgroundColor = new StyleColor(new Color32(75, 75, 75, 255));
        }
        if (dragger != null)
        {
            dragger.style.backgroundImage = null;
            dragger.style.unityBackgroundImageTintColor = StyleKeyword.None;
            dragger.style.backgroundColor = new StyleColor(new Color32(242, 242, 242, 255));
        }
        if (fill != null)
        {
            fill.style.backgroundColor = new StyleColor(new Color32(0, 165, 0, 255));
        }
    }

    public void Unbind()
    {
        // callbacks removed with hierarchy
    }

    public void Show() => Root.style.display = DisplayStyle.Flex;

    public void Hide() => Root.style.display = DisplayStyle.None;

    // UI setters used by coordinator
    public void SetSlider(int v)
    {
        slider.SetValueWithoutNotify(v);
    }

    public void SetSelectedFontPx(int px)
    {
        ToggleSel(smallOpt, px == 80);
        ToggleSel(normalOpt, px == 100);
        ToggleSel(largeOpt, px == 120);
    }

    void ToggleSel(VisualElement ve, bool on)
    {
        if (ve == null)
            return;
        ve.EnableInClassList("selected", on);
    }

    public void SetVolumeIcon(VolumeLevel level)
    {
        if (volumeIcon == null)
            return;
        volumeIcon.EnableInClassList("mute", level == VolumeLevel.Mute);
        volumeIcon.EnableInClassList("low", level == VolumeLevel.Low);
        volumeIcon.EnableInClassList("med", level == VolumeLevel.Med);
        volumeIcon.EnableInClassList("high", level == VolumeLevel.High);
    }
}
