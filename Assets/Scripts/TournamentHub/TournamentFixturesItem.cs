using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentFixturesItem : MonoBehaviour
{
    [SerializeField]
    TournamentFixturesItemTeam home, away;

    [SerializeField]
    CanvasGroup group;

    public void Populate(TournamentData.MatchData _data)
    {
        home.Populate(_data.HomeTeam, _data.isPlayed);
        away.Populate(_data.AwayTeam, _data.isPlayed);


        if (!_data.isPlayed && group != null) group.alpha = 0.3f;
    }
}
