using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MatchPlayerView : MonoBehaviour
{
	[SerializeField] private TMP_Text nameLabel = null;
	[SerializeField] private TMP_Text posLabel = null;
	[SerializeField] private TMP_Text ratingLabel = null;
	[SerializeField] private Image fatigueBar = null;

    public PlayerData Player;

    public void Populate(PlayerData _player)
    {
        Player = _player;
	    nameLabel.text = _player.FullName;
        posLabel.text = LocalizationController.Instance.GetShortPositionString(_player.Position);
        UpdateFatigue();
    }

    public void UpdateFatigue()
    {
        float fill = (float)Player.Fatigue / 100;
        fatigueBar.fillAmount = fill;

	    if (fill < 0.25f) fatigueBar.color = GameData.Instance.Colors.Negative;
	    else if (fill < 0.5f) fatigueBar.color = GameData.Instance.Colors.Warning;
	    else fatigueBar.color = GameData.Instance.Colors.Positive;;

        ratingLabel.text = Player.MatchStats.MatchRating.ToString("F1");
    }
}
