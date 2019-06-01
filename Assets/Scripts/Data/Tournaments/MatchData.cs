using System.Collections.Generic;

[System.Serializable]
public class MatchData
{
    public TeamMatchData HomeTeam;
    public TeamMatchData AwayTeam;
    public bool isPlayed;
    public int Round;


    public MatchData(TeamAttributes _home, TeamAttributes _away, int _round)
    {
        HomeTeam = new TeamMatchData(_home);
        AwayTeam = new TeamMatchData(_away);
        isPlayed = false;
        Round = _round;
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
    public TeamAttributes TeamAttributes;
    public TeamStatistics Statistics;
    public List<PlayerData> Scorers;
    public List<PlayerData> YellowCards;
    public List<PlayerData> RedCards;

    public TeamMatchData(TeamAttributes _attributes)
    {
        TeamAttributes = _attributes;
        Statistics = new TeamStatistics();
        Scorers = new List<PlayerData>();
        YellowCards = new List<PlayerData>();
        RedCards = new List<PlayerData>();
    }

    public void Reset()
    {
        Statistics = new TeamStatistics();
        Scorers = new List<PlayerData>();
        YellowCards = new List<PlayerData>();
        RedCards = new List<PlayerData>();
    }
}
