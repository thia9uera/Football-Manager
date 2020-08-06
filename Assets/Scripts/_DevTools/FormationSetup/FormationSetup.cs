using PrimitiveUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class FormationSetup : MonoBehaviour
{
    private static FormationSetup instance;
    public static FormationSetup Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(FormationSetup)) as FormationSetup;
            }
            return instance;
        }
    }

    public FormationData Formation;

    public RectTransform field;

    private FormationZone[] zones;
    public FormationZone SelectedZone;

    private List<Zone> formationZones;

    private float fieldWidth;
    private float fieldHeight;

    private StrokeStyle lineStyle;

    [SerializeField]
    private PrimitiveCanvas lines;

    [SerializeField]
    private TMP_InputField nameInput;

    [SerializeField]
    private TMP_Dropdown dropdown;

    List<string> formationNameList;

    private string folder = "Assets/Data/Formations/";

    private void Start()
    {
        fieldWidth = field.sizeDelta.x;
        fieldHeight = field.sizeDelta.y;
        lineStyle = new StrokeStyle(Color.white, 4f, StrokeScaleMode.Absolute);

        zones = field.GetComponentsInChildren<FormationZone>();

        Populate();
        UpdateDropdown();
    }

    private void Populate()
    {
        nameInput.text = Formation.Name;

        formationZones = new List<Zone>(Formation.Zones);
        foreach (FormationZone zone in zones)
        {
            zone.Deselect();
            zone.Activate(formationZones.Contains(zone.Zone));
        }

        UpdateConnections();
    }
    
    private void UpdateDropdown()
    {
        List<FormationData> fileList = new List<FormationData>(Tools.GetAtFolder<FormationData>("Data/Formations"));
        formationNameList = new List<string>();

        int selected = 0;
        for ( int i = 0; i < fileList.Count; i++)
        {
            FormationData data = fileList[i];
            formationNameList.Add(data.Name);
            if (data.Name == Formation.Name) selected = i;
        }
        print(selected);
        dropdown.ClearOptions();
        dropdown.AddOptions(formationNameList);
        dropdown.value = selected;
    }

    public void OnDropDownSelect()
    {
        List<FormationData> fileList = new List<FormationData>(Tools.GetAtFolder<FormationData>("Data/Formations"));
        foreach (FormationData data in fileList)
        {
            if (data.Name == formationNameList[dropdown.value])
            {
                Formation = data;
                Populate();
            }
        }
    }

    public void SelectZone(FormationZone zone)
    {
        if (SelectedZone != null)
        {
            SelectedZone.Deselect();
            if (SelectedZone != zone)
            {
                SelectedZone = zone;
                zone.Select();
            }
            else SelectedZone = null;
        }
        else
        {
            SelectedZone = zone;
            zone.Select();
        }
    }

    public void Connect(FormationZone zone)
    {
        if (SelectedZone == null) Debug.LogWarning("Select a zone first");
        else if (zone == SelectedZone) Debug.LogWarning("Cannot connect zone to itself");
        else
        {
            FormationData.Connection connection = new FormationData.Connection(SelectedZone.Zone, zone.Zone);

            if (!ConnectExists(connection))
            {
                Formation.Connections.Add(connection);
            }
        }

        UpdateConnections();
    }

    private bool ConnectExists(FormationData.Connection _connection)
    {
        bool exists = false;

        foreach(FormationData.Connection connection in Formation.Connections)
        {
            if (connection.Equals(_connection))
            {
                Formation.Connections.Remove(connection);
                return true;
            }
        }

        return exists;
    }

    private void UpdateConnections()
    {
        lines.Clear();

        foreach(FormationData.Connection connection in Formation.Connections)
        {
            DrawLine(GetZoneRect(connection.ZoneA), GetZoneRect(connection.ZoneB));
        }
    }

    private RectTransform GetZoneRect(Zone _zone)
    {
        RectTransform rect = null;

        foreach(FormationZone zone in zones)
        {
            if (zone.Zone == _zone) rect = zone.GetComponent<RectTransform>();
        }

        return rect;
    }

    private void DrawLine(RectTransform _pointA, RectTransform _pointB)
    {
        float startX = (_pointA.anchoredPosition.x + 50) / fieldWidth;
        float startY = (_pointA.anchoredPosition.y + 50) / fieldHeight;

        float endX = (_pointB.anchoredPosition.x + 50) / fieldWidth;
        float endY = (_pointB.anchoredPosition.y + 50) / fieldHeight;

        lines.DrawLine(new Vector2(startX, startY), new Vector2(endX, endY), lineStyle);
    }

    public void ActivateZone(FormationZone _zone)
    {
        if(_zone.Zone == Zone.OwnGoal)
        {
            Debug.LogWarning("Cannot remove Goal Keeper position");
            return;
        }

        if(formationZones.Contains(_zone.Zone))
        {
            formationZones.Remove(_zone.Zone);
            _zone.Deactivate();

            List<FormationData.Connection> list =new List<FormationData.Connection>(Formation.Connections);

            for(int i=0; i < list.Count; i++)
            {
                FormationData.Connection connection = list[i];
                if (connection.ZoneA == _zone.Zone || connection.ZoneB == _zone.Zone)
                {
                   list.RemoveAt(i);
                }
            }

            Formation.Connections = list;
            UpdateConnections();
            return;
        }

        if (formationZones.Count == 11) Debug.LogWarning("Cannot have more than 11 active zones");
        else
        {
            formationZones.Add(_zone.Zone);
            _zone.Activate(true);
        }
    }

    public void SaveChanges()
    {
        if (formationZones.Count < 11)
        {
            EditorUtility.DisplayDialog("NOT ENOUGH ZONES", "This formation has less than 11 zones", "OK");
            return;
        }

        //Formation.Zones = formationZones.ToArray();
        List<Zone> list = new List<Zone>();
        foreach(FormationZone zone in zones)
        {
            if (zone.isActive) list.Add(zone.Zone);
        }
        Formation.Zones = list.ToArray();

        string oldName = folder + Formation.name + ".asset";
        string newName = nameInput.text;
        AssetDatabase.RenameAsset(oldName, newName);
        Formation.Name = nameInput.text;
        EditorUtility.SetDirty(Formation);
        AssetDatabase.SaveAssets();

        UpdateConnections();
    }

    public void CreateNew()
    {
        FormationData data = ScriptableObject.CreateInstance<FormationData>();
        data.Name = "0-0-0";
        data.Zones = new Zone[] { Zone.OwnGoal };
        data.Connections = new List<FormationData.Connection>();
        AssetDatabase.CreateAsset(data, folder + data.Name + ".asset");
        AssetDatabase.SaveAssets();
        Formation = data;
        Populate();
    }
}
