using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentCreationMatch : MonoBehaviour
{
    [SerializeField]
    TournamentCreationMatchTeam homeTeam, awayTeam;

	public void Populate(TeamData _home, TeamData _away)
    {
	    homeTeam.Populate(_home);
	    awayTeam.Populate(_away);
    }
}
