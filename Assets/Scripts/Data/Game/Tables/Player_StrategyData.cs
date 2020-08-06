using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Player_Strategy {
	public string Name;
	public string Description;
	public float DefPosChance;
	public float OffPosChance;
	public float LeftPosChance;
	public float RightPosChance;
	public float PassingChance;
	public float LongPassChance;
	public float ShootingChance;
	public float CrossingChance;
	public float DribblingChance;
	public float OffsideTrickChance;
	public float MarkingChance;
	public float TacklingChance;
}

public class Player_StrategyData : ScriptableObject {
	public List<Player_Strategy> player_Strategys = new List<Player_Strategy>();
}

