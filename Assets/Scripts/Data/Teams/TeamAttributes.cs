﻿using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

[System.Serializable]
public class TeamAttributes
{
    public string Id;

    [Header("Details")]
    public string Name;
	[Tooltip("Max 3 characters")]
    public string Tag;

    [Space(10)]
    public Color PrimaryColor = Color.white;
    public Color SecondaryColor = Color.red;

    [Space(10)]
	public int Formation;
    public TeamStrategy Strategy;

    //[HideInInspector]
    public string[] SquadIds, SubstitutesIds;

    public TeamStatistics LifeTimeStats;
    public TeamTournamentStats TournamentStatistics;

    public bool IsUserControlled;

    [HideInInspector] public bool IsPlaceholder;
}

[System.Serializable]
public class TeamTournamentStats : SerializableDictionaryBase<string, TeamStatistics> { }
