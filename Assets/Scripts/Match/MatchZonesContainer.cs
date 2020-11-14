using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchZonesContainer : MonoBehaviour
{
    public List<FieldZone> ZoneList;

    public void AddZone(FieldZone _zone)
    {
        if (ZoneList == null) ZoneList = new List<FieldZone>();
        ZoneList.Add(_zone);
    }
}
