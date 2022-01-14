using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;

namespace SharpMods
{



public class ScriptExecutor : UnityEngine.MonoBehaviour
    #if UNITY_EDITOR
    ,IPostBuildPlayerScriptDLLs
#endif
{
    [Serializable]
    public class Script
    {
        public string scriptName;
        public string scriptContent;
    }
    public List<Script> scripts = new List<Script>();


    private void Awake()
    {
    }

    public Action update;
    private void Update()
    {
        update?.Invoke();
    }

    public Action onGui;
    private void OnGUI()
    {
        onGui?.Invoke();
    }

    public Action onGameStart;
    private void OnGameStart()
    {
        onGameStart?.Invoke();
    }

    public Action onCollisionEnter;
    private void OnCollisionEnter(Collision other)
    {
        onCollisionEnter?.Invoke();
    }


    // public int callbackOrder { get; }
    // public void OnPostBuildPlayerScriptDLLs(BuildReport report)
    // {
    //     SaveScript();
    // }
#if UNITY_EDITOR
    private void Reset()
    {
        SaveScript();
    }
    [ContextMenu("SaveScript")]
    private void SaveScript()
    {
        scripts.Clear();
        foreach(var a in GetComponents<MonoBehaviour>())
            if (a != this)
            {
                var s = MonoScript.FromMonoBehaviour(a);
                scripts.Add(new Script(){ scriptName = s.name, scriptContent = s.text});
            }
        // var assetsScriptsSaved = "Assets/Scripts/Saved/";
        // Directory.CreateDirectory(assetsScriptsSaved);
        // var newPath = assetsScriptsSaved + Path.GetFileName(path) + ".txt";
        // File.Copy(path, newPath);
        // AssetDatabase.Refresh();
        // scriptFile = AssetDatabase.LoadAssetAtPath<TextAsset>(newPath);
    }
    public int callbackOrder { get; }
    public void OnPostBuildPlayerScriptDLLs(BuildReport report)
    {
        SaveScript();
    }
#endif
}
}