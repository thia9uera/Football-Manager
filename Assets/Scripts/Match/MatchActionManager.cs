using UnityEngine;
using System.Collections.Generic;

public class MatchActionManager
{       
	//Modifiers
	private Game_Modifier modifiers;
	private float positionDebuff;
	private float attackingBonusLow;
	private float attackingBonusMedium;
	private float attackingBonusHigh;
	private int fatigueLow;
	private int fatigueMedium;
	private int fatigueHigh;
	private float offsideChance;
	private float faultChance;
	private float counterAttackChance;
	
	private PlayDiceRolls playDiceRolls;
	
	public MatchActionManager()
	{
		modifiers = GameData.Instance.GameModifiers;
		positionDebuff = modifiers.PositionDebuff;
		attackingBonusLow = modifiers.AttackBonusLow;
		attackingBonusMedium = modifiers.AttackBonusMedium;
		attackingBonusHigh = modifiers.AttackBonusHigh;
		fatigueLow = modifiers.FatigueLow;
		fatigueMedium = modifiers.FatigueMedium;
		fatigueHigh = modifiers.FatigueHigh;
		offsideChance = modifiers.OffsideChance;
		faultChance = modifiers.FaultChance;
		counterAttackChance = modifiers.CounterAttackChance;
		
		playDiceRolls = new PlayDiceRolls();
	}
	
	public PlayInfo GetOffensiveActionResults(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{		
		switch (_currentPlay.OffensiveAction)
		{
			case PlayerAction.Pass:
				_currentPlay.AttackFatigueRate = fatigueLow;
				_currentPlay = GetPassResults(_currentPlay, _lastPlay);
				break;
		
			case PlayerAction.LongPass:
				_currentPlay.AttackFatigueRate = fatigueMedium;
				_currentPlay = GetLongPassResults(_currentPlay);
				break;
		
			case PlayerAction.Dribble:
				_currentPlay.AttackFatigueRate = fatigueHigh;
				_currentPlay = GetDribbleResults(_currentPlay);
				break;
		
			case PlayerAction.Sprint:
				_currentPlay.AttackFatigueRate = fatigueMedium;
				_currentPlay = GetSprintResults(_currentPlay);
				break;
		
			case PlayerAction.Cross:
				_currentPlay.AttackFatigueRate = fatigueLow;
				_currentPlay = GetCrossResults(_currentPlay);              
				break;
		
			case PlayerAction.Shot:
				_currentPlay.AttackFatigueRate = fatigueLow;
				_currentPlay = GetShotResults(_currentPlay);	            
				break;
	
			case PlayerAction.Header:
				_currentPlay.AttackFatigueRate = fatigueMedium;
				_currentPlay = GetHeaderResults(_currentPlay);
				break;
		}
		
		return _currentPlay;
	}
	
	public PlayerAction GetOffensiveAction(MarkingType _marking, PlayerData _player, Zone _zone, bool _headerAvailable, float _counterAttack)
	{
		Zone zone = _player.Team.GetTeamZone(_zone);
		float bonus = 0;

		ActionChancePerZone zoneChance = GameData.Instance.ActionChancePerZone[(int)zone];

		float pass = _player.GetActionChance(PlayerAction.Pass, zoneChance, _marking, zone);
		float longPass = _player.GetActionChance(PlayerAction.LongPass, zoneChance, _marking, zone);
		float dribble = _player.GetActionChance(PlayerAction.Dribble, zoneChance, _marking, zone);
		float cross = _player.GetActionChance(PlayerAction.Cross, zoneChance, _marking, zone);
		float shoot = _player.GetActionChance(PlayerAction.Shot, zoneChance, _marking, zone);

		float header = 0f;
		if (_headerAvailable)
		{
			header = _player.GetActionChance(PlayerAction.Header, zoneChance, _marking, zone);
		}

		if (_counterAttack > 0)
		{
			cross *= 1.5f;
			longPass *= 1.5f;
		}

		if (_player.Zone == Zone.OwnGoal)
		{
			dribble = 0;
			shoot = 0;
			header = 0;
		}

		float sprint = 0f;
		if (_marking == MarkingType.None)
		{
			sprint = dribble * 1.5f; ;
			dribble = 0f;

			bonus = GetPlayerAttributeBonus(_player.Speed);
			if (Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt((sprint * 5) + (bonus / 10))) >= 20) sprint *= 2f;
		}

		float total = pass + longPass + dribble + cross + shoot + header + sprint;
		pass /= total;
		longPass /= total;
		dribble /= total;
		cross /= total;
		shoot /= total;
		header /= total;
		sprint /= total;

		List<KeyValuePair<PlayerAction, float>> list = new List<KeyValuePair<PlayerAction, float>>
		{
			new KeyValuePair<PlayerAction, float>(PlayerAction.Pass, pass),
			new KeyValuePair<PlayerAction, float>(PlayerAction.LongPass, longPass),
			new KeyValuePair<PlayerAction, float>(PlayerAction.Dribble, dribble),
			new KeyValuePair<PlayerAction, float>(PlayerAction.Cross, cross),
			new KeyValuePair<PlayerAction, float>(PlayerAction.Shot, shoot),
			new KeyValuePair<PlayerAction, float>(PlayerAction.Header, header),
			new KeyValuePair<PlayerAction, float>(PlayerAction.Sprint, sprint)
        };

		float random = Random.Range(0.00001f, 1f);
		float cumulative = 0f;
		PlayerAction action = PlayerAction.None;
		for (int i = 0; i < list.Count; i++)
		{
			cumulative += list[i].Value;
			if (random < cumulative)
			{
				action = list[i].Key;
				break;
			}
		}

		return action;
	}
	
