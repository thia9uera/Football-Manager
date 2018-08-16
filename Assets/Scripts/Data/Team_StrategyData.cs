using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Team_Strategy {
	public string Name;
	public string Description;
	public float DefPosChance;
	public float OffPosChance;
	public float PassingChance;
	public float ShootingChance;
	public float CrossingChance;
	public float DribblingChance;
	public float OffsideTrickChance;
	public float MarkingChance;
	public float TacklingChance;
	public float OwnGoal;
	public float LD;
	public float CD;
	public float RD;
	public float LDM;
	public float CDM;
	public float RDM;
	public float LM;
	public float CM;
	public float RM;
	public float LAM;
	public float CAM;
	public float RAM;
	public float LF;
	public float CF;
	public float RF;
	public float Box;
}

public class Team_StrategyData : ScriptableObject {
	public List<Team_Strategy> team_Strategys = new List<Team_Strategy>();
}

