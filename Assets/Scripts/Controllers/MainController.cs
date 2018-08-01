using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public static MainController Instance;

    public LocalizationData Localization;
    public MatchController Match;

    public Team_StrategyData TeamStrategyData;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        Localization.CurrentLanguage = LocalizationData.Language.English;
    }

    public void Start()
    {
        TeamData home = Resources.Load<TeamData>("Teams/Brasil");
        TeamData away = Resources.Load<TeamData>("Teams/Cadena_Rivers");
        Match.Populate(home, away);
    }

    public string GetPositionShortString(PlayerData.PlayerPosition _position)
    {
        string str = "";
        switch (_position)
        {
            case PlayerData.PlayerPosition.GK:
                str = "GK";
                break;

            case PlayerData.PlayerPosition.CD:
                str = "CD";
                break;

            case PlayerData.PlayerPosition.CM:
                str = "CM";
                break;

            case PlayerData.PlayerPosition.CF:
                str = "CF";
                break;
        }

        return str;
    }
}
