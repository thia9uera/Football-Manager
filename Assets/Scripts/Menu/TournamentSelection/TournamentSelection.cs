using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TournamentSelection : BaseScreen
{
    [SerializeField]
    TournamentCard cardTemplate;

    [SerializeField]
    Transform content;

    List<GameObject> itemList;
    List<TournamentData> tournamentList;

    public override void Show()
    {
        base.Show();
        MainController.Instance.CurrentTournament = null;
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
            case "StarsRequired": tournamentList = tournamentList.OrderBy(TournamentData => TournamentData.StarsRequired).ToList(); break;
            case "MostTeams": tournamentList = tournamentList.OrderByDescending(TournamentData => TournamentData.Teams.Count).ToList(); break;
        }

        foreach (TournamentData file in tournamentList)
        {
            foreach (TeamData team in file.Teams) team.InitializeTournamentData(file.Id);

            TournamentCard item = Instantiate(cardTemplate, content);
            item.Populate(file);
            itemList.Add(item.gameObject);
        }
    }

    public void CreateNew()
    {
        MainController.Instance.Screens.MainMenu.ShowSubmenu(ScreenType.TournamentCreation);
    }
}
