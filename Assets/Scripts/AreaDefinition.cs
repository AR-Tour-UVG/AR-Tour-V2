using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAreaDefinition", menuName = "AR-Tour/Area Definition")]
public class AreaDefinition : ScriptableObject
{
    [Header("Area Info")]
    [Tooltip("Name of the area")]
    [SerializeField] private string areaName;
    [Tooltip("Directions to the next area (if applicable)")]
    [SerializeField] private string nextAreaDirections; 
    [Header("Content References")]
    [Tooltip("Text file containing area description")]
    [SerializeField] private TextAsset areaText;          // put your .txt file here
    [Tooltip("List of audio clips for the area")]
    [SerializeField] private List<AudioClip> audioClips;  // drag in clips in desired order
    [Tooltip("Path to the area image in the Resources folder")]
    [SerializeField] private string areaImagePath; // path to image in Resources folder


    // Properties for read-only access
    public string AreaName => areaName;
    public string NextAreaDirections => nextAreaDirections;
    public TextAsset AreaText => areaText;
    public IReadOnlyList<AudioClip> AudioClips => audioClips;
    public string AreaImagePath => areaImagePath;
}
