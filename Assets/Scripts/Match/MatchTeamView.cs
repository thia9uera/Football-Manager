using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchTeamView : MonoBehaviour
{
    public List<MatchPlayerView> ItemList;

    public void Populate(TeamData _team, bool _resetFatigue = false)
    {
        PlayerData[] squad = _team.Squad;
        for(int i = 0; i < 11; i++)
        {
            PlayerData player = squad[i];
            player.ApplyBonus();

            MatchPlayerView item = ItemList[i];
            if (_resetFatigue) player.Fatigue = 100;
            item.Populate(player);        
        }
    }

    public void UpdateFatigue()
    {
        foreach (MatchPlayerView player in ItemList)
        {
            player.Player.Fatigue -= 0.05f * (100 / (float)player.Player.Stamina);
            player.UpdateFatigue();
        }
    }

    public void ResetFatigue()
    {
        foreach (MatchPlayerView player in ItemList)
        {
            player.Player.Fatigue = 100;
            player.UpdateFatigue();
        }

    }
    public void ModifyFatigue(float _modifier)
    {
        foreach (MatchPlayerView player in ItemList)
        {
            float newFatigue = player.Player.Fatigue * _modifier;
            if (newFatigue > 100f) newFatigue = 100f;
            player.Player.Fatigue = newFatigue;
            player.UpdateFatigue();
        }

    }
}
