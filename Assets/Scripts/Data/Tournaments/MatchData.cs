using System.Collections.Generic;

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
public struct TeamMatchData
{
    public TeamAttributes TeamAttributes;
    public TeamStatistics Statistics;
    public List<PlayerData> Scorers;
    public List<PlayerData> YellowCards;
    public List<PlayerData> RedCards;

    public void Reset()
    {
        Statistics = new TeamStatistics();
        Scorers = new List<PlayerData>();
        YellowCards = new List<PlayerData>();
        RedCards = new List<PlayerData>();
    }
}
