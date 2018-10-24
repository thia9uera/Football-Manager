using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardTeamView : MonoBehaviour
{

    [SerializeField]
    public TextMeshProUGUI nameLabel, winsLabel, lostsLabel, drawsLabel, goalslabel, goalsAgainstLabel;

    public TeamData Team;

    [SerializeField]
    private Image frame;


    public void Populate(TeamData _team, int _index)
    {
        Team = _team;

        nameLabel.text = _team.Name.ToString();
        winsLabel.text = _team.LifeTimeStats.TotalWins.ToString();
        lostsLabel.text = _team.LifeTimeStats.TotalLosts.ToString();
        drawsLabel.text = _team.LifeTimeStats.TotalDraws.ToString();
        goalslabel.text = _team.LifeTimeStats.TotalGoals.ToString();
        goalsAgainstLabel.text = _team.LifeTimeStats.TotalGoalsAgainst.ToString();

        if (_index % 2 != 0) frame.color = Color.gray;
    }
}
