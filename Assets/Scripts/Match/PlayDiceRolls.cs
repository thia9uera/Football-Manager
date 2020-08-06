using UnityEngine;

public class PlayDiceRolls
{
	private float value;
	private int excitment;
	private bool success;
	
	public  DiceRollResults GetAttackRollResult(float _value, int _bonus, bool _hasDefender)
	{
		value = _value;
		success = true;
		//Rool dice to check if bonus is applied
		int attackRoll = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(_value * 5), _bonus);
        
		// 20+ double player attacking
		if (attackRoll >= 20)
		{
			value *= 2;
			excitment = 1;
		}
        
		//10+ multiplies attack by roll amount
		else if (attackRoll >= 10)
		{
			value *= 1 + (float)(attackRoll - 9) / 100;
			excitment = 0;
		}
        
		//1- If unmarked fails, if marked debuf chances
		else if (attackRoll <= 1)
		{
			if (!_hasDefender)
			{
				success = false;
			}
			else
			{
				value  *= 0.75f;
				excitment = -1;
			}
		}
		
		DiceRollResults results = new DiceRollResults(value, excitment, success);
		return results;
	}
	
	public DiceRollResults GetDefenseRollResult(float _value, int _bonus)
	{
		value = _value;
		success = true;
		
		//Rool dice to check if bonus is applied
		int defenseRoll = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(value * 5), _bonus);

		// 20+ double player defending
		if (defenseRoll >= 20)
		{
			value *= 2f;
			excitment = 1;
		}
		//10+ multiplies defense by roll amount
		else if (defenseRoll >= 10)
		{
			value *= 1 + (float)(defenseRoll - 9) / 100;
			excitment = 0;
		}
		
		//1- Debuf
		else if (defenseRoll <= 1)
		{
			value *= 0.75f;
			excitment = -1;
		}
		
		DiceRollResults results = new DiceRollResults(value, excitment, success);
		return results;
	}
	
	public DiceRollResults GetShooterRollResult(float _value, int _bonus, PlayInfo _currentPlay)
	{
		value = _value;
		PlayInfo currentPlay = _currentPlay;
		int attackRoll = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(_value * 5), _bonus);

		if (attackRoll >= 20)
		{
			value *= 1.5f;
			currentPlay.Excitment = 1;
		}
		else if (attackRoll >= 10)
		{
			value *= 1 + (float)(attackRoll - 9) / 100;
			currentPlay.Excitment = 0;
		}
		else if (attackRoll <= 4)
		{
			// SwitchPossesion();
			currentPlay.Event = MatchEvent.CornerKick;
			if (currentPlay.OffensiveAction == PlayerAction.Shot)
			{
				currentPlay.Attacker.MatchStats.ShotsMissed++;
				currentPlay.Attacker.Team.MatchStats.ShotsMissed++;
			}
			else if (currentPlay.OffensiveAction == PlayerAction.Header)
			{
				currentPlay.Attacker.MatchStats.HeadersMissed++;
				currentPlay.Attacker.Team.MatchStats.HeadersMissed++;
			}
		}
		
		DiceRollResults results = new DiceRollResults(value, excitment, success, currentPlay);
		return results;
	}
}

public class DiceRollResults
{
	public float Value;
	public int Excitment;
	public bool Success;
	public PlayInfo CurrentPlay;
	
	public DiceRollResults (float _value, int _excitment, bool _success, PlayInfo _currentPlay = null)
	{
		Value = _value;
		Excitment = _excitment;
		Success = _success;
		CurrentPlay = _currentPlay;
	}
}
