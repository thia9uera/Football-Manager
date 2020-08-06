using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
	[SerializeField] private Team_StrategyData TeamStrategyData;
	[SerializeField] private Player_StrategyData PlayerStrategyData;
	[SerializeField] private Game_ModifierData GameModifierData;
	[SerializeField] private TargetPassPerZoneData TargetPassPerZoneData;
	[SerializeField] private TargetCrossPerZoneData TargetCrossPerZoneData;
	[SerializeField] private ActionChancePerZoneData ActionChancePerZoneData;
	
	[Space(10)]
	[SerializeField] private PosChanceData[] StrategyPosChanceData;
	
	public Game_Modifier GameModifiers { get { return GameModifierData.game_Modifiers[0]; }}
	public List<Player_Strategy> PlayerStrategies { get { return PlayerStrategyData.player_Strategys; }}
	public List<Team_Strategy> TeamStrategies { get { return TeamStrategyData.team_Strategys; }}
	public List<TargetPassPerZone> TargetPassPerZone { get { return TargetPassPerZoneData.targetPassPerZones;}}
	public List<TargetCrossPerZone> TargetCrossPerZone { get { return TargetCrossPerZoneData.targetCrossPerZones;}}
	public List<ActionChancePerZone> ActionChancePerZone { get { return ActionChancePerZoneData.actionChancePerZones;}}
	
	public Zones GetPosChanceData(TeamStrategy _strategy, Zone _zone)
	{
		foreach(PosChanceData data in GameData.Instance.StrategyPosChanceData)
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