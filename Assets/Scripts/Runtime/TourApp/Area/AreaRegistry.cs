using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
/// <summary>
/// Class <c>AreaRegistry</c> manages the registration and lookup of area instances in the scene.
/// </summary>
/// <remarks>
/// - Maintains a mapping between <see cref="AreaDefinition"/> and their corresponding GameObject instances.
/// - Automatically refreshes the registry when the scene changes.
/// </remarks>
public class AreaRegistry : MonoBehaviour
{
    // Maps AreaDefinition to its corresponding GameObject instance that has an AreaInstance component
    private readonly Dictionary<AreaDefinition, GameObject> _byDef = new();

    /// <summary>Gets all discovered area GameObjects in this scene.</summary>
    public IReadOnlyCollection<GameObject> AllObjects => _byDef.Values;

    /// <summary>
    /// Scans the scene for <see cref="AreaInstance"/> components and rebuilds the registry.
    /// </summary>
    /// <remarks>
    /// Emits a warning if duplicate <see cref="AreaDefinition"/> entries are found; the first is kept.
    /// </remarks>
    public void Refresh()
    {
        // Find all AreaInstance components in the scene, including inactive ones
        var found = FindObjectsByType<AreaInstance>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        _byDef.Clear(); // Clear existing registry

        // Populate the registry with found AreaInstances
        foreach (var ai in found)
        {
            // Skip if AreaInstance or its Definition is null
            if (!ai || !ai.Definition) continue;

            // Check for duplicates and log a warning if found
            if (_byDef.TryGetValue(ai.Definition, out var existing) && existing != ai.gameObject)
            {
                Debug.LogWarning($"[AreaRegistry] Duplicate AreaDefinition '{ai.Definition.name}'. Using first.", ai);
                continue;
            }
            // Register the AreaDefinition with its GameObject
            _byDef[ai.Definition] = ai.gameObject;
        }
    }

    /// <summary>
    /// Tries to get the GameObject for a given <c>AreaDefinition</c>.
    /// </summary>
    /// <param name="def">The area definition key.</param>
    /// <param name="areaGO">When this method returns, contains the GameObject if found; otherwise <c>null</c>.</param>
    /// <returns><c>true</c> if the GameObject exists in the registry; otherwise <c>false</c>.</returns>
    public bool TryGet(AreaDefinition def, out GameObject areaGO) =>
        _byDef.TryGetValue(def, out areaGO);

    /// <summary>
    /// Returns the area GameObjects belonging to a floor in the order defined by the floor.
    /// </summary>
    /// <param name="floor">The floor definition providing ordered areas.</param>
    /// <returns>An enumerable of GameObjects for each listed area present in the scene.</returns>
    /// <remarks>
    /// Emits a warning for any <see cref="AreaDefinition"/> in <paramref name="floor"/> with no corresponding instance.
    /// </remarks>
    public IEnumerable<GameObject> ForFloor(FloorDefinition floor)
    {
        // Iterate through the ordered areas in the floor definition
        foreach (var def in floor.OrderedAreas)
        {
            // Skip if the definition is null
            if (!def) continue;
            // Try to get the corresponding GameObject and yield it; log a warning if not found
            if (_byDef.TryGetValue(def, out var go)) yield return go;
            else Debug.LogWarning($"[AreaRegistry] Missing AreaInstance for '{def.name}' in this scene.", this);
        }
    }
}
