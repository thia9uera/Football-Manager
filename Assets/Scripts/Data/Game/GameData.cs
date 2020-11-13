using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
	[SerializeField] private Team_StrategyData teamStrategyData;
	[SerializeField] private Player_StrategyData playerStrategyData;
	[SerializeField] private Game_ModifierData gameModifierData;
	[SerializeField] private TargetPassPerZoneData targetPassPerZoneData;
	[SerializeField] private TargetCrossPerZoneData targetCrossPerZoneData;
	[SerializeField] private ActionChancePerZoneData actionChancePerZoneData;
	[SerializeField] private FormationsData formationsData;
	public GameColors Colors;
	
	[Space(10)]
	[SerializeField] private PosChanceData[] StrategyPosChanceData;
	
	public Game_Modifier GameModifiers { get { return gameModifierData.game_Modifiers[0]; }}
	public List<Player_Strategy> PlayerStrategies { get { return playerStrategyData.player_Strategys; }}
	public List<Team_Strategy> TeamStrategies { get { return teamStrategyData.team_Strategys; }}
	public List<TargetPassPerZone> TargetPassPerZone { get { return targetPassPerZoneData.targetPassPerZones;}}
	public List<TargetCrossPerZone> TargetCrossPerZone { get { return targetCrossPerZoneData.targetCrossPerZones;}}
	public List<ActionChancePerZone> ActionChancePerZone { get { return actionChancePerZoneData.actionChancePerZones;}}
	public FormationsData Formations { get { return formationsData; }}
	
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