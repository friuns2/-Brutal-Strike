using UnityEngine;

public interface IOnCollisionEnter
{
    void OnCollisionEnter(Collision other);
}
public class ForwardEventsCollisionHit : MonoBehaviour
{
    public MonoBehaviour target;
    public void OnCollisionEnter(Collision other)
    {
        (target as IOnCollisionEnter)?.OnCollisionEnter(other);        
    }
    
    
    // public void OnPlayerEnter(Player pl)
    // {
    //     (target as IOnPlayerEnter)?.OnPlayerEnter(pl);
    // }
    public void Reset()
    {
        target = (MonoBehaviour) (GetComponentInChildren<IOnCollisionEnter>() ?? GetComponentInParent<IOnCollisionEnter>());
    }
}
