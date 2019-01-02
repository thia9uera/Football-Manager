using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionshipCreation : MonoBehaviour
{
    [SerializeField]
    TournamentCreationMatch matchTemplate;

    [SerializeField]
    Transform content;

    List<TournamentCreationMatch> matchList;
    public List<TournamentData.MatchData> DataList;

    void Awake()
    {
        DataList = new List<TournamentData.MatchData>();
        matchList = new List<TournamentCreationMatch>();
    }

    public void AddTeam(TeamData _data)
    {
        TournamentCreation.Instance.TeamList.Add(_data);
        DataList.Clear();
        CreateMatchData();
    }

    public void RemoveTeam(TeamData _data)
    {
        TournamentCreation.Instance.TeamList.Remove(_data);
        DataList.Clear();
        CreateMatchData();
    }

    public void CreateMatchData()
    {
        List<TeamData> list = TournamentCreation.Instance.TeamList;
        TeamData homeTeam = null;
        TeamData awayTeam = null;
        List<TeamData> excludeList = new List<TeamData>();
        float teams = list.Count;
        int totalMatches = (int)((teams / 2) * (teams - 1));

        if (list.Count == 1)
        {
            TournamentData.MatchData match = new TournamentData.MatchData();
            DataList.Add(match);
            match.HomeTeam = new TournamentData.TeamMatchData();
            match.HomeTeam.Team = list[0];
            UpdateMatchList();
        }

        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (!excludeList.Contains(list[j]))
                    {
                        homeTeam = list[i];
                        awayTeam = list[j];
                        if ((i - j) % 2 == 0)
                        {
                            homeTeam = list[i];
                            awayTeam = list[j];
                        }
                        else
                        {
                            homeTeam = list[j];
                            awayTeam = list[i];
                        }

                        if (homeTeam != awayTeam)
                        {
                            TournamentData.MatchData data = new TournamentData.MatchData();
                            data.HomeTeam = new TournamentData.TeamMatchData();
                            data.HomeTeam.Team = homeTeam;

                            data.AwayTeam = new TournamentData.TeamMatchData();
                            data.AwayTeam.Team = awayTeam;

                            if (!DataList.Contains(data)) DataList.Add(data);
                        }
                    }
                    excludeList.Add(list[i]);
                }
            }
            UpdateMatchList();
        }
    }

    public void UpdateMatchList()
    {
        if(matchList.Count > 0) ClearMatchList();

        foreach(TournamentData.MatchData data in DataList)
        {
            TournamentCreationMatch match = Instantiate(matchTemplate, content);
            match.Populate(data);
            matchList.Add(match);
        }
    }

    public void ClearMatchList()
    {
        foreach(TournamentCreationMatch match in matchList)
        {
            Destroy(match.gameObject);
        }
        matchList.Clear();
    }
}
