// TourViewModel.cs
using System;
using UnityEngine;

public enum TourUIPhase
{
    WaitingForConnection, // scene loaded, not connected
    ReadyPrompt, // show Start/Ready action popup
    Navigating, // HUD directions/footer visible
    InAreaInfo, // InfoWidget showing
    FloorTransition, // between floors (elevators)
    TourComplete,
}

public sealed class TourViewModel
{
    // ---- State (read-only to views) ----
    public string CurrentArea { get; private set; } = "";
    public string NextArea { get; private set; } = "";
    public float ProgressNormalized { get; private set; } = 0f; // 0..1
    public float DistanceMeters { get; private set; } = 0f;
    public bool Connected { get; private set; } = false;
    public bool Paused { get; private set; } = true;
    public bool HasBegunTour { get; private set; } = false; // Start popup only once
    public TourUIPhase Phase { get; private set; } = TourUIPhase.WaitingForConnection;

    public AreaDefinition CurrentAreaDef { get; private set; }
    public AreaDefinition NextAreaDef { get; private set; }
    public FloorDefinition CurrentFloor { get; private set; }

    // ---- Signals ----
    public event Action Changed;
    public event Action<AreaDefinition> EnteredArea;
    public event Action<AreaDefinition> GuidingTo;
    public event Action<FloorDefinition> FloorBegan;
    public event Action<FloorDefinition> FloorEnded;
    public event Action TourCompletedEvent;
    public event Action ConnectionLost;
    public event Action ConnectionRestored;

    // ---- Mutators (binder/coordinators call) ----
    public void SetPhase(TourUIPhase p)
    {
        if (Phase == p)
            return;
        Phase = p;
        Changed?.Invoke();
    }

    public void SetConnected(bool on)
    {
        if (Connected == on)
            return;
        Connected = on;
        if (on)
            ConnectionRestored?.Invoke();
        else
            ConnectionLost?.Invoke();
        Changed?.Invoke();
    }

    public void SetPaused(bool paused)
    {
        if (Paused == paused)
            return;
        Paused = paused;
        Changed?.Invoke();
    }

    public void SetDistance(float meters)
    {
        meters = Mathf.Max(0f, meters);
        if (Mathf.Approximately(DistanceMeters, meters))
            return;
        DistanceMeters = meters;
        Changed?.Invoke();
    }

    public void SetProgress(float progressValue)
    {
        progressValue = Mathf.Clamp01(progressValue);
        if (Mathf.Approximately(ProgressNormalized, progressValue))
            return;
        ProgressNormalized = progressValue;
        Changed?.Invoke();
    }

    public void UpdateProgress(int visited, int total)
    {
        float v = (total > 0) ? visited / (float)total : 0f;
        SetProgress(v);
    }

    public void SetCurrentFloor(FloorDefinition f)
    {
        if (CurrentFloor == f)
            return;
        CurrentFloor = f;
        FloorBegan?.Invoke(f);
        Changed?.Invoke();
    }

    public void NotifyFloorEnded(FloorDefinition f)
    {
        FloorEnded?.Invoke(f);
        Changed?.Invoke();
    }

    public void NotifyGuidingTo(AreaDefinition next)
    {
        NextAreaDef = next;
        NextArea = next ? next.AreaName : "";
        GuidingTo?.Invoke(next);
        Changed?.Invoke();
    }

    public void NotifyEnteredArea(AreaDefinition def, int visited, int total)
    {
        CurrentAreaDef = def;
        CurrentArea = def ? def.AreaName : "";
        UpdateProgress(visited, total);
        EnteredArea?.Invoke(def);
        Changed?.Invoke();
    }

    public void NotifyTourCompleted()
    {
        SetPhase(TourUIPhase.TourComplete);
        TourCompletedEvent?.Invoke();
        Changed?.Invoke();
    }

    public void MarkTourBegan()
    {
        if (HasBegunTour)
            return;
        HasBegunTour = true;
        Changed?.Invoke();
    }

    public void ResetAll()
    {
        CurrentArea = NextArea = "";
        CurrentAreaDef = NextAreaDef = null;
        CurrentFloor = null;
        ProgressNormalized = 0f;
        DistanceMeters = 0f;
        Connected = false;
        Paused = true;
        HasBegunTour = false;
        Phase = TourUIPhase.WaitingForConnection;
        Changed?.Invoke();
    }
}
