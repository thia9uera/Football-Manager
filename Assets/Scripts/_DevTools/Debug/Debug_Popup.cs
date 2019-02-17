using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Debug_Popup : MonoBehaviour
{
    public TMP_InputField InputField;

    Field.Zone zone;
    float chance;

    [SerializeField]
    List<PosChanceData> posChandeDatas;

    private void Awake()
    {
        posChandeDatas = new List<PosChanceData>();

        PosChanceData[] dataList =  Tools.GetAtFolder<PosChanceData>("Data/PosChance");
        foreach (PosChanceData data in dataList) posChandeDatas.Add(data);
    }

    public void ShowPopup(float _chance, Field.Zone _zone)
    {
        gameObject.SetActive(true);
        zone = _zone;
        chance = _chance * 100;
        InputField.text = chance.ToString();
        InputField.Select();
    }

    Zones GetZones()
    {
        Zones zones = null;
        foreach (PosChanceData data in posChandeDatas)
        {
            if(data.Strategy == Debug_FieldView.Instance.TeamStrategy)
            {
                zones = data.posChancePerZones[(int)Debug_FieldView.Instance.TestPlayer.Attributes.Zone];
            }
        }

        return zones;
    }

    public void Confirm()
    {
        Zones data = GetZones();
        chance = float.Parse(InputField.text)/100;
        data.Position = ((Field.Zone)Debug_FieldView.Instance.DropDownPlayerPosition.value).ToString();

        switch (zone)
        {
            case Field.Zone.OwnGoal: data.OwnGoal = chance; break;
            case Field.Zone.BLD: data.BLD = chance; break;
            case Field.Zone.BRD: data.BRD = chance; break;
            case Field.Zone.LD: data.LD = chance; break;
            case Field.Zone.LCD: data.LCD = chance; break;
            case Field.Zone.CD: data.CD = chance; break;
            case Field.Zone.RCD: data.RCD = chance; break;
            case Field.Zone.RD: data.RD = chance; break;
            case Field.Zone.LDM: data.LDM = chance; break;
            case Field.Zone.LCDM: data.LCDM = chance; break;
            case Field.Zone.CDM: data.CDM = chance; break;
            case Field.Zone.RCDM: data.RCDM = chance; break;
            case Field.Zone.RDM: data.RDM = chance; break;
            case Field.Zone.LM: data.LM = chance; break;
            case Field.Zone.LCM: data.LCM = chance; break;
            case Field.Zone.CM: data.CM = chance; break;
            case Field.Zone.RCM: data.RCM = chance; break;
            case Field.Zone.RM: data.RM = chance; break;
            case Field.Zone.LAM: data.LAM = chance; break;
            case Field.Zone.LCAM: data.LCAM = chance; break;
            case Field.Zone.CAM: data.CAM = chance; break;
            case Field.Zone.RCAM: data.RCAM = chance; break;
            case Field.Zone.RAM: data.RAM = chance; break;
            case Field.Zone.LF: data.LF = chance; break;
            case Field.Zone.LCF: data.LCF = chance; break;
            case Field.Zone.CF: data.CF = chance; break;
            case Field.Zone.RCF: data.RCF = chance; break;
            case Field.Zone.RF: data.RF = chance; break;
            case Field.Zone.ALF: data.ALF = chance; break;
            case Field.Zone.ARF: data.ARF = chance; break;
            case Field.Zone.Box: data.Box = chance; break;
        }

        gameObject.SetActive(false);
        Debug_FieldView.Instance.Test();
        UnityEditor.EditorUtility.SetDirty(posChandeDatas[(int)Debug_FieldView.Instance.TeamStrategy]);
        UnityEditor.AssetDatabase.SaveAssets();
    }

    void Update()
    {
        if(Input.GetButton("Submit"))
        {
            Confirm();
        }
    }
}

