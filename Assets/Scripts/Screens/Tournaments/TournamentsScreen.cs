using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentsScreen : BaseScreen
{
	[SerializeField] private TournamentCard cardTemplate;
	[SerializeField] private Transform scrollContent;
	
	private List<TournamentCard> cardList;
	private List<TournamentData> activeTournaments;
	private List<TournamentData> otherTournaments;
	
	public override void Show()
	{
		base.Show();
				
		if(activeTournaments == null) UpdateTournamentLists();
				
		PopulateTournamentList();
	}
	
	private void PopulateTournamentList()
	{
		ResetList();
		
		TournamentCard card;
		foreach(TournamentData tournamentData in activeTournaments)
		{
			card = Instantiate(cardTemplate, scrollContent);
			card.Populate(tournamentData);
		}
	}
	
	private void ResetList()
	{
		if(cardList == null) cardList = new List<TournamentCard>();
		
		if(cardList.Count > 0)
		{
			foreach(TournamentCard card in cardList)
			{
				Destroy(card);
			}
		}
		
		cardList.Clear();
	}
	
	public void UpdateTournamentLists()
	{
		activeTournaments = new List<TournamentData>();
		foreach(string id in MainController.Instance.ActiveTournaments)
		{
			activeTournaments.Add(MainController.Instance.GetTournamentById(id));
		}
		
		otherTournaments = new List<TournamentData>();
		foreach(TournamentData tournament in MainController.Instance.AllTournaments)
		{
			if(!activeTournaments.Contains(tournament)) otherTournaments.Add(tournament);
		}
	}
}
