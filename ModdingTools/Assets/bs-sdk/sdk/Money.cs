using UnityEngine;

public class Money : GunBase, IPassive
{
    #if game
    public override void Awake()
    {
        base.Awake();
        pl.money = this;
    }
    public override void Reset()
    {
        
    }
    #endif
}