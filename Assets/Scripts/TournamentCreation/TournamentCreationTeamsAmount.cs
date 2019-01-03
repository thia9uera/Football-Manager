using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TournamentCreationTeamsAmount : MonoBehaviour
{
    int teamsAmount;
    public int TeamsAmount
    {
        set
        {
            teamsAmount = value;
            UpdateButtons();
        }
        get
        {
            return teamsAmount;
        }
    }

    int maxTeams = 48;
    int minTeams = 2;

    [SerializeField]
    Button BtnPlus, BtnMinus;

    [SerializeField]
    TextMeshProUGUI label;

    private void Start()
    {
        //UpdateButtons();
    }

    public void PlusHandler()
    {
        TeamsAmount += 2;
        UpdateButtons();
    }

    public void MinusHandler()
    {
        TeamsAmount -= 2;
        UpdateButtons();
    }

    void UpdateButtons()
    {
        BtnPlus.interactable = !(TeamsAmount == maxTeams);
        BtnMinus.interactable = !(TeamsAmount == minTeams);

        label.text = "Teams: " + TeamsAmount;
    }
}
