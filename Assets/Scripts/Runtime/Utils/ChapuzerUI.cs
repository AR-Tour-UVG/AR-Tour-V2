using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ChapuzerUI
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void triggerUIRefresh();
#elif UNITY_EDITOR && !UNITY_IOS
    private static void triggerUIRefresh() { }
#else
    private static void triggerUIRefresh() { }
#endif

    public static void RefreshUI()
    {
        triggerUIRefresh();
        Debug.Log("ChapuzerUI: UI refresh triggered.");
    }
}
