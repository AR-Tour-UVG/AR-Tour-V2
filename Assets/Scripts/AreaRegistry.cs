// AreaRegistry.cs
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AreaRegistry : MonoBehaviour
{
    private readonly Dictionary<AreaDefinition, GameObject> _byDef = new();

    /// <summary>All discovered area GameObjects in this scene.</summary>
    public IReadOnlyCollection<GameObject> AllObjects => _byDef.Values;

    private void Awake() => Refresh();

    /// <summary>Re-scan the scene for AreaInstance components.</summary>
    public void Refresh()
    {
    #if UNITY_2023_1_OR_NEWER
        var found = FindObjectsByType<AreaInstance>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
    #else
        var found = FindObjectsOfType<AreaInstance>(includeInactive: true);
    #endif

        _byDef.Clear();
        foreach (var ai in found)
        {
            if (!ai || !ai.Definition) continue;
            if (_byDef.TryGetValue(ai.Definition, out var existing) && existing != ai.gameObject)
            {
                Debug.LogWarning($"[AreaRegistry] Duplicate AreaDefinition '{ai.Definition.name}'. Using first.", ai);
                continue;
            }
            _byDef[ai.Definition] = ai.gameObject;
        }
    }

    /// <summary>Try get the area GameObject for a definition.</summary>
    public bool TryGet(AreaDefinition def, out GameObject areaGO) =>
        _byDef.TryGetValue(def, out areaGO);

    /// <summary>Ordered area GameObjects for a floor (skips missing with a warning).</summary>
    public IEnumerable<GameObject> ForFloor(FloorDefinition floor)
    {
        foreach (var def in floor.OrderedAreas)
        {
            if (!def) continue;
            if (_byDef.TryGetValue(def, out var go)) yield return go;
            else Debug.LogWarning($"[AreaRegistry] Missing AreaInstance for '{def.name}' in this scene.", this);
        }
    }
}
