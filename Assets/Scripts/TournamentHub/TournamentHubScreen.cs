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

    [SerializeField]
    TournamentFixtures fixtures;

    TournamentData.MatchData nextMatchData;

    [SerializeField]
    GameObject resetButton;

    public override void Show()
    {
        base.Show();
        currentTournament = MainController.Instance.CurrentTournament;
        titleLabel.text = currentTournament.Name;

        leaderboard.Populate(currentTournament.SortTeamsBy("Points"));
        fixtures.Populate(currentTournament.Matches, currentTournament.CurrentRound);

        if (currentTournament.CurrentRound < currentTournament.TotalRounds) PopulateNextMatch();
        else
        {
            nextMatch.gameObject.SetActive(false);
            resetButton.SetActive(true);
        }


        foreach (TeamData team in currentTournament.Teams)
        {
            foreach(PlayerData player in team.GetAllPlayers())
            {
                if(player.GetTournamentStatistics(currentTournament.Id) != null)
                {
                    PlayerData.Statistics stats = player.TournamentStatistics[currentTournament.Id];
                    if(stats.TotalGoals > 0) print(player.FirstName + " " + player.LastName + "  GOALS: " + stats.TotalGoals);
                }
            }
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
        MainController.Instance.ShowScreen(ScreenType.Match);
    }

    public void ResetTournament()
    {
        currentTournament.ResetTournament();
        Show();
    }

    public void SimulateTournament()
    {
        MainController.Instance.ShowScreen(ScreenType.Match);
        MainController.Instance.CurrentMatch = nextMatchData;
        MainController.Instance.Match.Populate(nextMatchData, true);
    }

    public void BackToMenu()
    {
        MainController.Instance.ShowScreen(ScreenType.MainMenu);
    }
}
