using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
// using Engine.Source;
using Microsoft.MixedReality.Toolkit.LightingTools;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using uSource;
using Debug = UnityEngine.Debug;
using RegistryView = Microsoft.Win32.RegistryView;
// ReSharper disable StringLastIndexOfIsCultureSpecific.1


public class BspLoaderHelper:EditorWindow
{
    
    [MenuItem("Brutal Strike/Bsp Loader")]
    public static void Window()
    {
        var w = GetWindow<BspLoaderHelper>();
        w.Start();
    }
    
    [Serializable]
    public class GamePath
    {
        public string path;
        public List<string> bspPaths = new List<string>();
    }
    public List<GamePath> games = new List<GamePath>();
    
    public void Start()
    {
        games.Clear();
        try
        {
            RegistryKey parentKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall", false);
            string[] nameList = parentKey.GetSubKeyNames();
            for (int i = 0; i < nameList.Length; i++)
            {
                RegistryKey regKey = parentKey.OpenSubKey(nameList[i]);
                var s = regKey?.GetValue("DisplayName")?.ToString();
                if (s != null && s.Contains("counter", StringComparison.OrdinalIgnoreCase) && s.Contains("strike", StringComparison.OrdinalIgnoreCase))
                {
                    var path = Path.GetFullPath(regKey.GetValue("InstallLocation") + "/");
                    var hlExe = path + "hl2.exe";
                    if (File.Exists(hlExe))
                    {
                        Debug.Log("found " + hlExe);
                        if (!games.Any(a => a.path == path))
                            games.Add(new GamePath() {path = path});
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        if (!string.IsNullOrEmpty(customGamePath))
            games.Add(new GamePath() {path = customGamePath});
        foreach (var a in games)
        {
            var mapsPath = a.path + "cstrike/maps/";
            if (Directory.Exists(mapsPath))
            {
                foreach (var f in Directory.GetFiles(mapsPath, "*.bsp"))
                {
                    if (!a.bspPaths.Contains(f))
                        a.bspPaths.Add(f);
                }
            }

            // if (!a.bspPaths.Contains(f))
                //     a.bspPaths.Add(f);
        }
    }
    public string customGamePath { get { return PlayerPrefs.GetString("custom"); } set { PlayerPrefs.SetString("custom", value); } }
    private void OnGUI()
    {

        if (games.Count == 0)
            GUILayout.Label("No Games found, please install CS:S");
        // if(GUILayout.Button("Create Cubemap"))
        //     CreateCubemap("Assets/");
        
        customGamePath = EditorGUILayout.TextField("game path", customGamePath);
        if (GUILayout.Button("Refresh"))
        {
            Start();
        }
        
        foreach (var game in games)
        {
            var gamePath = game.path;
            var gameName = Path.GetFileName(gamePath.TrimEnd('/','\\'));
            if (GUILayout.Button("Init " + gameName))
            {
                StringBuilder sb=new StringBuilder();
                foreach (var file in new[] {"cstrike/cstrike_pak_dir", "hl2/hl2_textures_dir", "hl2/hl2_misc_dir"})
                {
                    sb.AppendLine(Bracket(gamePath + "bin/vpk.exe") + Bracket(gamePath + file + ".vpk"));
                    sb.AppendLine("xcopy" + Bracket(gamePath + file) + Bracket(gamePath + "cstrike") + " /E /K /D /H /Y");

                }
                File.WriteAllText("init.bat",sb.ToString());
                Process.Start(new ProcessStartInfo("init.bat"){Verb = "runas"});
            }


            if (GUILayout.Button("open folder"))
                Process.Start("explorer.exe", gamePath);
            GUI.enabled = Directory.Exists(gamePath + "/cstrike/cstrike_pak_dir");
            
            foreach (var mapPath in game.bspPaths)
                if (GUILayout.Button("Load " + Path.GetFileName(mapPath)))
                {
                    LoadMap(mapPath, gamePath);
                }
            
            GUI.enabled = true;
        }
        
    }
    private void LoadMap(string mapPath, string gamePath)
    {
        var sc = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        var bspName = Path.GetFileNameWithoutExtension(mapPath);
        var scene = "Assets/" + bspName + ".unity";
        EditorSceneManager.SaveScene(sc, scene);
        
        uLoader.MapName = bspName;
        uLoader.RootPath = gamePath;
        uLoader.SaveAssetsToUnity = true;
        uLoader.ParseLights = false;
        uLoader.ParseLightmaps = true;
        // uLoader.BSPPath = mapPath;
        try
        {
            uLoader.DebugTime = new System.Diagnostics.Stopwatch();
            uLoader.DebugTimeOutput = new System.Text.StringBuilder();
            uLoader.DebugTime.Start();

            uLoader.Clear();
            uResourceManager.LoadMap(uLoader.MapName);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        var nav = new GameObject("NavMesh").AddComponent<NavMeshSurface>();
        nav.BuildNavMesh();

        var s = SceneManager.GetActiveScene().path;

        string dirPath = Path.GetDirectoryName(s) + "/_PakLevel/" + Path.GetFileNameWithoutExtension(s) + "/";
        Directory.CreateDirectory(dirPath);
        // SaveTextures(dirPath);
        SaveLightmaps(dirPath);
        StaticOcclusionCulling.GenerateInBackground();
        CreateCubemap( dirPath);
        var lit = FindObjectsOfType<Light>().FirstOrDefault(a => a.type == LightType.Directional);
        if (lit == null)
        {
            lit = new GameObject("directional light").AddComponent<Light>();
            lit.type = LightType.Directional;
            lit.intensity = .1f;
        }
        lit.shadows = LightShadows.Soft;
        
    }
    
    public void CreateCubemap(string  dirPath)
    {
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(.32f, .42f, .59f);
        var sk = RenderSettings.skybox;
        
        var cubemap = new Cubemap(512,DefaultFormat.LDR,TextureCreationFlags.None);
        
        cubemap.SetPixels((sk.GetTexture("_FrontTex") as Texture2D).GetPixels(), CubemapFace.NegativeZ);
        cubemap.SetPixels((sk.GetTexture("_BackTex") as Texture2D).GetPixels(), CubemapFace.PositiveZ);
        
        cubemap.SetPixels((sk.GetTexture("_LeftTex") as Texture2D).GetPixels(), CubemapFace.NegativeX);
        cubemap.SetPixels((sk.GetTexture("_RightTex") as Texture2D).GetPixels(), CubemapFace.PositiveX);


        cubemap.SetPixels(FlipText(sk.GetTexture("_UpTex") as Texture2D).GetPixels(), CubemapFace.PositiveY);
        cubemap.SetPixels(FlipText(sk.GetTexture("_DownTex") as Texture2D).GetPixels(), CubemapFace.NegativeY);
        cubemap.Apply();
        var cubemapPath = dirPath + "cubemap_skybox.png";
        File.WriteAllBytes(cubemapPath, CubeMapper.CreateCubemapTex(cubemap).EncodeToPNG());
        RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
        AssetDatabase.Refresh();
        RenderSettings.customReflection = AssetDatabase.LoadAssetAtPath<Cubemap>(cubemapPath);
        var skybox = RenderSettings.skybox = new Material(Shader.Find("Skybox/Cubemap"));
        skybox.SetTexture("_Tex", RenderSettings.customReflection);

    }
    
    Texture2D FlipText(Texture2D original)
    {
        // We create a new texture so we don't change the old one!
        Texture2D flip = new Texture2D(original.width,original.height);

        // These for loops are for running through each individual pixel and then replacing them in the new texture.
        for(int i=0; i < flip.width; i++) {
            for(int j=0; j < flip.height; j++) {
                flip.SetPixel(flip.width-i-1, flip.height-j-1, original.GetPixel(i,j));
            }
        }

        // We apply the changes to our new texture
        flip.Apply();
        // Then we send it on our marry little way!
        return flip;
    }
    

    private static void SaveTextures(string dirName)
    {
        foreach (var r in FindObjectsOfType<MeshRenderer>())
        {
            foreach (var m in r.sharedMaterials)
            {
                if (m != null && m.mainTexture != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(m.mainTexture))  && m.mainTexture is Texture2D t)
                {
                    t = ext.CloneTexture(t);
                    var fileName = dirName + Path.GetFileName(t.name+".png");
                    if (!File.Exists(fileName))
                    {
                        File.WriteAllBytes(fileName, t.EncodeToPNG());
                        AssetDatabase.Refresh();
                    }
                    m.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(fileName);
                }
            }
        }
    }
    private static void SaveLightmaps(string dirName)
    {
        for (var i = 0; i < LightmapSettings.lightmaps.Length; i++)
        {
            var lightmapDatas = LightmapSettings.lightmaps;
            LightmapData a = lightmapDatas[i];
            Texture2D dir = a.lightmapColor;

            
            var path = dirName + "/lm" + i + ".png";
            if (!File.Exists(path))
            {
                File.WriteAllBytes(path, dir.EncodeToPNG());
                AssetDatabase.Refresh();
            }
            lightmapDatas[i].lightmapColor = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            LightmapSettings.lightmaps = lightmapDatas;
        }
        var c = new GameObject("lightMap").AddComponent<LightMapSave>();
        c.Set(LightmapSettings.lightmaps.Select(a => a.lightmapColor).ToArray());
    }
    public string Bracket(string s)
    {
        return " \"" + s + "\" ";
    }
    public void OnInspectorGUI()
    {
        
        
        // try
        // {
        //     
        //     
        // }catch (Exception e) {
        //     Debug.Log(e);
        // }
    }
}


