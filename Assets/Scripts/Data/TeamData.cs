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
        Defensive,
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
        public int TotalShots;
        public int TotalHeaders;
        public int TotalSteals;
        public int TotalPasses;
        public int TotalLongPasses;
        public int TotalPassesMissed;
        public int TotalBoxCrosses;
        public int TotalFaults;
        public int TotalOffsides;
        public int TotalCornerKicks;
        public int TotalCounterAttacks;
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
        stats.TotalShots = 0;
        stats.TotalHeaders = 0;
        stats.TotalSteals = 0;
        stats.TotalPasses = 0;
        stats.TotalLongPasses = 0;
        stats.TotalPassesMissed = 0;
        stats.TotalBoxCrosses = 0;
        stats.TotalFaults = 0;
        stats.TotalOffsides = 0;
        stats.TotalCornerKicks = 0;
        stats.TotalCounterAttacks = 0;
}

    public void UpdateLifeTimeStats()
    {
        LifeTimeStats.TotalGoals += MatchStats.TotalGoals;
        LifeTimeStats.TotalGoalsAgainst += MatchStats.TotalGoalsAgainst;
        LifeTimeStats.TotalShots += MatchStats.TotalShots;
        LifeTimeStats.TotalHeaders += MatchStats.TotalHeaders;
        LifeTimeStats.TotalSteals += MatchStats.TotalSteals;
        LifeTimeStats.TotalPasses += MatchStats.TotalPasses;
        LifeTimeStats.TotalLongPasses += MatchStats.TotalLongPasses;
        LifeTimeStats.TotalPassesMissed += MatchStats.TotalPassesMissed;
        LifeTimeStats.TotalBoxCrosses += MatchStats.TotalBoxCrosses;
        LifeTimeStats.TotalFaults += MatchStats.TotalFaults;
        LifeTimeStats.TotalCounterAttacks += MatchStats.TotalCounterAttacks;



        ResetStatistics("Match");
    }
}
