using UnityEngine;

public class TournamentFixturesItem : MonoBehaviour
{
    [SerializeField]
    TournamentFixturesItemTeam home, away;

    [SerializeField]
    CanvasGroup group;

    public void Populate(MatchData _data)
    {
        TeamData homeTeam = MainController.Instance.GetTeamById(_data.HomeTeam.TeamAttributes.Id);
        TeamData awayTeam = MainController.Instance.GetTeamById(_data.AwayTeam.TeamAttributes.Id);
        home.Populate(_data.HomeTeam.TeamAttributes.Name, homeTeam.GetOveralRating().ToString(), _data.HomeTeam.Statistics.TotalGoals.ToString(), _data.HomeTeam.TeamAttributes.PrimaryColor, _data.isPlayed);
        away.Populate(_data.AwayTeam.TeamAttributes.Name, awayTeam.GetOveralRating().ToString(), _data.AwayTeam.Statistics.TotalGoals.ToString(), _data.AwayTeam.TeamAttributes.PrimaryColor, _data.isPlayed);


        if (!_data.isPlayed && group != null) group.alpha = 0.3f;
    }
}
