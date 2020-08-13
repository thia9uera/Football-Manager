using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabScreen : BaseScreen
{
	[Space(10)]
	public TabsController Tabs;
	
	protected List<BaseScreen> screenList;
	protected ScreenType prevScreen;
	protected ScreenType currentScreen;
	
	protected virtual void InitializeList()
	{
		Tabs.InitializeTabs();
		Tabs.manager = this;
	}
	
	public void ShowScreen(ScreenType _type)
	{
		prevScreen = currentScreen;
		foreach (BaseScreen screen in screenList)
		{
			if (screen.Type == _type)
			{
				screen.Show();
				currentScreen = screen.Type;
			}
			else screen.Hide();
		}
		if (prevScreen == ScreenType.None) prevScreen = currentScreen;
	}
}
