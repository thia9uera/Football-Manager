using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Names", menuName = "Data/PlayerNames", order = 1)]
public class RandomNamesData : ScriptableObject
{
    public List<string> FirstNames;
	public List<string> LastNames;
	public List<string> TeamNames;
}

public enum RandomNameType
{
	FirstName,
	LastName,
	FullName,
	TeamName
}

