using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewFloorDefinition", menuName = "AR-Tour/Floor Definition")]
public class FloorDefinition : ScriptableObject
{
    [Header("Floor Info")]
    [Tooltip("Name of the floor")]
    [SerializeField] private string floorName;

    [Header("Content References")]
    [Tooltip("List of areas in the order they should be visited on this floor.")]
    [SerializeField] private List<AreaDefinition> orderedAreas = new();
    [Tooltip("JSON file with anchors for this floor (TextAsset).")]
    [SerializeField] private TextAsset anchorMapJson;

    // Store scene path for runtime. In editor, we expose a SceneAsset and sync to this.
    [Header("Scene Binding")]
    [Tooltip("Unity scene path (auto-filled from SceneAsset in editor).")]
    [SerializeField] private string scenePath;

#if UNITY_EDITOR
    [Tooltip("Assign the scene asset; its path is stored into 'scenePath'.")]
    [SerializeField] private SceneAsset sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            var path = AssetDatabase.GetAssetPath(sceneAsset);
            if (!string.IsNullOrEmpty(path) && path != scenePath)
            {
                scenePath = path;
            }
        }

        if (anchorMapJson != null)
        {
            var jsonPath = AssetDatabase.GetAssetPath(anchorMapJson);
            if (!jsonPath.EndsWith(".json"))
            {
                Debug.LogError($"[FloorDefinition] Anchor map is not a .json file: {jsonPath}", this);
            }
        }
        else
        {
            Debug.LogError($"[FloorDefinition] No anchor map assigned for floor '{floorName}'", this);
        }
    }
#endif

    // Read-only accessors
    public string FloorName => floorName;
    public IReadOnlyList<AreaDefinition> OrderedAreas => orderedAreas;
    public string ScenePath => scenePath;
    public TextAsset AnchorMapJson => anchorMapJson;

    // Helpers
    public int IndexOf(AreaDefinition area) => orderedAreas?.IndexOf(area) ?? -1;

    public AreaDefinition GetNextAfter(AreaDefinition current)
    {
        var i = IndexOf(current);
        if (i < 0) return null;
        var next = i + 1;
        return next < orderedAreas.Count ? orderedAreas[next] : null;
    }

    public bool TryGetAnchorMap<T>(out T map)
    {
        map = default;
        if (anchorMapJson == null || string.IsNullOrEmpty(anchorMapJson.text)) return false;
        try { map = JsonUtility.FromJson<T>(anchorMapJson.text); return true; }
        catch { return false; }
    }
    
    public bool TryGetAnchorMapText(out string json)
    {
        json = anchorMapJson != null ? anchorMapJson.text : null;
        return !string.IsNullOrEmpty(json);
    }

}
