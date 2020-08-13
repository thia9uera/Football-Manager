using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Team", menuName = "Data/Team Data", order = 1)]
public class TeamData : ScriptableObject
{
    [Space(10)]
    public TeamAttributes Attributes;

    public string Id { get { return Attributes.Id; } set { Attributes.Id = value; } }

    public string Name { get { return Attributes.Name; } set { Attributes.Name = value; } }
    public string Tag { get { return Attributes.Tag; } set { Attributes.Tag = value; } }

    public Color PrimaryColor { get { return Attributes.PrimaryColor; } set { Attributes.PrimaryColor = value; } }
    public Color SecondaryColor { get { return Attributes.SecondaryColor; } set { Attributes.SecondaryColor = value; } }

	public TeamStrategy Strategy { get { return Attributes.Strategy; } set { Attributes.Strategy = value; } }
	public FormationData Formation
	{ 
		get 
		{ 
			return GameData.Instance.Formations.GetFormation((FormationType)Attributes.Formation);
		} 
		set 
		{ 
			int id = GameData.Instance.Formations.GetFormationId(value); 
			FormationSet = (FormationType)id;
		} 
	}

    [Space(10)]
    [Header("Players")]
    [Space(10)]
    public PlayerData[] Squad;
	public PlayerData[] Substitutes;
    
	[Space(10)]
	public FormationType FormationSet;

    [Space(10)]
    public PlayerData Captain;

    [HideInInspector]
    public int Stars;

    public TeamStatistics LifeTimeStats { get { return Attributes.LifeTimeStats; } set { Attributes.LifeTimeStats = value; } }
    public TeamTournamentStats TournamentStatistics { get { return Attributes.TournamentStatistics; } set { Attributes.TournamentStatistics = value; } }

    public TeamStatistics MatchStats;

    public bool IsUserControlled { get { return Attributes.IsUserControlled; } set { Attributes.IsUserControlled = value; } }

    public bool IsPlaceholder { get { return Attributes.IsPlaceholder; } set { Attributes.IsPlaceholder = value; } }

