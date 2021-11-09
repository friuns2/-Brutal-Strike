using System;
using System.Collections.Generic;
using EnumsNET;
using UnityEngine;

public class SkinTest : bs
{
    public SkinnedMeshRenderer original;
    public SkinnedMeshRenderer skin;
    [ContextMenu("CopySkin")]
    public void Start()
    {
        // sk2.sharedMesh.bindposes = sk1.sharedMesh.bindposes;
        // sk1.bones = sk2.bones;
        // ReplaceSkin(sk1.transform.GetTransforms().ToArray(),sk2);
        
        // RenameBones(sk1.transform.root.gameObject);
        // RenameBones(sk2.transform.root.gameObject);
        
        // var d =AssetBundle.LoadFromFile(@"C:\Users\friuns\Downloads\2F67D985C100B55E4A8EA2DACCC6B12A@unity3dwindows");
        // var g = d.LoadAllAssets<Object>().FirstOrDefault();
        // var r = ((GameObject) g).GetComponentInChildren<SkinnedMeshRenderer>();
        SkinReplacer.ReplaceSkin(original,skin);
        
    }
    
    public void RenameBones(GameObject g)
    {
        var component = g.GetComponentInChildren<Animator>();
        foreach (HumanBodyBones a in Enums.GetValues<HumanBodyBones>())
        {   
            Debug.Log(a);
            if (a != HumanBodyBones.LastBone)
            {
                var d = component.GetBoneTransform(a);
                if (d != null)
                    d.gameObject.name = a.ToString();
            }
        }
        
        
    }
    
}
public static class SkinReplacer
{
    public static Dictionary<ValueTuple<Renderer, Renderer>, Transform[]> boneCache = new Dictionary<ValueTuple<Renderer, Renderer>, Transform[]>();

    public static void ReplaceSkin(SkinnedMeshRenderer src, SkinnedMeshRenderer to)
    {
        var key = (src, to);
        if (!boneCache.TryGetValue(key, out Transform[] toBones))
        {
            toBones = to.bones;
            Transform[] srcBones = src.transform.parent.GetComponentsInChildren<Transform>(true);
            bool error=false;
            Debug2.Log("Applying skin from " + src.transform.parent.name + " to " + to.transform.parent.name);
            for (int i = 0; i < to.bones.Length; i++)
            {
                string name = to.bones[i].name;
                // if (name == "Bone01") name = "Mouth";
                if (name == "-- L Forearm twist") name = "Bip01 L Forearm";
                if (name == "-- R forearm twist") name = "Bip01 R Forearm";
                
                toBones[i] = srcBones.FirstOrDefault(a => a.name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (toBones[i] == null)
                {
                    if (PhotonNetwork.playerList.Length < 2 || Application.isEditor) Debug.LogError(" bone " + name + " notFound, please remove or rename it");
                    error = true;
                }
            }
            if (error)
            {
                if (Application.isEditor)
                {
                    var g = bs.Instantiate(to.transform.root);
                    g.gameObject.SetActive(false);
                }
                toBones = boneCache[key] = null;
                return;
            }
            boneCache[key] = toBones;
        }
        if (toBones == null) return;
        src.bones = toBones;
        src.sharedMesh = Application.isEditor ? bs.Instantiate(to.sharedMesh) : to.sharedMesh;
        src.materials =  to.GetSharedMaterials();
    }
}