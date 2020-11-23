#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TournamentLoadItem : MonoBehaviour
{
	[HideInInspector] public TournamentData Data;

	[SerializeField] private TMP_Text label = null;

    public void Populate(TournamentData _data)
    {
        Data = _data;
	    label.text = _data.Name + " (" + _data.Type.ToString() + ")";
	    gameObject.SetActive(true);
    }

    public void ClickHandler()
    {
        TournamentCreation.Instance.EditTournament(Data);
    }
    
	public void DeleteHandler()
	{
		TournamentCreation.Instance.DeleteTournament(Data);
	}
}   
#endif