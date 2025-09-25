using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewFloorDefinition", menuName = "Tour/Floor Definition")]
public class FloorDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string floorName;

    [Header("Visit Order (first -> last)")]
    [SerializeField] private List<AreaDefinition> orderedAreas = new List<AreaDefinition>();

    // Store scene path for runtime. In editor, we expose a SceneAsset and sync to this.
    [Header("Scene Binding")]
    [SerializeField, Tooltip("Unity scene path (auto-filled from SceneAsset in editor).")]
    private string scenePath;

#if UNITY_EDITOR
    [SerializeField, Tooltip("Assign the scene asset; its path is stored into 'scenePath'.")]
    private SceneAsset sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            var path = AssetDatabase.GetAssetPath(sceneAsset);
            if (!string.IsNullOrEmpty(path) && path != scenePath)
                scenePath = path;
        }
    }
#endif

    // Read-only accessors
    public string FloorName => floorName;
    public IReadOnlyList<AreaDefinition> OrderedAreas => orderedAreas;
    public string ScenePath => scenePath;

    // Helpers
    public int IndexOf(AreaDefinition area) => orderedAreas?.IndexOf(area) ?? -1;

    public AreaDefinition GetNextAfter(AreaDefinition current)
    {
        var i = IndexOf(current);
        if (i < 0) return null;
        var next = i + 1;
        return next < orderedAreas.Count ? orderedAreas[next] : null;
    }
}
