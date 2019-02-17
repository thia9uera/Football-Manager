using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MatchScoreView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timeLabel;

    [SerializeField]
    private TextMeshProUGUI homeScoreLabel;

    [SerializeField]
    private TextMeshProUGUI awayScoreLabel;

    [SerializeField]
    private TextMeshProUGUI homeNameLabel;

    [SerializeField]
    private TextMeshProUGUI awayNameLabel;

    [SerializeField]
    private Image homeFrame;

    [SerializeField]
    private Image awayFrame;

    public void Populate (string _homeTeamName, int _homeTeamScore, Color _homeTeamColor, string _awayTeamName, int _awayTeamScore, Color _awayTeamColor)
    {
        homeNameLabel.text = _homeTeamName.ToUpper();
        awayNameLabel.text = _awayTeamName.ToUpper();
        homeFrame.color = _homeTeamColor;
        awayFrame.color = _awayTeamColor;
        homeScoreLabel.text = _homeTeamScore.ToString();
        awayScoreLabel.text = _awayTeamScore.ToString();

    }

    public void UpdateTime(int _time)
    {
        float minutes = Mathf.Floor(_time / 60);
        float seconds = _time % 60;
        timeLabel.text = minutes.ToString("00") +  ":" + seconds.ToString("00");
    }

    public void UpdateScore(int _homeTeamScore, int _awayTeamScore)
    {
        homeScoreLabel.text = _homeTeamScore.ToString();
        awayScoreLabel.text = _awayTeamScore.ToString();
    }
}
