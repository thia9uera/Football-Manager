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

            float chance = TestPlayer.GetChancePerZone(zone.Zone);

            if (chance == 1f) zone.Populate(0);
            else if (chance == 0.5f) zone.Populate(1);
            else if (chance == 0.25f) zone.Populate(2);
            else if (chance == 0.1f) zone.Populate(3);
            else if (chance == 0f) zone.Populate(4);
        }
    }

    public void ValueChange()
    {
        PlayerData.PlayerPosition pos = (PlayerData.PlayerPosition)dropDown.value;
        TestPlayer.Position = pos;
    }
}
