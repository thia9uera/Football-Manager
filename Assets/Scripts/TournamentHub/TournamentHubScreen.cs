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

    [SerializeField]
    GameObject resetButton;

    public override void Show()
    {
        base.Show();
        currentTournament = MainController.Instance.CurrentTournament;
        titleLabel.text = currentTournament.Name;

        leaderboard.Populate(currentTournament.SortTeamsBy("Points"));

        if (currentTournament.CurrentRound < currentTournament.TotalRounds) PopulateNextMatch();
        else
        {
            nextMatch.gameObject.SetActive(false);
            resetButton.SetActive(true);
        }
    }

    public void PopulateNextMatch()
    {
        nextMatch.gameObject.SetActive(true);
        resetButton.SetActive(false);
        foreach (TournamentData.MatchData match in currentTournament.Matches)
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
        MainController.Instance.CurrentMatch = nextMatchData;
        MainController.Instance.Match.Populate(nextMatchData);
        MainController.Instance.ShowScreen(MainController.ScreenType.Match);
    }

    public void ResetTournament()
    {
        currentTournament.ResetTournament();
        Show();
    }
}
