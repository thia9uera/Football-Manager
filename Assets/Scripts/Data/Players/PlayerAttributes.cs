using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlayerAttributes
{
    public enum PlayerPosition
    {
        Goalkeeper,
        Defender,
        Midfielder,
        Forward,
    }

    public enum PlayerStrategy
    {
        Neutral,
        Defensive,
        Ofensive,
        Left,
        Right,
        LeftDefensive,
        RightDefensive,
        LeftOffensive,
        RightOffensive,
    }

    public string Id;

    [Header("Personal")]
    public string FirstName;
    public string LastName;


    [Space(10)]
    public Field.Zone Zone;
    public PlayerPosition Position;
    public PlayerStrategy Strategy;
    //public PerkData Perk;


    [Space(10)]
    [Header("Technical Attributes")]
    [Space(5)]

    [Range(0, 100)]
    public int Goalkeeping;

    [Space(10)]
    [Range(0, 100)]
    public int Passing;

    [Range(0, 100)]
    public int Dribbling;

    [Range(0, 100)]
    public int Crossing;

    [Space(10)]
    [Range(0, 100)]
    public int Tackling;

    [Range(0, 100)]
    public int Blocking;

    [Space(10)]
    [Range(0, 100)]
    public int Shooting;

    [Range(0, 100)]
    public int Heading;

    [Range(0, 100)]
    public int Freekick;

    [Range(0, 100)]
    public int Penalty;


    [Space(10)]
    [Header("Physical Attributes")]
    [Space(5)]

    [Range(0, 100)]
    public int Speed;

    [Range(0, 100)]
    public int Strength;

    [Range(0, 100)]
    public int Agility;

    [Range(0, 100)]
    public int Stamina;


    [Space(10)]
    [Header("Mental Attributes")]
    [Space(5)]

    [Range(0, 100)]
    public int Teamwork;

    [Range(0, 100)]
    public int Vision;

    [Range(0, 100)]
    public int Stability;

    public PlayerStatistics LifeTimeStats;

    [SerializeField]
    public List<PlayerStatistics> TournamentStatistics;
}

//[System.Serializable]
//public class PlayerTournamentStats : SerializableDictionaryBase<string, PlayerStatistics> { }
