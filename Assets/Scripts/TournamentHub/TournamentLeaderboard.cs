using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentLeaderboard : MonoBehaviour
{
    [SerializeField]
    TournamentLeaderboardItem itemTemplate;

    [SerializeField]
    Transform content;

    public void Populate(List<TeamData> _list, string _id)
    {
        Clear();
        int index = 1;
        foreach (TeamData data in _list)
        {
            TournamentLeaderboardItem item = Instantiate(itemTemplate, content);
            item.Populate(index, data.Name, data.TournamentStatistics[_id].Points + " pts", data.isUserControlled);
            item.gameObject.SetActive(true);
            index++;
        }
    }

    public void Populate(List<PlayerData> _list, string _tournamentId, string _param)
    {
        Clear();
        int index = 1;
        foreach (PlayerData data in _list)
        {
            TournamentLeaderboardItem item = Instantiate(itemTemplate, content);
            string param = "";
            switch(_param)
            {
                case "Goals" :  param = data.TournamentStatistics[_tournamentId].TotalGoals.ToString(); break;
            }

            item.Populate(index, data.FirstName + " " + data.LastName, param, data.Team.isUserControlled);
            item.gameObject.SetActive(true);
            index++;
        }
    }

    void Clear()
    {
        foreach(Transform t in content)
        {
            if(t.gameObject.activeSelf) Destroy(t.gameObject);
        }
    }
}
