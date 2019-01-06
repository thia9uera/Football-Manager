using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TournamentCard : MonoBehaviour
{
    public TournamentData Data;

    [SerializeField]
    TextMeshProUGUI titleLabel;

    [SerializeField]
    TextMeshProUGUI starsRequiredLabel;

    [SerializeField]
    TextMeshProUGUI _teamAmountLabel;

    [SerializeField]
    TextMeshProUGUI team_0;

    [SerializeField]
    TextMeshProUGUI team_1;

    [SerializeField]
    TextMeshProUGUI team_2;

    [SerializeField]
    TextMeshProUGUI score_0;

    [SerializeField]
    TextMeshProUGUI score_1;

    [SerializeField]
    TextMeshProUGUI score_2;


    public void Populate(TournamentData _data)
    {
        Data = _data;

        titleLabel.text = Data.Name;
        starsRequiredLabel.text = "Stars Required: " + Data.StarsRequired;
        _teamAmountLabel.text = Data.Teams.Count + " TEAMS";
        List<TeamData> list;
        if (Data.CurrentRound == 0)
        {   
            list = Data.SortTeamsBy("Name");
        }
        else
        {
            list = Data.SortTeamsBy("Points");
        }

        int max = 2;
        if (list.Count > 3) max = 3;
        team_2.text = "";
        score_2.text = "";
        TextMeshProUGUI[] team = new TextMeshProUGUI[3] { team_0, team_1, team_2 };
        TextMeshProUGUI[] score = new TextMeshProUGUI[3] { score_0, score_1, score_2 };
        for (int i = 0; i < max; i++)
        {
            TeamData tData = list[i];
            team[i].text = tData.Name + " (" + tData.GetOveralRating() + ")";
            if(tData.TournamentStatistics.ContainsKey(Data.Id)) score[i].text = tData.TournamentStatistics[Data.Id].Points + " pts";
        }

    }

    public void ClickHandler()
    {
        MainController.Instance.CurrentTournament = Data;
        MainController.Instance.Screens.ShowScreen(BaseScreen.ScreenType.TournamentHub);
    }
}
