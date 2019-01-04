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
        home.Populate(_data.HomeTeam);
        away.Populate(_data.AwayTeam);


        if (!_data.isPlayed) group.alpha = 0.3f;
    }
}
