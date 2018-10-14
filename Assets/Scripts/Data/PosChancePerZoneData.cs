using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PosChancePerZone {
	public string Position;
	public float OwnGoal;
	public float BLD;
	public float BRD;
	public float LD;
	public float LCD;
	public float RCD;
	public float RD;
	public float LDM;
	public float LCDM;
	public float RCDM;
	public float RDM;
	public float LM;
	public float LCM;
	public float RCM;
	public float RM;
	public float LAM;
	public float LCAM;
	public float RCAM;
	public float RAM;
	public float LF;
	public float LCF;
	public float RCF;
	public float RF;
	public float ALF;
	public float ARF;
	public float Box;
}

public class PosChancePerZoneData : ScriptableObject {
	public List<PosChancePerZone> posChancePerZones = new List<PosChancePerZone>();
}

