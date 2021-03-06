﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
	public static ScreenController Instance;
	
	[Header("Screens")]
	[Space(5)]
	public LoadingScreen Loading;
	public StartScreen Start;
	public CreateTeamScreen CreateTeam;
	public ManagerScreen Manager;
	public TournamentInfoScreen Tournament;
	public MatchScreen Match;
	
	[Space(10)]
    public MainMenu MainMenu;
    public TournamentHubScreen TournamentHub;   
	public SquadEdit EditSquad;


    [HideInInspector] public ScreenType PrevScreen;
	[HideInInspector] public ScreenType CurrentScreen;
    
	private List<BaseScreen> screenList;
	
	private void Awake()
	{
		if(Instance == null) Instance = this;
		
		screenList = new List<BaseScreen>();
		
		screenList.Add(Loading);
		screenList.Add(Start);
		screenList.Add(CreateTeam);
		screenList.Add(Manager);
		screenList.Add(Tournament);
		screenList.Add(Match);
	}

    public void ShowScreen(ScreenType _type)
    {
        PrevScreen = CurrentScreen;
        foreach (BaseScreen screen in screenList)
        {
            if (screen.Type == _type)
            {
                screen.Show();
                CurrentScreen = screen.Type;
            }
            else screen.Hide();
        }
	    if (PrevScreen == ScreenType.None) PrevScreen = CurrentScreen;
    }

    public void ShowPreviousScreen()
    {
        ShowScreen(PrevScreen);
    }
}
