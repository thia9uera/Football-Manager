using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public List<MatchData> Matches;
}
