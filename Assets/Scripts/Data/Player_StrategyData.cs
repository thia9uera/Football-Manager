using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Player_Strategy {
	public string Name;
	public string Description;
	public int DefPosChance;
	public int OffPosChance;
	public int ParPosChance;
	public int PassingChance;
	public int ShootingChance;
	public int CrossingChance;
	public int DribblingChance;
	public int OffsideTrickChance;
	public int MarkingChance;
}

public class Player_StrategyData : ScriptableObject {
	public List<Player_Strategy> player_Strategys = new List<Player_Strategy>();
}

