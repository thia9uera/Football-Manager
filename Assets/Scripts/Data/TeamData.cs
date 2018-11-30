using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Team", menuName = "Team Data", order = 1)]
public class TeamData : ScriptableObject
{
    [Header("Details")]
    public string Name;
    [Tooltip("Max 3 characters")]
    public string Tag;

    [Space(10)]
    public Color PrimaryColor;
    public Color SecondaryColor;

    public enum TeamFormation
    {
        _3_4_3,
        _3_5_2,
        _4_3_3,
        _3_3_4,

    }

    public enum TeamStrategy
    {
        Neutral,
        Offensive,
        Defense,
        Deadlock,
        WingsOffensive,
        CenterOffensive,
        Crossing,
        Pressure,
        ForceOffside  
    }

    [Space(10)]
    public TeamFormation Formation;
    public TeamStrategy Strategy;

    [Space(10)]
    [Header("Players")]
    public PlayerData[] Squad;
    public PlayerData[] Substitutes;

    [Space(10)]
    public PlayerData Captain;

    [System.Serializable]
    public class Statistics
    {
        public int TotalWins;
        public int TotalLosts;
        public int TotalDraws;
        public int TotalGoals;
        public int TotalGoalsAgainst;
    }

    public Statistics LifeTimeStats;
    public Statistics MatchStats;

    public Team_Strategy GetStrategy()
    {
        return MainController.Instance.TeamStrategyData.team_Strategys[(int)Strategy];
    }

    public void ResetStatistics(string _type)
    {
        Statistics stats;

        switch (_type)
        {
            default:
            case "Match": stats = MatchStats; break;
            case "LifeTime": stats = LifeTimeStats; break;
        }

        stats.TotalWins = 0;
        stats.TotalLosts = 0;
        stats.TotalDraws = 0;
        stats.TotalGoals = 0;
        stats.TotalGoalsAgainst = 0;
    }

    public void UpdateLifeTimeStats()
    {
        LifeTimeStats.TotalGoals += MatchStats.TotalGoals;
        LifeTimeStats.TotalGoalsAgainst += MatchStats.TotalGoalsAgainst;

        ResetStatistics("Match");
    }
}
