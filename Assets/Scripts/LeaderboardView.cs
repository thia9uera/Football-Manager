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

    private string playerSorting = "Name";
    private string teamSorting = "Name";

    public void Show()
    {
        gameObject.SetActive(true);
        SwitchLeaderboard(type);
    }

    public void Close()
    {
        ClearList();
        gameObject.SetActive(false);
        if(listPlayers != null) listPlayers.Clear();
        if (listTeams != null) listTeams.Clear();
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
                _player.ResetStatistics("LifeTime");
            }
        }
        if (listTeams != null)
        {
            foreach (TeamData _player in listTeams)
            {
                _player.ResetStatistics("LifeTime");
            }
        }
        if (type == LeaderboardType.Players) PopulatePlayers();
        else PopulateTeams();
    }

    public void SortBy(string _stat)
    {
        ClearList();
        if (type == LeaderboardType.Players) playerSorting = _stat;
        else teamSorting = _stat;

        if (type == LeaderboardType.Players)
        {
            switch (_stat)
            {
                case "Position": listPlayers = listPlayers.OrderBy(PlayerData => PlayerData.Zone).ToList(); break;
                case "Name": listPlayers = listPlayers.OrderBy(PlayerData => PlayerData.FirstName).ToList(); break;
                case "Goals": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.LifeTimeStats.TotalGoals).ToList(); break;
                case "Shots": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.LifeTimeStats.TotalShots).ToList(); break;
                case "ShotsMissed": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.LifeTimeStats.TotalShotsMissed).ToList(); break;
                case "Headers": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.LifeTimeStats.TotalHeaders).ToList(); break;
                case "HeadersMissed": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.LifeTimeStats.TotalHeadersMissed).ToList(); break;
                case "Passes": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.LifeTimeStats.TotalPasses).ToList(); break;
                case "Crosses": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.LifeTimeStats.TotalCrosses).ToList(); break;
                case "Faults": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.LifeTimeStats.TotalFaults).ToList(); break;
                case "Tackles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.LifeTimeStats.TotalTackles).ToList(); break;
                case "Dribbles": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.LifeTimeStats.TotalDribbles).ToList(); break;   
                case "Saves": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.LifeTimeStats.TotalSaves).ToList(); break;
                case "Presence": listPlayers = listPlayers.OrderByDescending(PlayerData => PlayerData.LifeTimeStats.TotalPresence).ToList(); break;
            }

            PopulatePlayers();
        }
        else
        {
            switch (_stat)
            {
                case "Name": listTeams = listTeams.OrderBy(TeamData => TeamData.Name).ToList(); break;
                case "Wins": listTeams = listTeams.OrderByDescending(TeamData => TeamData.LifeTimeStats.TotalWins).ToList(); break;
                case "Losts": listTeams = listTeams.OrderByDescending(TeamData => TeamData.LifeTimeStats.TotalLosts).ToList(); break;
                case "Draws": listTeams = listTeams.OrderByDescending(TeamData => TeamData.LifeTimeStats.TotalDraws).ToList(); break;
                case "Goals": listTeams = listTeams.OrderByDescending(TeamData => TeamData.LifeTimeStats.TotalGoals).ToList(); break;
                case "GoalsAgainst": listTeams = listTeams.OrderByDescending(TeamData => TeamData.LifeTimeStats.TotalGoalsAgainst).ToList(); break;

                case "Shots": listTeams = listTeams.OrderByDescending(TeamData => TeamData.LifeTimeStats.TotalShots).ToList(); break;
                case "Headers": listTeams = listTeams.OrderByDescending(TeamData => TeamData.LifeTimeStats.TotalHeaders).ToList(); break;
                case "Steals": listTeams = listTeams.OrderByDescending(TeamData => TeamData.LifeTimeStats.TotalSteals).ToList(); break;
                case "Passes": listTeams = listTeams.OrderByDescending(TeamData => TeamData.LifeTimeStats.TotalPasses).ToList(); break;
                case "LongPasses": listTeams = listTeams.OrderByDescending(TeamData => TeamData.LifeTimeStats.TotalLongPasses).ToList(); break;
                case "PassesMissed": listTeams = listTeams.OrderByDescending(TeamData => TeamData.LifeTimeStats.TotalPassesMissed).ToList(); break;

                case "BoxCrosses": listTeams = listTeams.OrderByDescending(TeamData => TeamData.LifeTimeStats.TotalBoxCrosses).ToList(); break;
                case "Faults": listTeams = listTeams.OrderByDescending(TeamData => TeamData.LifeTimeStats.TotalFaults).ToList(); break;
                case "CounterAttacks": listTeams = listTeams.OrderByDescending(TeamData => TeamData.LifeTimeStats.TotalCounterAttacks).ToList(); break;
            }

            PopulateTeams();
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
                    SortBy(playerSorting);
                }
                break;

            case "Teams":
                {
                    type = LeaderboardType.Teams;
                    headerPlayers.SetActive(false);
                    headerTeams.SetActive(true);
                    PopulateTeams();
                    SortBy(teamSorting);
                }
                break;
        }
    }

    public void SwitchLeaderboard(LeaderboardType _type)
    {
        ClearList();
        switch (_type)
        {
            case LeaderboardType.Players:
                {
                    type = LeaderboardType.Players;
                    headerPlayers.SetActive(true);
                    headerTeams.SetActive(false);
                    PopulatePlayers();
                    SortBy(playerSorting);
                }
                break;

            case LeaderboardType.Teams:
                {
                    type = LeaderboardType.Teams;
                    headerPlayers.SetActive(false);
                    headerTeams.SetActive(true);
                    PopulateTeams();
                    SortBy(teamSorting);
                }
                break;
        }
    }
}
