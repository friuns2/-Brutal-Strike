#define Final
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
#if game
public interface IPointable
{
    void OnPoint(Player pl);
}
#endif
public class Glass : Destructable
{
    public GameObject dead;
    public AudioClip2 clip2;
    
#if game
    public override void Start()
    {
        base.Start();
        
    }
        
    private void CreateDestruction(Vector3 hitPos)
    {
        var cl = GetComponentInChildren<Collider>();
        if (!cl) return;
        var sz = cl.bounds.size;
        if (sz.magnitude > .3f && CheckRayOrNearAndroid(pos) )
        {
            if (dead != null)
            {
                var g = InstantiateAddRot(dead, transform.position, Quaternion.identity);
                Destroy(g, 3);
                g.transform.localScale = sz;

                foreach (var rigidbody in g.GetComponentsInChildren<Rigidbody>())
                    if (rigidbody != null)
                    {
                        rigidbody.AddExplosionForce(50 * forceFix, hitPos, 1);
                        rigidbody.GetComponent<Renderer>().material = cl.GetComponent<Renderer>().sharedMaterial;
                    }
            }
            PlayClipAtPoint(clip2, pos);
        }
    }
#endif
}
//public interface IReset
//{
//    void OnReset();
//}