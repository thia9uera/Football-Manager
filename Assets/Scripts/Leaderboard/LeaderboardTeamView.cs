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
        winsLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.TotalWins);
        lostsLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.TotalLosts);
        drawsLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.TotalDraws);
        goalslabel.text = loc.FormatBigNumber(_team.LifeTimeStats.TotalGoals);
        goalsAgainstLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.TotalGoalsAgainst);
        shotsLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.TotalShots);
        headersLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.TotalHeaders);
        stealsLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.TotalSteals);
        passesLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.TotalPasses);
        passesLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.TotalPasses);
        longPassesLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.TotalLongPasses);
        passesMissedLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.TotalPassesMissed);
        boxCrossesLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.TotalBoxCrosses);
        faultsLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.TotalFaults);
        counterAttacksLabel.text = loc.FormatBigNumber(_team.LifeTimeStats.TotalCounterAttacks);

        if (_index % 2 != 0) frame.color = Color.white;
    }
}
