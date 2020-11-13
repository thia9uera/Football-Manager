using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public static MainController Instance;

    [Space(10)]
    [Header("Data")]
    [Space(5)]
    [SerializeField] private InitialData initialData;
    
	[Space(10)]
	public UserData User;
    
	public List<PlayerData> AllPlayers;
    public List<TeamData> AllTeams;
    public List<TournamentData> AllTournaments;
	public List<string> ActiveTournaments;

    public TeamData UserTeam;

	private void Awake()
    {
        if (Instance == null) Instance = this;
		
        Application.targetFrameRate = 60;
	    QualitySettings.vSyncCount = 0;
	    
	    UserTeam = initialData.UserTeam;
	    AllPlayers = initialData.AllPlayers;
	    AllTeams = initialData.AllTeams;
	    AllTournaments = initialData.AllTournaments;
	    
	    ActiveTournaments = new List<string>();
	    UserTeam.Reset();

	    initialData = null;
    }

    public void Start()
    {
	    if(LocalizationController.Instance != null) LocalizationController.Instance.Initialize();
	    if(ScreenController.Instance != null) 
	    {
		    ScreenController.Instance.ShowScreen(ScreenType.Loading);
	    	ScreenController.Instance.Loading.LoadScreenDelay(2, ScreenType.Start);
	    }
    }

    public void EditSquad()
    {
	    //Match.PauseGame(true);
	    //ScreenController.Instance.ShowScreen(ScreenType.EditSquad);
    }

    public void FinishSquadEdit(List<PlayerData> _in, List<PlayerData> _out)
    {
	    //ScreenController.Instance.ShowPreviousScreen();
	    //if (ScreenController.Instance.PrevScreen == ScreenType.Match) Match.UpdateTeams(_in, _out);   
    }
    
	public PlayerData GetPlayerById(string _id) { return AllPlayers.Single(PlayerData => PlayerData.Id == _id); }
	public TeamData GetTeamById(string _id) { return AllTeams.Single(TeamData => TeamData.Id == _id); }   
	public TournamentData GetTournamentById(string _id) { return AllTournaments.Single(TournamentData => TournamentData.Id == _id); }

    public List<PlayerData> SortPlayersBy(List<PlayerData> listPlayers, string _stat)
    {
        switch (_stat)
        {
            case "Position": listPlayers = listPlayers.OrderBy(PlayerData => PlayerData.Position).ToList(); break;
            case "Name": listPlayers = listPlayers.OrderBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Goals": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Goals).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Assists": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Assists).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Shots": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Shots).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "ShotsOnGoal": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.ShotsOnGoal).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "ShotsMissed": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.ShotsMissed).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Headers": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Headers).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "HeadersMissed": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.HeadersMissed).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "HeadersOnGoal": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.HeadersOnGoal).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Passes": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Passes).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Crosses": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Crosses).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "BoxCrosses": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.BoxCrosses).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Faults": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Faults).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Tackles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Tackles).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Dribbles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Dribbles).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Saves": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Saves).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Presence": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.Presence).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "AverageRating": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.AverageRating).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "MatchesPlayed": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.Attributes.LifeTimeStats.MatchesPlayed).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
        }

        return listPlayers;
    }

    public List<PlayerData> SortPlayersBy(List<PlayerData> listPlayers, string _stat, string _tournamentId)
    {
        switch (_stat)
        {
            case "Position": listPlayers = listPlayers.OrderBy(PlayerData => PlayerData.Position).ToList(); break;
            case "Name": listPlayers = listPlayers.OrderBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Goals": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TournamentStatistics(_tournamentId).Goals).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "GoalsByHeader": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TournamentStatistics(_tournamentId).GoalsByHeader).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Assists": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TournamentStatistics(_tournamentId).Assists).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Shots": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TournamentStatistics(_tournamentId).Shots).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "ShotsMissed": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TournamentStatistics(_tournamentId).ShotsMissed).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Headers": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TournamentStatistics(_tournamentId).Headers).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "HeadersMissed": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TournamentStatistics(_tournamentId).HeadersMissed).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Passes": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TournamentStatistics(_tournamentId).Passes).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Crosses": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TournamentStatistics(_tournamentId).Crosses).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Faults": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TournamentStatistics(_tournamentId).Faults).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Tackles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TournamentStatistics(_tournamentId).Tackles).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Dribbles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TournamentStatistics(_tournamentId).Dribbles).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Saves": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TournamentStatistics(_tournamentId).Saves).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Presence": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TournamentStatistics(_tournamentId).Presence).ThenBy(PlayerData => PlayerData.FirstName).ToList(); break;
        }

        return listPlayers;
    }

    public List<TeamData> SortTeamsBy(List<TeamData> listTeams, string _stat)
    {
        switch (_stat)
        {
            case "Name": listTeams = listTeams.OrderBy(TeamData => TeamData.Name).ToList(); break;
            case "Wins": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.Wins).ThenBy(TeamData => TeamData.Attributes.LifeTimeStats.Goals).ToList(); break;
            case "Losts": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.Losts).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "Draws": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.Draws).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "Goals": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.Goals).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "GoalsAgainst": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.GoalsAgainst).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "GoalsByHeader": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.GoalsByHeader).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "Shots": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.Shots).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "Headers": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.Headers).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "Steals": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.Steals).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "Passes": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.Passes).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "LongPasses": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.LongPasses).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "PassesMissed": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.PassesMissed).ThenBy(TeamData => TeamData.Name).ToList(); break;

            case "BoxCrosses": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.BoxCrosses).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "Faults": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.Faults).ThenBy(TeamData => TeamData.Name).ToList(); break;
            case "CounterAttacks": listTeams = listTeams.OrderByDescending(TeamData => TeamData.Attributes.LifeTimeStats.CounterAttacks).ThenBy(TeamData => TeamData.Name).ToList(); break;
        }

        return listTeams;
    }
}
