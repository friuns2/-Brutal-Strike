using System.Collections.Generic;
using System.Linq;
#if game
using Cysharp.Threading.Tasks;
#endif
using UnityEngine;

public class PhysxGun : GunBase
{
    public GameObject cubePrefab;
    public AudioClip2 metalHit;
    public AudioClip2 shoot;
    public AudioClip2 start;
    internal  float lastPlayTime;
    public float randomScale;
    public int maxItems = 20;
    public float spawnInterval = .5f;
    // public List<GameObject> addMeshCollider = new List<GameObject>();
    public AudioClip gravityGun;
    
    #if game
    public static FastList2<PhysxGunObj> cubes => GetInstances<PhysxGunObj>();

    private Animation handsAnimation ;
    
    public override void OnSelectGun(bool selected)
    {
        base.OnSelectGun(selected);
        if (!handsAnimation)
            handsAnimation = Hands.GetComponentInChildren<Animation>();
    }
    public override void OnLoadAsset()
    {

        foreach (Transform a in cubePrefab.transform.GetTransforms())
        {
            a.transform.localScale = Vector3.one / Mathf.Sqrt(a.GetComponent<Renderer>().bounds.size.magnitude);
            var physxGunObj = a.gameObject.Component<PhysxGunObj>();
            physxGunObj.rg = a.GetComponent<Rigidbody>();
            trs.Add(physxGunObj);
        }
        base.OnLoadAsset();
    }
    public  List<PhysxGunObj> trs = new List<PhysxGunObj>();
    private bool oldMouseButton;
    public override void Update2()
    {
        base.Update2();
        if (pl.deadOrKnocked) return;
        
        

        for (var i = cubes.Count - 1; i >= 0; i--)
        {
            var a = cubes[i];
            if (a.position.y < _Game.bounds.min.y)
                Destroy(a.gameObject);
        }


        var mouseButton = pl.InputGetKey(KeyCode.Mouse0) && TimeCached.time - shootTime > .5f;
        var mouseUp = oldMouseButton && !mouseButton;
        var mouseDown = !oldMouseButton && mouseButton;
        oldMouseButton = mouseButton;

        if (mouseButton)
        {
            pl.weaponAudioSource.pitch = Mathf.Min(pl.weaponAudioSource.pitch + Time.deltaTime / 2, 3);
            // animationState.time += UnityEngine.Random.value;
            // pl.HitTime = Time.time - .1f / 3;    
        }

        if (mouseDown)
        {
            pl.weaponAudioSource.loop = true;
            pl.weaponAudioSource.clip = gravityGun;
            pl.weaponAudioSource.Play();
            PlayAnimation(Anims.pressButton, .1f);
            pl.PlayOneShot(start);
            if (pl.observing)
                handsAnimation.Play("Start");
        }
        if (mouseUp)
        {
            pl.PlayOneShot(shoot);    
            pl.weaponAudioSource.Stop();
            pl.weaponAudioSource.pitch = 1;
            
            PlayAnimation(Anims.shoot2, .1f);
            if (pl.observing)
            {
                handsAnimation.Play();
                pl.HitTime = TimeCached.time;
                _ObsCamera.PlayDamageAnim();
                
            }
        }
        if (mouseButton||mouseUp)
        {
            
            if(lastCnt<maxItems && TimeElapsed(spawnInterval))
            // for (int i = 0; i <  maxItems-lastCnt; i++)
            {
                var normalized = Random.insideUnitCircle.normalized;
                normalized.y = Mathf.Max(normalized.y, 0);
                var plPos = pl.pos + pl.rot * normalized * Random.Range(4, 6);
                var cube = Instantiate(trs.Random(), plPos, Random.rotation);
                cube.createTime = TimeCached.time;
                Debug.DrawRay(pl.pos, plPos, Color.red, 10);
                if (randomScale > 0)
                    cube.transform.localScale = Random.insideUnitSphere * randomScale;
            }
            
            int cnt = 1;
            foreach (var a in GetInstances<PlayerSkin>())
            {
                if (a.isRagdoll)
                {
                    foreach (var r in a.rigidbodies)
                    {
                        var v = (pl.hpos + pl.Cam.forward * ( Mathf.Sqrt(lastCnt))) - r.position;
                        var sqrMagnitude = v.sqrMagnitude;
                        // var magnitude = Mathf.Sqrt(sqrMagnitude);        
                        r.AddForce(v.normalized * (Mathf.Max(200 - (sqrMagnitude * sqrMagnitude) * .1f, 50) * TimeCached.deltaTime * 10));
                    }
                }
            }
            foreach (PhysxGunObj o in cubes)
            {
                Rigidbody r = o.rg;
                var v = (pl.hpos + pl.Cam.forward * Mathf.Max(2, Mathf.Sqrt(lastCnt) )) - r.position;
                var sqrMagnitude = v.sqrMagnitude;
                var magnitude = Mathf.Sqrt(sqrMagnitude);

                var mass = r.mass;
                if (mass > 4 || magnitude > 25) continue;
                o.lastTime = TimeCached.time;
                if (magnitude < 10)
                {
                    cnt++;
                    r.detectCollisions = !(magnitude > 5 && r.velocity.magnitude < 2) || mass > 4;
                    o.gun = this;
                }

                if (mouseUp)
                {
                    shootTime = TimeCached.time;
                    if (magnitude < 3)
                    {
                        o.gun = this;
                        r.mass = 10;
                        o.enabled = true;
                        r.AddForce(pl.Cam.forward * 300, ForceMode.Impulse);
                    }
                }
                else
                {
                    r.velocity = Vector3.ClampMagnitude(r.velocity, 1 + magnitude + sqrMagnitude * 5);
                    r.AddForce(v.normalized * (Mathf.Max(200 - (sqrMagnitude * sqrMagnitude) * .1f, 50) * Time.deltaTime * 50));
                }
                // a.AddExplosionForce(explosionForce, transform.forward*10, radius);
            }
            lastCnt = cnt;
        }
        
        
        
    }
    private float shootTime;
    private int lastCnt;
#endif
}