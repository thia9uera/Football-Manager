using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadMainView : MonoBehaviour
{
    [SerializeField]
    private SquadSlotView[] slotList;

    public void Populate(PlayerData[] _list)
    {
        for(int i = 0; i < 11; i  ++)
        {
            slotList[i].Populate(_list[i]);
        }
    }
}
