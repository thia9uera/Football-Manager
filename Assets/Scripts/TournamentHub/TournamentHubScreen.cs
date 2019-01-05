using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class TournamentHubScreen : BaseScreen
{
    [SerializeField]
    TextMeshProUGUI titleLabel;

    TournamentData currentTournament;

    [SerializeField]
    TournamentLeaderboard leaderboard;

    [SerializeField]
    TournamentLeaderboard playersLeaderboard;

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

        leaderboard.Populate(currentTournament.SortTeamsBy("Points"), currentTournament.Id);
        fixtures.Populate(currentTournament.Matches, currentTournament.CurrentRound);

        if (currentTournament.CurrentRound < currentTournament.TotalRounds) PopulateNextMatch();
        else
        {
            nextMatch.gameObject.SetActive(false);
            resetButton.SetActive(true);
        }

        SortPlayerLeaderboardBy("Goals");
    }

    public void PopulateNextMatch()
    {
        nextMatch.gameObject.SetActive(true);
        //resetButton.SetActive(false);
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

    public void SortPlayerLeaderboardBy(string _param)
    {
        List<PlayerData> playersList = new List<PlayerData>();
        foreach (TeamData team in currentTournament.Teams)
        {
            foreach (PlayerData player in team.GetAllPlayers())
            {
                if (player.GetTournamentStatistics(currentTournament.Id) != null)
                {
                    PlayerData.Statistics stats = player.TournamentStatistics[currentTournament.Id];

                    switch(_param)
                    {
                        case "Goals":
                            if (stats.TotalGoals > 0) playersList.Add(player);
                            playersList = playersList.OrderByDescending(PlayerData => PlayerData.TournamentStatistics[currentTournament.Id].TotalGoals).ToList();
                            break;
                    }           
                }
            }
        }

        playersLeaderboard.Populate(playersList, currentTournament.Id, _param);
    }

    public void OpenLeaderboards()
    {
        MainController.Instance.Screens.ShowScreen(ScreenType.Leaderboards);
    }
}
