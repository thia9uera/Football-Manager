using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public static MainController Instance;

    [Space(10)]
    [Header("Data")]
    [Space(5)]
    public UserData User;
    public List<PlayerData> AllPlayers;
    public List<TeamData> AllTeams;
    public List<TournamentData> AllTournaments;

    [Space(10)]
    [Header("Controllers")]
    [Space(5)]
    public LocalizationData Localization;
    public MatchController Match;
    public DataController Data;
    public CalendarController Calendar;
    public ScreenController Screens;

    [Space(20)]

    public SquadSelectionView SquadSelection;

    public Team_StrategyData TeamStrategyData;
    public Player_StrategyData PlayerStrategyData;
    public Game_ModifierData Modifiers;
    public PosChanceData PosChancePerZone;
    public TargetPassPerZoneData TargetPassPerZone;
    public TargetCrossPerZoneData TargetCrossPerZone;

    public TournamentData CurrentTournament;
    public MatchData CurrentMatch;

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
        if(Screens != null) Screens.ShowScreen(BaseScreen.ScreenType.Loading);
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

    public List<PlayerData> SortPlayersBy(List<PlayerData> listPlayers, string _stat)
    {
        switch (_stat)
        {
            case "Position": listPlayers = listPlayers.OrderBy(PlayerData => PlayerData.Zone).ToList(); break;
            case "Name": listPlayers = listPlayers.OrderBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Goals": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Goals).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Shots": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Shots).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "ShotsMissed": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.ShotsMissed).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Headers": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Headers).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "HeadersMissed": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.HeadersMissed).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Passes": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Passes).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Crosses": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Crosses).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Faults": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Faults).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Tackles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Tackles).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Dribbles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Dribbles).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Saves": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Saves).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Presence": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Presence).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
        }

        return listPlayers;
    }

    public List<PlayerData> SortPlayersBy(List<PlayerData> listPlayers, string _stat, string _tournamentId)
    {
        switch (_stat)
        {
            case "Position": listPlayers = listPlayers.OrderBy(PlayerData => PlayerData.Zone).ToList(); break;
            case "Name": listPlayers = listPlayers.OrderBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Goals": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].Goals).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Shots": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].Shots).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "ShotsMissed": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].ShotsMissed).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Headers": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].Headers).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "HeadersMissed": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].HeadersMissed).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Passes": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].Passes).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Crosses": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].Crosses).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Faults": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].Faults).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Tackles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].Tackles).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Dribbles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].Dribbles).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Saves": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].Saves).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Presence": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].Presence).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
        }

        return listPlayers;
    }

    public PlayerData GetPlayerById(string _id) { return AllPlayers.Single(PlayerData => PlayerData.Id == _id); }

    public TeamData GetTeamById(string _id) { return AllTeams.Single(TeamData => TeamData.Id == _id); }

    public FormationData GetFormation(FormationData.TeamFormation _formation) { return Match.TeamFormations[(int)_formation]; }

    public List<TeamData> SortTeamsBy(List<TeamData> listTeams, string _stat)
    {
        switch (_stat)
        {
            case "Name": listTeams = listTeams.OrderBy(TeamData => TeamData.Name).ToList(); break;
            case "Wins": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.TotalWins).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "Losts": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.TotalLosts).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "Draws": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.TotalDraws).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "Goals": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.TotalGoals).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "GoalsAgainst": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.TotalGoalsAgainst).ThenBy(TeamData => TeamData.Name).ToList(); break;

            case "Shots": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.TotalShots).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "Headers": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.TotalHeaders).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "Steals": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.TotalSteals).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "Passes": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.TotalPasses).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "LongPasses": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.TotalLongPasses).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "PassesMissed": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.TotalPassesMissed).ThenBy(TeamData => TeamData.Name).ToList(); break;

            case "BoxCrosses": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.TotalBoxCrosses).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "Faults": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.TotalFaults).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "CounterAttacks": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.TotalCounterAttacks).ThenBy(TeamData => TeamData.Name).ToList(); break;
        }

        return listTeams;
    }
}
