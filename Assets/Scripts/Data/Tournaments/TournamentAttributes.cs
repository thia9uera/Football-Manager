[System.Serializable]
public struct TournamentAttributes
{
    public string Id;
    public string Name;
    public TournamentType Type;
    public string[] TeamIds;
    public MatchData[] Matches;
    public int TotalRounds;
	public int CurrentRound;
}

public enum TournamentType
{
    Championship,
    Cup,
}