	public bool IsAwayTeam = false;

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
	    return GameData.Instance.TeamStrategies[(int)Strategy];
    }

    public void ResetStatistics(string _type, string _id = "")
    {
        switch (_type)
        {
        	case "Match": MatchStats = new TeamStatistics(); break;
            case "LifeTime": LifeTimeStats = new TeamStatistics(); break;
            case "Tournament":
	            TournamentStatistics[_id] = new TeamStatistics();
                foreach (PlayerData player in GetAllPlayers()) player.ResetStatistics("Tournament", _id);
                break;
        }
    }

    public void UpdateLifeTimeStats(bool _updateMatchData = false, bool _isHomeTeam = false)
    {
        TeamStatistics data = MatchStats;

        UpdateStats(LifeTimeStats, data);

        if (MainController.Instance.CurrentTournament != null) UpdateTournamentStatistics(data);

        //Update players statistics
        foreach (PlayerData player in GetAllPlayers())
        {
            player.UpdateLifeTimeStats();
        }

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
        _stats.Wins += _data.Wins;
        _stats.Losts += _data.Losts;
        _stats.Draws += _data.Draws;
        _stats.Goals += _data.Goals;
        _stats.GoalsAgainst += _data.GoalsAgainst;
        _stats.Shots += _data.Shots;
        _stats.Headers += _data.Headers;
        _stats.Steals += _data.Steals;
        _stats.Passes += _data.Passes;
        _stats.LongPasses += _data.LongPasses;
        _stats.PassesMissed += _data.PassesMissed;
        _stats.BoxCrosses += _data.BoxCrosses;
        _stats.Faults += _data.Faults;
        _stats.CounterAttacks += _data.CounterAttacks;
    }

    public TeamStatistics GetTournamentStatistics(string _key)
    {
        TeamStatistics stats = TournamentStatistics[_key];

        return stats;
    }

    public void InitializeTournamentData(string _id)
    {
	    if (TournamentStatistics.ContainsKey(_id))
	    {
	    	ResetStatistics("Tournament", _id);	
	    	return;
	    }
	    
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
        int total = 0;
        foreach (PlayerData player in Squad)
        {
            total += player.GetOverall();
        }

        return total / Squad.Length;
    }

    public List<PlayerData> GetAllPlayers()
    {
        List<PlayerData> players = new List<PlayerData>();

        players.AddRange(Squad);
        players.AddRange(Substitutes);

        return players;
    }

    public PlayerData GetTopPlayerByAttribute(AttributeType _attribute, PlayerData[] _players, bool _includeSubs = false)
    {
        PlayerData best = null;
        int higher = 0;
        if (_includeSubs) Substitutes.CopyTo(_players, _players.Length);
        foreach (PlayerData player in _players)
        {
            if (player.GetPlayerAttribute(_attribute) > higher) best = player;
        }
        return best;
    }

    public bool IsStrategyApplicable(Zone _zone)
    {
        bool value = false;
	    Team_Strategy teamStrategy = GameData.Instance.TeamStrategies[(int)Strategy];

        switch (_zone)
        {
            case Zone.OwnGoal: value = teamStrategy.OwnGoal; break;
            case Zone.BLD: value = teamStrategy.BLD; break;
            case Zone.BRD: value = teamStrategy.BRD; break;

            case Zone.LD: value = teamStrategy.LD; break;
            case Zone.LCD: value = teamStrategy.LCD; break;
            case Zone.CD: value = teamStrategy.CD; break;
            case Zone.RCD: value = teamStrategy.RCD; break;
            case Zone.RD: value = teamStrategy.RD; break;

            case Zone.LDM: value = teamStrategy.LDM; break;
            case Zone.LCDM: value = teamStrategy.LCDM; break;
            case Zone.CDM: value = teamStrategy.CDM; break;
            case Zone.RCDM: value = teamStrategy.RCDM; break;
            case Zone.RDM: value = teamStrategy.RDM; break;

            case Zone.LM: value = teamStrategy.LM; break;
            case Zone.LCM: value = teamStrategy.LCM; break;
            case Zone.CM: value = teamStrategy.CM; break;
            case Zone.RCM: value = teamStrategy.RCM; break;
            case Zone.RM: value = teamStrategy.RM; break;

            case Zone.LAM: value = teamStrategy.LAM; break;
            case Zone.LCAM: value = teamStrategy.LCAM; break;
            case Zone.CAM: value = teamStrategy.CAM; break;
            case Zone.RCAM: value = teamStrategy.RCAM; break;
            case Zone.RAM: value = teamStrategy.RAM; break;

            case Zone.LF: value = teamStrategy.LF; break;
            case Zone.LCF: value = teamStrategy.LCF; break;
            case Zone.CF: value = teamStrategy.CF; break;
            case Zone.RCF: value = teamStrategy.RCF; break;
            case Zone.RF: value = teamStrategy.RF; break;

            case Zone.ALF: value = teamStrategy.ALF; break;
            case Zone.ARF: value = teamStrategy.ARF; break;
            case Zone.Box: value = teamStrategy.Box; break;
        }
        return value;
    }

    public void Reset()
    {
        TournamentStatistics = new TeamTournamentStats();
        ResetStatistics("LifeTime");
    }

    public void Initialize(bool _fromSaveData = false)
	{
		if(IsUserControlled) Debug.Log("INITIALIZE");
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
    
    
	public PlayerData GetAttackingPlayer(Zone _zone, PlayerData _excludePlayer = null, bool _forcePlayer = false, bool _isAwayTeam = false)
	{
		Zone zone = Field.Instance.GetTeamZone(_zone, _isAwayTeam);

		float chance = 0f;
		bool forcePlayer = _forcePlayer;

		List<PlayerData> players = new List<PlayerData>();

		foreach (PlayerData player in Squad)
		{
			chance = Field.Instance.CalculatePresence(player, zone, Strategy);

			if (forcePlayer)
			{
				if (chance > 0f) players.Add(player);
			}
			else
			{
				if (chance >= 1f) players.Add(player);
				else if (chance > 0 && chance >= Random.Range(0f, 1f)) players.Add(player);
			}
		}

		if (_excludePlayer != null && players.Contains(_excludePlayer)) players.Remove(_excludePlayer);

		return GetActivePlayer(players);
	}

	public PlayerData GetBestPlayerInArea(Zone _zone, AttributeType _attribute)
	{
		Zone zone = GetTeamZone(_zone);

		float chance = 0f;


		List<PlayerData> players = new List<PlayerData>();

		foreach (PlayerData player in Squad)
		{
			chance = Field.Instance.CalculatePresence(player, zone, Strategy);
			if (chance > 0f) players.Add(player);
              
		}

		return GetTopPlayerByAttribute(_attribute, players.ToArray());
	}

	public PlayerData GetDefendingPlayer(Zone _zone, PlayerData _excludePlayer = null, float _counterAttack = 0)
	{
		Zone zone = Field.Instance.GetTeamZone(_zone, IsAwayTeam);
        
		float chance = 0f;

		List<PlayerData> players = new List<PlayerData>();
		foreach (PlayerData player in Squad)
		{
			chance = Field.Instance.CalculatePresence(player, zone, Strategy);
			if (_counterAttack > 0)
			{
				chance *= 0.5f;
			}
			if (chance >= 1f)
			{
				players.Add(player);
			}
			else
			{
				if (chance > 0 && chance >= Random.Range(0f, 1f))
				{
					players.Add(player);
				}
			}
		}

		if (_excludePlayer != null && players.Contains(_excludePlayer)) players.Remove(_excludePlayer);

		return GetActivePlayer(players);
	}
	
	private PlayerData GetActivePlayer(List<PlayerData> _list)
	{
		PlayerData activePlayer = null;
		List<KeyValuePair<PlayerData, float>> compareList = new List<KeyValuePair<PlayerData, float>>();
		int bonus = 0;
		float total = 0f;

		foreach (PlayerData player in _list)
		{
			float stats = (float)(player.Speed + player.Vision) / 200;
			stats *= player.FatigueModifier();
			bonus = player.GetAttributeBonus((player.Vision + player.Speed) / 2);
			if (player.IsWronglyAssigned()) stats *= GameData.Instance.GameModifiers.PositionDebuff;

			int r = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(stats * 5) + bonus / 10);

			if (r >= 20) stats *= 1.5f;
			else if (r <= 1) stats *= 0.75f;

			total += stats;
			compareList.Add(new KeyValuePair<PlayerData, float>(player, stats));
		}

		float random = Random.Range(0f, 1f);
		float cumulative = 0f;

		for (int i = 0; i < compareList.Count; i++)
		{
			float value = compareList[i].Value / total;

			cumulative += value;
			if (random <= cumulative)
			{
				activePlayer = compareList[i].Key;
				break;
			}
		}

		//if (activePlayer != null) activePlayer.MatchStats.Presence++;
		return activePlayer;
	}
	
	public Zone GetTeamZone (Zone _zone)
	{
		if(!IsAwayTeam) return _zone;
		else
		{
			return Field.Instance.GetTeamZone(_zone, IsAwayTeam);
		}
	}
}
