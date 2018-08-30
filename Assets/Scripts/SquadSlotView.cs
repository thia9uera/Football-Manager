using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SquadSlotView : MonoBehaviour
{
    public PlayerData.PlayerPosition Position;

    public PlayerData Player;

    public SquadSelectionArrowsView Arrows;

    [SerializeField]
    private TextMeshProUGUI nameLabel;

    [SerializeField]
    private TextMeshProUGUI posLabel;

    [SerializeField]
    private Image fatigueBar;

    private SquadSelectionView controller;

    void Start()
    {
        controller = GetComponentInParent<SquadSelectionView>();
        posLabel.text = MainController.Instance.Localization.GetShortPositionString(Position);
        if (Position == PlayerData.PlayerPosition.GK) Arrows.gameObject.SetActive(false);
    }

    public void Populate(PlayerData  _player)
    {
        Player = _player;
        nameLabel.text = "<color=#999999>" + Player.Position + "</color>  " + Player.FirstName + " " + Player.LastName + " (" + Player.GetOverall() + ")";
        Player.AssignedPosition = Position;

        if (Player.Position != Position) posLabel.color = Color.red;
        else posLabel.color = Color.gray;

        if(Player.Position != PlayerData.PlayerPosition.GK)
        {
            Arrows.UpdateStrategy(Player.Strategy);
            Arrows.HideArrows(Position);
            UpdateFatigue();
        }
    }

    private void UpdateFatigue()
    {
        float fill = (float)Player.Fatigue / 100;
        fatigueBar.fillAmount = fill;

        if (fill < 0.25f) fatigueBar.color = Color.red;
        else if (fill < 0.5f) fatigueBar.color = Color.yellow;
        else fatigueBar.color = Color.green;
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
