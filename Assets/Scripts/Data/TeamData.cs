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

    public int TotalWins,
        TotalLosts,
        TotalDraws,
        TotalGoals,
        TotalGoalsAgainst;

    public Team_Strategy GetStrategy()
    {
        Team_StrategyData data = MainController.Instance.TeamStrategyData;

        Team_Strategy strategy = data.team_Strategys[(int)Strategy];

        return strategy;
    }

    public void ResetStatistics()
    {
        TotalWins = 0;
        TotalLosts = 0;
        TotalDraws = 0;
        TotalGoals = 0;
        TotalGoalsAgainst = 0;
    }
}
