using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorGUITable;

public class GameData : MonoBehaviour
{
	public GameModifiersData GameModifiers;
	[SerializeField] private ActionChancePerZoneTable actionChancePerZone = null;
	[SerializeField] private TeamStrategyData teamStrategyData = null;
	[SerializeField] private PlayerStrategyData playerStrategyData = null;
	[SerializeField] private TargetPassPerZoneData targetPassPerZoneData = null;
	[SerializeField] private CrossTargetPerZoneData targetCrossPerZoneData = null;
	[SerializeField] private FormationsData formationsData = null;
	public GameColors Colors;
	
	[Space(10)]
	[SerializeField] public PosChanceData[] StrategyPosChanceData = null;
	
	
	public List<PlayerStrategyData.PlayerStrategyChances> PlayerStrategies { get { return playerStrategyData.StrategyChances; }}
	public List<Team_Strategy> TeamStrategies { get { return teamStrategyData.team_Strategys; }}
	public List<TargetPassPerZone> TargetPassPerZone { get { return targetPassPerZoneData.targetPassPerZones;}}
	public List<CrossTargetPerZoneData.TargetCrossPerZone> TargetCrossPerZone { get { return targetCrossPerZoneData.CrossTargetChances;}}
	public List<ActionChancePerZoneTable.Actions> ActionChancePerZone { get { return actionChancePerZone.ActionsPerZone;}}
	public FormationsData Formations { get { return formationsData; }}
	
	public Zones GetPosChanceData(TeamStrategy _strategy, Zone _zone)
	{
		foreach(PosChanceData data in StrategyPosChanceData)
		{
			if (data.Strategy == _strategy) 
			{
				return data.posChancePerZones[(int)_zone];
			}
		}
		
		return null;
	}
	
	public static GameData Instance;
	private void Awake()
	{
		if(Instance == null) Instance = this;
	}
}