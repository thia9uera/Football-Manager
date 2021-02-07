using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Player", menuName = "Data/Player Data", order = 1)]
[System.Serializable]
public class PlayerData : ScriptableObject
{
    public PlayerAttributes Attributes;

    public string Id { get { return Attributes.Id; } set { Attributes.Id = value; } }

	public Sprite Portrait { get { return  AtlasManager.Instance.GetPortrait(Attributes.Portrait, Attributes.Position); } set { Attributes.Portrait = value.name; } }
    public string FirstName { get { return Attributes.FirstName; } set { Attributes.FirstName = value; } }
    public string LastName { get { return Attributes.LastName; } set { Attributes.LastName = value; } }

    public Zone Zone { get { return Attributes.Zone; } set { Attributes.Zone = value; } }
    public PlayerPosition Position { get { return Attributes.Position; } set { Attributes.Position = value; } }
    public PlayerStrategy Strategy { get { return Attributes.Strategy; } set { Attributes.Strategy = value; } }
    public SynergyGroup Synergy { get { return Attributes.Synergy; } set { Attributes.Synergy = value; } }

    public int Goalkeeping { get { return Attributes.Goalkeeping; } set { Attributes.Goalkeeping = value; } }
    public int Passing { get { return Attributes.Passing; } set { Attributes.Passing = value; } }
    public int Dribbling { get { return Attributes.Dribbling; } set { Attributes.Dribbling = value; } }
    public int Crossing { get { return Attributes.Crossing; } set { Attributes.Crossing = value; } }
    public int Tackling { get { return Attributes.Tackling; } set { Attributes.Tackling = value; } }
    public int Blocking { get { return Attributes.Blocking; } set { Attributes.Blocking = value; } }
    public int Shooting { get { return Attributes.Shooting; } set { Attributes.Shooting = value; } }
    public int Heading { get { return Attributes.Heading; } set { Attributes.Heading = value; } }
    public int Freekick { get { return Attributes.Freekick; } set { Attributes.Freekick = value; } }
    public int Penalty { get { return Attributes.Penalty; } set { Attributes.Penalty = value; } }

    public int Speed { get { return Attributes.Speed; } set { Attributes.Speed = value; } }
    public int Strength { get { return Attributes.Strength; } set { Attributes.Strength = value; } }
    public int Agility { get { return Attributes.Agility; } set { Attributes.Agility = value; } }
    public int Stamina { get { return Attributes.Stamina; } set { Attributes.Stamina = value; } }

    public int Teamwork { get { return Attributes.Teamwork; } set { Attributes.Teamwork = value; } }
    public int Vision { get { return Attributes.Vision; } set { Attributes.Vision = value; } }
    public int Stability { get { return Attributes.Stability; } set { Attributes.Stability = value; } }

    float fatigue;
    public float Fatigue
    {
        get
        {
            return fatigue;
        }
        set
        {
            fatigue = value;
            if (fatigue <= 0) fatigue = 0.01f;
        }
    }

    public float FatigueModifier()
    {
        float value = 0.5f + (0.5f * (fatigue / 100));
        //float value = 1f;
        return value;
    }

    public PlayerStatistics MatchStats;
    public TeamData Team;

    [HideInInspector]
    public float Prob_DefPosition,
        Prob_OffPosition,
        Prob_LeftPos,
        Prob_RightPos,
        Prob_Pass,
        Prob_Shoot,
        Prob_Fault,
        Prob_Crossing,
        Prob_Dribble,
        Prob_Fall,
        Prob_OffsideLine,
        Prob_Marking,
        Prob_Tackling,
        Prob_LongPass;


    private void Awake()
    {
        MatchStats = new PlayerStatistics();
    }

