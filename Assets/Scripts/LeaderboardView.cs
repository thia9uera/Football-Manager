using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LeaderboardView : MonoBehaviour
{
    [SerializeField]
    private Transform content;

    [SerializeField]
    private LeaderboardItemView itemTemplate;

    private List<PlayerData> list;


    public void Show()
    {
        gameObject.SetActive(true);
        Populate();
    }

    public void Close()
    {
        ClearList();
        gameObject.SetActive(false);
        list.Clear();
    }

    public void Populate()
    {
        if (list == null || list.Count == 0) list = new List<PlayerData>( Resources.FindObjectsOfTypeAll<PlayerData>());

        int i = 0;
        foreach(PlayerData player in list)
        {
            LeaderboardItemView item = Instantiate(itemTemplate, content);
            item.Populate(player, i);
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
        foreach (PlayerData _player in list)
        {
            _player.ResetStatistics();
        }
        Populate();
    }

    public void SortBy(string _stat)
    {
        ClearList();

        switch(_stat)
        {
            case "Position": list = list.OrderBy(PlayerData => PlayerData.Position).ToList(); break;
            case "Name": list = list.OrderBy(PlayerData => PlayerData.FirstName).ToList(); break;
            case "Goals": list = list.OrderByDescending(PlayerData => PlayerData.TotalGoals).ToList(); break;
            case "Shots": list = list.OrderByDescending(PlayerData => PlayerData.TotalShots).ToList(); break;
            case "Passes": list = list.OrderByDescending(PlayerData => PlayerData.TotalPasses).ToList(); break;
            case "Crosses": list = list.OrderByDescending(PlayerData => PlayerData.TotalCrosses).ToList(); break;
            case "Faults": list = list.OrderByDescending(PlayerData => PlayerData.TotalFaults).ToList(); break;
            case "Tackles": list = list.OrderByDescending(PlayerData => PlayerData.TotalTackles).ToList(); break;
            case "Dribbles": list = list.OrderByDescending(PlayerData => PlayerData.TotalDribbles).ToList(); break;
            case "Headers": list = list.OrderByDescending(PlayerData => PlayerData.TotalHeaders).ToList(); break;
            case "Saves": list = list.OrderByDescending(PlayerData => PlayerData.TotalSaves).ToList(); break;
        }

        Populate();
    }
}
