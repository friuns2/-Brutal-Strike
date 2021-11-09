using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipCollection", menuName = "Create/AudioClipCollection", order = 1)]
public class AudioClipCollection :ScriptableObject
{
    
    public AudioClip[] play;
}