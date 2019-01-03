using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TournamentCreation : MonoBehaviour
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

        //if (_data.Teams.Count > 0) Options.TeamsAmount.TeamsAmount = _data.Teams.Count;
        //else Options.TeamsAmount.TeamsAmount = 2;

        BtnCreateTournament.interactable = false;
        TeamList = new List<TeamData>();
        TeamList = new List<TeamData>(_data.Teams);
        Championship.CreateRounds();

        foreach(TeamData team in TeamList)
        {
            if (AvailableTeams.Contains(team)) AvailableTeams.Remove(team);
        }
        Options.TeamList.UpdateTeamList();

        if((TournamentData.TournamentType)Options.TypeDropDown.value == TournamentData.TournamentType.Championship)
        {
            //TODO show Championship tables
        }
    }
}
