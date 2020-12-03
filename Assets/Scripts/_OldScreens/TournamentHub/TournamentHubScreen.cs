using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TournamentHubScreen : BaseScreen
{
	[SerializeField] private TMP_Text titleLabel = null;
	[SerializeField] private GameObject resetButton = null;
	[SerializeField] private TournamentLeaderboard leaderboard = null;
	[SerializeField] private TournamentLeaderboard playersLeaderboard = null;
	[SerializeField] private TournamentNextMatch nextMatch = null;
	[SerializeField] private TournamentFixtures fixtures = null;

	private MatchData nextMatchData = null;
	private TournamentData currentTournament = null;

    public override void Show()
    {
        base.Show();
	    //currentTournament = MainController.Instance.CurrentTournament;
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
        foreach (MatchData match in currentTournament.Matches)
        {
            if(match.Round == currentTournament.CurrentRound)
            {
	            if(match.HomeTeam.TeamData.IsUserControlled || match.AwayTeam.TeamData.IsUserControlled)
                {
                    nextMatch.Populate(match);
                    nextMatchData = match;
                }
            }
        }
    }
    public void PlayNextMatch()
    {
	    //MainController.Instance.CurrentMatch = nextMatchData;
	    //MainController.Instance.Match.Populate(nextMatchData);
	    //MainController.Instance.Screens.MatchVisual.Populate(nextMatchData);
        ScreenController.Instance.ShowScreen(ScreenType.Match);
	    //MainController.Instance.Screens.ShowScreen(ScreenType.MatchVisual);
    }

    public void ResetTournament()
    {
        currentTournament.ResetTournament();
        Show();
    }

    public void BackToMenu()
    {
	    //ScreenController.Instance.ShowScreen(ScreenType.MainMenu);
    }

    public void SortPlayerLeaderboardBy(string _param)
    {
        List<PlayerData> playersList = new List<PlayerData>();
 
        foreach (PlayerData player in currentTournament.AllPlayers)
        {
            player.CheckTournament(currentTournament.Id);
            PlayerStatistics stats = player.TournamentStatistics(currentTournament.Id);

            switch(_param)
            {
                case "Goals":
                    if (stats.Goals > 0)
                    {
                        playersList.Add(player);
                    }
                    playersList = MainController.Instance.SortPlayersBy(playersList, _param, currentTournament.Id);
                    break;
            }           
        }

        playersLeaderboard.Populate(playersList, currentTournament.Id, _param);
    }

    public void OpenLeaderboards()
    {
	    // ScreenController.Instance.ShowScreen(ScreenType.Leaderboards);
    }

    public void EditSquad()
    {
	    //ScreenController.Instance.ShowScreen(ScreenType.EditSquad);
    }
}
