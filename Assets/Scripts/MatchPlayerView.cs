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
    private Image fatigueBar;

    public PlayerData Player;

    public void Populate(PlayerData _player)
    {
        string name = _player.FirstName + " " + _player.LastName;
        string pos = MainController.Instance.Localization.GetShortPositionString(_player.Position);
        Player = _player;
        nameLabel.text = "<color=#999999>"+pos+"</color>  " + name;
        UpdateFatigue();
    }

    public void UpdateFatigue()
    {
        float fill = (float)Player.Fatigue / 100;
        fatigueBar.fillAmount = fill;

        if(fill < 0.25f) fatigueBar.color = Color.red;
        else if (fill < 0.5f) fatigueBar.color = Color.yellow;
        else fatigueBar.color = Color.green;

    }
}
