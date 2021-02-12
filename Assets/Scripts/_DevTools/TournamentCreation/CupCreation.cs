using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CupCreation : ChampionshipCreation
{
	override public void CreateRounds()
	{
		//Get teams that were added
		DataList.Clear();
		List<TeamData> list = TournamentCreation.Instance.TeamList;
		List<TeamData> teams = new List<TeamData>(list);
				
		totalTeams = list.Count;
		if(totalTeams == 0) return;
		teamAmountLabel.text = totalTeams + " Teams";
		if (totalTeams % 2 != 0) totalTeams++;

		int i = 0;
		foreach(TeamData team in list)
		{
			teams[i] = team;
			i++;
		}

		//Fill gaps with placeholders
		placeholderList = new List<TeamData>();
		if (list.Count < totalTeams)
		{
			for (int t = i; t < totalTeams; t++)
			{
				teams.Add(placeholderTeam);
			}
		}

		//Create rounds
		int totalRounds = totalTeams - 1;
		int half = totalTeams / 2;
		List<TeamData> listA = new List<TeamData>(teams);
		List<TeamData> listB = new List<TeamData>(teams);
		TeamData homeTeam;
		TeamData awayTeam;
	    
		int day = (int)WeekDay.Thursday;
		int maxGamesPerDay = half < 6 ? half : 6;
		int totalGames = 0;
		int totalGameDays = 0;
		int weekDay = 0;
        
		
		UpdateMatchList();
	}
}
