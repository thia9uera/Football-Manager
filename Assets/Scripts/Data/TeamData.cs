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

    public class Statistics
    {
        public int TotalWins;
        public int TotalLosts;
        public int TotalDraws;
        public int TotalGoals;
        public int TotalGoalsAgainst;
    }

    public Statistics LifeTimeStats;
    public Statistics GameStats;

    public Team_Strategy GetStrategy()
    {
        return MainController.Instance.TeamStrategyData.team_Strategys[(int)Strategy];
    }

    public void ResetLifeTimeStatistics()
    {
        LifeTimeStats.TotalWins = 0;
        LifeTimeStats.TotalLosts = 0;
        LifeTimeStats.TotalDraws = 0;
        LifeTimeStats.TotalGoals = 0;
        LifeTimeStats.TotalGoalsAgainst = 0;
    }
}
