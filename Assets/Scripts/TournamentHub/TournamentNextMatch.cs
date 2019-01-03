using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentNextMatch : MonoBehaviour
{
    [SerializeField]
    TournamentCreationMatch match;

    public void Populate(TournamentData.MatchData _data)
    {
        match.Populate(_data);
    }
}
