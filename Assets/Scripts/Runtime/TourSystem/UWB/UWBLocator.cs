using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public struct Coordinate
{
    public float x;
    public float y;
}

public static class UWBLocator
{
    public static bool IsInitialized => isInitialized;
    private static bool isInitialized = false;
    private static string currentAnchorMap;

#if UNITY_IOS && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern IntPtr getCoords();

    [DllImport("__Internal")]
    private static extern void freeCString(IntPtr ptr);

    [DllImport("__Internal")]
    private static extern void setAnchorMap(string jsonUtf8);

    [DllImport("__Internal")]
    private static extern void start();

    [DllImport("__Internal")]
    private static extern void stop();
#elif UNITY_EDITOR && !UNITY_IOS
    private static bool hasWarned = false;

    private static IntPtr getCoords() => IntPtr.Zero;

    private static void freeCString(IntPtr ptr) { }

    private static void setAnchorMap(string jsonUtf8) { }

    private static void start() { }

    private static void stop() { }
#else
    private static IntPtr getCoords() => IntPtr.Zero;

    private static void freeCString(IntPtr ptr) { }

    private static void setAnchorMap(string jsonUtf8) { }

    private static void start() { }

    private static void stop() { }
#endif

    public static bool TryGetPosition(out Vector3 position)
    {
        position = default;

#if UNITY_EDITOR && !UNITY_IOS
        if (!hasWarned)
        {
            Debug.LogWarning(
                "[UWBLocator] Real time positioning is supported only on iOS device builds."
            );
            hasWarned = true;
        }
        return false;

#elif UNITY_IOS && !UNITY_EDITOR
        IntPtr coordsPtr = getCoords();
        if (coordsPtr == IntPtr.Zero)
        {
            Debug.LogWarning("[UWBLocator] Received null pointer for coordinates from UWB plugin.");
            return false;
        }

        try
        {
            string json = Marshal.PtrToStringAnsi(coordsPtr);
            Debug.Log($"[UWBLocator] JSON from plugin: {json}");

            if (string.IsNullOrEmpty(json) || json == "{}" || json.Contains("null"))
            {
                Debug.LogWarning(
                    "[UWBLocator] Received invalid JSON or null coordinates from UWB plugin."
                );
                return false;
            }

            var uwbPosition = JsonUtility.FromJson<Coordinate>(json);
            Debug.Log($"[UWBLocator] Parsed UWB Position - x: {uwbPosition.x}, y: {uwbPosition.y}");
            position = new Vector3(uwbPosition.x, 0f, uwbPosition.y);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[UWBLocator] Failed to parse UWB position JSON: {ex.Message}");
            return false;
        }
        finally
        {
            Debug.Log($"[UWBLocator] Freeing allocated string for coordinates.");
            freeCString(coordsPtr);
        }
#else
        Debug.LogWarning(
            "[UWBLocator] Real time positioning is supported only on iOS device builds."
        );
        return false;
#endif
    }

    public static void SetAnchorMap(string anchorMap)
    {
        if (string.IsNullOrWhiteSpace(anchorMap))
        {
            Debug.LogWarning("[UWBLocator] SetAnchorMap: Anchor map is null or empty.");
            return;
        }
        if (currentAnchorMap == anchorMap)
        {
            Debug.Log("[UWBLocator] SetAnchorMap: same Anchor map, no change.");
            return;
        }

#if UNITY_IOS && !UNITY_EDITOR
        try
        {
            setAnchorMap(anchorMap);
            currentAnchorMap = anchorMap;
            Debug.Log($"[UWBLocator] SetAnchorMap: Anchor map set to {anchorMap}.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[UWBLocator] SetAnchorMap: Failed setting anchor map: {ex.Message}");
        }
#else
        currentAnchorMap = anchorMap;
        Debug.Log("[UWBLocator] Not supported on this platform.");
#endif
    }

    public static void Start()
    {
#if UNITY_IOS && !UNITY_EDITOR
        try
        {
            start();
            Debug.Log("[UWBLocator] Native Plugin started.");
            isInitialized = true;
        }
        catch (Exception ex)
        {
            isInitialized = false;
            Debug.LogError($"[UWBLocator] Start failed: {ex.Message}");
        }
#endif
    }

    public static void Stop()
    {
#if UNITY_IOS && !UNITY_EDITOR
        try
        {
            stop();
            Debug.Log("[UWBLocator] Native Plugin stopped.");
            isInitialized = false;
            currentAnchorMap = null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[UWBLocator] Stop failed: {ex.Message}");
        }
#endif
    }
}
