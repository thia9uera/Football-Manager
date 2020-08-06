using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardView : BaseScreen
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

    [SerializeField]
    private TMP_Dropdown playersDropdown, teamsDropdown;

    [SerializeField]
    private List<string> playerStats, teamStats;

    private List<PlayerData> listPlayers;
    private List<TeamData> listTeams;

    public LeaderboardType type; 
    public Button btnPlayers;
    public Button btnTeams;

    public GameObject headerPlayers;
    public GameObject headerTeams;

    private string playerSorting = "Name";
    private string teamSorting = "Name";
    private string customSorting;

    [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField]
    int maxRows = 25;

    public override void Show()
    {
        base.Show();

        if (listPlayers == null || listPlayers.Count == 0) listPlayers = new List<PlayerData>(MainController.Instance.AllPlayers);
        if (listTeams == null || listTeams.Count == 0) listTeams = new List<TeamData> (MainController.Instance.AllTeams);

        playersDropdown.ClearOptions();
        playersDropdown.AddOptions(playerStats);
        customSorting = playerStats[playersDropdown.value];

        SwitchLeaderboard(type);
    }

    public void Close()
    {
        ClearList();
        if(listPlayers != null) listPlayers.Clear();
        if (listTeams != null) listTeams.Clear();
        ScreenController.Instance.ShowPreviousScreen();
    }

    public void PopulatePlayers()
    {
        btnPlayers.interactable = false;
        btnTeams.interactable = true;
        for(int i = 0; i < maxRows; i++)
        {
            PlayerData player = listPlayers[i];
            LeaderboardPlayerView item = Instantiate(playerTeamplate, content);
            float customStat = player.GetStatistic(customSorting);
            item.Populate(player, i, customStat);
        }
    }

    public void PopulateTeams()
    {
        btnPlayers.interactable = true;
        btnTeams.interactable = false;

        for (int i = 0; i < maxRows; i++)
        {
            TeamData team = listTeams[i];
            LeaderboardTeamView item = Instantiate(teamTeamplate, content);
            item.Populate(team, i);;
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
            listPlayers = MainController.Instance.SortPlayersBy(listPlayers, _stat);
            PopulatePlayers();
        }
        else
        {
            listTeams = MainController.Instance.SortTeamsBy(listTeams, _stat);
            PopulateTeams();
        }

        scrollRect.verticalNormalizedPosition = 1;
    }

    public void SwitchLeaderboard(string _type)
    {
        ClearList();
        switch(_type)
        {
            case "Players": SwitchLeaderboard(LeaderboardType.Players); break;
            case "Teams":SwitchLeaderboard(LeaderboardType.Teams); break;
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
                    SortBy(playerSorting);
                }
                break;

            case LeaderboardType.Teams:
                {
                    type = LeaderboardType.Teams;
                    headerPlayers.SetActive(false);
                    headerTeams.SetActive(true);
                    SortBy(teamSorting);
                }
                break;
        }
    }

    public void OnPlayersDropdownChange()
    {
        customSorting = playerStats[playersDropdown.value];
        SortBy(customSorting);
    }
}