    public void ApplyBonus()
    {
	    PlayerStrategyData.PlayerStrategyChances _playerStrategy = GameData.Instance.PlayerStrategies[(int)Strategy];

        Prob_DefPosition = _playerStrategy.DefPosChance;
        Prob_OffPosition = _playerStrategy.OffPosChance;
        Prob_LeftPos = _playerStrategy.LeftPosChance;
        Prob_RightPos = _playerStrategy.RightPosChance;
        Prob_Pass = _playerStrategy.PassingChance;
        Prob_LongPass = _playerStrategy.LongPassChance;
        Prob_Shoot = _playerStrategy.ShootingChance;
        Prob_Crossing = _playerStrategy.CrossingChance;
        Prob_Dribble = _playerStrategy.DribblingChance;
        Prob_OffsideLine = _playerStrategy.OffsideTrickChance;
        Prob_Marking = _playerStrategy.MarkingChance;
        Prob_Tackling = _playerStrategy.TacklingChance;

        MatchStats = new PlayerStatistics();
    }

    public int GetOverall()
    {
        int total = 0;

	    total += Passing;		// 1
	    total += Dribbling; 	// 2
	    total += Crossing;		// 3
	    total += Tackling;		// 4
	    total += Blocking;		// 5
	    total += Shooting;		// 6
	    total += Heading;		// 7
	    total += Speed; 		// 8
	    total += Strength;		// 9
	    total += Agility;		// 10
	    total += Stamina;		// 11
	    total += Teamwork;		// 12
	    total += Vision;		// 13
    	total += Stability; 	// 14

        switch (Position)
        {
            case PlayerPosition.Goalkeeper:
	            total += Goalkeeping;	// 15
	            total += Agility;		// 16
                total = total / 16;
                break;
            case PlayerPosition.Defender:
	            total += Tackling;		// 15
	            total += Blocking;		// 16
                total = total / 16;
                break;
            case PlayerPosition.Midfielder:
	            total += Dribbling;		// 15
	            total += Passing;		// 16
	            total += Crossing;		// 17
                total = total / 17;
                break;
            case PlayerPosition.Forward:
	            total += Dribbling;		// 15
	            total += Passing;		// 16
	            total += Shooting;		// 17
	            total += Heading;		// 18
                total = total / 18;
                break;
            default:
                total = total / 14;
                break;
        }

        return total;
    }
    
	public int GetSummaryAttribute(SummaryAttributeType _type)
	{
		int total = 0;
		
		switch(_type)
		{
			case SummaryAttributeType.Attack :
			{
				total += Dribbling;
				total += Shooting;
				total += Heading;
				total /= 3;
			}
			break;
			case SummaryAttributeType.Defense :
			{
				if(Position == PlayerPosition.Goalkeeper)
				{
					total += Tackling/2;
					total += Blocking/2;
					total += Goalkeeping;
				}
				else
				{
					total += Tackling;
					total += Blocking;
				}
				total /= 2;
			}
			break;
			case SummaryAttributeType.Physical :
			{
				total += Speed;
				total += Agility;
				total += Strength;
				total += Stamina;
				total /= 4;
			}
			break;
			case SummaryAttributeType.Tactical :
			{
				total += Vision;
				total += Teamwork;
				total += Passing;
				total += Crossing;
				total /= 4;
			}
			break;
		}
		
		return total;
	}

