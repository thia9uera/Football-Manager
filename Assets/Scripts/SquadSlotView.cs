using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SquadSlotView : MonoBehaviour
{
    public MatchController.FieldZone Zone;

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
        posLabel.text = Zone.ToString();
        if (Zone == MatchController.FieldZone.OwnGoal) Arrows.gameObject.SetActive(false);
    }

    public void Populate(PlayerData  _player)
    {
        Player = _player;
        nameLabel.text = "<color=#999999>" + MainController.Instance.Localization.GetShortPositionString(Player.Position) + "</color>  " + Player.FirstName + " " + Player.LastName + " (" + Player.GetOverall() + ")";
        Player.Zone = Zone;

        if (Player.IsWronglyAssigned()) posLabel.color = Color.red;
        else posLabel.color = Color.gray;

        if(Player.Zone != MatchController.FieldZone.OwnGoal)
        {
            Arrows.UpdateStrategy(Player.Strategy);
            Arrows.HideArrows(Zone);
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
