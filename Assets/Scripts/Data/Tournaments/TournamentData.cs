using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

[CreateAssetMenu(fileName = "Tournament", menuName = "Tournament Data", order = 2)]
public class TournamentData : ScriptableObject
{
    public TournamentAttributes Attributes;

    public string Id { get { return Attributes.Id; } set { Attributes.Id = value; } }
    public string Name { get { return Attributes.Name; } set { Attributes.Name = value; } }


    public TournamentAttributes.TournamentType Type { get { return Attributes.Type; } set { Attributes.Type = value; } }

    public int StarsRequired { get { return Attributes.StarsRequired; } set { Attributes.StarsRequired = value; } }

    public string[] TeamIds { get { return Attributes.TeamIds; } set { Attributes.TeamIds = value; } }
    public List<TeamData> Teams;
    public List<MatchData> Matches { get { return new List<MatchData>(Attributes.Matches); } set { Attributes.Matches = value.ToArray(); } }

    public int TotalRounds { get { return Attributes.TotalRounds; } set { Attributes.TotalRounds = value; } }
    public int CurrentRound { get { return Attributes.CurrentRound; } set { Attributes.CurrentRound = value; } }

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
            case "Points": list = list.OrderByDescending(TeamData => TeamData.TournamentStatistics[Id].Wins).ToList(); break;
            case "GoalsScored": list = list.OrderByDescending(TeamData => TeamData.TournamentStatistics[Id].Goals).ToList(); break;
            case "GoalsAgainst": list = list.OrderByDescending(TeamData => TeamData.TournamentStatistics[Id].GoalsAgainst).ToList(); break;
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

    public void LoadTeams()
    {
        Teams = new List<TeamData>();
        foreach(string id in TeamIds)
        {
            TeamData team = MainController.Instance.GetTeamById(id);
            Teams.Add(team);
        }
    }
}
