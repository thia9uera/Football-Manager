using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Team_Strategy {
	public string Name;
	public string Description;
	public float DefPosChance;
	public float OffPosChance;
	public float LeftPosChance;
	public float RighPosChance;
	public float PassingChance;
	public float ShootingChance;
	public float CrossingChance;
	public float DribblingChance;
	public float OffsideTrickChance;
	public float MarkingChance;
	public float TacklingChance;
	public float Target_OwnGoal;
	public float Target_LD;
	public float Target_CD;
	public float Target_RD;
	public float Target_LDM;
	public float Target_CDM;
	public float Target_RDM;
	public float Target_LM;
	public float Target_CM;
	public float Target_RM;
	public float Target_LAM;
	public float Target_CAM;
	public float Target_RAM;
	public float Target_LF;
	public float Target_CF;
	public float Target_RF;
	public float Target_Box;
	public bool OwnGoal;
	public bool LD;
	public bool CD;
	public bool RD;
	public bool LDM;
	public bool CDM;
	public bool RDM;
	public bool LM;
	public bool CM;
	public bool RM;
	public bool LAM;
	public bool CAM;
	public bool RAM;
	public bool LF;
	public bool CF;
	public bool RF;
	public bool Box;
}

public class Team_StrategyData : ScriptableObject {
	public List<Team_Strategy> team_Strategys = new List<Team_Strategy>();
}

