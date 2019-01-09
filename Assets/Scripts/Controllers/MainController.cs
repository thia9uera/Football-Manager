using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public static MainController Instance;

    public UserData User;

    public LocalizationData Localization;
    public MatchController Match;
    public SquadSelectionView SquadSelection;

    public ScreenController Screens;

    public Team_StrategyData TeamStrategyData;
    public Player_StrategyData PlayerStrategyData;
    public Game_ModifierData Modifiers;
    public PosChanceData PosChancePerZone;
    public TargetPassPerZoneData TargetPassPerZone;
    public TargetCrossPerZoneData TargetCrossPerZone;

    public TournamentData CurrentTournament;
    public MatchData CurrentMatch;

    public List<PlayerData> AllPlayers;
    public List<TeamData> AllTeams;
    public List<TournamentData> AllTournaments;

    public DataController Data;

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
            case "Goals": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.TotalGoals).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Shots": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.TotalShots).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "ShotsMissed": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.TotalShotsMissed).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Headers": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.TotalHeaders).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "HeadersMissed": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.TotalHeadersMissed).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Passes": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.TotalPasses).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Crosses": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.TotalCrosses).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Faults": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.TotalFaults).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Tackles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.TotalTackles).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Dribbles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.TotalDribbles).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Saves": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.TotalSaves).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Presence": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.TotalPresence).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
        }

        return listPlayers;
    }

    public List<PlayerData> SortPlayersBy(List<PlayerData> listPlayers, string _stat, string _tournamentId)
    {
        switch (_stat)
        {
            case "Position": listPlayers = listPlayers.OrderBy(PlayerData => PlayerData.Zone).ToList(); break;
            case "Name": listPlayers = listPlayers.OrderBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Goals": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].TotalGoals).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Shots": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].TotalShots).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "ShotsMissed": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].TotalShotsMissed).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Headers": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].TotalHeaders).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "HeadersMissed": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].TotalHeadersMissed).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Passes": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].TotalPasses).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Crosses": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].TotalCrosses).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Faults": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].TotalFaults).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Tackles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].TotalTackles).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Dribbles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].TotalDribbles).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Saves": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].TotalSaves).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Presence": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.TournamentStatistics[_tournamentId].TotalPresence).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
        }

        return listPlayers;
    }

    public PlayerData GetPlayerById(string _id)
    {
        PlayerData player = null;

        foreach (PlayerData p in AllPlayers)
        {
            if (p.Id == _id)
            {
                player = p;
                break;
            }
        }

        return player;
    }

    public TeamData GetTeamById(string _id)
    {
        TeamData team = null;

        foreach (TeamData t in AllTeams)
        {
            if (t.Id == _id)
            {
                team = t;
                break;
            }
        }
        if (team == null) print(_id + "    TEAMS: " + AllTeams.Count);
        return team;
    }

    public FormationData GetFormation(FormationData.TeamFormation _formation)
    {
        return Match.TeamFormations[(int)_formation];
    }

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
