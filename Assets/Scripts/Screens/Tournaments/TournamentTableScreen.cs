using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentTableScreen : BaseScreen
{
	[SerializeField] private TournamentLeaderboard leaderboard = null;
	[SerializeField] private TournamentInfoFixtures fixtures = null;
	
	[HideInInspector] public TournamentData Data;
	
	public override void Show()
	{
		base.Show();
		List<TeamData> teamList = Data.SortTeamsBy("Points");
		leaderboard.Populate(teamList, Data.Id);	
		
		fixtures.Populate(Data);
	}
	
	
}
	
