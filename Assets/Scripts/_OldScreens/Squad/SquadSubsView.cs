using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadSubsView : MonoBehaviour
{
	[SerializeField] private SquadSubView subTemplate = null;

    private List<SquadSubView> list;

    private SquadSelectionView controller;

    private void Awake()
    {
        controller = GetComponentInParent<SquadSelectionView>();
    }

    public void Populate(PlayerData[] _list)
    {
        if(list != null) foreach (SquadSubView sub in list) Destroy(sub.gameObject);

        list = new List<SquadSubView>();
        for (int i = 0; i < _list.Length; i++)
        {
            SquadSubView slot = Instantiate(subTemplate, transform);
            slot.Populate(_list[i]);
            list.Add(slot);
        }
    }

    public void SwapPlayers(PlayerData _in, PlayerData _out)
    {
        SquadSubView item;
        for (int i = 0; i < list.Count; i++)
        {
            item = list[i];
            if (item.Player == _in)
            {
                controller.Team.Substitutes.SetValue(_out, i);
            }
        }
        
        Populate(controller.Team.Substitutes);
    }
}
