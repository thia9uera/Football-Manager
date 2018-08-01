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
        PlayerData[] squad = _team.Squad;
        ItemList = new List<MatchPlayerView>();
        for(int i = 0; i < 11; i++)
        {
            PlayerData player = squad[i];
            player.ApplyBonus(_team.GetStrategy());
            //TODO Remove this when we have team setup
            player.AssignedPosition = player.Position;

            MatchPlayerView item = Instantiate(playerTemplate, transform);
            ItemList.Add(item);
            item.Populate(player);      
        }
    }
}
