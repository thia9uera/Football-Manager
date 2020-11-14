using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentNextMatch : MonoBehaviour
{
	[SerializeField] private TournamentFixturesItem match = null;

    public void Populate(MatchData _data)
    {
	    match.Populate(_data);
    }
}
