using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadEditSubstitutes : MonoBehaviour
{
    [SerializeField]
    private SquadEditPlayer playerTemplate;

    [SerializeField]
    private Transform scrollContent;

	private SquadScreen controller;

    public void Populate(PlayerData[] _players, SquadScreen _controller)
    {
        controller = _controller;
        foreach(PlayerData p in _players)
        {
            AddPlayer(p);
        }
    }

    public void AddPlayer(PlayerData _player)
    {
        SquadEditPlayer player = Instantiate(playerTemplate, scrollContent);
        player.PopulateSub(_player, controller, 0.5f);
    }

    public void Clear()
    {
        foreach(Transform t in scrollContent)
        {
            Destroy(t.gameObject);
        }
    }
}
