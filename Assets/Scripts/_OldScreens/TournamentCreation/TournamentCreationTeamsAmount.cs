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
            //UpdateButtons();
        }
        get
        {
            return teamsAmount;
        }
    }

    private int teamAmountIndex;
    private readonly int[] teamAmountList = {2, 4, 8, 16, 24, 32};

    [SerializeField]
    Button BtnPlus;

    [SerializeField]
    Button BtnMinus;

    [SerializeField]
    TextMeshProUGUI label;

    private void Start()
    {
        //TeamsAmount = teamAmountList[teamAmountIndex];
        UpdateButtons();
    }

    public void PlusHandler()
    {
        teamAmountIndex++;
        UpdateButtons();
    }

    public void MinusHandler()
    {
        teamAmountIndex--;
        UpdateButtons();
    }

    void UpdateButtons()
    {
        BtnPlus.interactable = !(teamAmountIndex >= teamAmountList.Length-1);
        BtnMinus.interactable = !(teamAmountIndex <= 0);

        TeamsAmount = teamAmountList[teamAmountIndex];

        label.text = "Teams: " + TeamsAmount;
    }
}
