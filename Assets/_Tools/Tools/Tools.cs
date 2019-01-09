#if UNITY_EDITOR
using System.Collections;
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
            string localPath = "Assets/" + path;
            int index = file.LastIndexOf("/", System.StringComparison.Ordinal);
            if (index > 0)
            {
                localPath += file.Substring(index);
            }
            Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));
            if (t != null) al.Add(t);
        }


        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++) result[i] = (T)al[i];
        return result;
    }

    public static T[] GetAtSubfolders<T>(string path)
    {
        ArrayList al = new ArrayList();
        string[] directories = Directory.GetDirectories(Application.dataPath + "/" + path);
        foreach (string dir in directories)
        {
            string directory = Path.GetFileName(dir);
            string[] subfolder = Directory.GetFiles(dir);
            foreach (string file in subfolder)
            {
                string fileName = Path.GetFileName(file);
                string localPath = "Assets/" + path + "/" + directory + "/" + fileName;
                Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));
                if (t != null) al.Add(t);
            }
        }

        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++) result[i] = (T)al[i];
        return result;
    }
}
#endif
