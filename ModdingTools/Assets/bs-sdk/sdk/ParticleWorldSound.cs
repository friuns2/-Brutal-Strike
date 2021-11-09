using UnityEngine;

public class ParticleWorldSound : bs
{
    public AudioClip2 clip;
    #if game
    public void OnParticleCollision(GameObject other)
    {
        PlayClipAtPoint(clip, pos);
    }
    #endif
}