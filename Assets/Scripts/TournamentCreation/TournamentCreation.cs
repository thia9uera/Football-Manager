using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TournamentCreation : MonoBehaviour
{
    public static TournamentCreation Instance;

    public TournamentCreationOptions Options;

    public ChampionshipCreation Championship;

    public List<TeamData> AvailableTeams;

    public GameObject Edit;
    public GameObject Load;

    public List<TeamData> TeamList;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddTeam(TeamData _team)
    {
        if(Options.TypeDropDown.value == (int)TournamentData.TournamentType.Championship)
        {
            Championship.AddTeam(_team);
            AvailableTeams.Remove(_team);
        }
    }

    public void RemoveTeam(TeamData _team)
    {
        if (Options.TypeDropDown.value == (int)TournamentData.TournamentType.Championship)
        {
            Championship.RemoveTeam(_team);
            AvailableTeams.Add(_team);
            Options.TeamList.UpdateTeamList();
        }
    }

    public void CreateTournament()
    {
        TournamentData tournament = ScriptableObject.CreateInstance<TournamentData>();
        List<TeamData> teams = new List<TeamData>(TeamList);
        List<TournamentData.MatchData> matches = new List<TournamentData.MatchData>(Championship.DataList);
        tournament.Name = Options.InputName.text;
        tournament.Type = (TournamentData.TournamentType)Options.TypeDropDown.value;
        tournament.StarsRequired = Options.StarsRequired.StarsRequired;

        switch((TournamentData.TournamentType)Options.TypeDropDown.value)
        {
            case TournamentData.TournamentType.Championship:
                tournament.Teams = teams;
                tournament.Matches = matches;
                tournament.TeamScoreboard = new List<TournamentData.TeamTournamentData>();
                foreach (TeamData team in TeamList)
                {
                    TournamentData.TeamTournamentData tData = new TournamentData.TeamTournamentData();
                    tData.Team = team;
                    tournament.TeamScoreboard.Add(tData);
                }
                break;
        }

        AssetDatabase.CreateAsset(tournament, "Assets/Resources/Tournaments/" + tournament.Name + ".asset");
        AssetDatabase.SaveAssets();

        LoadTournaments();
    }

    public void LoadTournaments()
    {
        Championship.DataList.Clear();
        Championship.ClearMatchList();
        TeamList.Clear();

        Edit.SetActive(false);
        Load.SetActive(true);
        Load.GetComponent<TournamentLoad>().LoadFiles();
    }

    public void EditTournament(TournamentData _data)
    {
        AvailableTeams = new List<TeamData>(Resources.LoadAll<TeamData>("Teams"));

        Edit.SetActive(true);
        Load.SetActive(false);

        Options.InputName.text = _data.Name;
        Options.TypeDropDown.value = (int)_data.Type;
        Options.StarsRequired.StarsRequired = _data.StarsRequired;

        TeamList = new List<TeamData>();
        if(_data.Teams.Count > 0)
        {
            TeamList = new List<TeamData>(_data.Teams);
        }
        
        foreach(TeamData team in TeamList)
        {
            if (AvailableTeams.Contains(team)) AvailableTeams.Remove(team);
        }
        Options.TeamList.UpdateTeamList();

        if((TournamentData.TournamentType)Options.TypeDropDown.value == TournamentData.TournamentType.Championship)
        {
            Championship.DataList.Clear();
            Championship.ClearMatchList();

            Championship.CreateMatchData();
        }
    }
}
