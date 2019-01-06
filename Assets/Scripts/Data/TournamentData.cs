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
        public TeamData.Statistics Statistics;
        public List<PlayerData> Scorers;
        public List<PlayerData> YellowCards;
        public List<PlayerData> RedCards;

        public void Reset()
        {
            Statistics = new TeamData.Statistics();
            Scorers = new List<PlayerData>();
            YellowCards = new List<PlayerData>();
            RedCards = new List<PlayerData>();
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

    public List<MatchData> Matches;

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
    public List<TeamData> SortTeamsBy(string _param)
    {
        List<TeamData> list = Teams;

        switch(_param)
        {
            case "Name": list = list.OrderBy(TeamData => TeamData.Name).ToList(); break;
            case "Points": list = list.OrderByDescending(TeamData => TeamData.TournamentStatistics[Id].TotalWins).ToList(); break;
            case "GoalsScored": list = list.OrderByDescending(TeamData => TeamData.TournamentStatistics[Id].TotalGoals).ToList(); break;
            case "GoalsAgainst": list = list.OrderByDescending(TeamData => TeamData.TournamentStatistics[Id].TotalGoalsAgainst).ToList(); break;
        }
        return list;
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
        Debug.Log("RESETED TOURNAMENT");
        foreach(MatchData match in Matches) match.Reset();
        foreach (TeamData data in Teams) data.ResetStatistics("Tournament", Id);
        CurrentRound = 0;
        Save();
        //AssetDatabase.SaveAssets();
    }

    public List<PlayerData> GetAllPlayers()
    {
        List<PlayerData> list = new List<PlayerData>();

        foreach (TeamData team in Teams)
        {
            list.AddRange(team.GetAllPlayers());
        }

        return list;
    }

    void Save()
    {
        //EditorUtility.SetDirty(this);
        //AssetDatabase.SaveAssets();
    }
}
