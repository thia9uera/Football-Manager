#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManagerEditor
{
	[MenuItem("Data Manager/Reset All Data")]
	public static void ResetAll()
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
	
	[MenuItem("Data Manager/Compile Game Data")]
	public static void CompileAll()
	{
		InitialData initialData = (InitialData) Tools.GetFile<InitialData>("Data/Misc/InitialData.asset");
		
		TeamData userTeam = (TeamData) Tools.GetFile<TeamData>("Data/Teams/_UserTeam.asset");
		
		PlayerData[] players = Tools.GetAtSubfolders<PlayerData>("Data/Players");
		TeamData[] teams = Tools.GetAtFolder<TeamData>("Data/Teams");
		TournamentData[] tournaments = Tools.GetAtFolder<TournamentData>("Data/Tournaments");
		
		PosChanceData[] posChanceDataList = Tools.GetAtFolder<PosChanceData>("Data/PosChance");
		foreach(PosChanceData chanceData in posChanceDataList)
		{
			for(int i = 0; i < chanceData.posChancePerZones.Count; i++)
			{
				Zones zones = chanceData.posChancePerZones[i];
				zones.Position = ((Zone)i).ToString();
			}
		}
		
		initialData.UserTeam = userTeam;
		initialData.AllPlayers = players.ToList();
		initialData.AllTeams = teams.ToList();
		initialData.AllTournaments = tournaments.ToList();		
		
		AssetDatabase.SaveAssets();
	}
	
	[MenuItem("Data Manager/Set Team Colors")]
	public static void SetTeamColors()
	{
		TeamData[] teams = Tools.GetAtFolder<TeamData>("Data/Teams");
		GameColors gameColors = (GameColors) Tools.GetFile<GameColors>("Data/Misc/GameColors.asset");
		List<Color> availableColors;
		List<string> modifiedTeams = new List<string>();

		
		foreach (TeamData team in teams)
		{
			availableColors = new List<Color>(gameColors.TeamColors);
			if(!availableColors.Contains(team.PrimaryColor))
			{				
				team.PrimaryColor = availableColors[Random.Range(0, availableColors.Count-1)];			
				modifiedTeams.Add(team.Name);
				EditorUtility.SetDirty(team);
			}
			
			if(!availableColors.Contains(team.SecondaryColor) || team.SecondaryColor == team.PrimaryColor)
			{
				availableColors.Remove(team.PrimaryColor);
				team.SecondaryColor = availableColors[Random.Range(0, availableColors.Count-1)];
				if(!modifiedTeams.Contains(team.Name)) modifiedTeams.Add(team.Name);
				EditorUtility.SetDirty(team);
			}
		}
				
		if(modifiedTeams.Count > 0)
		{
			string output = "TEAMS MODIFIED: \n\n";
			foreach(string teamName in modifiedTeams)
			{
				output += teamName + "\n";
			}
			EditorUtility.DisplayDialog("Team Colors Updated", output, "AUXTERLIBRE!");
		}
		else
		{
			EditorUtility.DisplayDialog("Zero Team Colors Updated", "No teams needed to have their colors changed. Good job!", "AUXTERLIBRE!");
		}		
		
		AssetDatabase.SaveAssets();
	}
}
#endif
