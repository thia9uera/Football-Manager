using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public static MainController Instance;

    public LocalizationData Localization;
    public MatchController Match;
    public SquadSelectionView SquadSelection;

    public Team_StrategyData TeamStrategyData;
    public Player_StrategyData PlayerStrategyData;
    public Game_ModifierData Modifiers;

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

    public void EditSquad(TeamData _team)
    {
        Match.PauseGame(true);
        Match.gameObject.SetActive(false);
        SquadSelection.Populate(_team);
    }

    public void FinishSquadEdit(List<PlayerData> _in, List<PlayerData> _out)
    {
        SquadSelection.gameObject.SetActive(false);
        Match.gameObject.SetActive(true);
        Match.UpdateTeams(_in, _out);      
    }
}
