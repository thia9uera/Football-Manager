using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TournamentCreation : BaseScreen
{
    public static TournamentCreation Instance;

    public TournamentCreationOptions Options;

    public ChampionshipCreation Championship;

    public List<TeamData> AvailableTeams;

    public GameObject Edit;
    public GameObject Load;

    public List<TeamData> TeamList;

    public Button BtnCreateTournament;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        Edit.SetActive(false);
        Load.SetActive(true);
    }

    public override void Show()
    {
        base.Show();
        Edit.SetActive(false);
        Load.SetActive(true);
        LoadTournaments();
    }

    public void AddTeam(TeamData _team)
    {
        if(Options.TypeDropDown.value == (int)TournamentAttributes.TournamentType.Championship)
        {
            Championship.AddTeam(_team);
            AvailableTeams.Remove(_team);
        }
    }

    public void RemoveTeam(string _teamId)
    {
        if (Options.TypeDropDown.value == (int)TournamentAttributes.TournamentType.Championship)
        {
            TeamData team = MainController.Instance.GetTeamById(_teamId);
            Championship.RemoveTeam(team);
            AvailableTeams.Add(team);
            Options.TeamList.UpdateTeamList();
        }
    }

    public void CreateTournament()
    {
        TournamentData tournament = ScriptableObject.CreateInstance<TournamentData>();
        List<TeamData> teams = new List<TeamData>(TeamList);
        List<MatchData> matches = new List<MatchData>(Championship.DataList);
        tournament.Name = Options.InputName.text;
        tournament.Type = (TournamentAttributes.TournamentType)Options.TypeDropDown.value;
        tournament.StarsRequired = Options.StarsRequired.StarsRequired;

        switch((TournamentAttributes.TournamentType)Options.TypeDropDown.value)
        {
            case TournamentAttributes.TournamentType.Championship:
                //tournament.Teams = teams;
                tournament.Matches = matches;
                int t = teams.Count;
                string[] teamIds = new string[t];
                tournament.TotalRounds = t - 1;
                for (int i = 0; i < t; i++)
                {
                    teamIds[i] = teams[i].Id;
                }
                tournament.Attributes.TeamIds = teamIds;
                break;
        }
        if (string.IsNullOrEmpty(tournament.Id)) tournament.Id = Guid.NewGuid().ToString();
        AssetDatabase.CreateAsset(tournament, "Assets/Data/Tournaments/" + tournament.Name + ".asset");
        AssetDatabase.SaveAssets();
        MainController.Instance.AllTournaments.Add(tournament);
        LoadTournaments();
    }

    public void LoadTournaments()
    {
        TeamList.Clear();

        Edit.SetActive(false);
        Load.SetActive(true);
        Load.GetComponent<TournamentLoad>().LoadFiles();
    }

    public void EditTournament(TournamentData _data)
    {
        AvailableTeams = new List<TeamData>(MainController.Instance.AllTeams);

        Edit.SetActive(true);
        Load.SetActive(false);

        Options.InputName.text = _data.Name;
        Options.TypeDropDown.value = (int)_data.Type;
        Options.StarsRequired.StarsRequired = _data.StarsRequired;

        BtnCreateTournament.interactable = false;
        TeamList = new List<TeamData>();
        TeamList = new List<TeamData>(_data.Teams);
        Championship.CreateRounds();

        foreach(TeamData team in TeamList)
        {
            if (AvailableTeams.Contains(team)) AvailableTeams.Remove(team);
        }
        Options.TeamList.UpdateTeamList();

        if((TournamentAttributes.TournamentType)Options.TypeDropDown.value == TournamentAttributes.TournamentType.Championship)
        {
            //TODO show Championship tables
        }
    }
}
