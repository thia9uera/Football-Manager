using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MatchPlayerView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameLabel;

    [SerializeField]
    private TextMeshProUGUI posLabel;

    [SerializeField]
    private Image fatigueBar;

    public PlayerData Player;

    [SerializeField]
    private Color red;

    [SerializeField]
    private Color yellow;

    [SerializeField]
    private Color green;

    public void Populate(PlayerData _player)
    {
        Player = _player;
        nameLabel.text = _player.FirstName + " " + _player.LastName;
        posLabel.text = MainController.Instance.Localization.GetShortPositionString(_player.Position);
        UpdateFatigue();
    }

    public void UpdateFatigue()
    {
        float fill = (float)Player.Fatigue / 100;
        fatigueBar.fillAmount = fill;

        if (fill < 0.25f) fatigueBar.color = red;
        else if (fill < 0.5f) fatigueBar.color = yellow;
        else fatigueBar.color = green;
    }
}
