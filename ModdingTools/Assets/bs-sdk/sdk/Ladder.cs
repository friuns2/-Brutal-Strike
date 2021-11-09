using UnityEngine;


public class Ladder : ItemBase, IOnPlayerEnter
{
//    public override void Load()
//    {
//        base.Load();
//        foreach (var a in GetComponentsInChildren<Collider>())
//        {
//            a.gameObject.layer = Layer.trigger;
//            a.enabled = true;
//        }
//    }
#if game
    public override void Start()
    {
        base.Start();
        InitTriggers();
    }

    public void OnPlayerEnter(Player pl, Trigger other, bool b)
    {
        //print("LadderOnTriggerEnter:");
        if(pl.IsMine)
            pl.RPCSetClimb(b);
    }
    
#endif
    
}

