using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
	[SerializeField] private DefaultTab[] tabList = null;
	
	public TabScreen manager;
	
	private ScreenType selectedTabType;
	
	public void OnTabSelected(ScreenType _screen)
	{
		manager.ShowScreen(_screen);
		selectedTabType = _screen;
		UpdateTabs();
	}
	
	public void SelectTab(ScreenType _screen)
	{
		foreach(DefaultTab tab in tabList)
		{
			if(tab.Screen == _screen) tab.OnValueChanged();
		}
	}
	
	private void UpdateTabs()
	{
		foreach(DefaultTab tab in tabList)
		{
			tab.Enable(tab.Screen != selectedTabType);
		}
	}
	
	public void InitializeTabs()
	{
		foreach(DefaultTab tab in tabList)
		{
			tab.Initialize();
		}
	}
}