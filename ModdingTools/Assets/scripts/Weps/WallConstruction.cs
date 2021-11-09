using UnityEngine;


public class WallConstruction : Wall
{
    #if game
    TerrainAligner ta { get { return gameObject.Component<TerrainAligner>(); } }
    public Collider[] col;//obsolete
    public override void Start()
    {
        if(!gameLoaded)
            return;
        
        if (!_LevelEditor.isActiveAndEnabled)
            if (terrainAlign && levelEditorCreated)
                ta.Execute();
            else
                terrainAlign = ta.enabled = false;
        base.Start();
    }
    public override void OnStopDrag()
    {
        if (terrainAlign)
            ta.Execute();
    }
    public override void OnStartDrag()
    {
        if (terrainAlign)
            ta.Flush();
    }
    public override void OnLoadAsset()
    {
//        ta.col = col;
        ta.OnLoadAsset();
        base.OnLoadAsset();
    }
    public bool terrainAlign = true;
    public override void Load(BinaryReader br)
    {
        base.Load(br);
        terrainAlign = br.ReadBoolean();
        if (terrainAlign)
            ta.terrainAlignOffset = br.ReadSingle();
    }
    public override void Save(BinaryWriter bw)
    {
        base.Save(bw);
        bw.Write(terrainAlign);
        if (terrainAlign)
            bw.Write(ta.terrainAlignOffset);
    }

    public override void OnLevelEditorGUI()
    {
        base.OnLevelEditorGUI();

        if (Toggle(ref terrainAlign, "terrain Align") || terrainAlign && Button("Refresh"))
            if (terrainAlign)
                ta.Execute();
            else
                ta.Flush();
        if (terrainAlign)
            ta.terrainAlignOffset = FloatField("terrain Align Offset", ta.terrainAlignOffset);
    }
    //public override void OnLoadAsset()
    //{
    //    base.OnLoadAsset();
    //    var mc = GetComponentInChildren<MeshCombiner>(true);
    //    if (mc)
    //    {
    //        mc.searchOptions.onlyActive=false;
    //        mc.combineOnStart = false;
    //        mc.CombineAll();
    //    }
    //}
//    private RandomPickable[] m_pickables;
//    private RandomPickable[] pickables { get { return GetComponentsInChildren<RandomPickable>(); } }

#endif
}