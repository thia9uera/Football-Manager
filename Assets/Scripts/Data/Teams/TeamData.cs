using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Team", menuName = "Team Data", order = 1)]
public class TeamData : ScriptableObject
{
    [Space(10)]
    public TeamAttributes Attributes;

    public string Id { get { return Attributes.Id; } set { Attributes.Id = value; } }

    public string Name { get { return Attributes.Name; } set { Attributes.Name = value; } }
    public string Tag { get { return Attributes.Tag; } set { Attributes.Tag = value; } }

    public Color PrimaryColor { get { return Attributes.PrimaryColor; } set { Attributes.PrimaryColor = value; } }
    public Color SecondaryColor { get { return Attributes.SecondaryColor; } set { Attributes.SecondaryColor = value; } }

    public FormationData.TeamFormation Formation { get { return Attributes.Formation; } set { Attributes.Formation = value; } }
    public TeamAttributes.TeamStrategy Strategy { get { return Attributes.Strategy; } set { Attributes.Strategy = value; } }

    [Space(10)]
    [Header("Players")]
    [Space(10)]
    public PlayerData[] Squad;
    public PlayerData[] Substitutes;

    [Space(10)]
    public PlayerData Captain;

    [HideInInspector]
    public int Stars;

    public TeamStatistics LifeTimeStats { get { return Attributes.LifeTimeStats; } set { Attributes.LifeTimeStats = value; } }
    public TeamTournamentStats TournamentStatistics { get { return Attributes.TournamentStatistics; } set { Attributes.TournamentStatistics = value; } }

    public TeamStatistics MatchStats;

    public bool IsUserControlled { get { return Attributes.IsUserControlled; } set { Attributes.IsUserControlled = value; } }

    public bool IsPlaceholder { get { return Attributes.IsPlaceholder; } set { Attributes.IsPlaceholder = value; } }

    private void Awake()
    {
        MatchStats = new TeamStatistics();
    }

    public TeamMatchData MatchData;
    public void ResetMatchData()
    {
        MatchData = new TeamMatchData();
        MatchData.TeamAttributes = Attributes;
        MatchData.Statistics = new TeamStatistics();
        MatchData.Scorers = new List<PlayerData>();
        MatchData.RedCards = new List<PlayerData>();
        MatchData.YellowCards = new List<PlayerData>();
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
            case "Match": MatchStats = new TeamStatistics(); break;
            case "LifeTime": LifeTimeStats = new TeamStatistics(); break;
            case "Tournament":
                TournamentStatistics[_id] = new TeamStatistics();
                foreach (PlayerData player in GetAllPlayers()) player.ResetStatistics("Tournament", _id);
                break;
        }
    }

    public void UpdateLifeTimeStats(bool _updateMatchData=false, bool _isHomeTeam=false)
    {
        TeamStatistics data = MatchStats;

        UpdateStats(LifeTimeStats, data);

        if (MainController.Instance.CurrentTournament != null) UpdateTournamentStatistics(data);

        //Update players statistics
        foreach (PlayerData player in GetAllPlayers()) player.UpdateLifeTimeStats();

        if (_updateMatchData)
        {
            MatchData.TeamAttributes = Attributes;
            MatchData.Statistics = data;

            if(_isHomeTeam) MainController.Instance.CurrentMatch.HomeTeam = MatchData;
            else MainController.Instance.CurrentMatch.AwayTeam = MatchData;
        }

        ResetStatistics("Match");
    }

    public void UpdateStats(TeamStatistics _stats, TeamStatistics _data)
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

    public TeamStatistics GetTournamentStatistics(string _key)
    {
        TeamStatistics stats = new TeamStatistics();

        stats = TournamentStatistics[_key];

        return stats;
    }

    public void InitializeTournamentData(string _id)
    {
        if (TournamentStatistics.ContainsKey(_id)) return;

        TournamentStatistics.Add(_id, new TeamStatistics());
    }

    void UpdateTournamentStatistics(TeamStatistics _stats)
    {
        TournamentData currentTournament = MainController.Instance.CurrentTournament;
        if (TournamentStatistics == null) Attributes.TournamentStatistics = new TeamTournamentStats();

        if (!TournamentStatistics.ContainsKey(currentTournament.Id))
        {
            TournamentStatistics.Add(currentTournament.Id, new TeamStatistics());
        }

        TeamStatistics tStats = GetTournamentStatistics(currentTournament.Id);

        UpdateStats(tStats, _stats);
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

    public void Reset()
    {
        TournamentStatistics = new TeamTournamentStats();
        ResetStatistics("LifeTime");
    }

    public void Initialize(bool _fromSaveData = false)
    {
        if(_fromSaveData)
        {
            SetSquad(Attributes.SquadIds);
            SetSubstitutes(Attributes.SubstitutesIds);
            return;
        }

        SetSquad(Squad);
        SetSubstitutes(Substitutes);
    }

    public void SetSquad(PlayerData[] _players)
    {
        Attributes.SquadIds = new string[11];
        for (int i = 0; i < _players.Length; i++)
        {
            Attributes.SquadIds[i] = _players[i].Id;
            _players[i].Team = this;
        }
    }

    public void SetSquad(string[] _playerIds)
    {
        List<PlayerData> list = new List<PlayerData>();
        for (int i = 0; i < _playerIds.Length; i++)
        {
            PlayerData player = MainController.Instance.GetPlayerById(_playerIds[i]);
            list.Add(player);
            player.Team = this;
        }
        Squad = list.ToArray();
    }

    public void SetSubstitutes(PlayerData[] _players)
    {
        Attributes.SubstitutesIds = new string[Substitutes.Length];
        for (int i = 0; i < _players.Length; i++)
        {
            Attributes.SubstitutesIds[i] = _players[i].Id;
            _players[i].Team = this;
        }
    }

    public void SetSubstitutes(string[] _playerIds)
    {
        List<PlayerData> list = new List<PlayerData>();
        for (int i = 0; i < _playerIds.Length; i++)
        {
            PlayerData player = MainController.Instance.GetPlayerById(_playerIds[i]);
            list.Add(player);
            player.Team = this;
        }
        Substitutes = list.ToArray();
    }
}
