using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TournamentCard_Old : MonoBehaviour
{
    public TournamentData Data;

	[SerializeField] TMP_Text titleLabel = null;
	[SerializeField] TMP_Text _teamAmountLabel = null;
	[SerializeField] TMP_Text team_0 = null;
	[SerializeField] TMP_Text team_1 = null;
	[SerializeField] TMP_Text team_2 = null;
	[SerializeField] TMP_Text score_0 = null;
	[SerializeField] TMP_Text score_1 = null;
	[SerializeField] TMP_Text score_2 = null;

    public void Populate(TournamentData _data)
    {
        Data = _data;

        titleLabel.text = Data.Name;
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
        TMP_Text[] team = new TMP_Text[3] { team_0, team_1, team_2 };
        TMP_Text[] score = new TMP_Text[3] { score_0, score_1, score_2 };
        for (int i = 0; i < max; i++)
        {
            TeamData tData = list[i];
	        team[i].text = tData.Name + " (" + tData.OveralRating + ")";
            if(tData.TournamentStatistics.ContainsKey(Data.Id)) score[i].text = tData.TournamentStatistics[Data.Id].Points + " pts";
        }

    }

    public void ClickHandler()
    {
	    //MainController.Instance.CurrentTournament = Data;
	    //ScreenController.Instance.ShowScreen(ScreenType.TournamentHub);
    }
}
