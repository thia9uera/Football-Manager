//----------------------------------------------
// SA: Google Spreadsheet Loader
// Copyright © 2014 SuperAshley Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace SuperAshley.GoogleSpreadSheet
{
    static public class SAGoogleSpreadsheetMenu
    {

        [MenuItem("Tools/SA/Open Spreadsheet Loader", false, 0)]
        static public void OpenSpreadsheetLoader()
        {
            EditorWindow.GetWindow<SAGoogleSpreadsheetLoader>(false, "Spreadsheet Loader", true).Show();
        }
    }
}