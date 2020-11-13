using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MatchData
{
    public TeamMatchData HomeTeam;
    public TeamMatchData AwayTeam;
    public bool isPlayed;
	public int Round;
	public int Day;
	public string TournamentName;
	public string TournamentId;

	public MatchData(TeamData _home, TeamData _away, int _round, int _day, string _tournamentName, string _tournamentId)
	{
		TeamAttributes attributes = _home.IsPlaceholder ? _home.Attributes : null;  	
		HomeTeam = new TeamMatchData(_home.Id, attributes);
		
		attributes = _away.IsPlaceholder ? _away.Attributes : null;  
		AwayTeam = new TeamMatchData(_away.Id, attributes);
        isPlayed = false;
	    Round = _round;
		Day = _day;
		TournamentName = _tournamentName;
		TournamentId = _tournamentId;
	}
	
	public MatchData(MatchData _data, int _round, int _day)
	{ 	
		HomeTeam = _data.AwayTeam;  
		AwayTeam = _data.HomeTeam;
		isPlayed = false;
		Round = _round;
		Day = _day;
		TournamentName = _data.TournamentName;
		TournamentId = _data.TournamentId;
	}

    public void Reset()
    {
        isPlayed = false;
        HomeTeam.Reset();
        AwayTeam.Reset();
    }
}

[System.Serializable]
public struct TeamMatchData
{
	public string TeamId;
	public TeamData TeamData
	{
		get
		{
			return MainController.Instance.GetTeamById(TeamId);
		}
	}
	public TeamStatistics Statistics;
    public List<PlayerData> Scorers;
    public List<PlayerData> YellowCards;
    public List<PlayerData> RedCards;

	public TeamMatchData(string _teamId, TeamAttributes _attributes)
    {
	    TeamId = _teamId;
        Statistics = new TeamStatistics();
        Scorers = new List<PlayerData>();
        YellowCards = new List<PlayerData>();
        RedCards = new List<PlayerData>();
    }

    public void Reset()
	{
		TeamId = "";
        Statistics = new TeamStatistics();
        Scorers = new List<PlayerData>();
        YellowCards = new List<PlayerData>();
        RedCards = new List<PlayerData>();
    }
}
