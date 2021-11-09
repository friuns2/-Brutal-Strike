using UnityEngine;

public class UnbiasLodGroup:QualityControllerBase{
    #if game
    public override void Awake()
    {
        base.Awake();
       lg = GetComponent<LODGroup>();
        oldLods = lg.GetLODs();

    }
    private LODGroup lg ;
    private LOD[] oldLods;
    public override void OnQualityChanged()
    {
        base.OnQualityChanged();
        var lods = (LOD[])oldLods.Clone();
        for (int i = 0; i < lods.Length - 1; i++)
            lods[i].screenRelativeTransitionHeight *= QualitySettings.lodBias;
        lg.SetLODs(lods);
    }
#endif
}