	private PlayInfo GetPassResults(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		if (_lastPlay != null &&_lastPlay.OffensiveAction == PlayerAction.Cross && _lastPlay.IsActionSuccessful)
		{
			_currentPlay.AttackerRoll = (float)(attacker.Passing + attacker.Agility + attacker.Vision + attacker.Teamwork + attacker.Heading) / 500;
			if (defender != null)
			{
				_currentPlay.DefensiveAction = PlayerAction.Block;
				_currentPlay.DefenderRoll = (float)(defender.Blocking + defender.Agility + defender.Vision + defender.Heading) / 400;
			}
		}
		else
		{
			_currentPlay.AttackerRoll = (float)(attacker.Passing + attacker.Agility + attacker.Vision + attacker.Teamwork) / 400;
			if (defender != null)
			{
				_currentPlay.DefensiveAction = PlayerAction.Block;
				_currentPlay.DefenderRoll = (float)(defender.Blocking + defender.Agility + defender.Vision) / 300;
			}
		}

		_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Passing);

		if (_currentPlay.Marking == MarkingType.Close) _currentPlay.AttackingBonusChance = Mathf.FloorToInt(_currentPlay.AttackingBonusChance * 0.9f);
		if (defender != null) _currentPlay.DefendingBonusChance = GetPlayerAttributeBonus(defender.Blocking);
		
