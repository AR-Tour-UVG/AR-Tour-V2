using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAreaDefinition", menuName = "Tour/Area Definition")]
public class AreaDefinition : ScriptableObject
{
    [Header("Area Info")]
    [Tooltip("Name of the area")]
    [SerializeField] private string areaName;
    [Header("Content References (Resources folder)")]
    [Tooltip("Text file containing area description")]
    [SerializeField] private TextAsset areaText;          // put your .txt file here
    [Tooltip("List of audio clips for the area")]
    [SerializeField] private List<AudioClip> audioClips;  // drag in clips in desired order

    // Properties for read-only access
    public string AreaName => areaName;
    public TextAsset AreaText => areaText;
    public IReadOnlyList<AudioClip> AudioClips => audioClips;
}
