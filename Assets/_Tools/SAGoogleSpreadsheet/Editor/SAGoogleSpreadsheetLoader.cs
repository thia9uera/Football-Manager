//----------------------------------------------
// SA: Google Spreadsheet Loader
// Copyright © 2014 SuperAshley Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using JsonFx.Json;
using UnityEngine.Networking;

namespace SuperAshley.GoogleSpreadSheet
{
    public class SAGoogleSpreadsheetLoader : EditorWindow
    {
        public enum GenerateState
        {
            Script,
            Asset
        }

        const string WORKSHEET_LIST_URL = "https://spreadsheets.google.com/feeds/worksheets/{0}/public/full?alt=json";

        string spreadsheetID = string.Empty;
        UnityWebRequest spreadsheetWWW;
        UnityWebRequest worksheetWWW;

        Vector2 worksheetScroll;
        Vector2 cellScroll;
        Vector2 cellTitleScroll;

        string sheetTitle = string.Empty;
        Dictionary<string, string> worksheetDict;
        List<List<string>> cellList;

        string selectedWorksheet = string.Empty;
        string saveScriptPath = string.Empty;
        string saveAssetPath = string.Empty;

        Dictionary<string, string>.Enumerator worksheetEnumerator;
        Action WorksheetLoaded;
        GenerateState currentState = GenerateState.Script;

        void OnEnable()
        {
            spreadsheetID = SASettings.SpreadsheetID;
            PopulateWorksheetDict(SASettings.WorksheetJSON);
            PopulateCellList(SASettings.CellsJSON);
            selectedWorksheet = SASettings.SelectedWorksheet;
            saveScriptPath = SASettings.ScriptFolder;
            saveAssetPath = SASettings.AssetFolder;
        }

        #region UI
        /// <summary>
        /// Draw the UI for this tool.
        /// </summary>s
        void OnGUI()
        {
            EditorGUIUtility.labelWidth = 80f;
            GUILayout.Space(3f);

            DrawSpreadsheetSettings();

            DrawSpreadsheetInfo();


            //SAEditorHelper.TestStyles();
        }

        void DrawSpreadsheetSettings()
        {
            // spreadsheet settings
            SAEditorHelper.DrawHeader("Spreadsheet Settings");
            SAEditorHelper.BeginContents();

            // spreadsheet ID
            GUI.changed = false;
            spreadsheetID = EditorGUILayout.TextField("Sheet ID", spreadsheetID);
            if (GUI.changed)
            {
                SASettings.SpreadsheetID = spreadsheetID;

                SASettings.WorksheetJSON = string.Empty;
                SASettings.CellsJSON = string.Empty;
                SASettings.SelectedWorksheet = string.Empty;
            }

            // load spread sheet button
            if (GUILayout.Button("Load", "LargeButtonMid") && (spreadsheetWWW == null || (spreadsheetWWW != null && spreadsheetWWW.isDone)))
            {
                if (worksheetDict != null)
                {
                    worksheetDict = null;
                    cellList = null;
                    selectedWorksheet = string.Empty;

                    SASettings.WorksheetJSON = string.Empty;
                    SASettings.CellsJSON = string.Empty;
                    SASettings.SelectedWorksheet = string.Empty;

                    WorksheetLoaded = null;
                }

                LoadSpreadsheetData();
            }

            SAEditorHelper.EndContents();
        }

        void DrawSpreadsheetInfo()
        {
            if (worksheetDict != null && worksheetDict.Count > 0)
            {
                // spreadsheet info
                if (!string.IsNullOrEmpty(sheetTitle))
                {
                    SAEditorHelper.DrawHeader(sheetTitle);
                    SAEditorHelper.BeginContents();

                    DrawWorksheetTab();

                    DrawWorksheetCells();

                    SAEditorHelper.EndContents();
                }
            }
        }

