using System.Collections.Generic;
using UnityEngine;

// Allows creation of AreaDefinition assets via the Unity Editor
[CreateAssetMenu(fileName = "NewAreaDefinition", menuName = "AR-Tour/Area Definition")]
/// <summary>
/// Class <c>AreaDefinition</c> represents data for an area in the AR Tour.
/// </summary>
/// <remarks>
/// - Stores metadata such as the area's name, description, audio clips, and associated image path.
/// - Intended to be created as a ScriptableObject asset.
/// </remarks>
public class AreaDefinition : ScriptableObject
{
    [Header("Area Info")]
    [Tooltip("Name of the area")]
    /// <summary>The name of the area.</summary>
    [SerializeField] private string areaName;

    [Tooltip("Directions to the next area (if applicable)")]
    /// <summary>Directions text to the next area (if applicable).</summary>
    [SerializeField] private string nextAreaDirections;

    [Header("Content References")]
    [Tooltip("Text file containing area description")]
    /// <summary>Text file containing the area's description.</summary>
    [SerializeField] private TextAsset areaText;

    [Tooltip("List of audio clips for the area")]
    /// <summary>List of audio clips associated with the area.</summary>
    [SerializeField] private List<AudioClip> audioClips;
    
    [Tooltip("Path to the area image in the Resources folder")]
    /// <summary>Path to the area image in the Resources folder.</summary>
    [SerializeField] private string areaImagePath;

    // Public properties to access private fields
    /// <summary>Gets the display name of the area.</summary>
    public string AreaName => areaName;

    /// <summary>Gets the directions text leading to the next area.</summary>
    public string NextAreaDirections => nextAreaDirections;

    /// <summary>Gets the text asset containing the area's description.</summary>
    public TextAsset AreaText => areaText;

    /// <summary>Gets the list of audio clips associated with the area.</summary>
    public IReadOnlyList<AudioClip> AudioClips => audioClips;

    /// <summary>Gets the path to the area image in the Resources folder.</summary>
    public string AreaImagePath => areaImagePath;
}
