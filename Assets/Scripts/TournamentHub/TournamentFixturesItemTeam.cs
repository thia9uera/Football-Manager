using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TournamentFixturesItemTeam : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nameLabel, scoreLabel;

    [SerializeField]
    Image frame;

    public void Populate(TournamentData.TeamMatchData _data)
    {
        nameLabel.text = _data.Team.Name;
        scoreLabel.text = _data.Score.ToString();
        frame.color = _data.Team.PrimaryColor;
    }
}
