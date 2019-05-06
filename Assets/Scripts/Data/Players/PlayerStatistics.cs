[System.Serializable]
public class PlayerStatistics
{
    public PlayerStatistics(){}
    public PlayerStatistics(string _tournamentId) { TournamentID = _tournamentId; }

    public int Passes;
    public int Crosses;
    public int BoxCrosses;
    public int Shots;
    public int ShotsOnGoal;
    public int ShotsMissed;
    public int Headers;
    public int Faults;
    public int Tackles;
    public int Dribbles;
    public int Goals;
    public int Saves;
    public int PassesMissed;
    public int HeadersMissed;
    public int HeadersOnGoal;
    public int DribblesMissed;
    public int CrossesMissed;
    public int Presence;
    public int Assists;
    public int MatchesPlayed;
    public float MatchRating = 6.0f;
    public float AverageRating;
    public string TournamentID;
}
