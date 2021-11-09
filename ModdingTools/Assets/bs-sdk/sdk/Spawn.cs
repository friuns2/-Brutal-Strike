using System;
using UnityEngine;

public class Spawn : ItemBase
{
    public TeamEnum team = TeamEnum.DeathMatch;
    #if game
    public PosRot spawn { get { return new PosRot(transform, true) {team = team}; } }
    public new Vector3 pos { get { return base.pos; } set { base.pos = value; } }
    public void OnDrawGizmos()
    {
        // Gizmos.DrawSphere(transform.position, 1);
    }

    protected override void OnCreate(bool b)
    {
        Register(this,b);
        base.OnCreate(b);
    }
#endif
}