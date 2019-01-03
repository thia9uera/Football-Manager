﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TournamentSelection : MonoBehaviour
{
    [SerializeField]
    TournamentCard cardTemplate;

    [SerializeField]
    Transform content;

    List<GameObject> itemList;
    List<TournamentData> fileList;

    private void Start()
    {
        itemList = new List<GameObject>();
        Populate();
    }

    public void Populate()
    {
        fileList = new List<TournamentData>(Resources.LoadAll<TournamentData>("Tournaments"));
        SortCarousel("StarsRequired");
    }

    void SortCarousel(string _param)
    {
        if (itemList.Count > 0)
        {
            foreach (GameObject go in itemList)
            {
                Destroy(go);
            }
            itemList.Clear();
        }

        switch (_param)
        {
            case "StarsRequired": fileList = fileList.OrderBy(TournamentData => TournamentData.StarsRequired).ToList(); break;
            case "MostTeams": fileList = fileList.OrderByDescending(TournamentData => TournamentData.Teams.Count).ToList(); break;
        }

        foreach (TournamentData file in fileList)
        {
            TournamentCard item = Instantiate(cardTemplate, content);
            item.Populate(file);
            itemList.Add(item.gameObject);
        }
    }
}
