﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardTeamView : MonoBehaviour
{
	[SerializeField] private TMP_Text nameLabel = null;
	[SerializeField] private TMP_Text winsLabel = null;
	[SerializeField] private TMP_Text lostsLabel = null;
	[SerializeField] private TMP_Text drawsLabel = null;
	[SerializeField] private TMP_Text goalslabel = null;
	[SerializeField] private TMP_Text goalsAgainstLabel = null;
	[SerializeField] private TMP_Text shotsLabel = null;
	[SerializeField] private TMP_Text headersLabel = null;
	[SerializeField] private TMP_Text stealsLabel = null;
	[SerializeField] private TMP_Text passesLabel = null;
	[SerializeField] private TMP_Text longPassesLabel = null;
	[SerializeField] private TMP_Text passesMissedLabel = null;
	[SerializeField] private TMP_Text boxCrossesLabel = null;
	[SerializeField] private TMP_Text faultsLabel = null;
	[SerializeField] private TMP_Text counterAttacksLabel = null;
	[SerializeField] private Image frame = null;

	[HideInInspector] public TeamData Team;

    public void Populate(TeamData _team, int _index)
    {
        Team = _team;

	    LocalizationController loc = LocalizationController.Instance;

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
