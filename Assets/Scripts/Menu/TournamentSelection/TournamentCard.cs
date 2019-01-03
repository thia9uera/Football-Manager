using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TournamentCard : MonoBehaviour
{
    public TournamentData Data;

    [SerializeField]
    TextMeshProUGUI titleLabel, starsRequiredLabel, team_0, team_1, team_2, score_0, score_1, score_2;

    public void Populate(TournamentData _data)
    {
        Data = _data;

        titleLabel.text = Data.Name;
        starsRequiredLabel.text = "Stars Required: " + Data.StarsRequired;

        List<TournamentData.TeamTournamentData> list;
        if (Data.CurrentRound == 0)
        {   
            list = Data.SortTeamsBy("Name");
        }
        else
        {
            list = Data.SortTeamsBy("Points");
        }

        TournamentData.TeamTournamentData tData = list[0];
        team_0.text = tData.Team.Name + " (" + tData.Team.GetOveralRating() + ")"; ;
        score_0.text = tData.Points + "pts";

        tData = list[1];
        team_1.text = tData.Team.Name + " (" + tData.Team.GetOveralRating() + ")";
        score_1.text = tData.Points + "pts";

        if(list.Count > 2)
        {
            tData = list[2];
            team_2.text = tData.Team.Name + " (" + tData.Team.GetOveralRating() + ")"; ;
            score_2.text = tData.Points + " pts";
        }
        else
        {
            team_2.text = "";
            score_2.text = "";
        }
    }

    public void ClickHandler()
    {
        MainController.Instance.CurrentTournament = Data;
        MainController.Instance.ShowScreen(MainController.ScreenType.TournamentHub);
    }
}
