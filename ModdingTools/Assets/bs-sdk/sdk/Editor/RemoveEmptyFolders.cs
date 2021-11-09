using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class BuildToolsSDK : EditorWindow
{
    [MenuItem("Tools/RemoveEmptyFolders")]
    static void RemoveEmptyFolders()
    {

        var s_Results = new List<DirectoryInfo>();
        var assetsDir = Application.dataPath + Path.DirectorySeparatorChar;
        GetEmptyDirectories(new DirectoryInfo(assetsDir), s_Results);

        if (0 < s_Results.Count)
        {
            foreach (var d in s_Results)
                FileUtil.DeleteFileOrDirectory(d.FullName);
            AssetDatabase.Refresh();
        }
        
    }
    static bool GetEmptyDirectories(DirectoryInfo dir, List<DirectoryInfo> results)
    {
        bool isEmpty = true;
        try
        {
            isEmpty = dir.GetDirectories().Count(x => !GetEmptyDirectories(x, results)) == 0	// Are sub directories empty?
                      && dir.GetFiles("*.*").All(x => x.Extension == ".meta");	// No file exist?
        }
        catch
        {
        }

        // Store empty directory to results.
        if (isEmpty)
            results.Add(dir);
        return isEmpty;
    }
}