using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MatchScoreView : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI timeLabel = null;
	[SerializeField] private TextMeshProUGUI homeScoreLabel = null;
	[SerializeField] private TextMeshProUGUI awayScoreLabel = null;
	[SerializeField] private TextMeshProUGUI homeNameLabel = null;
	[SerializeField] private TextMeshProUGUI awayNameLabel = null;
	[SerializeField] private Image homeFrame = null;
	[SerializeField] private Image awayFrame = null;

    public void Populate (string _homeTeamName, int _homeTeamScore, Color _homeTeamColor, string _awayTeamName, int _awayTeamScore, Color _awayTeamColor)
    {
        homeNameLabel.text = _homeTeamName.ToUpper();
        awayNameLabel.text = _awayTeamName.ToUpper();
        homeFrame.color = _homeTeamColor;
        awayFrame.color = _awayTeamColor;
        homeScoreLabel.text = _homeTeamScore.ToString();
        awayScoreLabel.text = _awayTeamScore.ToString();

    }

	public void UpdateTime(uint _time, byte _turn)
    {
	    float minutes = _turn;
        float seconds = _time % 60;
        timeLabel.text = minutes.ToString("00") +  ":" + seconds.ToString("00");
    }

    public void UpdateScore(int _homeTeamScore, int _awayTeamScore)
    {
        homeScoreLabel.text = _homeTeamScore.ToString();
        awayScoreLabel.text = _awayTeamScore.ToString();
    }
}
