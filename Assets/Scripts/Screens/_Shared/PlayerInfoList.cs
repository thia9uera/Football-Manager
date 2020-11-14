using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoList : MonoBehaviour
{
	[SerializeField] private Transform content = null;
	[SerializeField] private PlayerInfoRow rowTemplate = null;
	
	private List<PlayerInfoRow> rowList;
	
	public void Populate (List<PlayerData> _data)
	{
		foreach(PlayerData player in _data) 
		{
			PlayerInfoRow row = Instantiate(rowTemplate, content);
			row.Populate(player);
		}
	}
	
	private void ClearList()
	{
		if(rowList != null) 
		{
			foreach(PlayerInfoRow row in rowList) Destroy(row);
		}
		rowList = new List<PlayerInfoRow>();
	}
}
