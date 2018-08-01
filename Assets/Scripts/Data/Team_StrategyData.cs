using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Team_Strategy {
	public string Name;
	public string Description;
	public int DefPosChance;
	public int OffPosChance;
	public int PassingChance;
	public int ShootingChance;
	public int CrossingChance;
	public int DribblingChance;
	public int OffsideTrickChance;
	public int MarkingChance;
	public int OwnGoal;
	public int LD;
	public int CD;
	public int RD;
	public int LDM;
	public int CDM;
	public int RDM;
	public int LM;
	public int CM;
	public int RM;
	public int LAM;
	public int CAM;
	public int RAM;
	public int LF;
	public int CF;
	public int RF;
	public int Box;
}

public class Team_StrategyData : ScriptableObject {
	public List<Team_Strategy> team_Strategys = new List<Team_Strategy>();
}

