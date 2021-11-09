using System.Collections.Generic;
using System.Linq;
using ObscuredFloat = System.Single;using ObscuredBool = System.Boolean;using ObscuredInt = System.Int32;using ObscuredVector3 =UnityEngine.Vector3;using ObscuredQuaternion =UnityEngine.Quaternion;using ObscuredString  = System.String;
using UnityEngine;
using UnityEngine.Serialization;



public class Attachment : AttachmentBase
{
    [Header("       Attachment settings")]
    [FieldAtrStart]
    public int order;
    //public ObscuredFloat inAccuracyFactor = 1;
    public AttachmentSlots attachmentSlot;
    public AttachmentType attachment;
    public AudioClip2 sound;
    public Vector3 gripScale=Vector3.one;
    //public SubsetList<WeaponType> weaponTypes = new SubsetList<WeaponType>(Enum<WeaponType>.values);
    [FieldAtrEnd]
    protected bool tmp;

    public List<GunBase> AttachedTo = new List<GunBase>();
    #if game
    public override void OnPlConnected(PhotonPlayer photonPlayer)
    {
        base.OnPlConnected(photonPlayer);
        if (customServer) return;
        if (AttachedTo.Count > 0)
            RpcSyncAttachments();
    }

    public override void OnTake(GunInfo info)
    {
        base.OnTake(info);
        for (var i = -1; i < pl.guns.Count; i++)
        {
            var gun = i == -1 ? pl.curGun : pl.guns[i];
            if (gun is Weapon w && w.have && w.AttachmentAvailable(this) && !w.GetAttachmentAtSlot(attachmentSlot))
            {
                w.AddAttachment(this);
                break;
            }
        }
    }

    public void RpcSyncAttachments()
    {
        var ids = TempList<int>.GetTempList();
        foreach (var a in AttachedTo)
            ids.Add(a.id);

        CallRPC(SetAttachments, ids.ToArray());
    }
    public override void Reset()
    {
        AttachedTo.Clear();
        base.Reset();
    }
    [PunRPCBuffered]
    public void SetAttachments(int[] ids)
    {
        AttachedTo.Clear();
        foreach (var id in ids)
            foreach (var g in pl.guns)
                if (g.id == id)
                    AttachedTo.Add(g);
        OnCountChangedRefresh();

    }
    //[PunRPC]
    //public override void SetCount(int Count)
    //{
    //    if (AttachedTo.Count > Count)
    //        AttachedTo.Clear();
    //    base.SetCount(Count);
    //}
    #endif    
}
