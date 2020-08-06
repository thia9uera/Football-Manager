using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Debug_Popup : MonoBehaviour
{
    public TMP_InputField InputField;

    Zone zone;
    float chance;

    [SerializeField]
    List<PosChanceData> posChandeDatas;

    private void Awake()
    {
        posChandeDatas = new List<PosChanceData>();

        PosChanceData[] dataList =  Tools.GetAtFolder<PosChanceData>("Data/PosChance");
        foreach (PosChanceData data in dataList) posChandeDatas.Add(data);
    }

    public void ShowPopup(float _chance, Zone _zone)
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
        data.Position = ((Zone)Debug_FieldView.Instance.DropDownPlayerPosition.value).ToString();

        switch (zone)
        {
            case Zone.OwnGoal: data.OwnGoal = chance; break;
            case Zone.BLD: data.BLD = chance; break;
            case Zone.BRD: data.BRD = chance; break;
            case Zone.LD: data.LD = chance; break;
            case Zone.LCD: data.LCD = chance; break;
            case Zone.CD: data.CD = chance; break;
            case Zone.RCD: data.RCD = chance; break;
            case Zone.RD: data.RD = chance; break;
            case Zone.LDM: data.LDM = chance; break;
            case Zone.LCDM: data.LCDM = chance; break;
            case Zone.CDM: data.CDM = chance; break;
            case Zone.RCDM: data.RCDM = chance; break;
            case Zone.RDM: data.RDM = chance; break;
            case Zone.LM: data.LM = chance; break;
            case Zone.LCM: data.LCM = chance; break;
            case Zone.CM: data.CM = chance; break;
            case Zone.RCM: data.RCM = chance; break;
            case Zone.RM: data.RM = chance; break;
            case Zone.LAM: data.LAM = chance; break;
            case Zone.LCAM: data.LCAM = chance; break;
            case Zone.CAM: data.CAM = chance; break;
            case Zone.RCAM: data.RCAM = chance; break;
            case Zone.RAM: data.RAM = chance; break;
            case Zone.LF: data.LF = chance; break;
            case Zone.LCF: data.LCF = chance; break;
            case Zone.CF: data.CF = chance; break;
            case Zone.RCF: data.RCF = chance; break;
            case Zone.RF: data.RF = chance; break;
            case Zone.ALF: data.ALF = chance; break;
            case Zone.ARF: data.ARF = chance; break;
            case Zone.Box: data.Box = chance; break;
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

