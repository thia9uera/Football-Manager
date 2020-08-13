using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InitialData", menuName = "Data/Initial Data", order = 1)]
public class InitialData : ScriptableObject
{
	public TeamData UserTeam;
	public List<PlayerData> AllPlayers;
	public List<TeamData> AllTeams;
	public List<TournamentData> AllTournaments;
	
	[Space(10)]
	public List<TournamentData> InitialTournaments;
}
