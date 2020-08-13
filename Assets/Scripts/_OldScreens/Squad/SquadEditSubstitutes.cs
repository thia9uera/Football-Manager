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

	public void Populate(List<PlayerData> _players, SquadScreen _controller)
    {
	    controller = _controller;
	    float delay = 0;
	    foreach(PlayerData player in _players)
        {
		    AddPlayer(player, delay);
		    delay += 0.1f;
        }
    }

	public void AddPlayer(PlayerData _player, float _delay = 0)
    {
        SquadEditPlayer player = Instantiate(playerTemplate, scrollContent);
	    player.PopulateSub(_player, controller, _delay);
    }

    public void Clear()
    {
        foreach(Transform t in scrollContent)
        {
            Destroy(t.gameObject);
        }
    }
    
	public void EnablePlayer(bool _value)
	{
		
	}
}
