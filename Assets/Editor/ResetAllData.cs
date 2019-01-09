#if UNITY_EDITOR
using UnityEditor;

public class ResetAllData
{
    [MenuItem("Manager/Reset All Data")]
    static void ResetAll()
    {
        PlayerData[] players = Tools.GetAtSubfolders<PlayerData>("Data/Players");
        TeamData[] teams = Tools.GetAtFolder<TeamData>("Data/Teams");
        TournamentData[] tournaments = Tools.GetAtFolder<TournamentData>("Data/Tournaments");

        foreach (PlayerData player in players)
        {
            player.Reset();
            if(string.IsNullOrEmpty(player.Id)) player.Id = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(player);
        }
        foreach (TeamData team in teams)
        {
            team.Reset();
            if (string.IsNullOrEmpty(team.Id)) team.Id = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(team);
        }
        foreach (TournamentData tournament in tournaments)
        {
            tournament.ResetTournament();
            if (string.IsNullOrEmpty(tournament.Id)) tournament.Id = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(tournament);
        }

        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Data reseted",
               players.Length + "  Players, " + teams.Length + " Teams and " + tournaments.Length + " Tournaments reseted.",
               "AUXTERLIBRE!");
    }
}
#endif