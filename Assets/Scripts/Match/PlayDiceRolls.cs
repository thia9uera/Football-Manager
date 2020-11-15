using UnityEngine;

public class PlayDiceRolls
{
	public  PlayInfo GetAttackRollResult(PlayInfo _playInfo)
	{
		float value = _playInfo.AttackerRoll;
		int bonus = _playInfo.AttackingBonusChance;
		bool success = true;
		int excitment = 0;
		
		//Rool dice to check if bonus is applied
		int attackRoll = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(value * 5), bonus);
        
		// 20+ double player attacking
		if (attackRoll >= 20)
		{
			value *= 2;
			excitment = 1;
		}
        
		// 10+ multiplies attack by roll amount
		else if (attackRoll >= 10)
		{
			value *= 1 + (float)(attackRoll - 9) / 100;
			excitment = 0;
		}
        
		// 1- If unmarked = fails, if marked = debuf chances
		else if (attackRoll <= 1)
		{
			if (_playInfo.Defender != null)
			{
				success = false;
				value = 0;
			}
			else
			{
				value  *= 0.75f;
				excitment = -1;
			}
		}
		
		_playInfo.AttackerRoll = value;
		_playInfo.IsActionSuccessful = success;
		_playInfo.OffenseExcitment = excitment;
		return _playInfo;
	}
	
	public PlayInfo GetDefenseRollResult(PlayInfo _playInfo)
	{
		float value = _playInfo.DefenderRoll;
		int bonus = _playInfo.DefendingBonusChance;
		int excitment = 0;
		
		//Rool dice to check if bonus is applied
		int defenseRoll = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(value * 5), bonus);

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
		
		_playInfo.DefenderRoll = value;
		_playInfo.OffenseExcitment = excitment;
		return _playInfo;
	}
	
	public PlayInfo GetShotOnGoalResult(PlayInfo _playInfo)
	{
		//ATTACKER
		
		//Rool dice to check if bonus is applied
		int attackRoll = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(_playInfo.AttackerRoll * 5), _playInfo.AttackingBonusChance);
		Debug.Log("SHOOTER RESULTS");
		
		Debug.Log("Attack Dice Roll: " + attackRoll);
		if (attackRoll >= 20)
		{
			_playInfo.AttackerRoll *= 1.5f;
			_playInfo.OffenseExcitment = 1;
		}
		else if (attackRoll >= 10)
		{
			_playInfo.AttackerRoll *= 1 + (float)(attackRoll - 9) / 100;
			_playInfo.OffenseExcitment = 0;
		}
		else if (attackRoll <= 4)
		{
			_playInfo.Event = MatchEvent.CornerKick;
			_playInfo.AttackerRoll = 0;
			_playInfo.OffenseExcitment = -1;
		}
		Debug.Log("Attack Value: " + _playInfo.AttackerRoll);
		
		//DEFENDER
		
		//Rool dice to check if bonus is applied
		float defenseRoll = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(_playInfo.DefenderRoll * 5), GetPlayerAttributeBonus(_playInfo.Defender.Goalkeeping));
		Debug.Log("DEFENDER RESULTS");
		
		Debug.Log("Defender Dice Roll: " + defenseRoll);
		if (defenseRoll >= 20)
		{
			_playInfo.DefenderRoll *= 2f;
			_playInfo.DefenseExcitment = 1;
		}
		else if (defenseRoll >= 10)
		{
			_playInfo.DefenderRoll *= 1 + (float)(defenseRoll - 9) / 100;
			_playInfo.DefenseExcitment = 0;
		}
		else if (defenseRoll <= 1)
		{
			_playInfo.DefenderRoll *= 0.5f;
			_playInfo.Event = MatchEvent.CornerKick;
			_playInfo.DefenseExcitment = -1;
		}
		
		Debug.Log("Defense Value: " + _playInfo.DefenderRoll);
		
		return _playInfo;
	}
	
	public static int GetPlayerAttributeBonus(int _attribute)
	{
		int bonus = 0;
		if (_attribute > 70)
		{
			bonus = _attribute - 70;
		}

		return bonus;
	}
}
