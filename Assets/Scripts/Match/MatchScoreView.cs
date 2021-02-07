using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MatchScoreView : MonoBehaviour
{
	[SerializeField] private TMP_Text tournamentNameLabel = null;
	[SerializeField] private TMP_Text timeLabel = null;
	[SerializeField] private TMP_Text homeScoreLabel = null;
	[SerializeField] private TMP_Text awayScoreLabel = null;
	[SerializeField] private TMP_Text homeNameLabel = null;
	[SerializeField] private TMP_Text awayNameLabel = null;
	[SerializeField] private Image homeFrame = null;
	[SerializeField] private Image awayFrame = null;

	public void Populate (string _tournamentName, string _homeTeamName, int _homeTeamScore, Color _homeTeamColor, string _awayTeamName, int _awayTeamScore, Color _awayTeamColor)
	{
		tournamentNameLabel.text = _tournamentName;
        homeNameLabel.text = _homeTeamName.ToUpper();
        awayNameLabel.text = _awayTeamName.ToUpper();
        homeFrame.color = _homeTeamColor;
        awayFrame.color = _awayTeamColor;
        homeScoreLabel.text = _homeTeamScore.ToString();
        awayScoreLabel.text = _awayTeamScore.ToString();

    }

	public void UpdateTime(int _time)
    {
	    timeLabel.text = _time + "'";
    }

    public void UpdateScore(int _homeTeamScore, int _awayTeamScore)
    {
        homeScoreLabel.text = _homeTeamScore.ToString();
        awayScoreLabel.text = _awayTeamScore.ToString();
    }
}
