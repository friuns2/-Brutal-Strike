using UnityEngine;

public class PhysxGunObj:bs,IOnStartGame
{
    public PhysxGun gun;
    
    public Rigidbody rg;
#if game
    
    public override void Awake()
    {
        base.Awake();
        Register(this, true);
        Register<IOnStartGame>(this, true);
        lastTime = Time.time;
    }
    internal float lastTime;
    public float lastAttack;
    public override void OnDestroy()
    {
        base.OnDestroy();
        Register(this, false);
        Register<IOnStartGame>(this, false);
    }
    
    
    public void OnStartGame()
    {
        Destroy(gameObject);
    }
    private void OnBecameInvisible()
    {
        if (Time.time - lastTime > 10)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        lastTime = Time.time;
    }
    private static ContactPoint[] contacts = new ContactPoint[3];
    public float createTime;
    private void OnCollisionStay(Collision h)
    {
        
        var cnt = h.GetContacts(contacts);
        if (h.rigidbody == null && (qualityLevel < (AndroidDevice ? QualityLevel.VeryHigh : QualityLevel.Medium)))
            return;

        var force = h.relativeVelocity.magnitude;
        if (force > 6 )
            for (int i = 0; i < cnt; i++)
            {
                _Game.EmitParticles(contacts[i].point, contacts[i].normal, h.rigidbody == null ? _Game.res.dust : _Game.res.spark, maxParticlesPerSecond: 10);
                if (Time.time-gun.lastPlayTime  > .04f)
                PlayClipAtPoint(gun.metalHit,h.contacts[i].point,force/10);
                gun.lastPlayTime = Time.time;
                break;
            }
    }
#endif

}