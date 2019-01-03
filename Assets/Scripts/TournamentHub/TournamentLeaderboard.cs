using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentLeaderboard : MonoBehaviour
{
    [SerializeField]
    TournamentLeaderboardItem itemTemplate;

    [SerializeField]
    Transform content;

    public void Populate(List<TournamentData.TeamTournamentData> _list)
    {
        Clear();
        int index = 1;
        foreach (TournamentData.TeamTournamentData data in _list)
        {
            TournamentLeaderboardItem item = Instantiate(itemTemplate, content);
            item.Populate(index, data.Team.Name, data.Points + " pts", data.Team.isUserControlled);
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
