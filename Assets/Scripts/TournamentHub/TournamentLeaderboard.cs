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
            item.Populate(index, data.Name, data.TournamentStatistics[_id].Points + " pts", data.IsUserControlled);
            item.gameObject.SetActive(true);
            index++;
        }
    }

    public void Populate(List<PlayerData> _list, string _tournamentId, string _param)
    {
        Clear();
        int index = 1;
        print("LIST SIZE: " + _list.Count);
        foreach (PlayerData data in _list)
        {
            TournamentLeaderboardItem item = Instantiate(itemTemplate, content);
            string param = "";
            switch(_param)
            {
                case "Goals" :  param = data.Attributes.TournamentStatistics[_tournamentId].TotalGoals.ToString(); break;
            }
            print("TOURNAMENT EXISTS: " + data.Attributes.TournamentStatistics.ContainsKey(_tournamentId));
            print("PLAYER NAME: " + data.FirstName);
            print("GOALS: " + param);
            print("ITEM: " + item.name);
            print("TEAM: " + data.Team.Name);
            item.Populate(index, data.GetFullName(), param, data.Team.IsUserControlled);
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
