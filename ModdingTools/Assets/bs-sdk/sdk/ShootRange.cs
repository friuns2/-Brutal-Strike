// using System;
// using UnityEngine;
//
// public class ShootRange : Destructable
// {
//     public int hitCount;
//     public float averageAccuracy;
//     public float averageDamage;
//     #if game
//     private float hitTime=-100;
//     public override void OnStartGame()
//     {
//         Destroy(gameObject);
//     }
//     public Transform center;
//     
//     public override void RPCAddLife(float damage, int pv = -1, int weapon = -1, HumanBodyBones collId = HumanBodyBones.Hips, Vector3 hitPos = new Vector3())
//     {
//         float m = Mathf.Max(0,1f - (center.position.DistanceTo(hitPos) / center.lossyScale.x*2));
//
//         if (pv == _ObsPlayer.viewId)
//         {
//             hitTime = Time.time;
//             hitCount++;
//             averageAccuracy += m;
//             averageDamage = damage;
//         }
//         base.RPCAddLife(damage, pv, weapon, collId, hitPos);
//         
//     }
//     
//     public void Update()
//     {
//         if (TimeCached.time - hitTime <2)
//             _Hud.CenterTextUpd("Accuracy " + Mathf.RoundToInt(averageAccuracy / hitCount * 100) + " damage: " + averageDamage);
//         else
//             hitTime = averageAccuracy = averageDamage = 0;
//     }
// //    public override void RPCAddLife(int damage, int pv = -1, int weapon = -1, HumanBodyBones collId = HumanBodyBones.Hips, Vector3 pos = new Vector3())
// //    {
// //        var killedBy = ToObject<Player>(pv);
// //        if (killedBy)
// //            killedBy.CallRPC(killedBy.SetKills, killedBy.Kills + 1);
// //        
// //        base.RPCAddLife(damage, pv, weapon, collId, pos);
// //    }
// #endif
// }