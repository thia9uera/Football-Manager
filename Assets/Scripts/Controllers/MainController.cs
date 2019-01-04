using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public static MainController Instance;

    public LocalizationData Localization;
    public MatchController Match;
    public SquadSelectionView SquadSelection;
    public TournamentHubScreen TournamentHub;
    public List<BaseScreen> ScreenList;
    ScreenType prevScreen;
    ScreenType currentScreen;

    public Team_StrategyData TeamStrategyData;
    public Player_StrategyData PlayerStrategyData;
    public Game_ModifierData Modifiers;
    public PosChanceData PosChancePerZone;
    public TargetPassPerZoneData TargetPassPerZone;
    public TargetCrossPerZoneData TargetCrossPerZone;

    public TournamentData CurrentTournament;
    public TournamentData.MatchData CurrentMatch;

    public enum ScreenType
    {
        None,
        MainMenu,
        TournamentHub,
        Match,
        EditSquad,
    }

    public void Awake()
    {
        if (Instance == null) Instance = this;
        Localization.Initialize();

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    public void Start()
    {
        //TeamData home = Resources.Load<TeamData>("Teams/Crossing");
        //TeamData away = Resources.Load<TeamData>("Teams/Cadena_Rivers");
        //if(Match != null) Match.Populate(home, away);
        ShowScreen(ScreenType.MainMenu);
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

    public void ShowScreen(ScreenType _type)
    {
        prevScreen = currentScreen;
        foreach (BaseScreen screen in ScreenList)
        {
            if (screen.Type == _type)
            {
                screen.Show();
                currentScreen = screen.Type;
            }
            else screen.Hide();
        }
        if (prevScreen == ScreenType.None) prevScreen = currentScreen;
    }

    public void ShowPreviousScreen()
    {
        ShowScreen(prevScreen);
    }
}
