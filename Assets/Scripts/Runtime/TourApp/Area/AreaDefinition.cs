using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

    [Header("Content References")]
    [Tooltip("Text containing area description")]
    /// <summary>Text containing the area's description.</summary>
    [SerializeField] private string areaText;

    [Tooltip("List of audio clips for the area")]
    /// <summary>List of audio clips associated with the area.</summary>
    [SerializeField] private List<AudioClip> audioClips;

    [Tooltip("Image file representing the area (for reference only)")]
    /// <summary>Image in the Resources folder.</summary>
    [SerializeField] private Texture2D areaImage;

    // Public properties to access private fields
    /// <summary>Gets the display name of the area.</summary>
    public string AreaName => areaName;

    /// <summary>Gets the text asset containing the area's description.</summary>
    public string AreaText => areaText;

    /// <summary>Gets the list of audio clips associated with the area.</summary>
    public IReadOnlyList<AudioClip> AudioClips => audioClips;

    /// <summary>Gets the image representing the area.</summary>
    public Texture2D AreaImage => areaImage;

    private void OnValidate()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || BuildPipeline.isBuildingPlayer)
            return;

        if (string.IsNullOrWhiteSpace(areaName))
        {
            Debug.LogError($"[AreaDefinition] Area name is empty in asset '{name}'", this);
        }
        if (string.IsNullOrWhiteSpace(areaText))
        {
            Debug.LogWarning($"[AreaDefinition] Area text is empty in area '{areaName}'. Ignore if not needed.", this);
        }
        if (audioClips == null || audioClips.Count == 0)
        {
            Debug.LogWarning($"[AreaDefinition] No audio clips assigned for area '{areaName}'. Ignore if not needed.", this);
        }
        else
        {
            for (int i = 0; i < audioClips.Count; i++)
            {
                if (audioClips[i] == null)
                {
                    Debug.LogError($"[AreaDefinition] Null audio clip at index {i} in area '{areaName}'", this);
                }
            }
        }
        if (areaImage == null)
        {
            Debug.LogWarning($"[AreaDefinition] No area image assigned for area '{areaName}'. Ignore if not needed.", this);
        }
    }
}
