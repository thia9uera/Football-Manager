using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TackleChancePerZone {
	public string Zone;
	public int Chance;
}

public class TackleChancePerZoneData : ScriptableObject {
	public List<TackleChancePerZone> tackleChancePerZones = new List<TackleChancePerZone>();
}

