﻿using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Game_Modifier {
	public float PositionDebuff;
	public float FaultChance;
	public float OffsideChance;
	public float AttackBonusLow;
	public float AttackBonusMedium;
	public float AttackBonusHigh;
	public int FatigueLow;
	public int FatigueMedium;
	public int FatigueHigh;
	public float FatigueRecoverHalfTime;
	public float CounterAttackChance;
}

public class Game_ModifierData : ScriptableObject {
	public List<Game_Modifier> game_Modifiers = new List<Game_Modifier>();
}

