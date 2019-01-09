[System.Serializable]
public struct TournamentAttributes
{
    public enum TournamentType
    {
        Championship,
        Cup,
    }

    public string Id;
    public string Name;
    public TournamentType Type;
    public int StarsRequired;
    public string[] TeamIds;
    public MatchData[] Matches;
}
