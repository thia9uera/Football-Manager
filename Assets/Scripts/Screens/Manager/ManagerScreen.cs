using System.Collections;
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
	
	[Space(10)]
	[SerializeField] private MatchButton nextMatchButton = null;
	
	public override void Show()
	{
		base.Show();
		
		InitializeList();
		Tabs.SelectTab(ScreenType.Squad);
		teamNameLabel.text = MainController.Instance.UserTeam.Name;
		nextMatchButton.Populate(CalendarController.Instance.NextMatch);
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
		ScreenController.Instance.Match.Populate(CalendarController.Instance.NextMatch);
		ScreenController.Instance.ShowScreen(ScreenType.Match);
	}
}
