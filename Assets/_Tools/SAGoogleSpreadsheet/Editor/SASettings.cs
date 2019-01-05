//----------------------------------------------
// SA: Google Spreadsheet Loader
// Copyright © 2014 SuperAshley Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SuperAshley.GoogleSpreadSheet
{
    public class SASettings
    {
        static public string SpreadsheetID
        {
            get { return EditorPrefs.GetString("SASpreadsheetID", "1TtZA6pNTPko10hFZ-OL3q-6GNfwqnvxjxD3cTx04wsY"); }
            set { EditorPrefs.SetString("SASpreadsheetID", value); }
        }

        static public string WorksheetJSON
        {
            get { return EditorPrefs.GetString("SAWorksheetJSON", string.Empty); }
            set { EditorPrefs.SetString("SAWorksheetJSON", value); }
        }

        static public string CellsJSON
        {
            get { return EditorPrefs.GetString("SACellsJSON", string.Empty); }
            set { EditorPrefs.SetString("SACellsJSON", value); }
        }

        static public string SelectedWorksheet
        {
            get { return EditorPrefs.GetString("SASelectedWorksheet", string.Empty); }
            set { EditorPrefs.SetString("SASelectedWorksheet", value); }
        }

        static public string ScriptFolder
        {
            get { return EditorPrefs.GetString("SAScriptFolder", "Assets"); }
            set { EditorPrefs.SetString("SAScriptFolder", value); }
        }

        static public string AssetFolder
        {
            get { return EditorPrefs.GetString("SAAssetFolder", "Assets"); }
            set { EditorPrefs.SetString("SAAssetFolder", value); }
        }
    }
}