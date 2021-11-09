using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class SaveLightmap:MonoBehaviour
{
    public int index=-1;
    public Vector4 scaleOffset;
    public Texture2D lightmapColor;
    public Texture2D shadowMask;
    public Texture2D dir;
    [ContextMenu("Save lm")]
    public void Save()
    {
        if (!gameObject.scene.isLoaded) return;
        var r = GetComponent<Renderer>();
        index = r.lightmapIndex;
        scaleOffset = r.lightmapScaleOffset;
        // Debug.Log(index);
        lightmapColor = LightmapSettings.lightmaps[index].lightmapColor;
        shadowMask = LightmapSettings.lightmaps[index].shadowMask;
        dir = LightmapSettings.lightmaps[index].lightmapDir;
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
    }
    
    public void Awake()
    {
        if(index == -1)
            Save();
        LoadLightmap();
    }
    public void LoadLightmap()
    {
        var r = GetComponent<Renderer>();
        r.lightmapIndex = index;
        r.lightmapScaleOffset = scaleOffset;
//            LightmapSettings.lightmaps.IndexOf(a => a.lightmapColor == lightmapColor);
    }
    public void OnDestroy()
    {
        var r = GetComponent<Renderer>();
        r.lightmapIndex = -1;

    }
}

