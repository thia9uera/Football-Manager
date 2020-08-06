using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Strategy {
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
	public int Goal;
	public int GK;
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

public class StrategyData : ScriptableObject {
	public List<Strategy> strategys = new List<Strategy>();
}

