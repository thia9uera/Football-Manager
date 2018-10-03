using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TargetPassPerZone {
	public string Position;
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

public class TargetPassPerZoneData : ScriptableObject {
	public List<TargetPassPerZone> targetPassPerZones = new List<TargetPassPerZone>();
}

