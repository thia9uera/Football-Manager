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

    //public MatchController.Player Player;

    public void Populate(PlayerData _player)
    {
        string name = _player.FirstName + " " + _player.LastName;
        string pos = MainController.Instance.Localization.GetShortPositionString(_player.Position);
        nameLabel.text = "<color=#999999>"+pos+"</color>  " + name;
    }

    public void UpdateFatique(float _fill)
    {
        fatigueBar.fillAmount = _fill;
    }
}
