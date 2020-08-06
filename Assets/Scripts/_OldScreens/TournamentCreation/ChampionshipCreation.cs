using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChampionshipCreation : MonoBehaviour
{
    [SerializeField]
    TournamentCreationMatch matchTemplate;

    [SerializeField]
    TextMeshProUGUI roundLabelTemplate;

    [SerializeField]
    Transform content;

    List<GameObject> matchList;
    public List<MatchData> DataList;

    List<TeamData> placeholderList;
    int totalTeams;

    [SerializeField]
    private Toggle homeAwayTeams;

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
        int rounds = totalTeams - 1;
        int half = totalTeams / 2;
        List<TeamData> listA = new List<TeamData>(teams);
        List<TeamData> listB = new List<TeamData>(teams);
        TeamData homeTeam;
        TeamData awayTeam;
        for (int r = 0; r < rounds; r++)
        {
            for (int t = 0; t < half; t++)
            {
                if (r % 2 == 0)
                {
                    homeTeam = listA[t];
                    awayTeam = listA[rounds - t];
                }
                else
                {
                    homeTeam = listA[rounds - t];
                    awayTeam = listA[t];
                }

                MatchData data = new MatchData(homeTeam.Attributes, awayTeam.Attributes, r);
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
            List<MatchData> dList = new List<MatchData>(DataList);
            foreach(MatchData data in dList)
            {
                MatchData d = new MatchData(data.AwayTeam.TeamAttributes, data.HomeTeam.TeamAttributes, data.Round + rounds);
                DataList.Add(d);
            }
        }

        UpdateMatchList();
    }

    public void UpdateMatchList()
	{
		if(matchList == null) matchList = new List<GameObject>();
        if(matchList.Count > 0) ClearMatchList();
        int round = -1;
        foreach(MatchData data in DataList)
        {
            if(data.Round > round)
            {
                round = data.Round;
                TextMeshProUGUI txt = Instantiate(roundLabelTemplate, content);
                txt.text = "Round " + (round + 1);
                matchList.Add(txt.gameObject);
            }
            TournamentCreationMatch match = Instantiate(matchTemplate, content);
            match.Populate(data);
            matchList.Add(match.gameObject);
        }

        TournamentCreation.Instance.BtnCreateTournament.interactable = (placeholderList.Count <= 0);
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
