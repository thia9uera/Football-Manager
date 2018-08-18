using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debug_FieldView : MonoBehaviour
{
    public PlayerData TestPlayer;

    public Dropdown dropDown;

    public Team_StrategyData teamStrategy;

    public void Start()
    {
        TestPlayer.Position = TestPlayer.AssignedPosition = PlayerData.PlayerPosition.GK;
        TestPlayer.ApplyBonus(teamStrategy.team_Strategys[0]);
    }

    public void Test()
    {
        Debug_ZoneView zone;
        foreach(Transform t in transform)
        {
            zone = t.GetComponent<Debug_ZoneView>();

            float chance = CalculatePresence(TestPlayer, zone.Zone);

            if (chance >= 1f)
            {
                zone.Populate(1f);
            }
            else
            {
                zone.Populate(chance);
            }
        }
    }

    public void ValueChange()
    {
        PlayerData.PlayerPosition pos = (PlayerData.PlayerPosition)dropDown.value;
        TestPlayer.Position = TestPlayer.AssignedPosition = pos;
    }

    private float CalculatePresence(PlayerData _player, MatchController.FieldZone _zone)
    {
        float chance = _player.GetChancePerZone(_zone);
        print(chance);
        if (chance < 1f && chance > 0f)
        {
            chance *= ((float)(_player.Speed + _player.Vision) / 200) * (_player.Fatigue / 100);
        }
        return chance;
    }
}
