using System;
using UnityEngine;

public static class TourSignals
{
    public static PathProvider CurrentPathProvider { get; private set; }
    public static event Action<PathProvider> PathProviderChanged;

    public static TourUIPhase Phase { get; private set; } = TourUIPhase.WaitingForConnection;
    public static event Action<TourUIPhase> PhaseChanged;

    public static bool Connected { get; private set; }
    public static event Action<bool> ConnectedChanged;

    public static bool Paused { get; private set; } = true;
    public static event Action<bool> PausedChanged;

    public static void SetPathProvider(PathProvider pp)
    {
        if (CurrentPathProvider == pp)
            return;
        CurrentPathProvider = pp;
        PathProviderChanged?.Invoke(pp);
    }

    public static void SetPhase(TourUIPhase p)
    {
        if (Phase == p)
            return;
        Phase = p;
        PhaseChanged?.Invoke(p);
    }

    public static void SetConnected(bool c)
    {
        if (Connected == c)
            return;
        Connected = c;
        ConnectedChanged?.Invoke(c);
    }

    public static void SetPaused(bool p)
    {
        if (Paused == p)
            return;
        Paused = p;
        PausedChanged?.Invoke(p);
    }
}