		return ApplyBuffs(_currentPlay);
	}
	
	private PlayInfo GetLongPassResults(PlayInfo _currentPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		_currentPlay.AttackerRoll = (float)(attacker.Passing + attacker.Agility + attacker.Vision + attacker.Teamwork + attacker.Strength) / 500;
		if (defender != null)
		{
			_currentPlay.DefensiveAction = PlayerAction.Block;
			_currentPlay.DefenderRoll = (float)(defender.Blocking + defender.Agility + defender.Vision) / 300;
		}

		_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Passing);

		if (_currentPlay.Marking == MarkingType.Close) _currentPlay.AttackerRoll *= 0.9f;
		if (defender != null) _currentPlay.DefendingBonusChance = GetPlayerAttributeBonus(defender.Blocking);
		
		return ApplyBuffs(_currentPlay);
	}
	
	private PlayInfo GetDribbleResults(PlayInfo _currentPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		if (defender != null)
		{
			_currentPlay.DefensiveAction = PlayerAction.Tackle;
			_currentPlay.DefenderRoll = (float)(defender.Tackling + defender.Agility + defender.Speed) / 300;
			_currentPlay.DefendingBonusChance = GetPlayerAttributeBonus(defender.Tackling);
		}

		_currentPlay.AttackerRoll = (float)(attacker.Dribbling + attacker.Agility + attacker.Speed) / 300;
		_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Tackling);


		if (_currentPlay.Marking == MarkingType.Close) _currentPlay.AttackerRoll *= 0.75f;
		
		return ApplyBuffs(_currentPlay);
	}
	
	private PlayInfo GetSprintResults(PlayInfo _currentPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		_currentPlay.AttackerRoll = (float)(attacker.Agility + attacker.Speed) / 200;
		_currentPlay.AttackingBonusChance = 100;
		_currentPlay.AttackerRoll *= 2f;
		
		return ApplyBuffs(_currentPlay);
	}
	
	private PlayInfo GetCrossResults(PlayInfo _currentPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		if (defender != null)
		{
			_currentPlay.DefensiveAction = PlayerAction.Block;
			_currentPlay.DefenderRoll = (float)(defender.Blocking + defender.Agility + defender.Vision) / 300;
			_currentPlay.DefendingBonusChance = GetPlayerAttributeBonus(defender.Blocking);
		}

		_currentPlay.AttackerRoll = (float)(attacker.Crossing + attacker.Agility + attacker.Vision + attacker.Teamwork) / 400;
		_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Crossing);
		if (_currentPlay.Marking == MarkingType.Close) _currentPlay.AttackerRoll *= 0.75f;
		
		return ApplyBuffs(_currentPlay);
	}
	
	private PlayInfo GetShotResults(PlayInfo _currentPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		if (defender != null)
		{
			_currentPlay.DefensiveAction = PlayerAction.Block;
			_currentPlay.DefenderRoll = (float)(defender.Blocking + defender.Agility + defender.Vision + defender.Speed) / 400;
			_currentPlay.DefendingBonusChance = GetPlayerAttributeBonus(defender.Blocking);
		}

		_currentPlay.AttackerRoll = (float)(attacker.Shooting + attacker.Agility + attacker.Strength) / 300;
		_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Shooting);
		if (_currentPlay.Marking == MarkingType.Close) _currentPlay.AttackerRoll *= 0.75f;
		
		return ApplyBuffs(_currentPlay);
	}
	
	private PlayInfo GetHeaderResults(PlayInfo _currentPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		if (defender != null)
		{
			_currentPlay.DefensiveAction = PlayerAction.Block;
			_currentPlay.DefenderRoll = (float)(defender.Heading + defender.Blocking + defender.Agility + defender.Vision) / 400;
			_currentPlay.DefendingBonusChance = GetPlayerAttributeBonus(defender.Blocking);
		}

		_currentPlay.AttackerRoll = (float)(attacker.Heading + attacker.Agility + attacker.Strength) / 300;
		_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Heading);
                
		if (_currentPlay.Marking == MarkingType.Close) _currentPlay.AttackerRoll *= 0.75f;
		
		return ApplyBuffs(_currentPlay);
	}
	
	public PlayInfo GetShotResults(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		float distanceModifier = 1f;
		int bonusChance = 0;

		Zone zone = attacker.Team.GetTeamZone(_currentPlay.Zone);

		switch (zone)
		{
			default:
			distanceModifier = 0.1f;
			break;

			case Zone.Box:
			distanceModifier = 1f;
			break;

			case Zone.LAM:
			case Zone.RAM:
			distanceModifier = 0.5f;
			break;

			case Zone.LCAM:
			case Zone.CAM:
			case Zone.RCAM:
			distanceModifier = 0.65f;
			break;

			case Zone.LF:
			case Zone.RF:
			distanceModifier = 0.75f;
			break;

			case Zone.ALF:
			case Zone.ARF:
			distanceModifier = 0.35f;
			break;

			case Zone.LCF:
			case Zone.CF:
			case Zone.RCF:
			distanceModifier = 0.8f;
			break;
		}

		if (_currentPlay.OffensiveAction == PlayerAction.Shot)
		{
			if (_lastPlay.Event == MatchEvent.Fault)
			{
				_currentPlay.AttackerRoll = (float)(attacker.Freekick + attacker.Strength) / 200;
				bonusChance = GetPlayerAttributeBonus(attacker.Freekick);
			}
			else if (_lastPlay.Event == MatchEvent.Penalty)
			{
				_currentPlay.AttackerRoll = (float)(attacker.Penalty + attacker.Strength) / 200;
				bonusChance = GetPlayerAttributeBonus(attacker.Penalty);
			}
			else
			{
				_currentPlay.AttackerRoll = (float)(attacker.Shooting + attacker.Strength) / 200;
				bonusChance = GetPlayerAttributeBonus(attacker.Shooting);
			}
		}
		else if (_currentPlay.OffensiveAction == PlayerAction.Header)
		{
			_currentPlay.AttackerRoll = (float)(attacker.Heading + attacker.Strength) / 200;
			bonusChance = GetPlayerAttributeBonus(attacker.Heading);
		}

		_currentPlay.AttackerRoll *= attacker.FatigueModifier();
		_currentPlay.AttackerRoll *= distanceModifier;

		if (_currentPlay.Marking == MarkingType.Close) _currentPlay.AttackerRoll *= 0.5f;
		_currentPlay.AttackerRoll *= _currentPlay.AttackingBonus;

		DiceRollResults attackRoll =  playDiceRolls.GetShooterRollResult(_currentPlay.AttackerRoll, bonusChance, _currentPlay);
		_currentPlay.AttackerRoll = attackRoll.Value;
		_currentPlay = attackRoll.CurrentPlay;

		_currentPlay.DefenderRoll = ((float)defender.Goalkeeping + defender.Agility) / 200;
		_currentPlay.DefenderRoll *= defender.FatigueModifier();
		float defenseRoll = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(_currentPlay.DefenderRoll * 5), GetPlayerAttributeBonus(defender.Goalkeeping));

		int defenseExcitement = 0;
		if (defenseRoll >= 20)
		{
			_currentPlay.DefenderRoll *= 2f;
			defenseExcitement = 1;
		}
		else if (defenseRoll >= 10)
		{
			_currentPlay.DefenderRoll *= 1 + (float)(defenseRoll - 9) / 100;
			defenseExcitement = 0;
		}
		else if (defenseRoll <= 1)
		{
			_currentPlay.DefenderRoll *= 0.5f;
			defenseExcitement = -1;
		}

		//CHECK ATTACKING X DEFENDING
		if (_currentPlay.AttackerRoll  <= _currentPlay.DefenderRoll)
		{
			_currentPlay.Excitment = defenseExcitement;

			int roll = Dice.Roll(20);
			if (defenseExcitement == -1) _currentPlay.Event = MatchEvent.CornerKick;
			else if (defenseExcitement == 0 && roll < 5) _currentPlay.Event = MatchEvent.CornerKick;
			else
			{
				_currentPlay.Event = _lastPlay.Event == MatchEvent.Penalty ? MatchEvent.PenaltySaved : MatchEvent.ShotSaved;
				if (_currentPlay.OffensiveAction == PlayerAction.Shot) _currentPlay.Attacker.MatchStats.ShotsOnGoal++;
				else if (_currentPlay.OffensiveAction == PlayerAction.Header) _currentPlay.Attacker.MatchStats.HeadersOnGoal++;
			}
			_currentPlay.IsActionSuccessful =  false;
			if (_currentPlay.Event == MatchEvent.CornerKick)
			{
				if (_currentPlay.OffensiveAction == PlayerAction.Shot)
				{
					_currentPlay.Attacker.MatchStats.ShotsMissed++;
					_currentPlay.Attacker.Team.MatchStats.ShotsMissed++;
				}
				else if (_currentPlay.OffensiveAction == PlayerAction.Header)
				{
					_currentPlay.Attacker.MatchStats.HeadersMissed++;
					_currentPlay.Attacker.Team.MatchStats.HeadersMissed++;
				}
			}
		}
		else
		{
			_currentPlay.IsActionSuccessful = true;
			_currentPlay.Event = MatchEvent.Goal;
		}
		
		return _currentPlay;
	}
	
	public PlayInfo GetActionSuccess(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = (_currentPlay.Defender != null) ? _currentPlay.Defender : null;
		
		Zone zone =_currentPlay.TargetZone = _currentPlay.Zone;
		PlayerAction lastAction = PlayerAction.None;
		bool isLastActionSuccessful = false;
		MarkingType marking = _currentPlay.Marking;
		if (_lastPlay != null)
		{
			lastAction = _lastPlay.OffensiveAction;
			isLastActionSuccessful = _lastPlay.IsActionSuccessful;
		}
        
		bool isTackling = false;
		float agilityBonus;        
		int attackBonusChance = 0;
		int defenseBonusChance = 0;     
		_currentPlay.AttackFatigueRate = fatigueLow;
		_currentPlay.DefenseFatigueRate = fatigueLow;
		int attackExcitment = 0;
		int defenseExcitement = 0;
		float fault = faultChance;
		_currentPlay.AttackingBonus = _lastPlay != null ? _lastPlay.AttackingBonus : 1;
		
		TeamData attackingTeam = _currentPlay.Attacker.Team;
		TeamData defendingTeam = (_currentPlay.Defender != null) ? _currentPlay.Defender.Team : null;
	    
		//Check if offside
		if ((int)attackingTeam.GetTeamZone(zone) > 14 && _currentPlay.Event == MatchEvent.None) //AFTER MIDFIELD
		{
			float offside = offsideChance;
			if (defender != null)
			{
				offside *= defender.Prob_OffsideLine;
				if (defender.Team.IsStrategyApplicable(defendingTeam.GetTeamZone(zone)))
					offside *= GameData.Instance.TeamStrategies[(int)defender.Team.Strategy].OffsideTrickChance;
			}
            
			if (offside >= Random.Range(0f, 1f))
			{
				_currentPlay.Event = MatchEvent.Offside;
				_currentPlay.IsActionSuccessful = false;
				return _currentPlay;
			}
		}
        
		//If attacker has no action = fail
		if(_currentPlay.OffensiveAction == PlayerAction.None)
		{
			_currentPlay.IsActionSuccessful = false;
			return _currentPlay;
		}
		else _currentPlay = GetOffensiveActionResults(_currentPlay, _lastPlay);
        
		//Rool dice to check if bonus is applied
		DiceRollResults attackRoll = playDiceRolls.GetAttackRollResult(_currentPlay.AttackerRoll, attackBonusChance, defender != null);
		if(!attackRoll.Success)
		{
			_currentPlay.IsActionSuccessful = false;
			return _currentPlay;
		}
		
		attackExcitment = attackRoll.Excitment;
		_currentPlay.AttackerRoll = attackRoll.Value;
		_currentPlay.AttackerRoll *= attacker.FatigueModifier();
		
		//Check if tackling is really happening  
		if (defender == null)
		{
			isTackling = false;
			_currentPlay.DefensiveAction = PlayerAction.None;
		}
		else
		{
			float tackleChance = 0.75f * GameData.Instance.ActionChancePerZone[(int)zone].Tackle * defender.Prob_Tackling;
			if (marking == MarkingType.Close) tackleChance *= 1.25f;

			if (defender.Team.IsStrategyApplicable(defendingTeam.GetTeamZone(zone)))
			{
				tackleChance *= GameData.Instance.TeamStrategies[(int)defender.Team.Strategy].TacklingChance;
			}

			isTackling |= tackleChance >= Random.Range(0f, 1f);
		}

		if (isTackling)
		{
			//Roll dice
			DiceRollResults defenseRoll = playDiceRolls.GetDefenseRollResult(_currentPlay.DefenderRoll, defenseBonusChance);
			_currentPlay.DefenderRoll = defenseRoll.Value;
			defenseExcitement = defenseRoll.Excitment;

			agilityBonus = (float)defender.GetAttributeBonus(defender.Agility) / 100;
			agilityBonus *= defender.FatigueModifier();
			fault *= (1f - agilityBonus);
			
			_currentPlay.DefenderRoll *= defender.FatigueModifier();
			_currentPlay.DefenseFatigueRate = fatigueMedium;
            

			//Check if tackle resulted in a fault
			if (fault >= Random.Range(0f, 1f))
			{
				if (_currentPlay.AttackingTeam.GetTeamZone(zone) == Zone.Box) _currentPlay.Event = MatchEvent.Penalty;
				else _currentPlay.Event = MatchEvent.Fault;

				_currentPlay.IsActionSuccessful = false;
			}

			else
			{
				_currentPlay.IsActionSuccessful |= _currentPlay.AttackerRoll > _currentPlay.DefenderRoll;
				_currentPlay.IsActionDefended = !_currentPlay.IsActionSuccessful;
			}

			defender.MatchStats.Tackles++;
		}

		else
		{
			float difficulty = Random.Range(0f, 1f);
			float bonus = (float)attacker.GetOverall() / 100;
			if (bonus > 0) difficulty -= bonus;

			_currentPlay.IsActionSuccessful |= _currentPlay.AttackerRoll > difficulty;
		}

		attacker.Fatigue -= _currentPlay.AttackFatigueRate * (25 / (float)attacker.Stamina);

		if (defender == null) _currentPlay.DefensiveAction = PlayerAction.None;
		else defender.Fatigue -= _currentPlay.DefenseFatigueRate * (25 / (float)defender.Stamina);

		if (_currentPlay.IsActionSuccessful) _currentPlay.Excitment = attackExcitment;
		else _currentPlay.Excitment = defenseExcitement;

		return _currentPlay;
	}
	
	public PlayInfo ResolveAction(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{
		PlayInfo currentPlay = _currentPlay;
		PlayInfo lastPlay = _lastPlay;
        
		MarkingType marking = currentPlay.Marking;
		PlayerAction offensiveAction = currentPlay.OffensiveAction;
		
		currentPlay = GetActionSuccess(currentPlay, lastPlay);
	    
		if (currentPlay.IsActionSuccessful)
		{
			if (offensiveAction == PlayerAction.Shot || offensiveAction == PlayerAction.Header) currentPlay.Event = MatchEvent.ShotOnGoal;
			else 
			{
				currentPlay.TargetZone = Field.Instance.GetTargetZone(currentPlay.Zone, currentPlay.Event, currentPlay.OffensiveAction, currentPlay.Attacker.Team.Strategy);
				switch(currentPlay.OffensiveAction)
				{
					case PlayerAction.LongPass:
					case PlayerAction.Pass:
						currentPlay.Attacker.MatchStats.Passes++;
						currentPlay.Attacker.Team.MatchStats.Passes++;
						break;
					case PlayerAction.Cross:
						currentPlay.Attacker.MatchStats.Crosses++;
						currentPlay.Attacker.Team.MatchStats.Crosses++;
						if(currentPlay.AttackingTeam.GetTeamZone(currentPlay.TargetZone) == Zone.Box)
						{
							currentPlay.Attacker.MatchStats.BoxCrosses++;
							currentPlay.Attacker.Team.MatchStats.BoxCrosses++;
						}
						break;
					case PlayerAction.Dribble: currentPlay.Attacker.MatchStats.Dribbles++; break;
				}
			}       

		}
		else
		{
			currentPlay.AttackingBonus = 1f;
			if (offensiveAction == PlayerAction.Shot || offensiveAction == PlayerAction.Header) currentPlay.Event = MatchEvent.ShotMissed;

			switch (currentPlay.OffensiveAction)
			{
			case PlayerAction.LongPass:
			case PlayerAction.Pass:
				currentPlay.Attacker.MatchStats.PassesMissed++;
				currentPlay.Attacker.Team.MatchStats.PassesMissed++;
				break;
			case PlayerAction.Cross:
				currentPlay.Attacker.MatchStats.CrossesMissed++;
				currentPlay.Attacker.Team.MatchStats.CrossesMissed++;
				break;
			case PlayerAction.Dribble: currentPlay.Attacker.MatchStats.DribblesMissed++; break;
			}

			if (currentPlay.Event == MatchEvent.Fault)
			{
				currentPlay.Defender.MatchStats.Faults++;
				currentPlay.Defender.Team.MatchStats.Faults++;
			}
			else
			{
				if(currentPlay.Defender != null) currentPlay.CounterAttack = CheckCounterAttack(currentPlay.Defender.Team, currentPlay.DefendingTeam.GetTeamZone(currentPlay.Zone));
			}
		}
		
		return currentPlay;
	}
	
	public float CheckCounterAttack(TeamData _defendingTeam, Zone _zone)
	{
		float counterAttack = counterAttackChance;
		counterAttack *= GameData.Instance.TeamStrategies[(int)_defendingTeam.Strategy].CounterAttackChance;
		float counterRoll = Random.Range(0, 1f);

		if ((int)_defendingTeam.GetTeamZone(_zone) < 17 && counterAttack > counterRoll)
		{
			counterAttack = 4;
			_defendingTeam.MatchStats.CounterAttacks++;
		}
		return counterAttack;
	}
	
	private PlayInfo ApplyBuffs(PlayInfo _currentPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		//Give bonus based on type of marking
		if (_currentPlay.Marking == MarkingType.Close) _currentPlay.AttackingBonus *= attackingBonusHigh;
		else if (_currentPlay.Marking == MarkingType.Distance) _currentPlay.AttackingBonus *=  attackingBonusMedium;
		else if (_currentPlay.Marking == MarkingType.None) _currentPlay.AttackingBonus *=  attackingBonusLow;
		// In case defender is the Goalkeeper
		if (defender != null && defender.Zone == Zone.OwnGoal)
		{
			_currentPlay.DefenderRoll = (float)(defender.Goalkeeping + defender.Agility + defender.Vision + defender.Speed) / 400;
			_currentPlay.DefendingBonusChance = GetPlayerAttributeBonus(defender.Goalkeeping);
		}
		
		//Apply fatigue modifier to attacking stat
		_currentPlay.AttackerRoll *= attacker.FatigueModifier();
		
		//Apply debuf if player is not assigned to their original position
		if (attacker.IsWronglyAssigned()) _currentPlay.AttackerRoll *= positionDebuff;
		
		//Apply attack bonus
		_currentPlay.AttackerRoll *= _currentPlay.AttackingBonus;
		_currentPlay.AttackingBonus = _currentPlay.AttackingBonus;
		
		return _currentPlay;
	}
   
	private int GetPlayerAttributeBonus(int _attribute)
	{
		int bonus = 0;
		if (_attribute > 70)
		{
			bonus = _attribute - 70;
		}

		return bonus;
	}
}

