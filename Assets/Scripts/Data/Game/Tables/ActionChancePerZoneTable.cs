using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorGUITable;

[CreateAssetMenu(fileName = "ActionChancePerZone", menuName = "Data/Action Chance Per Zone", order = 1)]
public class ActionChancePerZoneTable : ScriptableObject
{
	[System.Serializable]
	public class Actions 
	{
		public Zone Zone;
		public float Dribble;
		public float Sprint;
		public float Pass;
		public float LongPass;
		public float Cross;
		public float Shot;
		public float Tackle;
	}
	
	[Table("Zone:Width(100)", "Dribble", "Sprint", "Pass", "LongPass", "Cross", "Shot", "Tackle")]
	public List<Actions> ActionsPerZone;
}
