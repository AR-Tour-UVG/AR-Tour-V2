using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AreaRegistry : MonoBehaviour
{
    // Internal dictionary mapping AreaDefinition to their GameObject instances
    private readonly Dictionary<AreaDefinition, GameObject> _byDef = new();

    // Gets all discovered area GameObjects in this scene.
    public IReadOnlyCollection<GameObject> AllObjects => _byDef.Values;

    
    public void Refresh()
    {
        var found = FindObjectsByType<AreaInstance>(FindObjectsInactive.Include, FindObjectsSortMode.None);
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

    public bool TryGet(AreaDefinition def, out GameObject areaGO) =>
        _byDef.TryGetValue(def, out areaGO);

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
