using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debug_FieldView : MonoBehaviour
{
    public PlayerData TestPlayer;

    public Dropdown dropDown;

    public void Start()
    {
        TestPlayer.Position = PlayerData.PlayerPosition.GK;
    }

    public void Test()
    {
        Debug_ZoneView zone;
        foreach(Transform t in transform)
        {
            zone = t.GetComponent<Debug_ZoneView>();

            float chance = CalculatePresence(TestPlayer, zone.Zone);

            if (chance == 1f) zone.Populate(0);
            else zone.Populate(chance);
 
        }
    }

    public void ValueChange()
    {
        PlayerData.PlayerPosition pos = (PlayerData.PlayerPosition)dropDown.value;
        TestPlayer.Position = pos;
    }

    private float CalculatePresence(PlayerData _player, MatchController.FieldZone _zone)
    {
        float chance = 0f;
        float playerTacticsBonus = 0f;
        float teamTacticsBonus = 1f;

        chance = _player.GetChancePerZone(_zone);
        if(chance < 1f) chance = _player.GetChancePerZone(_zone) * (playerTacticsBonus + teamTacticsBonus) * ((((float)_player.Speed + (float)_player.Vision) / 200) * (_player.Fatigue / 100));

        return chance;
    }
}