    public float GetChancePerZone(Zone _zone)
	{
		Zones _chancePerZone = GameData.Instance.GetPosChanceData(Team.Strategy, _zone);
		float pct = 0f;
        
		switch(Zone)
        {
            case Zone.OwnGoal: pct = _chancePerZone.OwnGoal; break;
            case Zone.BLD: pct = _chancePerZone.BLD; break;
            case Zone.BRD: pct = _chancePerZone.BRD; break;
            case Zone.LD: pct = _chancePerZone.LD; break;
            case Zone.LCD: pct = _chancePerZone.LCD; break;
            case Zone.CD: pct = _chancePerZone.CD; break;
            case Zone.RCD: pct = _chancePerZone.RCD; break;
            case Zone.RD: pct = _chancePerZone.RD; break;
            case Zone.LDM: pct = _chancePerZone.LDM; break;
            case Zone.LCDM: pct = _chancePerZone.LCDM; break;
            case Zone.CDM: pct = _chancePerZone.CDM; break;
            case Zone.RCDM: pct = _chancePerZone.RCDM; break;
            case Zone.RDM: pct = _chancePerZone.RDM; break;
            case Zone.LM: pct = _chancePerZone.LM; break;
            case Zone.LCM: pct = _chancePerZone.LCM; break;
            case Zone.CM: pct = _chancePerZone.CM; break;
            case Zone.RCM: pct = _chancePerZone.RCM; break;
            case Zone.RM: pct = _chancePerZone.RM; break;
            case Zone.LAM: pct = _chancePerZone.LAM; break;
            case Zone.LCAM: pct = _chancePerZone.LCAM; break;
            case Zone.CAM: pct = _chancePerZone.CAM; break;
            case Zone.RCAM: pct = _chancePerZone.RCAM; break;
            case Zone.RAM: pct = _chancePerZone.RAM; break;
            case Zone.LF: pct = _chancePerZone.LF; break;
            case Zone.LCF: pct = _chancePerZone.LCF; break;
            case Zone.CF: pct = _chancePerZone.CF; break;
            case Zone.RCF: pct = _chancePerZone.RCF; break;
            case Zone.RF: pct = _chancePerZone.RF; break;
            case Zone.ALF: pct = _chancePerZone.ALF; break;
            case Zone.ARF: pct = _chancePerZone.ARF; break;
            case Zone.Box: pct = _chancePerZone.Box; break;
        }
        
		if(Zone == Zone.OwnGoal && _zone != Zone.OwnGoal && pct > 0)
		{
			Debug.LogFormat("{0}   CHANCE: {1}     TEAM STRATEGY: {2}   ZONE: {3}", FullName, pct, Team.Strategy.ToString(), _zone.ToString());
		}
        
        return pct;
    }

	public float GetActionChance(PlayerAction _action, ActionChancePerZoneTable.Actions _zoneChance, MarkingType _marking, Zone _zone)
    {
        float chance = 0f;
        float bonus = 0f;
        Team_Strategy teamStrategy = Team.GetStrategy();

        switch (_action)
        {

            case PlayerAction.Pass:
                chance = _zoneChance.Pass * Prob_Pass;
                bonus = GetAttributeBonus(Passing);
                if (_marking == MarkingType.Close) chance *= 2f;
                if (Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt((chance * 5) + (bonus / 10)), 100) >= 20) chance *= 2f;
                if (Team.IsStrategyApplicable(_zone)) chance *= teamStrategy.PassingChance;
                break;

            case PlayerAction.LongPass:
                float longPass = _zoneChance.LongPass * Prob_LongPass;
                bonus = GetAttributeBonus(Mathf.FloorToInt((float)(Passing + Strength) / 2));
                if (_marking == MarkingType.Close) longPass *= 1.75f;
                if (Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt((longPass * 5) + (bonus / 10)), 100) >= 20) longPass *= 2f;
                if (Team.IsStrategyApplicable(_zone)) chance *= teamStrategy.LongPassChance;
                break;

