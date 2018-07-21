using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchPlayerView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameLabel;

    //public MatchController.Player Player;

    public void Populate(PlayerData _player)
    {
        string name = _player.FirstName + " " + _player.LastName;
        string pos = MainController.Instance.GetPositionShortString(_player.Position);
        nameLabel.text = "<color=#999999>"+pos+"</color>  " + name;
    }
}
