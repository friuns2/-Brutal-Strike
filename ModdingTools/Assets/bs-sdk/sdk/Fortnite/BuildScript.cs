using System.Collections.Generic;
using doru.MeshTest;
using UnityEngine;

public class BuildScript : Destructable
{
    public Shader shader;
    public float placementValue = 100;
    public Material[][] Materials;
    public List<MeshTest> meshTests = new List<MeshTest>();
    public bool inited;
    public float speed = 12;
    #if game
    [ContextMenu("Execute")]
    public override void Start()
    {
        base.Start();
        if (inited) return;
        inited = true;
        // renderers = GetComponentsInChildren<Renderer>();
        Materials = new Material[renderers.Length][];
        for (var i = 0; i < renderers.Length; i++)
        {
            var renderer = renderers[i];
            var meshTest = renderer.Component<MeshTest>();
            meshTests.Add(meshTest);
            meshTest.Awake2();
            Materials[i] = renderer.sharedMaterials;
            foreach (var b in renderer.materials)
                b.shader = shader;
        }
        UpdateValues();
    }
    // public override void OnInstanciate(ItemBase prefab)
    // {
    //     // meshTests = (prefab as BuildScript).meshTests;
    //     Materials =(prefab as BuildScript).Materials;
    //     base.OnInstanciate(prefab);
    // }
    public void Break()
    {
        if (qualityLevelAndroid > QualityLevel.Low)
            foreach (var a in meshTests)
                a.Break();
            // foreach (var b in Materials)
            // foreach (var c in b)
                // c.shader = shader;
            // enabled = break2 = true;
    }
    // public override void OnLoadAsset()
    // {
    //     base.OnLoadAsset();
    //     Start2();
    // }
    public override void OnValidate()
    {
        base.OnValidate();
        if (!shader)
            shader = Shader.Find("BitshiftProgrammer/SurfaceFortnite");
//        UpdateValues();
    }
    private void UpdateValues()
    {
        if (renderers == null) return;
        foreach (var a in renderers)
        foreach (var b in a.materials)
        {
            b.SetFloat("_Placement", Mathf.Max(0, placementValue));
        }
    }
    // public bool linear;
    void Update()
    {
        // if(linear)
        
            placementValue -= Time.unscaledDeltaTime * speed;
        // else
            // placementValue = Mathf.Lerp(placementValue, 0, Time.deltaTime * speed*.01f);
        UpdateValues();
        if (placementValue < .1f)
        {
            enabled = false;
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].sharedMaterials = Materials[i];
        }
    }
#endif
}