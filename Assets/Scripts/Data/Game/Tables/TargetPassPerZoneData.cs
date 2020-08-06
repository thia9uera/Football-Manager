using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TargetPassPerZone {
	public string Position;
	public float OwnGoal;
	public float BLD;
	public float BRD;
	public float LD;
	public float LCD;
	public float CD;
	public float RCD;
	public float RD;
	public float LDM;
	public float LCDM;
	public float CDM;
	public float RCDM;
	public float RDM;
	public float LM;
	public float LCM;
	public float CM;
	public float RCM;
	public float RM;
	public float LAM;
	public float LCAM;
	public float CAM;
	public float RCAM;
	public float RAM;
	public float LF;
	public float LCF;
	public float CF;
	public float RCF;
	public float RF;
	public float ALF;
	public float ARF;
	public float Box;
}

public class TargetPassPerZoneData : ScriptableObject {
	public List<TargetPassPerZone> targetPassPerZones = new List<TargetPassPerZone>();
}

