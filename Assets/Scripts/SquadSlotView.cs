using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SquadSlotView : MonoBehaviour
{
    public PlayerData.PlayerPosition Position;

    public PlayerData Player;

    [SerializeField]
    private TextMeshProUGUI nameLabel;


    public void Populate(PlayerData  _player)
    {
        Player = _player;
        nameLabel.text = Player.FirstName + " " + Player.LastName;
    }
}
