using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChampionshipCreation : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI teamAmountLabel;
	[SerializeField] private TournamentCreationMatch matchTemplate;
    [SerializeField] private TextMeshProUGUI roundLabelTemplate;
	[SerializeField] private Transform content;
	[SerializeField] private Toggle homeAwayTeams;

	private List<GameObject> matchList;
    public List<MatchData> DataList;

    private List<TeamData> placeholderList;
    private int totalTeams;

    void Awake()
    {
        DataList = new List<MatchData>();
        matchList = new List<GameObject>();
    }

    public void AddTeam(TeamData _data)
	{
        TournamentCreation.Instance.TeamList.Add(_data);
        DataList.Clear();
        CreateRounds();
    }

    public void RemoveTeam(TeamData _data)
	{
        TournamentCreation.Instance.TeamList.Remove(_data);
        DataList.Clear();
        CreateRounds();
    }

    public void CreateRounds()
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
                TeamData team = ScriptableObject.CreateInstance<TeamData>();
                team.Attributes = new TeamAttributes();
                placeholderList.Add(team);
                teams.Add(team);
                team.Name = "MISSING TEAM " + (t + 1);
                team.IsPlaceholder = true;
            }
        }

        //Create rounds
	    int totalRounds = totalTeams - 1;
        int half = totalTeams / 2;
        List<TeamData> listA = new List<TeamData>(teams);
        List<TeamData> listB = new List<TeamData>(teams);
        TeamData homeTeam;
	    TeamData awayTeam;
	    
	    int day = (int)WeekDay.Sunday;
		int maxGamesPerDay = half < 5 ? half : 5;
		int totalGames = 0;
		int totalGameDays = 0;
	    int weekDay = 0;
        
	    for (int round = 0; round < totalRounds; round++)
	    {
            for (int t = 0; t < half; t++)
            {
                if (round % 2 == 0)
                {
                    homeTeam = listA[t];
	                awayTeam = listA[totalRounds - t];
	                
                }
                else
                {
                    homeTeam = listA[totalRounds - t];
	                awayTeam = listA[t];               
                }
				

	            if(round > totalGameDays || totalGames % maxGamesPerDay == 0) 
	            {
	            	if(weekDay == 0) day += 3; // if it's Sunday skips to Wednesday
	            	else day += 4; // if it's Wednesday skips to Sunday
	            	weekDay = (weekDay == 0) ? 1 : 0;
	            	totalGameDays++;
	            }
	            
	            totalGames++;

	            MatchData data = new MatchData(homeTeam.Attributes, awayTeam.Attributes, round, day);
                DataList.Add(data);
            }

            //Rotate teams
            listA = new List<TeamData>(listB);
            for(int k = 1; k < teams.Count; k++)
            {
                if(k == teams.Count-1)
                {
                    listA[1] = listB[k];
                }
                else
                {
                    listA[k + 1] = listB[k];
                }
            }
            listB = new List<TeamData>(listA);
	    }

        if(homeAwayTeams.isOn)
        {
        	if(totalGames < maxGamesPerDay) day += 7;
        	MatchData matchData;
	        List<MatchData> dList = new List<MatchData>(DataList);
	        int round;
            foreach(MatchData data in dList)
            {
            	round = data.Round + totalRounds;
            	if(totalGames % maxGamesPerDay == 0) 
	            {
	            	if(weekDay == 0) day += 3; // if it's Sunday skips to Wednesday
	            	else day += 4; // if it's Wednesday skips to Sunday
	            	weekDay = (weekDay == 0) ? 1 : 0;
	            }

	            matchData = new MatchData(data.AwayTeam.TeamAttributes, data.HomeTeam.TeamAttributes, round, day);
	            DataList.Add(matchData);
	            totalGames++;
            }
        }

        UpdateMatchList();
    }

    public void UpdateMatchList()
	{
		if(DataList.Count == 0) return;
		if(matchList == null) matchList = new List<GameObject>();
        if(matchList.Count > 0) ClearMatchList();
		int round = -1;
		int day = DataList[0].Day;
		AddDateLabel(day);
        foreach(MatchData data in DataList)
        {
	        if(data.Day != day)
            {
		        day = data.Day;
		        AddDateLabel(day);
            }
            TournamentCreationMatch match = Instantiate(matchTemplate, content);
            match.Populate(data);
	        matchList.Add(match.gameObject);
	        //Debug.Log(data.HomeTeam.TeamAttributes.Name + " X " + data.AwayTeam.TeamAttributes.Name + "  -  " + CalendarController.Instance.GetDay(data.Day));
        }
	}
    
	private void AddDateLabel(int _day)
	{
		TextMeshProUGUI txt = Instantiate(roundLabelTemplate, content);
		txt.text = CalendarController.Instance.GetDay(_day) + " - " + CalendarController.Instance.GetWeekDay(_day);
		matchList.Add(txt.gameObject);
	}

    public void ClearMatchList()
	{
		if(matchList == null) return;
        foreach(GameObject match in matchList)
        {
            Destroy(match.gameObject);
        }
        matchList.Clear();
    }
}
