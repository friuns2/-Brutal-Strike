using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public interface IEvent
{

}
public interface IOnControllerColliderHit : IEvent
{
    void OnControllerColliderHit(ControllerColliderHit hit);
    void OnCollisionEnter(Collision hit);

}


public class ForwardEvents : MonoBehaviour
{
    public Transform target;
    public bool autoFillTarget = true;
    #if game
    public List<IOnPostRender> IOnPostRender = new List<IOnPostRender>(2);
    public List<IOnAnimatorIK> IOnAnimatorIK = new List<IOnAnimatorIK>(2);
    public List<IOnControllerColliderHit> IOnControllerColliderHit = new List<IOnControllerColliderHit>(2);
    List<IEvent> IEvents = new List<IEvent>();
    
    public void Start()
    {
        Reset();
        if (!transform.parent && !target) return;
        if (target)
            target.GetComponents(IEvents);
        else if (autoFillTarget)
            transform.parent.GetComponentsInParent(true, IEvents);


        foreach (var IEvent in IEvents)
        {
            IOnPostRender.AddIfNotNull(IEvent as IOnPostRender);
            IOnControllerColliderHit.AddIfNotNull(IEvent as IOnControllerColliderHit);
            IOnAnimatorIK.AddIfNotNull(IEvent as IOnAnimatorIK);
        }

        
    }
    public void OnTransformParentChanged()
    {
        Start();
    }

    private void Reset()
    {
        IOnPostRender.Clear();
        IOnAnimatorIK.Clear();
        IOnControllerColliderHit.Clear();
    }
    
    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!enabled) return;
        foreach (var a in IOnControllerColliderHit)
            a.OnControllerColliderHit(hit);
    }
    // public void OnAnimatorIK(int layer)
    // {
    //     if (!enabled) return;
    //     foreach (var a in IOnAnimatorIK)
    //         a.OnAnimatorIK(layer);
    // }
    public void OnPreRender()
    {
        if (!enabled) return;
        foreach (var a in IOnPostRender)
            a.OnPreRender();
    }
    public void OnPostRender()
    {
        if (!enabled) return;
        foreach (var a in IOnPostRender)
            a.OnPostRender();
    }
    public void OnAnimatorMove() //"unity feature" dont remove
    {
        
    }
    public void SendEvent(string eventName) //for animation events warnings
    {
        
    }
    // void InstantiateEffect(int o)
    // {
    //     instantiateEffect?.Invoke(null);
    // }
    
    
    public void PlayAudio(AudioClip clip)
    {
        target.GetComponent<AudioSource>().PlayOneShot(clip);
    }
    
    
    [ContextMenu("ResetCamera")]
    public void ResetProjectionMatrix()
    {
        GetComponent<Camera>().ResetWorldToCameraMatrix();
        GetComponent<Camera>().ResetCullingMatrix();
        GetComponent<Camera>().ResetProjectionMatrix();
    }
    private void OnCollisionEnter(Collision other)
    {
        foreach (var a in IOnControllerColliderHit)
            a.OnCollisionEnter(other);
    }
#endif   
    public Action<object> instantiateEffect;
    public void PlayAttackSoundSpecial()
    {
        instantiateEffect?.Invoke(EventType2.ScreaSpecial);
    }
    public void PlayAttackSound()
    {
        instantiateEffect?.Invoke(EventType2.Scream);
    }
    public void InstantiateEffect(Object o)
    {
        instantiateEffect?.Invoke(o);
        // target.BroadcastMessage("OnParticleCollision",other);
    }
}
public enum EventType2
{
    None,
    Scream,
    ScreaSpecial
}
