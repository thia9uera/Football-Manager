using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TournamentFixtures : MonoBehaviour
{
    [SerializeField]
    TournamentFixturesItem itemTemplate;

    [SerializeField]
    TextMeshProUGUI roundLabelTemplate;

    [SerializeField]
    Transform content;

    List<GameObject> itemList;

    public void Populate(List<TournamentData.MatchData> _list, int _currentRound)
    {
        Clear();
        int round = -1;
        foreach (TournamentData.MatchData data in _list)
        {
            if (data.Round > round)
            {
                round = data.Round;
                TextMeshProUGUI txt = Instantiate(roundLabelTemplate, content);
                txt.text = "Round " + (round + 1);
                if (round > _currentRound) txt.color = Color.gray;
                itemList.Add(txt.gameObject);
            }
            TournamentFixturesItem match = Instantiate(itemTemplate, content);
            match.Populate(data);
            itemList.Add(match.gameObject);
        }
    }

    void Clear()
    {
        if (itemList == null) itemList = new List<GameObject>();
        foreach (GameObject item in itemList)
        {
            Destroy(item.gameObject);
        }
        itemList.Clear();
    }
}
