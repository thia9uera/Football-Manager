using System;
using System.Collections.Generic;
using UnityEngine;
using EditorGUITable;


[CreateAssetMenu(fileName = "GameModifiersData", menuName = "Data/Game Modifiers Data", order = 1)]
public class GameModifiersData : ScriptableObject
{
	[Space(10)]
	[Header("Chances")]
	public float FaultChance;
	public float OffsideChance;
	public float CounterAttackChance;
	
	[Space(10)]
	[Header("Attack Bonus")]
	public float AttackBonusLow;
	public float AttackBonusMedium;
	public float AttackBonusHigh;
	
	[Space(10)]
	[Header("Fatigue")]
	public int FatigueLow;
	public int FatigueMedium;
	public int FatigueHigh;
	public float FatigueRecoverHalfTime;
	
	[Space(20)]
	[Tooltip("% reduction of stats when player is not correctly assigned to their position")]
	public float PositionDebuff;
	
}
