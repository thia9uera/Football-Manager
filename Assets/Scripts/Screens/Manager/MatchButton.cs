using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchButton : MonoBehaviour
{
	[SerializeField] private TMP_Text tournamentlabel = null;
	[SerializeField] private TMP_Text awayTeamLabel = null;
	
	public void Populate(MatchData _matchData)
	{
		TeamData homeTeam = _matchData.HomeTeam.TeamData;
		TeamData awayTeam = _matchData.AwayTeam.TeamData;
		
		TeamData adversary = homeTeam.IsUserControlled ? awayTeam : homeTeam;
		
		tournamentlabel.text = _matchData.TournamentName;
		awayTeamLabel.text = adversary.Name + " (" + adversary.OveralRating + ")";
	}
}
