using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchTeamView : MonoBehaviour
{
	[SerializeField] private MatchPlayerView[] itemList = null;

    public void Populate(TeamData _team, bool _resetFatigue = false)
    {
        PlayerData[] squad = _team.Squad;
        for(int i = 0; i < 11; i++)
        {
            PlayerData player = squad[i];
            player.ApplyBonus();

            MatchPlayerView item = itemList[i];
            if (_resetFatigue) player.Fatigue = 100;
            item.Populate(player);
        }

        if(_resetFatigue)
        {
            foreach (PlayerData p in _team.Substitutes) p.Fatigue = 100;
        }
    }
    
	public void UpdatePlayers(TeamData _team)
	{
		MatchPlayerView playerView;
		for(int i = 0; i < itemList.Length; i++)
		{
			if(itemList[i].Player != _team.Squad[i])
			{
				playerView = itemList[i];
				playerView.SwitchPlayer(_team.Squad[i]);
			}
		}
	}

    public void UpdateFatigue()
    {
        foreach (MatchPlayerView player in itemList)
        {
            player.Player.Fatigue -= 0.05f * (100 / (float)player.Player.Stamina);
            player.UpdateFatigue();
        }
    }

    public void ResetFatigue()
    {
        foreach (MatchPlayerView player in itemList)
        {
            player.Player.Fatigue = 100;
            player.UpdateFatigue();
        }

    }
    public void ModifyFatigue(float _modifier)
    {
        foreach (MatchPlayerView player in itemList)
        {
            float newFatigue = player.Player.Fatigue * _modifier;
            if (newFatigue > 100f) newFatigue = 100f;
            player.Player.Fatigue = newFatigue;
            player.UpdateFatigue();
        }

    }
}
