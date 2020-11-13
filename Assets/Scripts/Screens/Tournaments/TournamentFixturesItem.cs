using UnityEngine;
using UnityEngine.UI;

public class TournamentFixturesItem : MonoBehaviour
{
	[SerializeField] private TournamentFixturesItemTeam home, away;
	[SerializeField] private CanvasGroup group;
	[SerializeField] private Image frameImage;

	public void Populate(MatchData _data, uint _idx = 0)
    {
	    TeamData homeTeam = MainController.Instance.GetTeamById(_data.HomeTeam.TeamId);
	    TeamData awayTeam = MainController.Instance.GetTeamById(_data.AwayTeam.TeamId);
        
	    Color homeColor = homeTeam.PrimaryColor;
	    Color awayColor = awayTeam.PrimaryColor == homeTeam.PrimaryColor ? awayTeam.SecondaryColor : awayTeam.PrimaryColor;

	    home.Populate(homeTeam.Name, homeTeam.OveralRating.ToString(), _data.HomeTeam.Statistics.Goals.ToString(), homeColor, _data.isPlayed);
	    away.Populate(awayTeam.Name, awayTeam.OveralRating.ToString(), _data.AwayTeam.Statistics.Goals.ToString(), awayColor, _data.isPlayed);


	    if (_idx % 2 == 0) frameImage.color = GameData.Instance.Colors.LightGray;
	    if (homeTeam.IsUserControlled || awayTeam.IsUserControlled) frameImage.color = GameData.Instance.Colors.PlayerColor;
    }
}
