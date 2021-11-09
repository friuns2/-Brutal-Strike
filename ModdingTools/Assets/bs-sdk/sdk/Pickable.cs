using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Pickable : RespawnableBase,IOnPlayerNear
{
    [FieldAtrStart]
    
    public WeaponShopCollection weapons = new WeaponShopCollection();
    [FieldAtrEnd] private int tmp3423;
    [HideInInspector]
    public WeaponShopCollection weaponsReset = new WeaponShopCollection();
    public bool enableMoney;
    public bool infinite;
    public bool disabled;
    
    #if game
    [PunRPC,ViaServer]
    public void Buy(int plid, int gunId)//old
    {
        var pl = ToObject<Player>(plid);
        
        Buy(plid,gunId,TakeCount(pl.gunsDict[gunId], false));
    }
    public override void OnReset()
    {
        base.OnReset();
        foreach (var a in weaponsReset)
            weapons[a.Key] = weaponsReset[a.Key];
        weaponsReset.Clear();
    }


    [PunRPC,ViaServer]
    public virtual void Buy(int plid, int gunId,int take)
    {
        
        
        var pl = ToObject<Player>(plid);
        
        
        pl.waitingForTake = 0;
        //pl.print("take");
        GunBase gun = pl.gunsDict[gunId];


        
        
        
        GunInfo gunInfo = weapons[gunId];
        if (gunInfo.count == 0)
            return;

        if (!weaponsReset.ContainsKey(gunId))
            weaponsReset[gunId] = weapons[gunId].Clone();
        
        if (pl.observing)
            _Hud.CenterText(t+(enableMoney ? "you bought " : "you took ") + gun.name, 2);

        

        if (infinite)
            weapons.Add(gun,take);


        pl.PlayTakeSound();
        
        gun.OnTake(gunInfo.Take(take)/*new GunInfo() {count = take, secondaryCount = i.secondaryCount != 0 ? i.secondaryCount : (int) gun.secondaryCountDef}*/);

        gun.dropOwner = m_dropOwner ? m_dropOwner : photonView.isSceneView ? null : owner.pl; 
        if (enableMoney)
        {
            pl.inGameMoney -= gun.price;
            gun.bought++;
        }
    }
    public Player m_dropOwner;
    private int TakeCount(GunBase gun,bool autoTake)
    {
        var gunDropAmmount = infinite ? (int) gun.dropAmmount : Mathf.Min(weapons[gun.id].count, gun.dropAmmount);
        var canTakeAmmount = gun.CanTakeAmmount(autoTake);
        return Mathf.Min(canTakeAmmount, gunDropAmmount);
    }

    [ContextMenu("Fill")]
    public void Fill()
    {
        foreach (var a in weapons)
        {
            var gunInfo = a.Value;
            gunInfo.arrayId = a.Key;
            var gun = FindObjectsOfType<GunBase>().FirstOrDefault(b=>b.id == a.Key);
            gunInfo.count = gun.dropAmmount;
            gunInfo.secondaryCount = gun.secondaryCountDef;
        }
    }

    public virtual void AutoTake(Player pl)
    {
        
        foreach (GunBase gun in pl.guns)
        {
            var gunInfo = weapons[gun.id];
            if (gunInfo.count > 0  && !gun.useless && gun.canTake)
            {
                var cnt = TakeCount(gun, true);
                if (cnt>0 && gunInfo.IsBetterThan(null))
                {
                    RPCBuyOrTake(pl, gun);
                    break;
                }
                else if (gun is Armor)
                {
                    GunBase replace = pl.guns.FirstOrDefault(a => a.have && gun.groupNumber == a.groupNumber && gunInfo.IsBetterThan(a.info));
                    if (replace)
                    {
                        replace.RpcDropWeapon();
                        RPCBuyOrTake(pl, gun);
                        break;
                    }
                }
            }
        }
        
        
    }
    public void RPCBuyOrTake(Player pl, GunBase gun)
    {
        
        if (!pl.gunsDict[gun.id].canTake)
            pl.SameGroupGun(gun)?.RpcDropWeapon();

        var takeCount = TakeCount(pl.gunsDict[gun.id], false);
        if (takeCount > 0)
            CallRPC(Buy, pl.viewId, gun.id, takeCount);
    }

    
    // [PunRPC]
    // public void Take(int plid, GunInfo info)
    // {
    //     var pl = ToObject<Player>(plid);
    //     //pl.print("take");
    //     pl.waitingForTake = 0;
    //     if (!enabled)
    //     {
    //         Debug.LogError("disabled");
    //         return;
    //     }
    //
    //     var gun = pl.gunsDict[arrayID];
    //     gun.OnTake(info);
    
    //     
    // }
    //
    //
    //
    // private void RPCTake(Player pl)
    // {
    //     var wep = pl.gunsDict[arrayID];
    //     if (pl.waitingForTake != 0)
    //         Debug.LogError("Wait for take did not reset");
    //
    //     
    //     if (pl.observing)
    //         _Hud.CenterText(Concat("you pickup ", name), 1);
    //     pl.waitingForTake = Time.time;
    //     CallRPC2(Take, PhotonTargets.AllViaServer, pl.viewId, gunInfo.Clone());
    // }
    public virtual bool AvailableTo(Player pl)
    {
        return !disabled;
    }
    
 
    
#endif

    
}