using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class SmokeGrenadeBullet : GrenadeBullet
{
    [FormerlySerializedAs("trigger")] 
    public Collider botVisibility;
    public AudioClip2 smokeSound;
    #if game
    public override void Awake()
    {
        base.Awake();
        explosion.gameObject.SetActive(false);
    }

    public override void Explode()
    {
        
//        enabled = false;
        explosion.gameObject.SetActive(true);
//        explosion.transform.parent = null;
        
        rigidbody.angularDrag = 100;

        PlayClipAtPoint(smokeSound, pos, 1);
        StartCoroutine(StartCor());
    }
    
    public void LateUpdate()
    {
        explosion.transform.rotation = Quaternion.identity;
    }
    
    private IEnumerator StartCor()
    {
        yield return new WaitForSeconds(8);
        botVisibility.transform.parent = null;
        botVisibility.transform.rotation = Quaternion.identity;
        botVisibility.enabled = true;
        yield return new WaitForSeconds(40);
        enabled = botVisibility.enabled = false;
        
        
    }
#endif
}