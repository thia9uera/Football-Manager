using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardItemView : MonoBehaviour
{
    [SerializeField]    public TextMeshProUGUI posLabel, nameLabel, goalsLabel, shotsLabel, passesLabel, crossesLabel, faultsLabel, tacklesLabel, dribblesLabel, headersLabel, savesLabel;

    public PlayerData Player;

    [SerializeField]
    private Image frame; 


    public void Populate(PlayerData _player, int _index)
    {
        Player = _player;

        posLabel.text = MainController.Instance.Localization.GetShortPositionString(_player.Position);
        nameLabel.text = _player.FirstName + " " + _player.LastName;
        goalsLabel.text = _player.TotalGoals.ToString();
        shotsLabel.text = _player.TotalShots.ToString();
        passesLabel.text = _player.TotalPasses.ToString();
        crossesLabel.text = _player.TotalCrosses.ToString();
        faultsLabel.text = _player.TotalFaults.ToString();
        tacklesLabel.text = _player.TotalTackles.ToString();
        dribblesLabel.text = _player.TotalDribbles.ToString();
        headersLabel.text = _player.TotalHeaders.ToString();
        savesLabel.text = _player.TotalSaves.ToString();

        if (_index % 2 != 0) frame.color = Color.gray;
    }
}
