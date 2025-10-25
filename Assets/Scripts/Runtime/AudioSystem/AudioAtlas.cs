using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioAtlas", menuName = "AR Audio/Audio Atlas")]
public sealed class AudioAtlas : ScriptableObject
{
    public AudioClip tourStart;
    public AudioClip floorReady;
    public AudioClip connecting;
    public AudioClip connectionLost;
    public AudioClip navigating; // short cue, not a loop
    public AudioClip settingsPreview;
    public AudioClip tourComplete;
}
