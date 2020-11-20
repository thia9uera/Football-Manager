using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
	[SerializeField] private DefaultTab[] tabList = null;
	
	public TabScreen manager;
	
	public ScreenType SelectedTabType = ScreenType.Squad;
	
	public void OnTabSelected(ScreenType _screen)
	{
		manager.ShowScreen(_screen);
		SelectedTabType = _screen;
		UpdateTabs();
	}
	
	public void SelectTab(ScreenType _screen)
	{
		foreach(DefaultTab tab in tabList)
		{
			if(tab.Screen == _screen) tab.OnClickManger();
		}
	}
	
	private void UpdateTabs()
	{
		foreach(DefaultTab tab in tabList)
		{
			tab.Enable(tab.Screen != SelectedTabType);
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