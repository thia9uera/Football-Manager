using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Tournament", menuName = "Tournament Data", order = 2)]
public class TournamentData : ScriptableObject
{
    public string Name;

    public enum TournamentType
    {
        Championship,
        Cup,
    }

    public TournamentType Type;

    public List<TeamData> Teams;

    public int StarsRequired = 0;

    [System.Serializable]
    public class TeamMatchData
    {
        public TeamData Team;
        public int Score;
        public List<PlayerData> Scorers;
        public List<PlayerData> YellowCards;
        public List<PlayerData> RedCards;
    }

    [System.Serializable]
    public class MatchData
    {
        public TeamMatchData HomeTeam;
        public TeamMatchData AwayTeam;
        public bool isPlayed;
    }

    [System.Serializable]
    public class TeamTournamentData
    {
        public TeamData Team;
        public int Points;
        public int GoalsScored;
        public int GoalsAgainst;
        public int TotalYellowCards;
        public int TotalRedCards;
        public List<PlayerData> YellowCardList;
        public List<PlayerData> RedCarList;
    }

    public List<MatchData> Matches;
    public List<TeamTournamentData> TeamScoreboard;

    /// <summary>
    /// Gets the teams ordered by parameter.
    /// - Name
    /// - Points
    /// - GoalsScored
    /// - GoalsAgainst
    /// - YellowCards
    /// - RedCards
    /// </summary>
    /// <returns>The leaderboard.</returns>
    /// <param name="_param">Parameter.</param>
    public List<TeamTournamentData> GetLeaderboard(string _param)
    {
        List<TeamTournamentData> list = TeamScoreboard;

        switch(_param)
        {
            case "Name": list = list.OrderBy(TeamTournamentData => TeamTournamentData.Team.Name).ToList(); break;
            case "Points": list = list.OrderByDescending(TeamTournamentData => TeamTournamentData.Points).ToList(); break;
            case "GoalsScored": list = list.OrderByDescending(TeamTournamentData => TeamTournamentData.GoalsScored).ToList(); break;
            case "GoalsAgainst": list = list.OrderByDescending(TeamTournamentData => TeamTournamentData.GoalsAgainst).ToList(); break;
            case "YellowCards": list = list.OrderByDescending(TeamTournamentData => TeamTournamentData.TotalYellowCards).ToList(); break;
            case "RedCards": list = list.OrderByDescending(TeamTournamentData => TeamTournamentData.TotalRedCards).ToList(); break;
        }

        return list;
    }


}
