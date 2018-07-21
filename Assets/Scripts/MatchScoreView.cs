using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchScoreView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timeLabel;

    [SerializeField]
    private TextMeshProUGUI scoreLabel;

    public void UpdateTime(int _time)
    {
        //float minutes = Mathf.Floor(_time / 60);
        //float seconds = _time % 60;
        timeLabel.text = _time.ToString("00") +  "'";
    }

    public void UpdateScore(string _homeTeamName, int _homeTeamScore, string _homeTeamColor, string _awayTeamName, int _awayTeamScore, string _awayTeamColor)
    {
        scoreLabel.text = "<color=#"+_homeTeamColor + ">" + _homeTeamName +"</color>" + "  " + _homeTeamScore.ToString() + "  X  " + _awayTeamScore.ToString() + "  " + "<color=#" + _awayTeamColor + ">" + _awayTeamName + "</color>";
    }
}
