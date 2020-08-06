using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SquadSubView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI posLabel;

    [SerializeField]
    private TextMeshProUGUI nameLabel;

    public PlayerData Player;

    private SquadSelectionView controller;

    private void Awake()
    {
        controller = GetComponentInParent<SquadSelectionView>();
    }

    public void Populate(PlayerData _data)
    {
        Player = _data;
	    posLabel.text = LocalizationController.Instance.GetShortPositionString(Player.Position);
        nameLabel.text = Player.FirstName + " " + Player.LastName + " (" + Player.GetOverall() + ")";
    }

    public void OnMousedown()
    {
        controller.StartDragging(Player);
    }
}
