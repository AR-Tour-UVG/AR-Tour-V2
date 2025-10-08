using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTourDefinition", menuName = "AR-Tour/Tour Definition")]
public class TourDefinition : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Name of the tour")]
    [SerializeField] private string tourName;

    [Header("Floor Order (first -> last)")]
    [Tooltip("List of floors in the order they should be visited in this tour.")]
    [SerializeField] private List<FloorDefinition> orderedFloors = new();

    public string TourName => tourName;
    public IReadOnlyList<FloorDefinition> OrderedFloors => orderedFloors;

    public int IndexOf(FloorDefinition floor) => orderedFloors?.IndexOf(floor) ?? -1;

    public FloorDefinition GetNextAfter(FloorDefinition current)
    {
        var i = IndexOf(current);
        if (i < 0) return null;
        var ni = i + 1;
        return ni < orderedFloors.Count ? orderedFloors[ni] : null;
    }

    public int TotalAreasCount()
    {
        int n = 0;
        foreach (var f in orderedFloors)
            if (f != null && f.OrderedAreas != null) n += f.OrderedAreas.Count;
        return n;
    }

    public IEnumerable<AreaDefinition> EnumerateAllAreas()
    {
        foreach (var f in orderedFloors)
            if (f != null && f.OrderedAreas != null)
                foreach (var a in f.OrderedAreas)
                    if (a != null) yield return a;
    }
}
