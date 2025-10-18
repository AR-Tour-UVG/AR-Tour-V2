using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFloorDefinition", menuName = "AR-Tour/Floor Definition")]
public class FloorDefinition : ScriptableObject
{
    [Header("Floor Info")]
    [Tooltip("Name of the floor")]
    [SerializeField]
    private string floorName;

    [Header("Content References")]
    [Tooltip("List of areas in the order they should be visited on this floor.")]
    [SerializeField]
    private List<AreaDefinition> orderedAreas = new();

    [Tooltip("JSON file with anchors for this floor (TextAsset).")]
    [SerializeField]
    private TextAsset anchorMapJson;

    [Tooltip("Path to the scene asset in the build (read-only).")]
    [HideInInspector, SerializeField]
    private string scenePath;

#if UNITY_EDITOR
    [Tooltip("Assign the scene asset; its path is stored into 'scenePath'.")]
    [SerializeField]
    private SceneAsset sceneAsset;
#endif

    // Read-only accessors
    public string FloorName => floorName;
    public IReadOnlyList<AreaDefinition> OrderedAreas => orderedAreas;
    public TextAsset AnchorMapJson => anchorMapJson;
    public string ScenePath => scenePath;

    // Helpers
    public int IndexOf(AreaDefinition area) => orderedAreas?.IndexOf(area) ?? -1;

    public AreaDefinition GetNextAfter(AreaDefinition current)
    {
        var i = IndexOf(current);
        if (i < 0)
            return null;
        var next = i + 1;
        return next < orderedAreas.Count ? orderedAreas[next] : null;
    }

    public bool TryGetAnchorMap<T>(out T map)
    {
        map = default;
        if (anchorMapJson == null || string.IsNullOrEmpty(anchorMapJson.text))
            return false;
        try
        {
            map = JsonUtility.FromJson<T>(anchorMapJson.text);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool TryGetAnchorMapText(out string json)
    {
        json = anchorMapJson != null ? anchorMapJson.text : null;
        return !string.IsNullOrEmpty(json);
    }
}
