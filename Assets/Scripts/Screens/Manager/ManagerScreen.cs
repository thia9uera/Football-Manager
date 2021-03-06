﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManagerScreen : TabScreen
{
	[Header("Screens")]
	public SquadScreen SquadScreen;
	public TournamentsScreen TournamentsScreen;
	public SettingsScreen SettingsScreen;
	public CalendarScreen CalendarScreen;
	
	[Space(10)]
	[SerializeField] private TMP_Text teamNameLabel = null;
	[SerializeField] private TabsController tabs = null;
	
	[Space(10)]
	[SerializeField] private MatchButton nextMatchButton = null;
	
	private void Awake()
	{
		InitializeList();
	}
	
	public override void Show()
	{
		base.Show();
		
		if(tabs.SelectedTabType == ScreenType.None) Tabs.SelectTab(ScreenType.Squad);
		else Tabs.SelectTab(tabs.SelectedTabType);

		teamNameLabel.text = MainController.Instance.UserTeam.Name + " (" + MainController.Instance.UserTeam.OveralRating + ")";
		nextMatchButton.Populate(CalendarController.Instance.NextUserMatchData);
	}
	
	override protected void InitializeList()
	{
		base.InitializeList();
		if(screenList != null) return;
		screenList = new List<BaseScreen>();
		screenList.Add(SquadScreen);
		screenList.Add(TournamentsScreen);
		screenList.Add(SettingsScreen);
		screenList.Add(CalendarScreen);
	}
	
	public void OnMatchButtonPressed()
	{
		ScreenController.Instance.Manager.SquadScreen.Hide();
		ScreenController.Instance.ShowScreen(ScreenType.Match);
		ScreenController.Instance.Match.Populate(CalendarController.Instance.NextUserMatchData);
	}
}
