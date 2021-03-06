﻿#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentLoad : MonoBehaviour
{
    [SerializeField] private TournamentLoadItem fileTemplate = null;
	[SerializeField] private Transform content = null;

	private List<TournamentData> fileList;
	private List<GameObject> itemList;

    public void LoadFiles()
    {
        if (itemList == null) itemList = new List<GameObject>();
        if(itemList.Count > 0)
        {
            foreach(GameObject go in itemList)
            {
                Destroy(go);
            }
            itemList.Clear();
        }

        fileList = new List<TournamentData>(Tools.GetAtFolder<TournamentData>("Data/Tournaments"));
        foreach (TournamentData tournament in fileList)
        {
            tournament.LoadTeams();
            TournamentLoadItem item = Instantiate(fileTemplate, content);
	        item.Populate(tournament);
            itemList.Add(item.gameObject);
        }
    }
}
#endif