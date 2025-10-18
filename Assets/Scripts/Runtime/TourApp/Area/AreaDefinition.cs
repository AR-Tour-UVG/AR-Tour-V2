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
    [SerializeField]
    private string areaName;

    [Header("Content References")]
    [Tooltip("Text containing area description")]
    /// <summary>Text containing the area's description.</summary>
    [SerializeField]
    private string areaText;

    [Tooltip("List of audio clips for the area")]
    /// <summary>List of audio clips associated with the area.</summary>
    [SerializeField]
    private List<AudioClip> audioClips;

    [Tooltip("Image file representing the area (for reference only)")]
    /// <summary>Image in the Resources folder.</summary>
    [SerializeField]
    private Texture2D areaImage;

    // Public properties to access private fields
    /// <summary>Gets the display name of the area.</summary>
    public string AreaName => areaName;

    /// <summary>Gets the text asset containing the area's description.</summary>
    public string AreaText => areaText;

    /// <summary>Gets the list of audio clips associated with the area.</summary>
    public IReadOnlyList<AudioClip> AudioClips => audioClips;

    /// <summary>Gets the image representing the area.</summary>
    public Texture2D AreaImage => areaImage;
}
