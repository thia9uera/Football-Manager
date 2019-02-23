using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchSimulationScreen : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI feedbackLabel;

    public void UpdateFeedback(string _str)
    {
        feedbackLabel.text = _str;
    }

    public void UpdateFeedback(MatchData _data)
    {
        string home = _data.HomeTeam.TeamAttributes.Name;
        string homeScore = _data.HomeTeam.Statistics.Goals.ToString();
        string away = _data.AwayTeam.TeamAttributes.Name;
        string awayScore = _data.AwayTeam.Statistics.Goals.ToString();

        feedbackLabel.text = home + "  " + homeScore + "  X  " + awayScore + "  " + away;
    }
}
