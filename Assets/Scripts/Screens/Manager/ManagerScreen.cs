using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerScreen : TabScreen
{
	[Header("Screens")]
	public SquadScreen SquadScreen;
	public TournamentsScreen TournamentsScreen;
	public SettingsScreen SettingsScreen;
	public CalendarScreen CalendarScreen;
	
	public override void Show()
	{
		base.Show();
		
		InitializeList();
		Tabs.SelectTab(ScreenType.Squad);
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
}
