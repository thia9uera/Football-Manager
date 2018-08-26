using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchTeamView : MonoBehaviour
{
    public List<MatchPlayerView> ItemList;

    [SerializeField]
    private MatchPlayerView playerTemplate;

    public void Populate(TeamData _team)
    {
        if(ItemList != null) foreach (MatchPlayerView item in ItemList) Destroy(item.gameObject);

        PlayerData[] squad = _team.Squad;
        ItemList = new List<MatchPlayerView>();
        for(int i = 0; i < 11; i++)
        {
            PlayerData player = squad[i];
            player.ApplyBonus(_team.GetStrategy());
            player.Fatigue = 100;

            MatchPlayerView item = Instantiate(playerTemplate, transform);
            ItemList.Add(item);
            item.Populate(player);      
        }
    }

    public void UpdateFatigue()
    {
        foreach (MatchPlayerView player in ItemList)
        {
            player.UpdateFatigue();
        }
    }
}
