#define Final
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using ObscuredFloat = System.Single;using ObscuredBool = System.Boolean;
using ObscuredVector3 =UnityEngine.Vector3;using ObscuredQuaternion =UnityEngine.Quaternion;using ObscuredString  = System.String;
// ReSharper disable AccessToModifiedClosure
public class Grenade : WeaponBase
{
    [FieldAtrStart()]
    [Header("       grenade settings")]
    public ObscuredFloat startThrowLength = .5f;
    public ObscuredFloat throwGrenadeLength = .5f;
    public ObscuredFloat Force = 15;
    public ObscuredFloat Up = 3;
    public ObscuredFloat time = 5;
    
    
    public bool detonator;
    [FieldAtrEnd()]
    protected int tmp;

    
    public AudioClip2 grenadeHit;
    public AudioClip2 grenateThrowWarn;
    public GrenadeBullet grenadePrefab;
#if game
    public void OnEnable()
    {
        StartCoroutine2(RegisterUpdate(UpdateCor()),stopOnDisable:true);
    }
    public override void Start()
    {
        if (grenadePrefab.explosion)
            explosionParticle = grenadePrefab.explosion;
        base.Start();
    }
    // public override void Start()
    // {
    //     base.Start();
    //     StartCoroutine2(RegisterUpdate(UpdateCor()));
    // }
    
    IEnumerable UpdateCor()
    {

        if (detonator && pl.MouseButton1)
        {
            RPCPlayAnimation(Anims.shoot2, .1f);
            yield return new WaitForSeconds(startThrowLength);
            foreach (var a in grenadeThrown)
                if (a)
                {
                    a.CallRPC(a.Explode);
                    Count--;
                }
            grenadeThrown.Clear();
            yield break;
        }
        
        if (active &&pl.MouseButtonAny && have && IsMine && Count-grenadeThrown.Count>0)
        {
            RPCPlayAnimation(Anims.startShoot, .1f);
            float timeStart = Time.time;
            yield return new WaitForSeconds(startThrowLength * .7f);
            var grenadeCooking = roomSettings.grenadeCooking && pl.MouseButton1;
            yield return new WaitUntil(() => !pl.MouseButtonAny || grenadeCooking && Time.time - timeStart > time);
            if (!grenadeCooking)
            {
                timeStart = Time.time;
                if (!active) yield break;
            }


            RPCPlayAnimation(Anims.shoot, .1f);
            //pl.PlayOneShot(grenateThrowWarn);
            yield return new WaitForSeconds(throwGrenadeLength *.5f);
            
            if (IsMine)
                RPCThrowGrenade(pl.hpos, cam.forward * Force + cam.up * Up, Time.time - timeStart, PhotonNetwork.AllocateViewID());
            pl.unpressKey.Add(KeyCode.Mouse0);
            pl.unpressKey.Add(KeyCode.Mouse1);

            yield return new WaitForSeconds(throwGrenadeLength);


            if (!detonator)
            {

                if (Count > 0)
                    RPCPlayAnimation(Anims.draw);
                else if (IsMine)
                    pl.LastGun();

                yield return new WaitForSeconds(drawTime);
            }
        }
    }
    public override void OnDisable()
    {
        if (!detonator)
            StopAllCoroutines();
    }
    public override void ResetBullets()
    {
        StopAllCoroutines();
        base.ResetBullets();
    }
    private void RPCThrowGrenade(Vector3 plHpos, Vector3 camForward, float timeStart, int allocateViewID)
    {
        if (room.sets.mpVersion < 18)
            CallRPC(ThrowGrenade, plHpos, camForward, timeStart);
        else
            CallRPC(ThrowGrenade, plHpos, camForward, timeStart, allocateViewID);
    }
    private List<GrenadeBullet> grenadeThrown = new List<GrenadeBullet>();
    [PunRPC]
    public void ThrowGrenade(Vector3 pos, Vector3 v, float offset, int viewID)
    {
        if(_Game.started && !detonator)
            Count -= 1;

        var bullet = InstantiateAddRot(grenadePrefab, pos, Quaternion.identity).Component<GrenadeBullet>();
        if (detonator)
            grenadeThrown.Add(bullet);
        bullet.Component<PhotonView>().viewID = viewID;
        bullet.timeStart = Time.time - offset;
        Physics.IgnoreCollision(bullet.collider, pl.controller.controller);
        bullet.collider.gameObject.layer = Layer.grenade;
        bullet.gun = this; 
        bullet.rigidbody.velocity = v;
        pl.PlayOneShot(grenateThrowWarn);

        foreach (Transform a in bullet.transform.GetTransforms())
            a.position = placeholder.position;
        
    }
    [PunRPC]
    public void ThrowGrenade(Vector3 pos, Vector3 v, float offset)
    {
        ThrowGrenade(pos, v, offset, PhotonNetwork.AllocateViewID());
    }
    [PunRPC]
    public override void SetMouseButton(bool value, Vector3 hpos, int ip, Vector3 dc)
    {
        base.SetMouseButton(value, hpos, ip, dc);
    }

#if UNITY_EDITOR
    public override void ParseAnimEditor(Anims anim, AnimationClip clip)
    {
        if (anim == Anims.startShoot)
            startThrowLength = clip.length;
        if (anim == Anims.shoot)
            throwGrenadeLength = clip.length;
        base.ParseAnimEditor(anim, clip);
    }

    public override void CtxParse2(string key, string value)
    {
        base.CtxParse2(key, value);
        if (key == "Damage")
            damage = int.Parse(value);
    }
#endif

#endif
}