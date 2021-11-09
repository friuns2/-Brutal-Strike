using UnityEngine;

public class LightLamp:ItemBase
{
    
#if game
    public override void Start()
    {
        LoaderSettingsPrefs.onQualityChanged.AddListener(OnQualityChanged);
        OnQualityChanged();
        base.Start();
    }
    private void OnQualityChanged()
    {
        light.enabled = userSettings.qualityLevel > QualityLevel.Medium;
    }

    public override void OnDestroy()
    {
        
        base.OnDestroy();
        LoaderSettingsPrefs.onQualityChanged.RemoveListener(OnQualityChanged);
        
    }   
    
    public override void Load(BinaryReader br)
    {
        base.Load(br);
        light.color = br.ReadColor();
    }
    public override void Save(BinaryWriter bw)
    {
        base.Save(bw);
        bw.Write(light.color);
    }
    
#endif
}