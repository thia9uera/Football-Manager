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

    public void Populate(TournamentData.TeamMatchData _data, bool _isPlayed)
    {
        nameLabel.text = _data.Team.Name + " (" + _data.Team.GetOveralRating() + ")";
        scoreLabel.text = _data.Statistics.TotalGoals.ToString();
        frame.color = _data.Team.PrimaryColor;

        scoreLabel.gameObject.SetActive(_isPlayed);
    }
}
