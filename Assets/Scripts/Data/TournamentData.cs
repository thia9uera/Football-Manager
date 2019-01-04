using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

[CreateAssetMenu(fileName = "Tournament", menuName = "Tournament Data", order = 2)]
public class TournamentData : ScriptableObject
{
    public string Name;
    public string Id;

    public enum TournamentType
    {
        Championship,
        Cup,
    }

    public TournamentType Type;

    public int StarsRequired = 0;

    public List<TeamData> Teams;

    [System.Serializable]
    public class TeamMatchData
    {
        public TeamData Team;
        public int Score;
        public List<PlayerData> Scorers;
        public List<PlayerData> YellowCards;
        public List<PlayerData> RedCards;
        public int Points;

        public void Reset()
        {
            Score = 0;
            Scorers = new List<PlayerData>();
            YellowCards = new List<PlayerData>();
            RedCards = new List<PlayerData>();
            Points = 0;
        }
    }

    [System.Serializable]
    public class MatchData
    {
        public TeamMatchData HomeTeam;
        public TeamMatchData AwayTeam;
        public bool isPlayed;
        public int Round;

        public void Reset()
        {
            isPlayed = false;
            HomeTeam.Reset();
            AwayTeam.Reset();
        }
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

        public void Reset()
        {
            Points = 0;
            GoalsScored = 0;
            GoalsAgainst = 0;
            TotalYellowCards = 0;
            TotalRedCards = 0;
            YellowCardList = new List<PlayerData>();
            RedCarList = new List<PlayerData>();
        }
    }

    public PlayerData PlayerStatistics;
    public List<MatchData> Matches;
    public List<TeamTournamentData> TeamScoreboard;

    [Space(10)]
    public int TotalRounds;
    public int CurrentRound;

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
    public List<TeamTournamentData> SortTeamsBy(string _param)
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

    public void UpdateTeamScoreboard(TeamMatchData _home, TeamMatchData _away)
    {
        foreach(TeamTournamentData team in TeamScoreboard)
        {
            if(team.Team == _home.Team)
            {
                UpdateTeamStats(team, _home, _away);
            }
            else if(team.Team == _away.Team)
            {
                UpdateTeamStats(team, _away, _home);
            }
        }
        Save();
    }

    void UpdateTeamStats(TeamTournamentData _data, TeamMatchData _team, TeamMatchData _opponent)
    {
        TeamTournamentData team = _data;

        team.Points += _team.Points;
        team.GoalsScored += _team.Score;
        team.GoalsAgainst += _opponent.Score;
        team.TotalYellowCards += _team.YellowCards.Count();
        team.TotalRedCards += _team.RedCards.Count();
        //TODO Add cards logic
        if (team.YellowCardList == null) team.YellowCardList = new List<PlayerData>();
        if (team.RedCarList == null) team.RedCarList = new List<PlayerData>();
        team.YellowCardList.AddRange(_team.YellowCards);
        team.RedCarList.AddRange(_team.RedCards);
    }

    public MatchData GetNextMatch(bool _isSimulating)
    {
        MatchData data = null;
        foreach (MatchData match in Matches)
        {
            if (_isSimulating)
            {
                if (!match.isPlayed)
                {
                    data = match;
                }
            }
            else
            {
                if (match.Round == CurrentRound && !match.isPlayed)
                {
                    data = match;
                }
            }
        }

        if (data == null)
        {
            if (!_isSimulating) CurrentRound++;
            else CurrentRound = TotalRounds;
            Save();
        }
        else
        {
            MainController.Instance.CurrentMatch = data;
        }

        return data;
    }

    public void ResetTournament()
    {
        foreach(MatchData match in Matches) match.Reset();
        foreach (TeamTournamentData data in TeamScoreboard) data.Reset();
        CurrentRound = 0;
        Save();
    }

    public List<PlayerData> GetTopScorers()
    {
        List<PlayerData> list = new List<PlayerData>();

        foreach(TeamTournamentData data in TeamScoreboard)
        {

        }

        return list;
    }

    void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
}
