using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ActionChancePerZone {
	public string Zone;
	public int Dribble;
	public int Sprint;
	public int Pass;
	public int LongPass;
	public int Cross;
	public int Shot;
	public int Tackle;
}

public class ActionChancePerZoneData : ScriptableObject {
	public List<ActionChancePerZone> actionChancePerZones = new List<ActionChancePerZone>();
}

