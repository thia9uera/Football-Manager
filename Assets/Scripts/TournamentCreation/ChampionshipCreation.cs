using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChampionshipCreation : MonoBehaviour
{
    [SerializeField]
    TournamentCreationMatch matchTemplate;

    [SerializeField]
    TextMeshProUGUI roundLabelTemplate;

    [SerializeField]
    Transform content;

    List<GameObject> matchList;
    public List<TournamentData.MatchData> DataList;

    List<TeamData> placeholderList;
    int totalTeams;

    void Awake()
    {
        DataList = new List<TournamentData.MatchData>();
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
                TeamData team = new TeamData();
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
        TeamData homeTeam = null;
        TeamData awayTeam = null;
        for (int r = 0; r < rounds; r++)
        {
            for (int t = 0; t < half; t++)
            {
                homeTeam = listA[t];
                awayTeam = listA[rounds - t];

                TournamentData.MatchData data = new TournamentData.MatchData();
                data.HomeTeam = new TournamentData.TeamMatchData();
                data.HomeTeam.Team = homeTeam;

                data.AwayTeam = new TournamentData.TeamMatchData();
                data.AwayTeam.Team = awayTeam;

                data.Round = r;
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
        }

        UpdateMatchList();
    }

    public void UpdateMatchList()
    {
        if(matchList.Count > 0) ClearMatchList();
        int round = -1;
        foreach(TournamentData.MatchData data in DataList)
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
        foreach(GameObject match in matchList)
        {
            Destroy(match.gameObject);
        }
        matchList.Clear();
    }
}
