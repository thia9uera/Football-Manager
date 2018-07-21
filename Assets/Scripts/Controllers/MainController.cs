using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public static MainController Instance;

    public Localization Localization;
    public MatchController Match;

    public void Awake()
    {
        if (Instance == null) Instance = this;

        Localization = GetComponent<Localization>();
    }

    public void Start()
    {
        TeamData home = Resources.Load<TeamData>("Teams/Avalanches");
        TeamData away = Resources.Load<TeamData>("Teams/Comanches");
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
