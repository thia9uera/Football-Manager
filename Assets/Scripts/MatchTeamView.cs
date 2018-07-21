using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchTeamView : MonoBehaviour
{
    public string TeamName;
    public MatchPlayerView[] PlayerList;

    public Color TeamColor;

    public int Keeper;
    public int BackAtt;
    public int BackDef;
    public int CenterAtt;
    public int CenterDef;
    public int ForwardAtt;
    public int ForwardDef;

    public void Populate(PlayerData[] _squad)
    {
        for(int i = 0; i < _squad.Length; i++)
        {
            PlayerData player = _squad[i];
            PlayerList[i].Populate(player);

            switch(player.Position)
            {
                case PlayerData.PlayerPosition.GK:
                    Keeper += player.Goalkeeping + player.Speed;
                    break;

                case PlayerData.PlayerPosition.CD:
                    BackAtt += player.Passing + player.Agility;
                    BackDef += player.Tackling + player.Strength;
                    break;

                case PlayerData.PlayerPosition.CM:
                    CenterAtt += player.Passing +  player.Agility;
                    CenterDef += player.Tackling + player.Strength;
                    break;
                case PlayerData.PlayerPosition.CF:
                    ForwardAtt += player.Shooting + player.Agility;
                    ForwardDef += player.Tackling + player.Strength;
                    break;
            }
        }
    }
}
