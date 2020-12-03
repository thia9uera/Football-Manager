using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchSquadScreen : SquadScreen
{
	[SerializeField] private ButtonDefault confirmButton = null;
	[SerializeField] private TMP_Text maxSubsMessage = null;
	
	private List<PlayerData> initialSubs;
	
	private List<PlayerData> playersIn;
	private List<PlayerData> playersOut;
	
	private int substitutionsTotal;
	private int substitutionsCurrent;
	private const int MAX_SUBSTITUTIONS = 3;
	
	private void Awake()
	{
		saveOnHide = false;		
	}
	
	public void Initialize()
	{
		substitutionsTotal = 0;
		maxSubsMessage.text = LocalizationController.Instance.Localize("msg_SubsLimit").Replace("{0}", MAX_SUBSTITUTIONS.ToString());
	}
	
	override public void Show()
	{
		base.Show();
				
		initialSubs = new List<PlayerData>(teamData.Substitutes);
		substitutionsCurrent = 0;
		
		CheckSubsLimit();
	}
	
	public void OnConfirmButtonClick()
	{
		SaveChanges();
		MatchController.Instance.UpdateHomeTeam(teamData, playersIn, playersOut);
	}
	
	public void OnCancelButtonClick()
	{
		substitutionsTotal = substitutionsCurrent;
		MatchController.Instance.UnpauseGame();
	}
	
	override public void AddPlayer(PlayerData _player, SquadEditPlayer _obj, SquadEditPlayer _squadSlot=null) 
	{
		base.AddPlayer(_player, _obj, _squadSlot);
		CheckSubsLimit();
	}
	
	private void CheckSubsLimit()
	{
		List<PlayerData> squadList = new List<PlayerData>(teamData.Squad);
		List<PlayerData> subsList = new List<PlayerData>(teamData.Substitutes);
		
		playersIn = new List<PlayerData>();
		playersOut = new List<PlayerData>();

		foreach (PlayerData player in initialSubs)
		{
			if (!subsList.Contains(player)) playersIn.Add(player);
		}
		foreach (PlayerData player in subsList)
		{
			if (!initialSubs.Contains(player)) playersOut.Add(player);
		}
		
		substitutionsCurrent = playersIn.Count;
		field.MarkPlayersAsNew(playersIn);
		
		var hasReachedSubsLimit = substitutionsCurrent + substitutionsTotal > MAX_SUBSTITUTIONS;
		confirmButton.Enabled = !hasReachedSubsLimit;
		maxSubsMessage.gameObject.SetActive(hasReachedSubsLimit);
	}
	
}
