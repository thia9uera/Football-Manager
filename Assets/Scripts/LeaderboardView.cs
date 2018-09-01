using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LeaderboardView : MonoBehaviour
{
    public enum LeaderboardType
    {
        Players,
        Teams
    }

    [SerializeField]
    private Transform content;

    [SerializeField]
    private LeaderboardPlayerView playerTeamplate;

    [SerializeField]
    private LeaderboardTeamView teamTeamplate;

    private List<PlayerData> listPlayers;
    private List<TeamData> listTeams;

    public LeaderboardType type; 
    public Button btnPlayers;
    public Button btnTeams;

    public GameObject headerPlayers;
    public GameObject headerTeams;

    public void Show()
    {
        gameObject.SetActive(true);
        if (type == LeaderboardType.Players)
        {
            PopulatePlayers();

        }
        else
        {
            PopulateTeams();

        }
    }

    public void Close()
    {
        ClearList();
        gameObject.SetActive(false);
        listPlayers.Clear();
    }

    public void PopulatePlayers()
    {
        btnPlayers.interactable = false;
        btnTeams.interactable = true;

        if (listPlayers == null || listPlayers.Count == 0) listPlayers = new List<PlayerData>( Resources.FindObjectsOfTypeAll<PlayerData>());

        int i = 0;
        foreach(PlayerData player in listPlayers)
        {
            LeaderboardPlayerView item = Instantiate(playerTeamplate, content);
            item.Populate(player, i);
            i++;
        }
    }

    public void PopulateTeams()
    {
        btnPlayers.interactable = true;
        btnTeams.interactable = false;

        if (listTeams == null || listTeams.Count == 0) listTeams = new List<TeamData>( Resources.FindObjectsOfTypeAll<TeamData>());

        int i = 0;
        foreach(TeamData team in listTeams)
        {
            LeaderboardTeamView item = Instantiate(teamTeamplate, content);
            item.Populate(team, i);
            i++;
        }
    }

    public void ClearList()
    {
        foreach (Transform t in content)
        {
            Destroy(t.gameObject);
        }      
    }

    public void ResetStats()
    {
        ClearList();
        if (listPlayers != null)
        {
            foreach (PlayerData _player in listPlayers)
            {
                _player.ResetStatistics();
            }
        }
        if (listTeams != null)
        {
            foreach (TeamData _player in listTeams)
            {
                _player.ResetStatistics();
            }
        }
        if (type == LeaderboardType.Players) PopulatePlayers();
        else PopulateTeams();
    }

    public void SortBy(string _stat)
    {
        ClearList();

        if (type == LeaderboardType.Players)
        {
            switch (_stat)
            {
                case "Position": listPlayers = listPlayers.OrderBy(PlayerData => PlayerData.Position).ToList(); break;
                case "Name": listPlayers = listPlayers.OrderBy(PlayerData => PlayerData.FirstName).ToList(); break;
                case "Goals": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TotalGoals).ToList(); break;
                case "Shots": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TotalShots).ToList(); break;
                case "Passes": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TotalPasses).ToList(); break;
                case "Crosses": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TotalCrosses).ToList(); break;
                case "Faults": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TotalFaults).ToList(); break;
                case "Tackles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TotalTackles).ToList(); break;
                case "Dribbles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TotalDribbles).ToList(); break;
                case "Headers": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TotalHeaders).ToList(); break;
                case "Saves": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.TotalSaves).ToList(); break;
            }

            PopulatePlayers();
        }
        else
        {
            switch (_stat)
            {
                case "Name": listTeams = listTeams.OrderBy(TeamData => TeamData.Name).ToList(); break;
                case "Wins": listTeams = listTeams.OrderByDescending(TeamData => TeamData.TotalWins).ToList(); break;
                case "Losts": listTeams = listTeams.OrderByDescending(TeamData => TeamData.TotalLosts).ToList(); break;
                case "Draws": listTeams = listTeams.OrderByDescending(TeamData => TeamData.TotalDraws).ToList(); break;
                case "Goals": listTeams = listTeams.OrderByDescending(TeamData => TeamData.TotalGoals).ToList(); break;
                case "GoalsAgainst": listTeams = listTeams.OrderByDescending(TeamData => TeamData.TotalGoalsAgainst).ToList(); break;
            }
        }
    }

    public void SwitchLeaderboard(string _type)
    {
        ClearList();
        switch(_type)
        {
            case "Players":
                {
                    type = LeaderboardType.Players;
                    headerPlayers.SetActive(true);
                    headerTeams.SetActive(false);
                    PopulatePlayers();
                }
                break;

            case "Teams":
                {
                    type = LeaderboardType.Teams;
                    headerPlayers.SetActive(false);
                    headerTeams.SetActive(true);
                    PopulateTeams();
                }
                break;
        }
    }
}
