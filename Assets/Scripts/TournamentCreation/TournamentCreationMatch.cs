using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentCreationMatch : MonoBehaviour
{
    [SerializeField]
    TournamentCreationMatchTeam homeTeam, awayTeam;

    public void Populate(TournamentData.MatchData _data)
    {
        homeTeam.Populate(_data.HomeTeam);
        if (_data.AwayTeam != null)
        {
            awayTeam.Populate(_data.AwayTeam);
        }
    }
}
