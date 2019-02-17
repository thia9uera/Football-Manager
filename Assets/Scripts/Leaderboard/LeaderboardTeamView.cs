using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardTeamView : MonoBehaviour
{

    [SerializeField]
    public TextMeshProUGUI nameLabel, winsLabel, lostsLabel, drawsLabel, goalslabel, goalsAgainstLabel, shotsLabel, headersLabel,
        stealsLabel, passesLabel, longPassesLabel, passesMissedLabel, boxCrossesLabel, faultsLabel, counterAttacksLabel;

    public TeamData Team;

    [SerializeField]
    private Image frame;


    public void Populate(TeamData _team, int _index)
    {
        Team = _team;

        LocalizationData loc = MainController.Instance.Localization;

        nameLabel.text = _team.Name;
        winsLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.Wins);
        lostsLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.Losts);
        drawsLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.Draws);
        goalslabel.text = loc.FormatBigNumber(_team.LifeTimeStats.Goals);
        goalsAgainstLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.GoalsAgainst);
        shotsLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.Shots);
        headersLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.Headers);
        stealsLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.Steals);
        passesLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.Passes);
        passesLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.Passes);
        longPassesLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.LongPasses);
        passesMissedLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.PassesMissed);
        boxCrossesLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.BoxCrosses);
        faultsLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.Faults);
        counterAttacksLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.CounterAttacks);

        if (_index % 2 != 0) frame.color = Color.white;
    }
}
