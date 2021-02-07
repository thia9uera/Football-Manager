using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorGUITable;

[CreateAssetMenu(fileName = "PlayerStrategyData", menuName = "Data/Player Strategy Data", order = 1)]
public class PlayerStrategyData : ScriptableObject
{
	[System.Serializable]
	public class PlayerStrategyChances
	{
		public PlayerStrategy Strategy;
		public float DefPosChance;
		public float OffPosChance;
		public float LeftPosChance;
		public float RightPosChance;
		public float PassingChance;
		public float LongPassChance;
		public float ShootingChance;
		public float CrossingChance;
		public float DribblingChance;
		public float OffsideTrickChance;
		public float MarkingChance;
		public float TacklingChance;
	}
	
	[Table]
	public List<PlayerStrategyChances> StrategyChances;
}