            case PlayerAction.Dribble:
                chance = _zoneChance.Dribble * Prob_Dribble;
                bonus = GetAttributeBonus(Dribbling);
                if (_marking == MarkingType.Close) chance *= 0.5f;
                else if (_marking == MarkingType.Distance) chance *= 1.5f;
                if (Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt((chance * 5) + (bonus / 10))) >= 20) chance *= 2f;
                if (Team.IsStrategyApplicable(_zone)) chance *= teamStrategy.DribblingChance;
                break;

            case PlayerAction.Cross:
                chance = _zoneChance.Cross * Prob_Crossing;
                bonus = GetAttributeBonus(Crossing);
                if (_marking == MarkingType.Close) chance *= 0.5f;
                if (Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt((chance * 5) + (bonus / 10))) >= 20) chance *= 2f;
                if (Team.IsStrategyApplicable(_zone)) chance *= teamStrategy.CrossingChance;
                break;

            case PlayerAction.Shot:
                chance = _zoneChance.Shot * Prob_Shoot;
                bonus = GetAttributeBonus(Shooting);
                if (_marking == MarkingType.Close) chance *= 0.5f;
                else if (_marking == MarkingType.None) chance *= 3f;
                if (Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt((chance * 5) + (bonus / 10))) >= 20) chance *= 2f;
                if (Team.IsStrategyApplicable(_zone)) chance *= teamStrategy.ShootingChance;
                break;

            case PlayerAction.Header:
                chance = (_zoneChance.Shot + Prob_Shoot) * 1.5f;
                bonus = GetAttributeBonus(Heading);
                if (_marking == MarkingType.Distance) chance *= 2f;
                else if (_marking == MarkingType.None) chance *= 3f;
                if (Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt((chance * 5) + (bonus / 10))) >= 20) chance *= 2f;
                if (Team.IsStrategyApplicable(_zone)) chance *= teamStrategy.ShootingChance;
                break;
        }

        return chance;
    }

    AltPosition GetAltPosition(Zone _zone)
    {
        AltPosition pos = AltPosition.None;
	    Vector2 posMatrix = Field.Instance.Matrix[(int)Zone];
        int posX = (int)posMatrix.x;
        int posY = (int)posMatrix.y;

	    Vector2 altPosMatrix = Field.Instance.Matrix[(int)_zone];
        int altPosX = (int)altPosMatrix.x;
        int altPosY = (int)altPosMatrix.y;

        if(altPosY == posY)
        {
            if (altPosX < posX) pos = AltPosition.Left;
            else if (altPosX > posX) pos = AltPosition.Right;
        }
        else if (altPosY > posY)
        {
            if (altPosX < posX) pos = AltPosition.LeftOffensive;
            else if (altPosX > posX) pos = AltPosition.RightOffensive;
            else if (altPosX == posX) pos = AltPosition.Offensive;
        }
        else if (altPosY < posY)
        {
            if (altPosX < posX) pos = AltPosition.LeftDefensive;
            else if (altPosX > posX) pos = AltPosition.RightDefensive;
            else if (altPosX == posX) pos = AltPosition.Defensive;
        }

        return pos;
    }

    public int GetPlayerAttribute(AttributeType _playerAttributes)
    {
        int value = 0;

        switch(_playerAttributes)
        {
            case AttributeType.Agility: value = Agility; break;
            case AttributeType.Blocking: value = Blocking; break;
            case AttributeType.Crossing: value = Crossing; break;
            case AttributeType.Dribbling: value = Dribbling; break;
            case AttributeType.Freekick: value = Freekick; break;
            case AttributeType.Goalkeeping: value = Goalkeeping; break;
            case AttributeType.Heading: value = Heading; break;
            case AttributeType.Passing: value = Passing; break;
            case AttributeType.Penalty: value = Penalty; break;
            case AttributeType.Shooting: value = Shooting; break;
            case AttributeType.Speed: value = Speed; break;
            case AttributeType.Stability: value = Stability; break;
            case AttributeType.Stamina: value = Stamina; break;
            case AttributeType.Strength: value = Strength; break;
            case AttributeType.Tackling: value = Tackling; break;
            case AttributeType.Teamwork: value = Teamwork; break;
            case AttributeType.Vision: value = Vision; break;
        }

        return value;
    }

    public void ResetStatistics(string _type, string _id="")
    {
        switch(_type)
        {
            default:
            case "Match" : MatchStats = new PlayerStatistics(); break;
            case "LifeTime" : Attributes.LifeTimeStats = new PlayerStatistics(); break;
            case "Tournament" :
                if (TournamentStatistics(_id) != null)
                {
                    PlayerStatistics tStats = TournamentStatistics(_id);
                    tStats = new PlayerStatistics();
                }
                break;
        }
    }

	public void UpdateLifeTimeStats(string _tournamentId)
    {
        PlayerStatistics stats = MatchStats;
        if (Attributes.LifeTimeStats == null) Attributes.LifeTimeStats = new PlayerStatistics();
        UpdateStats(Attributes.LifeTimeStats, stats);
	    UpdateTournamentStatistics(stats, _tournamentId);

        ResetStatistics("Match");
    }

    void UpdateStats(PlayerStatistics _stats, PlayerStatistics _data)
    {
        _stats.MatchesPlayed++;
        _stats.Goals += _data.Goals;
        _stats.Assists += _data.Assists;
        _stats.Passes += _data.Passes;
        _stats.BoxCrosses += _data.BoxCrosses;
        _stats.Crosses += _data.Crosses;
        _stats.Faults += _data.Faults;
        _stats.Tackles += _data.Tackles;
        _stats.Dribbles += _data.Dribbles;
        _stats.Headers += _data.Headers;
        _stats.Saves += _data.Saves;
        _stats.Shots += _data.Shots;
        _stats.ShotsOnGoal += _data.ShotsOnGoal;
        _stats.CrossesMissed += _data.CrossesMissed;
        _stats.DribblesMissed += _data.DribblesMissed;
        _stats.HeadersMissed += _data.HeadersMissed;
        _stats.HeadersOnGoal += _data.HeadersOnGoal;
        _stats.PassesMissed += _data.PassesMissed;
        _stats.ShotsMissed += _data.ShotsMissed;
        _stats.Presence += _data.Presence;
        _stats.AverageRating = (_stats.AverageRating + _data.MatchRating) / _stats.MatchesPlayed;
    }

    public PlayerStatistics GetTournamentStatistics(string _key)
    {
        PlayerStatistics stats = new PlayerStatistics();

        //if (Attributes.TournamentStatistics != null && Attributes.TournamentStatistics.ContainsKey(_key)) stats = Attributes.TournamentStatistics[_key];

        if (Attributes.TournamentStatistics != null)
        {
            foreach(PlayerStatistics tournament in Attributes.TournamentStatistics)
            {
                if(tournament.TournamentID == _key)
                {
                    stats = tournament;
                }
             //   stats = Attributes.TournamentStatistics[_key];
            }
            
        }

        return stats;
    }

	void UpdateTournamentStatistics(PlayerStatistics _stats, string _tournamentid)
    {
        if (Attributes.TournamentStatistics == null) Attributes.TournamentStatistics = new List<PlayerStatistics>();

        //if (!Attributes.TournamentStatistics.ContainsKey(currentTournament.Id))
        //{
        // Attributes.TournamentStatistics.Add(currentTournament.Id, new PlayerStatistics());
        // }

        CheckTournament(_tournamentid);
        
        PlayerStatistics tStats = GetTournamentStatistics(_tournamentid);

        UpdateStats(tStats, _stats);
    }

    public PlayerStatistics TournamentStatistics(string _id)
    {
        foreach(PlayerStatistics stats in Attributes.TournamentStatistics)
        {
            if (stats.TournamentID == _id) return stats;
        }
        return null;
    }

    public void CheckTournament(string _id)
    {
        bool tournamentExists = false;
        foreach (PlayerStatistics tournament in Attributes.TournamentStatistics)
        {
            if (tournament.TournamentID == _id)
            {
                tournamentExists = true;
            }
        }

        if (!tournamentExists)
        {
            Attributes.TournamentStatistics.Add(new PlayerStatistics(_id));
        }
    }
    
    public string FullName
    {
	    get { return FirstName + " " + LastName; } 
    }
    
	public string ShortName
	{
		get { return FirstName[0] + ". " + LastName; } 
	}

    public bool IsWronglyAssigned()
    {
        bool value = false;
        int zone = (int)Zone;

        switch (Position)
        {
            case PlayerPosition.Goalkeeper:
                if (zone != 0) value = true;
                break;

            case PlayerPosition.Defender:
	            if (zone < 3 || zone > 7) value = true;
                break;

            case PlayerPosition.Midfielder:
	            if (zone < 8 || zone > 22) value = true;
                break;

            case PlayerPosition.Forward:
	            if (zone < 23) value = true;
                break;
        }      
        return value;
    }

    public void Reset()
    {
        ResetStatistics("LifeTime");
        Team = null;
        Attributes.TournamentStatistics = new List<PlayerStatistics>();
    }

    public int GetAttributeBonus(int _attribute)
    {
        int bonus = 0;
        if (_attribute > 70)
        {
            bonus = _attribute - 70;
        }

        return bonus;
    }
    
    public int GetSynergyBonus(SynergyGroup _group)
    {
        int value = 0;

        switch(Synergy)
        {
            case SynergyGroup.Evil:
                if (_group == SynergyGroup.Evil) value = -1;
                else if (_group == SynergyGroup.Good) value = -1;
                else if (_group == SynergyGroup.Neutral) value = 0;
                else if (_group == SynergyGroup.NeutralEvil) value = 0;
                else if (_group == SynergyGroup.NeutralGood) value = -1;
                break;

            case SynergyGroup.NeutralGood:
                if (_group == SynergyGroup.Evil) value = -1;
                else if (_group == SynergyGroup.Good) value = +1;
                else if (_group == SynergyGroup.Neutral) value = 0;
                else if (_group == SynergyGroup.NeutralEvil) value = -1;
                else if (_group == SynergyGroup.NeutralGood) value = +1;
                break;

            case SynergyGroup.Good:
                if (_group == SynergyGroup.Evil) value = -1;
                else if (_group == SynergyGroup.Good) value = +1;
                else if (_group == SynergyGroup.Neutral) value = +1;
                else if (_group == SynergyGroup.NeutralEvil) value = 0;
                else if (_group == SynergyGroup.NeutralGood) value = +1;
                break;

            case SynergyGroup.Neutral:
                if (_group == SynergyGroup.Evil) value = 0;
                else if (_group == SynergyGroup.Good) value = +1;
                else if (_group == SynergyGroup.Neutral) value = 0;
                else if (_group == SynergyGroup.NeutralEvil) value = -1;
                else if (_group == SynergyGroup.NeutralGood) value = 0;
                break;

            case SynergyGroup.NeutralEvil:
                if (_group == SynergyGroup.Evil) value = 0;
                else if (_group == SynergyGroup.Good) value = 0;
                else if (_group == SynergyGroup.Neutral) value = -1;
                else if (_group == SynergyGroup.NeutralEvil) value = +1;
                else if (_group == SynergyGroup.NeutralGood) value = -1;
                break;
        }

        return value;
    }

    public float GetStatistic(string _var)
    {
        float value = 0;

        PlayerStatistics stats = Attributes.LifeTimeStats;

        switch(_var)
        {
            case "Passes": value = stats.Passes; break;
            case "Crosses": value = stats.Crosses; break;
            case "BoxCrosses": value = stats.BoxCrosses; break;
            case "Shots": value = stats.Shots; break;
            case "ShotsOnGoal": value = stats.ShotsOnGoal; break;
            case "Headers": value = stats.Headers; break;
            case "HeadersOnGoal": value = stats.HeadersOnGoal; break;
            case "Faults": value = stats.Faults; break;
            case "Tackles": value = stats.Tackles; break;
            case "Dribbles": value = stats.Dribbles; break;
            case "Goals": value = stats.Goals; break;
            case "Saves": value = stats.Saves; break;
            case "PassesMissed": value = stats.PassesMissed; break;
            case "ShotsMissed": value = stats.ShotsMissed; break;
            case "HeadersMissed": value = stats.HeadersMissed; break;
            case "DribblesMissed": value = stats.DribblesMissed; break;
            case "CrossesMissed": value = stats.CrossesMissed; break;
            case "Presence": value = stats.Presence; break;
            case "MatchesPlayed": value = stats.MatchesPlayed; break;
            case "Assists": value = stats.Assists; break;
            case "MatchRating": value = stats.MatchRating; break;
            case "AverageRating": value = stats.AverageRating; break;
        }
        return value;
    }
}

