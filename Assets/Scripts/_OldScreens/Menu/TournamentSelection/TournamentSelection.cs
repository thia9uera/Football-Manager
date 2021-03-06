﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TournamentSelection : BaseScreen
{
	[SerializeField] private TournamentCard_Old cardTemplate = null;
	[SerializeField] private Transform content = null;

	private List<GameObject> itemList;
	private List<TournamentData> tournamentList;

    public override void Show()
    {
        base.Show();
        Populate();
    }

    public void Populate()
    {
        tournamentList = MainController.Instance.AllTournaments;
        SortCarousel("StarsRequired");
    }

    void SortCarousel(string _param)
    {
        if (itemList == null) itemList = new List<GameObject>();
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
            case "MostTeams": tournamentList = tournamentList.OrderByDescending(TournamentData => TournamentData.Teams.Count).ToList(); break;
        }

        foreach (TournamentData file in tournamentList)
        {
            file.LoadTeams();
            foreach (TeamData team in file.Teams) team.InitializeTournamentData(file.Id);

            TournamentCard_Old item = Instantiate(cardTemplate, content);
            item.Populate(file);
            itemList.Add(item.gameObject);
        }
    }

    public void CreateNew()
    {
	    //ScreenController.Instance.MainMenu.ShowSubmenu(ScreenType.TournamentCreation);
    }
}
