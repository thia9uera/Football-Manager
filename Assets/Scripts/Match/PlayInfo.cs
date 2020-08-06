﻿public class PlayInfo
{
	public int Turn;
	
	public TeamData AttackingTeam;
	public TeamData DefendingTeam;
	
	public PlayerData Attacker;
	public float AttackerRoll;
	public float AttackingBonusChance;
	public float AttackingBonus;
	public int AttackFatigueRate;
	public PlayerAction OffensiveAction;
	public bool IsActionSuccessful;

	public PlayerData Defender;
	public float DefenderRoll;
	public float DefendingBonusChance;
	public float DefendingBonus;
	public int DefenseFatigueRate;
	public PlayerAction DefensiveAction;
	public bool IsActionDefended;

	public MarkingType Marking;
	public MatchEvent Event;
	public PlayerData Assister;
	public int Excitment;
	public Zone Zone;
	public Zone TargetZone;

	public float CounterAttack;
	
	public static PlayInfo CopyPlay(PlayInfo _original)
	{
		PlayInfo _play = new PlayInfo();
		
		_play.Turn = _original.Turn + 1;
		
		_play.AttackingTeam = _original.AttackingTeam;
		_play.DefendingTeam = _original.DefendingTeam;
		
		_play.Attacker = _original.Attacker;
		_play.AttackerRoll = _original.AttackerRoll;
		_play.AttackingBonusChance = _original.AttackingBonusChance;
		_play.AttackingBonus = _original.AttackingBonus;
		_play.AttackFatigueRate = _original.AttackFatigueRate;
		_play.OffensiveAction = _original.OffensiveAction;
		_play.IsActionSuccessful = _original.IsActionSuccessful;
		
		_play.Defender = _original.Defender;
		_play.DefenderRoll = _original.DefenderRoll;
		_play.DefendingBonusChance = _original.DefendingBonusChance;
		_play.DefendingBonus = _original.DefendingBonus;
		_play.DefenseFatigueRate = _original.DefenseFatigueRate;
		_play.DefensiveAction = _original.DefensiveAction;
		_play.IsActionDefended = _original.IsActionDefended;
		
		_play.Marking = _original.Marking;
		_play.Event = _original.Event;
		_play.Assister = _original.Assister;		
		_play.Excitment = _original.Excitment;
		_play.Zone = _original.Zone;
		_play.TargetZone = _original.TargetZone;
		
		_play.CounterAttack = _original.CounterAttack;
		
		return _play;
	}
}
