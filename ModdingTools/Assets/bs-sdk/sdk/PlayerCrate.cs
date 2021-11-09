



using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCrate : WeaponShopBase,IOnPlayerStay
{
    #if game
    public override void Start()
    {
        base.Start();
        DestroyAfter(); 
    }
    protected override void OnCreate(bool b)
    {
        base.OnCreate(b);
        RegisterMaxLimit(this, b, 2);
    }
    
    
    public void OnPlayerStay(Player pl, Trigger other)
    {
        // if (other.Input2.GetKeyDown(KeyCode.F, null, "Loot"))
        // {
        //     foreach (KeyValuePair<int, GunInfo> a in weapons.ToArray())
        //     {
        //         if(other.gunsDict[a.Key].canTake)
        //             CallRPC(Buy, other.viewId, a.Key);
        //     }
        // }
    }
 #endif   
}