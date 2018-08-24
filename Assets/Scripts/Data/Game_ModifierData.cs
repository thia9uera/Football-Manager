using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Game_Modifier {
	public float PositionDebuff;
	public float FaultChance;
	public float AttackBonusLow;
	public float AttackBonusMediun;
	public float AttackBonusHigh;
	public float FatigueLow;
	public float FatigueMedium;
	public float FatigueHigh;
}

public class Game_ModifierData : ScriptableObject {
	public List<Game_Modifier> game_Modifiers = new List<Game_Modifier>();
}

