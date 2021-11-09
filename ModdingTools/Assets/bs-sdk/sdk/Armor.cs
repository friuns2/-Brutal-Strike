using System;
using ObscuredFloat = System.Single;using ObscuredBool = System.Boolean;using ObscuredInt = System.Int32;using ObscuredVector3 =UnityEngine.Vector3;using ObscuredQuaternion =UnityEngine.Quaternion;using ObscuredString  = System.String;
using UnityEngine;

public class Armor : GunBase, IPassive
{

    public AudioClip2 hitSound;
    public ObscuredFloat shield = .5f;
    public HumanBodyBones[] body = new HumanBodyBones[0];
#if game
    public ObscuredFloat defLife { get { return secondaryCountDef; } }
    internal ObscuredFloat life { get { return info.secondaryCount; } set { info.secondaryCount =value; } }
    public override GunInfo CreateInfo()
    {
        return new ArmorGunInfo(this);
    }
    //public override void OnBuy()
    //{
    //    life = defLife;
    //    bought++;
    //    count = 1;
    //    //base.OnBuy();
    //}
    //public override void OnReset()
    //{
    //    base.OnReset();
    //    if (have)
    //        life = defLife;
    //}
    [PunRPC]
    public void SetLife(float setLife)
    {
        pl.armorSoundPlay = TimeCached.time;
        pl.PlaySoundLoud(hitSound,.3f);
        if (life > 0 && setLife <= 0)
        {
            info.count--;
            OnCountChangedRefresh();
        }
        else
        {
            life = setLife;
            // if (life <= 0)
            //     life = 0;
        }

    }

    public float RPCDamage(float damage, GunBase wep)
    {
        
        if (life > 0)
        {
            CallRPC(SetLife, life - Mathf.Abs(damage));
            return damage * Mathf.Max(1f - shield / wep?.WeaponArmorRatio??1, 0);
        }
        return damage;
    }

    public override string CompareText(GunBase gun,WeaponPickable pck)
    {
        var s = base.CompareText(gun,pck);
        var wep = gun as Armor;
        if (!wep) return s;

//        s = Append(s, life, wep.life, "Armor");
        s = Append(s, shield,wep.shield, "Shield");
        return s;
    }
#endif
    
}