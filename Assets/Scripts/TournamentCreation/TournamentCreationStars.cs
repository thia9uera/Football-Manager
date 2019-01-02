using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TournamentCreationStars : MonoBehaviour
{
    int starsRequired;
    public int StarsRequired
    {
        set
        {
            starsRequired = value;
            UpdateButtons();
        }
        get
        {
            return starsRequired;
        }
    }

    public int MaxStars = 6;

    [SerializeField]
    Button BtnPlus, BtnMinus;

    [SerializeField]
    TextMeshProUGUI label;

    private void Start()
    {
        UpdateButtons();
    }

    public void PlusHandler()
    {
        StarsRequired++;
        UpdateButtons();
    }

    public void MinusHandler()
    {
        StarsRequired--;
        UpdateButtons();
    }

    void UpdateButtons()
    {
        BtnPlus.interactable = !(StarsRequired == MaxStars);
        BtnMinus.interactable = !(StarsRequired == 0);

        label.text = "Stars Required: " + StarsRequired;
    }
}
