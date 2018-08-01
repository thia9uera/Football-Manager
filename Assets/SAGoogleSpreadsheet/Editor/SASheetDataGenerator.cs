//----------------------------------------------
// SA: Google Spreadsheet Loader
// Copyright © 2014 SuperAshley Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SuperAshley.GoogleSpreadSheet
{
    public class SASheetDataClassGenerator
    {
        const string FIELD_FORMAT = "\tpublic {0} {1};\n";

        static public void GenerateClass(string name, List<List<string>> cellList)
        {
            name = name.Replace(" ", "");
            string classData = "using UnityEngine;\nusing System.Collections.Generic;\n\n[System.Serializable]\npublic class " + name + " {\n";
            for (int col = 0; col < cellList[0].Count; col++)
            {
                string fieldName = cellList[0][col];
                string typeName = "";
                if (fieldName[0] == 'n')
                {
                    typeName = "int";
                }
                else if (fieldName[0] == 's')
                {
                    typeName = "string";
                }
                else if (fieldName[0] == 'f')
                {
                    typeName = "float";
                }
                else if (fieldName[0] == 'b')
                {
                    typeName = "bool";
                }

                classData += string.Format(FIELD_FORMAT, typeName, fieldName.Substring(2));
            }
            classData += "}\n\n";

            string dataClassName = name + "Data";

            classData += "public class " + dataClassName + " : ScriptableObject {\n\tpublic List<" + name + "> " + name[0].ToString().ToLower() + name.Substring(1) + "s = new List<" + name + ">();\n}\n";

            StreamWriter writer = File.CreateText(Application.dataPath + SASettings.ScriptFolder.Replace("Assets", "") + "/" + dataClassName + ".cs");
            writer.WriteLine(classData);
            writer.Close();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CreateAsset(string name, List<List<string>> cellList)
        {
            name = name.Replace(" ", "");
            string dataClassName = name + "Data";
            Type assetType = GetTypeByName(dataClassName);
            if (assetType != null)
            {
                var dataHolder = AssetDatabase.LoadAssetAtPath(SASettings.AssetFolder + "/" + dataClassName + ".asset", assetType);
                if (dataHolder == null)
                {
                    dataHolder = ScriptableObject.CreateInstance(assetType);
                    AssetDatabase.CreateAsset(dataHolder, SASettings.AssetFolder + "/" + dataClassName + ".asset");
                }

                Type dataType = GetTypeByName(name);

                FieldInfo dataListField = assetType.GetField(name[0].ToString().ToLower() + name.Substring(1) + "s");
                var dataList = dataListField.GetValue(dataHolder);
                dataList.GetType().GetMethod("Clear").Invoke(dataList, null);
                for (int row = 1; row < cellList.Count; row++)
                {
                    Debug.Log("Data Type : " + dataType);
                    var data = Activator.CreateInstance(dataType);
                    for (int col = 0; col < cellList[row].Count; col++)
                    {
                        Debug.Log("col : " + col);
                        if (string.IsNullOrEmpty(cellList[row][col]))
                            continue;

                        FieldInfo info = dataType.GetField(cellList[0][col].Substring(2));
                        object value = null;
                        if (info.FieldType == typeof(int))
                        {
                            value = int.Parse(cellList[row][col]);
                        }
                        else if (info.FieldType == typeof(float))
                        {
                            value = float.Parse(cellList[row][col]);
                        }
                        else if (info.FieldType == typeof(string))
                        {
                            value = cellList[row][col];
                        }
                        info.SetValue(data, value);
                    }
                    dataList.GetType().GetMethod("Add").Invoke(dataList, new object[] { data });
                }

                EditorUtility.SetDirty(dataHolder);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Cannot find the script " + dataClassName + ", please generate the script first.", "ok");
            }
        }

        public static Type GetTypeByName(string name)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name == name)
                        return type;
                }
            }

            return null;
        }
    }
}