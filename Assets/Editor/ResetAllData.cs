#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

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
            EditorUtility.SetDirty(player);
        }
        foreach (TeamData team in teams)
        {
            team.Reset();
            EditorUtility.SetDirty(team);
        }
        foreach (TournamentData tournament in tournaments)
        {
            tournament.ResetTournament();
            EditorUtility.SetDirty(tournament);
        }

        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Data reseted",
               players.Length + "  Players, " + teams.Length + " Teams and " + tournaments.Length + " Tournaments reseted.",
               "AUXTERLIBRE!");
    }
}
#endif