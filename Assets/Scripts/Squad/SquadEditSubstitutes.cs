using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadEditSubstitutes : MonoBehaviour
{
    [SerializeField]
    private SquadEditPlayer playerTemplate;

    [SerializeField]
    private Transform scrollContent;

    private SquadEdit controller;

    public void Populate(PlayerData[] _players, SquadEdit _controller)
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
        player.PopulateSub(_player, controller);
    }

    public void Clear()
    {
        foreach(Transform t in scrollContent)
        {
            Destroy(t.gameObject);
        }
    }
}
