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

    [SerializeField]
    private TextMeshProUGUI posLabel;

    private SquadSelectionView controller;

    void Start()
    {
        controller = GetComponentInParent<SquadSelectionView>();
        posLabel.text = MainController.Instance.Localization.GetShortPositionString(Position);
    }

    public void Populate(PlayerData  _player)
    {
        Player = _player;
        nameLabel.text = "<color=#999999>" + Player.Position + "</color>  " + Player.FirstName + " " + Player.LastName + " (" + Player.GetOverall() + ")";
        Player.AssignedPosition = Position;

        if (Player.Position != Position) posLabel.color = Color.red;
        else posLabel.color = Color.gray;
    }

    public void OnMouseEnter()
    {
        controller.selectedSlot = this;
    }

    public void OnMouseExit()
    {
        controller.selectedSlot = null;
    }
}
