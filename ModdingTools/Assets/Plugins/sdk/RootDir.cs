using System;
using System.IO;
using UnityEngine;

public static class RootDir
{
    public static string path;
    static RootDir()
    {
        if (!Application.isMobilePlatform)
        {
            var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            try
            {
                var info = Directory.CreateDirectory(docs + "/BrutalStrike/");
                if (info.Exists)
                    path = info.FullName;
            }
            catch
            {
            }
        }

        if (path == null)
            path = Application.persistentDataPath + "/";
        Directory.CreateDirectory(path);
    }
    public static string GetPath(string s)
    {
        
        var p = path + s;
        try
        {
            Directory.CreateDirectory(p);
        }
        catch
        {
            Debug.LogError("failed to create directory " + p);
            if (Application.isEditor) throw;
        }
        return p.TrimEnd('/') + "/";
    }
}