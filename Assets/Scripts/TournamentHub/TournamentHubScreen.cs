using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TournamentHubScreen : BaseScreen
{
    [SerializeField]
    TextMeshProUGUI titleLabel;

    TournamentData currentTournament;

    [SerializeField]
    TournamentLeaderboard leaderboard;

    [SerializeField]
    TournamentNextMatch nextMatch;

    TournamentData.MatchData nextMatchData;

    public override void Show()
    {
        base.Show();
        currentTournament = MainController.Instance.CurrentTournament;
        titleLabel.text = currentTournament.Name;

        leaderboard.Populate(currentTournament.TeamScoreboard);
        PopulateNextMatch();
    }

    public void PopulateNextMatch()
    {
        foreach(TournamentData.MatchData match in currentTournament.Matches)
        {
            if(match.Round == currentTournament.CurrentRound)
            {
                if(match.HomeTeam.Team.isUserControlled || match.AwayTeam.Team.isUserControlled)
                {
                    nextMatch.Populate(match);
                    nextMatchData = match;
                }
            }
        }
    }

    public void PlayNextMatch()
    {
        MainController.Instance.Match.Populate(nextMatchData);
        MainController.Instance.ShowScreen(MainController.ScreenType.Match);
    }
}
