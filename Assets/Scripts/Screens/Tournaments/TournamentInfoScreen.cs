using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentInfoScreen : TabScreen
{
	[Header("Screens")]
	public TournamentTableScreen TournamentTable;
	public TournamentLeaderboardScreen TournamentLeaderboard;
	
	[HideInInspector] public TournamentData Data;
	
	override public void Show()
	{
		base.Show();
		InitializeList();
		TournamentTable.Data = Data;
		Tabs.SelectTab(ScreenType.TournamentTable);
	}
	
	public void OnBackButtonClicked()
	{
		ScreenController.Instance.ShowScreen(ScreenType.Manager);
	}
	
	override protected void InitializeList()
	{
		base.InitializeList();
		if(screenList != null) return;
		screenList = new List<BaseScreen>();
		screenList.Add(TournamentTable);
	}
}
