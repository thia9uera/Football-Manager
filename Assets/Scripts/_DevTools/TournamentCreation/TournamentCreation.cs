#if (UNITY_EDITOR)
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
	public CupCreation Cup;
    
    public GameObject Edit;
	public GameObject Load;
	
	[HideInInspector] public List<TeamData> AvailableTeams;
	[HideInInspector] public List<TeamData> TeamList;
    
	[SerializeField] private Toggle homeAwayTeams = null;
	
	private InitialData initialData;
	private TournamentData editingTournament;
	private bool isNewTournament;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
	private void Start()
	{
		//CalendarController.Instance.InitializeCalendar(2020);
		initialData = (InitialData) Tools.GetFile<InitialData>("Data/Misc/InitialData.asset");
		editingTournament = null;
		Show();
	}

    public override void Show()
    {
        base.Show();
	    LoadTournaments();
	    //ChangeType(TournamentType.Championship);
    }

    public void AddTeam(TeamData _team)
    {
        if(Options.TypeDropDown.value == (int)TournamentType.Championship)
        {
            Championship.AddTeam(_team);
            AvailableTeams.Remove(_team);
        }
    }

    public void RemoveTeam(string _teamId)
    {
        if (Options.TypeDropDown.value == (int)TournamentType.Championship)
        {
            TeamData team = MainController.Instance.GetTeamById(_teamId);
            Championship.RemoveTeam(team);
            AvailableTeams.Add(team);
            Options.TeamList.UpdateTeamList();
        }
    }

    public void ChangeType(TournamentType _type)
    {
        switch (_type)
        {
            case TournamentType.Championship:
	            Championship.gameObject.SetActive(true);
	        	//Championship.ClearMatchList();
                Cup.gameObject.SetActive(false);
                break;
            case TournamentType.Cup:
                Championship.gameObject.SetActive(false);
	            Cup.gameObject.SetActive(true);
                break;
        }
    }

    public void CreateTournament()
	{
		TournamentData tournament;
		if(!isNewTournament) tournament = editingTournament;
		else 
		{
			tournament = ScriptableObject.CreateInstance<TournamentData>();
			//tournament.Id = Guid.NewGuid().ToString();
		}
        List<TeamData> teams = new List<TeamData>(TeamList);
        List<MatchData> matches = new List<MatchData>(Championship.DataList);
        tournament.Name = Options.InputName.text;
		tournament.Type = (TournamentType)Options.TypeDropDown.value;
		tournament.Id = Championship.TournamentId;

        switch((TournamentType)Options.TypeDropDown.value)
        {
            case TournamentType.Championship:
                //tournament.Teams = teams;
                tournament.Matches = matches;
                int t = teams.Count;
                string[] teamIds = new string[t];
	            tournament.TotalRounds = t - 1;
	            if(homeAwayTeams.isOn) tournament.TotalRounds *= 2;
                for (int i = 0; i < t; i++)
                {
                    teamIds[i] = teams[i].Id;
                }
                tournament.Attributes.TeamIds = teamIds;
                break;
        }
		
		if(isNewTournament) AssetDatabase.CreateAsset(tournament, "Assets/Data/Tournaments/" + tournament.Name + ".asset");
		EditorUtility.SetDirty(tournament);
	    AssetDatabase.SaveAssets();
		
        LoadTournaments();
		//DataController.Instance.SaveGame();
		editingTournament = null;
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
		editingTournament = _data;
        AvailableTeams = new List<TeamData>(MainController.Instance.AllTeams);

        Edit.SetActive(true);
        Load.SetActive(false);

        Options.InputName.text = _data.Name;
		Options.TypeDropDown.value = (int)_data.Type;
		Championship.TournmanentName = Options.InputName.text;
		
		isNewTournament = editingTournament.Id == "Template";
		
		if(isNewTournament) 
		{
			Championship.TournamentId = Guid.NewGuid().ToString();		
		}
		else
		{
			Championship.TournamentId = editingTournament.Id;
		}

        TeamList = new List<TeamData>();
        TeamList = new List<TeamData>(_data.Teams);
        Championship.CreateRounds();

        foreach(TeamData team in TeamList)
        {
            if (AvailableTeams.Contains(team)) AvailableTeams.Remove(team);
        }
        Options.TeamList.UpdateTeamList();

        if((TournamentType)Options.TypeDropDown.value == TournamentType.Championship)
        {
	        Options.ChampionshipSetup();
        }
    }
   
	public void DeleteTournament(TournamentData _data)
	{
		AssetDatabase.DeleteAsset("Assets/Data/Tournaments/" + _data.Name + ".asset");
		LoadTournaments();
	}
	
	public void OnTournamentNameChange()
	{
		Championship.TournmanentName = Options.InputName.text;
	}
}
#endif
