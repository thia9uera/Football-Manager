﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SquadSlotView : MonoBehaviour
{
	public Zone Zone;
	public PlayerData Player;
	public SquadSelectionArrowsView Arrows;
	
	[SerializeField] private TextMeshProUGUI nameLabel = null;
	[SerializeField] private TextMeshProUGUI posLabel = null;
	[SerializeField] private Image fatigueBar = null;
	
    private SquadSelectionView controller;

    void Start()
    {
        controller = GetComponentInParent<SquadSelectionView>();
        posLabel.text = Zone.ToString();
        if (Zone == Zone.OwnGoal) Arrows.gameObject.SetActive(false);
    }

    public void Populate(PlayerData  _player)
    {
        Player = _player;
	    nameLabel.text = "<color=#999999>" + LocalizationController.Instance.GetShortPositionString(Player.Position) + "</color>  " + Player.FirstName + " " + Player.LastName + " (" + Player.GetOverall() + ")";
        Player.Zone = Zone;

        if (Player.IsWronglyAssigned()) posLabel.color = Color.red;
        else posLabel.color = Color.gray;

        if(Player.Zone != Zone.OwnGoal)
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
