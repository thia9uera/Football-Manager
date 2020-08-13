using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentTableScreen : BaseScreen
{
	[SerializeField] private TournamentLeaderboard leaderboard;
	
	public TournamentData Data;
	
	public override void Show()
	{
		base.Show();
		List<TeamData> teamList = Data.SortTeamsBy("Points");
		Debug.Log("TEAM LIST COUNT: " + teamList.Count);
		leaderboard.Populate(teamList, Data.Id);	
		
		Debug.Log("SHOW TOURNAMENT TABLE");
	}
}
	
