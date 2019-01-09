#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class Tools
{
    public static T[] GetAtFolder<T>(string path)
    {
        ArrayList al = new ArrayList();
        string[] folder = Directory.GetFiles(Application.dataPath + "/" + path);
        foreach (string file in folder)
        {
            string filePath = file.Replace("\\", "/");
            string localPath = "Assets/" + path;
            int index = filePath.LastIndexOf("/");
            if (index > 0)
            {
                localPath += filePath.Substring(index);
            }
            Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));
            if (t != null) al.Add(t);
        }


        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++)
            result[i] = (T)al[i];

        return result;
    }

    public static T[] GetAtSubfolders<T>(string path)
    {
        ArrayList al = new ArrayList();
        string[] directories = Directory.GetDirectories(Application.dataPath + "/" + path);
        foreach (string dir in directories)
        {
            string directory = dir.Replace("\\", "/");
            string[] subfolder = Directory.GetFiles(directory);
            foreach (string file in subfolder)
            {
                string filePath = file.Replace("\\", "/");
                string localPath = "Assets/" + path;
                int index = file.LastIndexOf("/");

                if (index > 0)
                {
                    localPath += filePath.Substring(index);
                }
                Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));
                if (t != null) al.Add(t);
            }

        }

        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++)
        {
            result[i] = (T)al[i];
        }

        return result;
    }
}
#endif
