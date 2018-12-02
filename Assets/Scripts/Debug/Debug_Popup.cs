using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Debug_Popup : MonoBehaviour
{
    public TMP_InputField InputField;

    MatchController.FieldZone zone;
    float chance;
	
    public void ShowPopup(float _chance, MatchController.FieldZone _zone)
    {
        gameObject.SetActive(true);
        zone = _zone;
        chance = _chance * 100;
        InputField.text = chance.ToString();
        InputField.Select();
    }

    public void Confirm()
    {
        Zones data = MainController.Instance.Match.TeamStrategies[(int)Debug_FieldView.Instance.TeamStrategy].PosChance.posChancePerZones[(int)Debug_FieldView.Instance.TestPlayer.Zone];
        chance = float.Parse(InputField.text)/100;
        data.Position = zone.ToString();

        switch (zone)
        {
            case MatchController.FieldZone.OwnGoal: data.OwnGoal = chance; break;
            case MatchController.FieldZone.BLD: data.BLD = chance; break;
            case MatchController.FieldZone.BRD: data.BRD = chance; break;
            case MatchController.FieldZone.LD: data.LD = chance; break;
            case MatchController.FieldZone.LCD: data.LCD = chance; break;
            case MatchController.FieldZone.CD: data.CD = chance; break;
            case MatchController.FieldZone.RCD: data.RCD = chance; break;
            case MatchController.FieldZone.RD: data.RD = chance; break;
            case MatchController.FieldZone.LDM: data.LDM = chance; break;
            case MatchController.FieldZone.LCDM: data.LCDM = chance; break;
            case MatchController.FieldZone.CDM: data.CDM = chance; break;
            case MatchController.FieldZone.RCDM: data.RCDM = chance; break;
            case MatchController.FieldZone.RDM: data.RDM = chance; break;
            case MatchController.FieldZone.LM: data.LM = chance; break;
            case MatchController.FieldZone.LCM: data.LCM = chance; break;
            case MatchController.FieldZone.CM: data.CM = chance; break;
            case MatchController.FieldZone.RCM: data.RCM = chance; break;
            case MatchController.FieldZone.RM: data.RM = chance; break;
            case MatchController.FieldZone.LAM: data.LAM = chance; break;
            case MatchController.FieldZone.LCAM: data.LCAM = chance; break;
            case MatchController.FieldZone.CAM: data.CAM = chance; break;
            case MatchController.FieldZone.RCAM: data.RCAM = chance; break;
            case MatchController.FieldZone.RAM: data.RAM = chance; break;
            case MatchController.FieldZone.LF: data.LF = chance; break;
            case MatchController.FieldZone.LCF: data.LCF = chance; break;
            case MatchController.FieldZone.CF: data.CF = chance; break;
            case MatchController.FieldZone.RCF: data.RCF = chance; break;
            case MatchController.FieldZone.RF: data.RF = chance; break;
            case MatchController.FieldZone.ALF: data.ALF = chance; break;
            case MatchController.FieldZone.ARF: data.ARF = chance; break;
            case MatchController.FieldZone.Box: data.Box = chance; break;
        }

        gameObject.SetActive(false);
        Debug_FieldView.Instance.Test();
        MainController.Instance.Match.TeamStrategies[(int)Debug_FieldView.Instance.TeamStrategy].PosChance.SetDirty();
    }

    void Update()
    {
        if(Input.GetButton("Submit"))
        {
            Confirm();
        }
    }
}

