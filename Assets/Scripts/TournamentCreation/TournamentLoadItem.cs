using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TournamentLoadItem : MonoBehaviour
{
    public TournamentData Data;

    TextMeshProUGUI label;

    private void Awake()
    {
        label = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Populate(TournamentData _data)
    {
        Data = _data;
        label.text = _data.Name + " <color=#999999> (" + _data.Type.ToString() + ")";
    }

    public void ClickHandler()
    {
        TournamentCreation.Instance.EditTournament(Data);
    }
}   
