using System.Collections.Generic;
using UnityEngine;

// Allows creation of AreaDefinition assets via the Unity Editor
[CreateAssetMenu(fileName = "NewAreaDefinition", menuName = "AR Tour/Area Definition")]
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

    [Tooltip("Audio clip for the area")]
    /// <summary>Audio clip associated with the area.</summary>
    [SerializeField]
    private AudioClip audioClip;

    [Tooltip("Image file representing the area (for reference only)")]
    /// <summary>Image in the Resources folder.</summary>
    [SerializeField]
    private Sprite areaImage;

    [Header("UI Configuration")]
    [Tooltip("Icon representing the area in the UI")]
    [SerializeField]
    private Sprite areaIcon;

    [Tooltip("Whether to show area info in the UI")]
    [SerializeField]
    private bool showInfo = true;

    // Public properties to access private fields
    /// <summary>Gets the display name of the area.</summary>
    public string AreaName => areaName;

    /// <summary>Gets the text asset containing the area's description.</summary>
    public string AreaText => areaText;

    /// <summary>Gets the audio clip associated with the area.</summary>
    public AudioClip AudioClip => audioClip;

    /// <summary>Gets the image representing the area.</summary>
    public Sprite AreaImage => areaImage;

    /// <summary>Indicates whether to show area info in the UI.</summary>
    public bool ShowInfo => showInfo;

    /// <summary>Gets the icon representing the area in the UI.</summary>
    public Sprite AreaIcon => areaIcon;
}
