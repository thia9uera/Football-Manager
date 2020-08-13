#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompileData
{
	[MenuItem("Manager/Compile Game Data")]
	static void CompileAll()
	{
		InitialData initialData = (InitialData) Tools.GetFile<InitialData>("Data/Misc/InitialData.asset");
		Debug.Log(initialData);
		
		TeamData userTeam = (TeamData) Tools.GetFile<TeamData>("Data/Teams/_UserTeam.asset");
		
		PlayerData[] players = Tools.GetAtSubfolders<PlayerData>("Data/Players");
		TeamData[] teams = Tools.GetAtFolder<TeamData>("Data/Teams");
		TournamentData[] tournaments = Tools.GetAtFolder<TournamentData>("Data/Tournaments");
		
		initialData.UserTeam = userTeam;
		initialData.AllPlayers = players.ToList();
		initialData.AllTeams = teams.ToList();
		initialData.AllTournaments = tournaments.ToList();		
		
		AssetDatabase.SaveAssets();
	}
}
#endif
