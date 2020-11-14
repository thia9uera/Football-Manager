using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TournamentCard : MonoBehaviour
{
	[HideInInspector] public TournamentData Data;
	
	[SerializeField] private TextMeshProUGUI titleLabel = null;
	
	public void Populate(TournamentData _data)
	{
		Data = _data;
		
		titleLabel.text = Data.Name;
		
		gameObject.SetActive(true);
	}
    
    
	public void OnClickHandler()
	{
		ScreenController.Instance.Tournament.Data = Data;
		ScreenController.Instance.ShowScreen(ScreenType.TournamentInfo);
	}
}
