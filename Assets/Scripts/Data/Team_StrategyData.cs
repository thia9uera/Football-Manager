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
	public float LongPassChance;
	public float ShootingChance;
	public float CrossingChance;
	public float DribblingChance;
	public float OffsideTrickChance;
	public float MarkingChance;
	public float TacklingChance;
	public float CounterAttackChance;
	public float Target_OwnGoal;
	public float Target_BLD;
	public float Target_BRD;
	public float Target_LD;
	public float Target_LCD;
	public float Target_RCD;
	public float Target_RD;
	public float Target_LDM;
	public float Target_LCDM;
	public float Target_RCDM;
	public float Target_RDM;
	public float Target_LM;
	public float Target_LCM;
	public float Target_RCM;
	public float Target_RM;
	public float Target_LAM;
	public float Target_LCAM;
	public float Target_RCAM;
	public float Target_RAM;
	public float Target_LF;
	public float Target_LCF;
	public float Target_RCF;
	public float Target_RF;
	public float Target_ALF;
	public float Target_ARF;
	public float Target_Box;
	public bool OwnGoal;
	public bool BLD;
	public bool BRD;
	public bool LD;
	public bool LCD;
	public bool RCD;
	public bool RD;
	public bool LDM;
	public bool LCDM;
	public bool RCDM;
	public bool RDM;
	public bool LM;
	public bool LCM;
	public bool RCM;
	public bool RM;
	public bool LAM;
	public bool LCAM;
	public bool RCAM;
	public bool RAM;
	public bool LF;
	public bool LCF;
	public bool RCF;
	public bool RF;
	public bool ALF;
	public bool ARF;
	public bool Box;
}

public class Team_StrategyData : ScriptableObject {
	public List<Team_Strategy> team_Strategys = new List<Team_Strategy>();
}

