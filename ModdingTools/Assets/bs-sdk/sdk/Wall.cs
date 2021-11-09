using UnityEngine;

public partial class Wall : ItemBase
{
    [FieldAtr]
    public float density = 1;
    public bool noColliders;

#if game
    public override void OnLoadAsset()
    {
        foreach (var a in GetComponentsInChildren<Collider>())
            if (a.gameObject.layer == Layer.def)
                a.gameObject.layer = a.isTrigger?Layer.trigger:Layer.level;

        base.OnLoadAsset();
    }
    public override void Save(BinaryWriter bw)
    {
        bw.Write(noColliders);
        bw.Write(offset);
        base.Save(bw);
    }
    public override void Load(BinaryReader br)
    {
        noColliders = br.ReadBoolean();        
        offset = br.ReadVector3();
        base.Load(br);
    }

    public override void Load()
    {
        base.Load();
        if (noColliders)
            foreach (var a in GetComponentsInChildren<Collider>())
                a.enabled = false;


    }
    public override void OnReset()
    {

    }
#endif
}