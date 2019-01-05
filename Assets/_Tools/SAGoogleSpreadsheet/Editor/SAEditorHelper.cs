//----------------------------------------------
// SA: Google Spreadsheet Loader
// Copyright © 2014 SuperAshley Entertainment
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace SuperAshley.GoogleSpreadSheet
{
    public class SAEditorHelper
    {

        static public void DrawHeader(string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(3f);
            GUILayout.Label("<b>" + text + "</b>", "dockarea", GUILayout.MinWidth(20f));
            GUILayout.Space(2f);
            GUILayout.EndHorizontal();
        }

        static public void DrawSubHeader(string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(3f);
            GUILayout.Label(text, "VCS_StickyNote", GUILayout.MinWidth(20f));
            GUILayout.Space(2f);
            GUILayout.EndHorizontal();
        }

        static public void BeginContents()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
        }

        static public void EndContents()
        {
            GUILayout.Space(3f);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }
    }
}