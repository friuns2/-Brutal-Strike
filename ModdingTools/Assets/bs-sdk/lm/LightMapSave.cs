using System;
using System.Linq;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UnityEngine;


[ExecuteInEditMode]
public class LightMapSave:MonoBehaviour,IOnInspectorGUI
{
    public Texture2D[] lms = new Texture2D[0];
    [ContextMenu("Awake")]
    public void Awake()
    {
        if (lms.Length > 0)
            Set(lms);
    }
    public void Set(Texture2D[] texture2Ds)
    {
        lms = texture2Ds;
        LightmapSettings.lightmaps = texture2Ds.Select(a => new LightmapData() {lightmapColor = a}).ToArray();
        LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional; 
        var mrs = FindObjectsOfType<MeshRenderer>();
        foreach(var mr in mrs)
        if (mr.lightmapIndex != -1)
        {
            var s = mr.GetComponent<SaveLightmap>();
            try
            {
                if (!s) s = mr.gameObject.AddComponent<SaveLightmap>();
                else
                    s.Save();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
    public void OnInspectorGUI()
    {
#if UNITY_EDITOR 
        LightmapData[] lightmapDatas = LightmapSettings.lightmaps;
        foreach (LightmapData a in lightmapDatas)
        {
            a.lightmapColor =(Texture2D) EditorGUILayout.ObjectField(a.lightmapColor,typeof(Texture2D));
            
        }
        LightmapSettings.lightmaps = lightmapDatas;
#endif
    }
}