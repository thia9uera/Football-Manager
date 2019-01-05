using RotaryHeart.Lib.SerializableDictionary;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Team", menuName = "Team Data", order = 1)]
public class TeamData : ScriptableObject
{
    [Header("Details")]
    public string Name;
    [Tooltip("Max 3 characters")]
    public string Tag;

    [Space(10)]
    public Color PrimaryColor = Color.white;
    public Color SecondaryColor = Color.red;

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

    [HideInInspector]
    public int Stars;

    [System.Serializable]
    public class Statistics
    {
        public int Points = 0;
        public int TotalWins = 0;
        public int TotalLosts = 0;
        public int TotalDraws = 0;
        public int TotalGoals = 0;
        public int TotalGoalsAgainst = 0;
        public int TotalShots = 0;
        public int TotalHeaders = 0;
        public int TotalSteals = 0;
        public int TotalPasses = 0;
        public int TotalLongPasses = 0;
        public int TotalPassesMissed = 0;
        public int TotalBoxCrosses = 0;
        public int TotalFaults = 0;
        public int TotalOffsides = 0;
        public int TotalCornerKicks = 0;
        public int TotalCounterAttacks = 0;
    }

    public Statistics LifeTimeStats;
    public Statistics MatchStats;
    [System.Serializable]
    public class TournamentStats : SerializableDictionaryBase<string, Statistics> { }
    public TournamentStats TournamentStatistics;

    public bool isUserControlled;

    [HideInInspector]
    public bool IsPlaceholder;

    public TournamentData.TeamMatchData MatchData;
    public void ResetMatchData()
    {
        MatchData = new TournamentData.TeamMatchData();
        MatchData.Team = this;
        MatchData.Statistics = new Statistics();
        MatchData.Scorers = new List<PlayerData>();
        MatchData.RedCards = new List<PlayerData>();
        MatchData.YellowCards = new List<PlayerData>();

        Save();
    }


    public Team_Strategy GetStrategy()
    {
        return MainController.Instance.TeamStrategyData.team_Strategys[(int)Strategy];
    }

    public void ResetStatistics(string _type, string _id = "")
    {
        switch (_type)
        {
            default:
            case "Match": MatchStats = new Statistics(); break;
            case "LifeTime": LifeTimeStats = new Statistics(); break;
            case "Tournament":
                TournamentStatistics[_id] = new Statistics();
                foreach (PlayerData player in GetAllPlayers()) player.ResetStatistics("Tournament", _id);
                break;
        }

        Save();
    }

    public void UpdateLifeTimeStats(bool _updateMatchData=false, bool _isHomeTeam=false)
    {
        Statistics data = MatchStats;

        UpdateStats(LifeTimeStats, data);

        if (MainController.Instance.CurrentTournament != null) UpdateTournamentStatistics(data);

        //Update players statistics
        foreach (PlayerData player in GetAllPlayers()) player.UpdateLifeTimeStats();

        if (_updateMatchData)
        {
            MatchData.Team = this;
            MatchData.Statistics = data;

            if(_isHomeTeam) MainController.Instance.CurrentMatch.HomeTeam = MatchData;
            else MainController.Instance.CurrentMatch.AwayTeam = MatchData;
        }


        ResetStatistics("Match");
    }

    public void UpdateStats(Statistics _stats, Statistics _data)
    {
        _stats.Points += _data.Points;
        _stats.TotalWins += _data.TotalWins;
        _stats.TotalLosts += _data.TotalLosts;
        _stats.TotalDraws += _data.TotalDraws;
        _stats.TotalGoals += _data.TotalGoals;
        _stats.TotalGoalsAgainst += _data.TotalGoalsAgainst;
        _stats.TotalShots += _data.TotalShots;
        _stats.TotalHeaders += _data.TotalHeaders;
        _stats.TotalSteals += _data.TotalSteals;
        _stats.TotalPasses += _data.TotalPasses;
        _stats.TotalLongPasses += _data.TotalLongPasses;
        _stats.TotalPassesMissed += _data.TotalPassesMissed;
        _stats.TotalBoxCrosses += _data.TotalBoxCrosses;
        _stats.TotalFaults += _data.TotalFaults;
        _stats.TotalCounterAttacks += _data.TotalCounterAttacks;
    }

    public Statistics GetTournamentStatistics(string _key)
    {
        Statistics stats = null;

        stats = TournamentStatistics[_key];

        return stats;
    }

    public void InitializetournamentData(string _id)
    {
        if (TournamentStatistics.ContainsKey(_id)) return;

        TournamentStatistics.Add(_id, new Statistics());
    }

    void UpdateTournamentStatistics(Statistics _stats)
    {
        TournamentData currentTournament = MainController.Instance.CurrentTournament;
        //if (TournamentStatistics == null) TournamentStatistics = new TournamentStats();

        if (!TournamentStatistics.ContainsKey(currentTournament.Id))
        {
            TournamentStatistics.Add(currentTournament.Id, new Statistics());
        }

        Statistics tStats = GetTournamentStatistics(currentTournament.Id);

        UpdateStats(tStats, _stats);

        Save();
    }

    public int GetOveralRating()
    {
        int overal = 0;

        int total = 0;
        foreach (PlayerData player in Squad)
        {
            total += player.GetOverall();
        }

        overal = total / Squad.Length;

        return overal;
    }

    public List<PlayerData> GetAllPlayers()
    {
        List<PlayerData> players = new List<PlayerData>();

        players.AddRange(Squad);
        players.AddRange(Substitutes);

        return players;
    }

    void Save()
    {
        EditorUtility.SetDirty(this);
        //AssetDatabase.SaveAssets();
    }
}
