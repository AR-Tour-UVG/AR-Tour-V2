// AreaDefinition.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>AreaDefinition</c> defines the structure for an area in the AR tour application.
/// </summary>
[CreateAssetMenu(fileName = "NewAreaDefinition", menuName = "AR-Tour/Area Definition")] // Allows creating new AreaDefinition assets from the Unity menu
public class AreaDefinition : ScriptableObject
{
    // Serialized fields for IN Unity Inspector configuration
    [Header("Area Info")]
    [Tooltip("Name of the area")]
    [SerializeField] private string areaName;
    [Tooltip("Directions to the next area (if applicable)")]
    [SerializeField] private string nextAreaDirections;

    [Header("Content References")]
    [Tooltip("Text file containing area description")]
    [SerializeField] private TextAsset areaText;
    [Tooltip("List of audio clips for the area")]
    [SerializeField] private List<AudioClip> audioClips;
    [Tooltip("Path to the area image in the Resources folder")]
    [SerializeField] private string areaImagePath;


    // Properties for read-only access
    public string AreaName => areaName; // Name of the area
    public string NextAreaDirections => nextAreaDirections; // Directions text to the next area (optional)
    public TextAsset AreaText => areaText; // Text asset for area description
    public IReadOnlyList<AudioClip> AudioClips => audioClips; // List of audio clips for the area
    public string AreaImagePath => areaImagePath; // Path to the area image in Resources
}
