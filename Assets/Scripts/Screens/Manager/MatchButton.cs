using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchButton : MonoBehaviour
{
	[SerializeField] private TMP_Text tournamentlabel;
	[SerializeField] private TMP_Text homeTeamLabel;
	[SerializeField] private TMP_Text awayTeamLabel;
	
	public void Populate(MatchData _matchData)
	{
		TeamData homeTeam = _matchData.HomeTeam.TeamData;
		TeamData awayTeam = _matchData.AwayTeam.TeamData;
		
		tournamentlabel.text = _matchData.TournamentName;
		homeTeamLabel.text = homeTeam.Name + " (" + homeTeam.OveralRating + ")";
		awayTeamLabel.text = awayTeam.Name + " (" + awayTeam.OveralRating + ")";
	}
}
