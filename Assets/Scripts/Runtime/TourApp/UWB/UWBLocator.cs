using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Text;
using Unity.VisualScripting;


// ----- Data Structures -----
/// <summary>
/// Represents a 2D position in Unity's space.
/// </summary>
[Serializable]
public struct Coordinate
{
    public float x; // x-coordinate in Unity's space
    public float y; // y-coordinate in Unity's space
}

// ----- Native Bridge -----
/// <summary>
/// Provides access to Ultra Wideband (UWB) native plugin and maps them to Unity coordinates.
/// </summary>
public static class UWBLocator
{
    public static bool IsInitialized => isInitialized;
    private static bool isInitialized = false;
    private static string currentAnchorMap; // cache to avoid redundant sets

    // Check if the platform is iOS and import the required native functions
#if !UNITY_IOS || UNITY_EDITOR
    // Log-once guard for non-iOS/editor runs
    private static bool hasWarned = false;

    // Returns pointer to a null-terminated JSON string allocated with strdup (must be freed).
    [DllImport("__Internal")] private static extern IntPtr getCoords();

    // Frees the JSON string allocated by getCoords().
    [DllImport("__Internal")] private static extern void freeCString(IntPtr ptr);

    // Sets the anchor map in the native plugin.
    [DllImport("__Internal")] private static extern void setAnchorMap(string jsonUtf8);

    // Configure plugin to use uwb anchor map
    [DllImport("__Internal", EntryPoint = "start")] private static extern void uwb_start();

#else
    // Stubs implementation for non-iOS platforms
    private static IntPtr getCoords() => IntPtr.Zero; // Always returns null pointer
    private static void freeCString(IntPtr ptr) { } // No-op for non-iOS platforms
    private static void setAnchorMap(string jsonUtf8) { } // No-op for non-iOS platforms
    private static void uwb_start() { } // No-op for non-iOS platforms

#endif

    /// <summary>
    /// Attempts to retrieve the latest UWB position from the native plugin.
    /// If successful, the position will be returned transformed into Unity's coordinate space.
    /// </summary>
    /// <param name="position">The retrieved UWB position in Unity's coordinate space.</param>
    /// <returns>True if the position was successfully retrieved and parsed; otherwise, false.</returns>
    public static bool TryGetPosition(out Vector3 position)
    {
        position = default; // Initialize position

        // Check if the platform is iOS before attempting to retrieve the position
#if !UNITY_IOS || UNITY_EDITOR
        // Warn only the firts time it runs on non iOS device
        if (!hasWarned)
        {
            Debug.LogWarning("[UWBLocator] Real time positioning is supported only on iOS device builds.");
            hasWarned = true; // Set the flag to true after the first warning
        }
        return false; // Not running on iOS, return false
#else
        IntPtr coordsPtr = getCoords(); // Call the native function to get the coordinates
        // Validate the pointer 
        if (coordsPtr == IntPtr.Zero)
        {
            Debug.LogWarning("[UWBLocator] getCoords() returned null pointer.");
            return false; // Failed to get coordinates
        }

        // Try to extract coordinate values from JSON
        try
        {
            // Read the JSON string from the pointer
            string json = Marshal.PtrToStringAnsi(coordsPtr);
            Debug.Log($"[UWBLocator] JSON from plugin: {json}");

            // Filter invalid JSON or null coordinate cases
            if (string.IsNullOrEmpty(json) || json == "{}" || json.Contains("null"))
            {
                Debug.LogWarning("[UWBLocator] Received invalid JSON or null coordinates from UWB plugin.");
                return false; // Invalid JSON or null coordinates
            }

            // Parse the JSON string into a Coordinate object
            Coordinate uwbPosition = JsonUtility.FromJson<Coordinate>(json);
            // Map plugin X -> Unity X, plugin Y -> Unity Z, ignore plugin Z -> Unity Y
            position = new Vector3(uwbPosition.x, 0f, uwbPosition.y);
            return true; // Successfully retrieved and parsed position
        }
        catch (Exception ex)
        {
            Debug.LogError($"[UWBLocator] Failed to parse UWB position JSON: {ex.Message}");
            return false; // Failed to parse JSON
        }
        finally
        {
            // Always free the allocated string
            Debug.Log($"[UWBLocator] Freeing allocated string for coordinates.");
            freeCString(coordsPtr);
        }
#endif
    }

    public static void SetAnchorMap(string anchorMap)
    {
        // Check if the anchor map is null or empty
        if (string.IsNullOrWhiteSpace(anchorMap))
        {
            Debug.LogWarning("[UWBLocator] SetAnchorMap: Anchor map is null or empty.");
            return;
        }
        if (currentAnchorMap == anchorMap && isInitialized)
        {
            Debug.Log("[UWBLocator] SetAnchorMap: same Anchor map, no change.");
            return;
        }

#if UNITY_IOS && !UNITY_EDITOR
        try
        {
            setAnchorMap(anchorMap); // Set the anchor map
            currentAnchorMap = anchorMap; // Update the cached anchor map
            Debug.Log($"[UWBLocator] SetAnchorMap: Anchor map set to {anchorMap}.");

            if (!isInitialized)
            {
                uwb_start(); // Start the UWB plugin
                isInitialized = true;
                Debug.Log("[UWBLocator] Native Plugin started.");
            }
            else
            {
                Debug.Log("[UWBLocator] Native Plugin already started.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[UWBLocator] SetAnchorMap: Failed setting anchor map: {ex.Message}");
        }
#else
        currentAnchorMap = anchorMap; // Update the cached anchor map
        Debug.Log("[UWBLocator] Not supported on this platform.");
#endif
    }
}