using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentCreationMatch : MonoBehaviour
{
    [SerializeField]
    TournamentCreationMatchTeam homeTeam, awayTeam;

    public void Populate(MatchData _data)
    {
        homeTeam.Populate(_data.HomeTeam);
        awayTeam.Populate(_data.AwayTeam);
    }
}