        void DrawWorksheetTab()
        {
            SAEditorHelper.DrawSubHeader("Worksheets");
            worksheetScroll = EditorGUILayout.BeginScrollView(worksheetScroll, GUILayout.Height(40f));

            EditorGUILayout.BeginHorizontal();
            foreach (KeyValuePair<string, string> pair in worksheetDict)
            {
                if (pair.Key == selectedWorksheet)
                    GUI.color = Color.blue;
                if (GUILayout.Button(pair.Key, "U2D.createRect") && (worksheetWWW == null || (worksheetWWW != null && worksheetWWW.isDone)))
                {
                    selectedWorksheet = pair.Key;
                    SASettings.SelectedWorksheet = selectedWorksheet;
                    cellScroll = Vector2.zero;

                    if (cellList != null)
                    {
                        cellList = null;

                        SASettings.CellsJSON = string.Empty;
                    }

                    LoadWorksheetData(pair.Value);
                }

                GUI.color = Color.white;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Generate script for all sheets", "LargeButtonMid", GUILayout.Width(300f)))
            {
                currentState = GenerateState.Script;
                GenerateScriptAll();
            }
            if (GUILayout.Button("Create asset for all sheets", "LargeButtonMid", GUILayout.Width(300f)))
            {
                currentState = GenerateState.Asset;
                GenerateScriptAll();
            }
        }

        void DrawWorksheetCells()
        {
            if (cellList != null && cellList.Count > 0)
            {
                SAEditorHelper.DrawSubHeader("Cells Preview");
                cellTitleScroll.x = cellScroll.x;
                cellTitleScroll.y = 0f;

                Color transparent = new Color(0, 0, 0, 0);
                GUI.color = transparent;

                GUIStyle scrollbarStyle = new GUIStyle(GUI.skin.horizontalScrollbar);
                scrollbarStyle.fixedHeight = scrollbarStyle.fixedWidth = 0;

                EditorGUILayout.BeginScrollView(cellTitleScroll, false, true, scrollbarStyle, GUI.skin.verticalScrollbar, GUI.skin.scrollView, GUILayout.Height(20f));

                GUI.color = Color.white;

                GUI.backgroundColor = Color.green;
                EditorGUILayout.BeginHorizontal();
                for (int col = 0; col < cellList[0].Count; col++)
                {
                    GUILayout.Label(cellList[0][col], GUILayout.Width(100f), GUILayout.Height(20f));
                }
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndScrollView();

                cellScroll = EditorGUILayout.BeginScrollView(cellScroll, true, true, GUILayout.Height(200f));
                for (int row = 1; row < cellList.Count; row++)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (row == 0)
                        GUI.backgroundColor = Color.green;
                    for (int col = 0; col < cellList[row].Count; col++)
                    {
                        GUILayout.Label(cellList[row][col], GUILayout.Width(100f), GUILayout.Height(20f));
                    }
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();

                DrawGenerateButtons();
            }
        }

        void DrawGenerateButtons()
        {
            GUILayout.Space(3f);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Script", "LargeButtonMid", GUILayout.Width(150f)))
            {
                SASheetDataClassGenerator.GenerateClass(selectedWorksheet, cellList);
            }
            GUILayout.Space(3f);
            if (GUILayout.Button("Choose Script Folder", "LargeButtonMid", GUILayout.Width(150f)))
            {
                string path = EditorUtility.OpenFolderPanel("Choose Folder", Application.dataPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    saveScriptPath = path.Replace(Application.dataPath, "Assets");
                    SASettings.ScriptFolder = saveScriptPath;
                }
            }
            GUILayout.Space(3f);
            EditorGUILayout.BeginVertical();
            GUILayout.Space(4f);
            GUILayout.Label(saveScriptPath, GUILayout.Height(20f));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3f);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Asset", "LargeButtonMid", GUILayout.Width(150f)))
            {
                SASheetDataClassGenerator.CreateAsset(selectedWorksheet, cellList);
            }
            GUILayout.Space(3f);
            if (GUILayout.Button("Choose Asset Folder", "LargeButtonMid", GUILayout.Width(150f)))
            {
                string path = EditorUtility.OpenFolderPanel("Choose Folder", Application.dataPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    saveAssetPath = path.Replace(Application.dataPath, "Assets");
                    SASettings.AssetFolder = saveAssetPath;
                }
            }
            GUILayout.Space(3f);
            EditorGUILayout.BeginVertical();
            GUILayout.Space(4f);
            GUILayout.Label(saveAssetPath, GUILayout.Height(20f));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        void LoadSpreadsheetData()
        {
            string url = string.Format(WORKSHEET_LIST_URL, spreadsheetID);

            spreadsheetWWW = new UnityWebRequest(url);

            EditorApplication.update += CheckLoadSpreadsheetData;
        }

        void CheckLoadSpreadsheetData()
        {
            if (spreadsheetWWW.isDone)
            {
                EditorApplication.update -= CheckLoadSpreadsheetData;

                if (string.IsNullOrEmpty(spreadsheetWWW.error))
                {
                    string resJson = spreadsheetWWW.url;
                    SASettings.WorksheetJSON = resJson;

                    PopulateWorksheetDict(resJson);
                }
                else
                {
                    Debug.LogError(spreadsheetWWW.error);
                }

                Repaint();
            }
        }

        void PopulateWorksheetDict(string resJson)
        {
            if (string.IsNullOrEmpty(resJson))
                return;

            Dictionary<string, object> resDict = JsonReader.Deserialize<Dictionary<string, object>>(resJson);
            Dictionary<string, object> feedDict = resDict["feed"] as Dictionary<string, object>;
            Dictionary<string, object> titleDict = feedDict["title"] as Dictionary<string, object>;

            sheetTitle = (string)titleDict["$t"];
            worksheetDict = new Dictionary<string, string>();

            Dictionary<string, object>[] entryArray = feedDict["entry"] as Dictionary<string, object>[];
            foreach (Dictionary<string, object> entry in entryArray)
            {
                Dictionary<string, object> worksheetTitleDict = entry["title"] as Dictionary<string, object>;
                string worksheetTitle = (string)worksheetTitleDict["$t"];

                Dictionary<string, object>[] linkArray = entry["link"] as Dictionary<string, object>[];
                foreach (Dictionary<string, object> link in linkArray)
                {
                    string url = (string)link["href"];
                    if (url.Contains("/cells/"))
                    {
                        worksheetDict.Add(worksheetTitle, url + "?alt=json");
                        break;
                    }
                }
            }
        }

        void LoadWorksheetData(string url)
        {
            worksheetWWW = new UnityWebRequest(url);

            EditorApplication.update += CheckLoadWorksheetData;
        }

        void CheckLoadWorksheetData()
        {
            if (worksheetWWW.isDone)
            {
                EditorApplication.update -= CheckLoadWorksheetData;
                if (string.IsNullOrEmpty(worksheetWWW.error))
                {
                    string resJson = worksheetWWW.downloadHandler.text;
                    SASettings.CellsJSON = resJson;

                    PopulateCellList(resJson);

                    if (WorksheetLoaded != null)
                    {
                        WorksheetLoaded();
                    }
                }
                else
                {
                    Debug.LogError(worksheetWWW.error);
                }

                Repaint();
            }
        }

        void PopulateCellList(string resJson)
        {
            if (string.IsNullOrEmpty(resJson))
                return;

            Dictionary<string, object> resDict = JsonReader.Deserialize<Dictionary<string, object>>(resJson);
            Dictionary<string, object> feedDict = resDict["feed"] as Dictionary<string, object>;
            Dictionary<string, object>[] entryArray = feedDict["entry"] as Dictionary<string, object>[];

            cellList = new List<List<string>>();

            foreach (Dictionary<string, object> entry in entryArray)
            {
                Dictionary<string, object> cellDict = entry["gs$cell"] as Dictionary<string, object>;

                int row = int.Parse((string)cellDict["row"]);
                int col = int.Parse((string)cellDict["col"]);
                string inputValue = (string)cellDict["$t"];

                while (cellList.Count < row)
                    cellList.Add(new List<string>());

                while (cellList[row - 1].Count < col)
                    cellList[row - 1].Add(string.Empty);

                cellList[row - 1][col - 1] = inputValue;
            }
        }

        void GenerateScriptAll ()
        {
            worksheetEnumerator = worksheetDict.GetEnumerator();

            GenerateNext();
        }

        void GenerateNext ()
        {
            if (worksheetEnumerator.MoveNext())
            {
                selectedWorksheet = worksheetEnumerator.Current.Key;
                SASettings.SelectedWorksheet = selectedWorksheet;
                cellScroll = Vector2.zero;

                if (cellList != null)
                {
                    cellList = null;
                    SASettings.CellsJSON = string.Empty;
                }

                WorksheetLoaded = OnWorksheetLoaded;
                LoadWorksheetData(worksheetEnumerator.Current.Value);
            }
            else
            {
                WorksheetLoaded = null;
            }
        }

        void OnWorksheetLoaded ()
        {
            if (currentState == GenerateState.Script)
            {
                SASheetDataClassGenerator.GenerateClass(selectedWorksheet, cellList);
            }
            else if (currentState == GenerateState.Asset)
            {
                SASheetDataClassGenerator.CreateAsset(selectedWorksheet, cellList);
            }

            GenerateNext();
        }
    }
}