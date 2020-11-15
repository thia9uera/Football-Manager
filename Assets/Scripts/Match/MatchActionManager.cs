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
				_currentPlay = GetPassRolls(_currentPlay, _lastPlay);
				break;
		
			case PlayerAction.LongPass:
				_currentPlay.AttackFatigueRate = fatigueMedium;
				_currentPlay = GetLongPassRolls(_currentPlay);
				break;
		
			case PlayerAction.Dribble:
				_currentPlay.AttackFatigueRate = fatigueHigh;
				_currentPlay = GetDribbleRolls(_currentPlay);
				break;
		
			case PlayerAction.Sprint:
				_currentPlay.AttackFatigueRate = fatigueMedium;
				_currentPlay = GetSprintRolls(_currentPlay);
				break;
		
			case PlayerAction.Cross:
				_currentPlay.AttackFatigueRate = fatigueLow;
				_currentPlay = GetCrossRolls(_currentPlay);              
				break;
		
			case PlayerAction.Shot:
				_currentPlay.AttackFatigueRate = fatigueLow;
				_currentPlay = GetShotRolls(_currentPlay);	            
				break;
	
			case PlayerAction.Header:
				_currentPlay.AttackFatigueRate = fatigueMedium;
				_currentPlay = GetHeaderRolls(_currentPlay);
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
	
	private PlayInfo GetPassRolls(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		if (_lastPlay != null &&_lastPlay.OffensiveAction == PlayerAction.Cross && _lastPlay.IsActionSuccessful)
		{
			_currentPlay.AttackerRoll = ActionRoll.HeaderPass(attacker);
			if (defender != null)
			{
				_currentPlay.DefensiveAction = PlayerAction.Block;
				_currentPlay.DefenderRoll = ActionRoll.HeaderBlock(defender);
			}
		}
		else
		{
			_currentPlay.AttackerRoll = ActionRoll.Pass(attacker);
			if (defender != null)
			{
				_currentPlay.DefensiveAction = PlayerAction.Block;
				_currentPlay.DefenderRoll = ActionRoll.Block(defender);
			}
		}

		_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Passing);

		if (_currentPlay.Marking == MarkingType.Close) _currentPlay.AttackingBonusChance = Mathf.FloorToInt(_currentPlay.AttackingBonusChance * 0.9f);
		if (defender != null) _currentPlay.DefendingBonusChance = GetPlayerAttributeBonus(defender.Blocking);
		
		return ApplyBuffs(_currentPlay);
	}
	
	private PlayInfo GetLongPassRolls(PlayInfo _currentPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		_currentPlay.AttackerRoll = ActionRoll.LongPass(attacker);
		if (defender != null)
		{
			_currentPlay.DefensiveAction = PlayerAction.Block;
			_currentPlay.DefenderRoll = ActionRoll.Block(defender);
		}

		_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Passing);

		if (_currentPlay.Marking == MarkingType.Close) _currentPlay.AttackerRoll *= 0.9f;
		if (defender != null) _currentPlay.DefendingBonusChance = GetPlayerAttributeBonus(defender.Blocking);
		
		return ApplyBuffs(_currentPlay);
	}
	
	private PlayInfo GetDribbleRolls(PlayInfo _currentPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		_currentPlay.AttackerRoll = ActionRoll.Dribble(attacker);
		_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Tackling);
		if (_currentPlay.Marking == MarkingType.Close) _currentPlay.AttackerRoll *= 0.75f;
		
		if (defender != null)
		{
			_currentPlay.DefensiveAction = PlayerAction.Tackle;
			_currentPlay.DefenderRoll = ActionRoll.Tackle(defender);
			_currentPlay.DefendingBonusChance = GetPlayerAttributeBonus(defender.Tackling);
		}
		
		return ApplyBuffs(_currentPlay);
	}
	
	private PlayInfo GetSprintRolls(PlayInfo _currentPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		_currentPlay.AttackerRoll = ActionRoll.Sprint(attacker);
		_currentPlay.AttackingBonusChance = 100;
		_currentPlay.AttackerRoll *= 2f;
		
		return ApplyBuffs(_currentPlay);
	}
	
	private PlayInfo GetCrossRolls(PlayInfo _currentPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		if (defender != null)
		{
			_currentPlay.DefensiveAction = PlayerAction.Block;
			_currentPlay.DefenderRoll = ActionRoll.Block(defender);
			_currentPlay.DefendingBonusChance = GetPlayerAttributeBonus(defender.Blocking);
		}

		_currentPlay.AttackerRoll = ActionRoll.Cross(attacker);
		_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Crossing);
		if (_currentPlay.Marking == MarkingType.Close) _currentPlay.AttackerRoll *= 0.75f;
		
		return ApplyBuffs(_currentPlay);
	}
	
	private PlayInfo GetShotRolls(PlayInfo _currentPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		if (defender != null)
		{
			_currentPlay.DefensiveAction = PlayerAction.Block;
			_currentPlay.DefenderRoll = ActionRoll.Block(defender);
			_currentPlay.DefendingBonusChance = GetPlayerAttributeBonus(defender.Blocking);
		}

		_currentPlay.AttackerRoll = ActionRoll.Shoot(attacker);
		_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Shooting);
		if (_currentPlay.Marking == MarkingType.Close) _currentPlay.AttackerRoll *= 0.75f;
		
		return ApplyBuffs(_currentPlay);
	}
	
	private PlayInfo GetHeaderRolls(PlayInfo _currentPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		
		if (defender != null)
		{
			_currentPlay.DefensiveAction = PlayerAction.Block;
			_currentPlay.DefenderRoll = ActionRoll.HeaderBlock(defender);
			_currentPlay.DefendingBonusChance = GetPlayerAttributeBonus(defender.Blocking);
		}

		_currentPlay.AttackerRoll = ActionRoll.Header(attacker);
		_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Heading);
                
		if (_currentPlay.Marking == MarkingType.Close) _currentPlay.AttackerRoll *= 0.75f;
		
		return ApplyBuffs(_currentPlay);
	}
	
	public PlayInfo GetShotResults(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{
		PlayerData attacker = _currentPlay.Attacker;
		PlayerData defender = _currentPlay.Defender;
		Zone zone = attacker.Team.GetTeamZone(_currentPlay.Zone);
		float distanceModifier = DistanceModifiers.ShotModifier(zone);
		
		if (_currentPlay.OffensiveAction == PlayerAction.Shot)
		{
			if (_lastPlay.Event == MatchEvent.Fault)
			{
				_currentPlay.AttackerRoll = ActionRoll.Freekick(attacker);
				_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Freekick);
			}
			else if (_lastPlay.Event == MatchEvent.Penalty)
			{
				_currentPlay.AttackerRoll = ActionRoll.Penalty(attacker);
				_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Penalty);
			}
			else
			{
				_currentPlay.AttackerRoll = ActionRoll.Shoot(attacker);
				_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Shooting);
			}
		}
		else if (_currentPlay.OffensiveAction == PlayerAction.Header)
		{
			_currentPlay.AttackerRoll = ActionRoll.Header(attacker);
			_currentPlay.AttackingBonusChance = GetPlayerAttributeBonus(attacker.Heading);
		}
		
		_currentPlay.AttackerRoll *= attacker.FatigueModifier();
		_currentPlay.AttackerRoll *= distanceModifier;
		_currentPlay.AttackerRoll *= _currentPlay.AttackingBonus;

		if (_currentPlay.Marking == MarkingType.Close) _currentPlay.AttackerRoll *= 0.5f;
		
		_currentPlay.DefenderRoll = ActionRoll.Keeper(defender);
		_currentPlay.DefenderRoll *= defender.FatigueModifier();
		
		//Roll dice for shooter and keeper
		_currentPlay =  playDiceRolls.GetShotOnGoalResult(_currentPlay);
		
		//CHECK ATTACKING X DEFENDING
		if (_currentPlay.AttackerRoll  <= _currentPlay.DefenderRoll)
		{
			int roll = Dice.Roll(20);
			if (_currentPlay.DefenseExcitment == 0 && roll < 5) _currentPlay.Event = MatchEvent.CornerKick;
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
		PlayerData defender = null;
		
		TeamData attackingTeam = _currentPlay.Attacker.Team;
		TeamData defendingTeam = null;
		
		if(_currentPlay.Defender != null)
		{
			defender = _currentPlay.Defender;
			defendingTeam = _currentPlay.DefendingTeam;
		}
		
		Zone zone =_currentPlay.Zone;		
		PlayerAction lastAction = PlayerAction.None;
		bool isLastActionSuccessful = false;
		MarkingType marking = _currentPlay.Marking;		
		bool isTackling = false;
		float fault = faultChance;
		float agilityBonus; 
		
		_currentPlay.AttackFatigueRate = fatigueLow;
		_currentPlay.DefenseFatigueRate = fatigueLow;		
		_currentPlay.AttackingBonus = 1;
		_currentPlay.TargetZone = zone;
		
		if (_lastPlay != null)
		{
			lastAction = _lastPlay.OffensiveAction;
			isLastActionSuccessful = _lastPlay.IsActionSuccessful;
			_currentPlay.AttackingBonus = _lastPlay.AttackingBonus;
		}
	          
		//If attacker has no action = fail
		if(_currentPlay.OffensiveAction == PlayerAction.None)
		{
			_currentPlay.IsActionSuccessful = false;
			return _currentPlay;
		}
		else _currentPlay = GetOffensiveActionResults(_currentPlay, _lastPlay);
		
		_currentPlay = playDiceRolls.GetAttackRollResult(_currentPlay);
		_currentPlay.AttackerRoll *= attacker.FatigueModifier();
		if(_currentPlay.AttackerRoll <= 0) return _currentPlay;
		
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
			_currentPlay = playDiceRolls.GetDefenseRollResult(_currentPlay);

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

		return _currentPlay;
	}
	
	private bool CheckOffside(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{
		if (_lastPlay.OffensiveAction == PlayerAction.Pass || _lastPlay.OffensiveAction == PlayerAction.Cross || _lastPlay.OffensiveAction == PlayerAction.LongPass || _lastPlay.OffensiveAction == PlayerAction.ThroughPass)
		{
			if((int)_currentPlay.AttackingTeam.GetTeamZone(_currentPlay.Zone) > 14 && _currentPlay.Event == MatchEvent.None) //AFTER MIDFIELD
			{
				float offside = offsideChance;
				if (_currentPlay.Defender != null)
				{
					offside *= _currentPlay.Defender.Prob_OffsideLine;
					if (_currentPlay.DefendingTeam.IsStrategyApplicable(_currentPlay.DefendingTeam.GetTeamZone(_currentPlay.Zone)))
						offside *= GameData.Instance.TeamStrategies[(int)_currentPlay.DefendingTeam.Strategy].OffsideTrickChance;
				}
            
				return (offside >= Random.Range(0f, 1f));
			}
			else return false;
		}
		else return false;
		
	}
	
	public PlayInfo ResolveAction(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{
		PlayInfo currentPlay = _currentPlay;
		PlayInfo lastPlay = _lastPlay;
        
		MarkingType marking = currentPlay.Marking;
		PlayerAction offensiveAction = currentPlay.OffensiveAction;

		if(_lastPlay != null && CheckOffside(_currentPlay, _lastPlay))
		{
			_currentPlay.Event = MatchEvent.Offside;
			_currentPlay.IsActionSuccessful = false;
			return _currentPlay;
		}

		currentPlay = GetActionSuccess(currentPlay, lastPlay);
	    
		if (currentPlay.IsActionSuccessful)
		{
			if (offensiveAction == PlayerAction.Shot || offensiveAction == PlayerAction.Header)
			{
				currentPlay.Event = MatchEvent.ShotOnGoal;
			}
			else 
			{
				
				if(currentPlay.Event == MatchEvent.KickOff) currentPlay.TargetZone = Zone.CM;
				else currentPlay.TargetZone = Field.Instance.GetTargetZone(currentPlay.Zone, currentPlay.Event, currentPlay.OffensiveAction, currentPlay.Attacker.Team.Strategy);
				switch(currentPlay.OffensiveAction){
					
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
						
					case PlayerAction.Dribble: 
						currentPlay.Attacker.MatchStats.Dribbles++; 
						break;
				}
			}       
		}
		else
		{
			currentPlay.AttackingBonus = 1f;
			switch (currentPlay.OffensiveAction) {
				
			case PlayerAction.Shot:
			case PlayerAction.Header:
				currentPlay.Event = MatchEvent.ShotMissed;
				break;
				
			case PlayerAction.LongPass:
			case PlayerAction.Pass:
				currentPlay.Attacker.MatchStats.PassesMissed++;
				currentPlay.Attacker.Team.MatchStats.PassesMissed++;
				break;
				
			case PlayerAction.Cross:
				currentPlay.Attacker.MatchStats.CrossesMissed++;
				currentPlay.Attacker.Team.MatchStats.CrossesMissed++;
				break;
				
			case PlayerAction.Dribble:
				currentPlay.Attacker.MatchStats.DribblesMissed++;
				break;
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
		
		return _currentPlay;
	}
   
	private int GetPlayerAttributeBonus(int _attribute)
	{
		return PlayDiceRolls.GetPlayerAttributeBonus(_attribute);
	}
}

