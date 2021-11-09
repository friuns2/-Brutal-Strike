using UnityEngine;

public class RandomAudioPlay:MonoBehaviour
{
    public AudioClip[] audioClip;
    public void OnEnable()
    {
        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip.Random();
        audioSource.Play();
    }
}