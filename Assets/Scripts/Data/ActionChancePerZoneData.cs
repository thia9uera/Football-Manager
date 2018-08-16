using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ActionChancePerZone {
	public string Zone;
	public float Dribble;
	public float Sprint;
	public float Pass;
	public float LongPass;
	public float Cross;
	public float Shot;
	public float Tackle;
}

public class ActionChancePerZoneData : ScriptableObject {
	public List<ActionChancePerZone> actionChancePerZones = new List<ActionChancePerZone>();
}

