using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadMainView : MonoBehaviour
{
    [SerializeField]
    private SquadSlotView[] slotList;

    private SquadSelectionView controller;

    private void Awake()
    {
        controller = GetComponentInParent<SquadSelectionView>();
    }

    public void Populate(PlayerData[] _list)
    {
        for (int i = 0; i < 11; i  ++)
        {
            slotList[i].Populate(_list[i]);
        }
    }
    public void SwapPlayers(PlayerData _in, PlayerData _out)
    {
        SquadSlotView slot;
        for (int i = 0; i < slotList.Length; i++)
        {
            slot = slotList[i];
            if(slot.Player == _out)
            {
                controller.Team.Squad.SetValue(_in, i);
            }
        }
    }
}
