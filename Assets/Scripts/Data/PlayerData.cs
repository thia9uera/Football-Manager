using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Player", menuName = "Player Data", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Personal")]
    public string FirstName;
    public string LastName;

    public enum PlayerPosition
    {
        GK,
        LD,
        CD,
        RD,
        LDM,
        CDM,
        RDM,
        LM,
        CM,
        RM,
        LAM,
        CAM,
        RAM,
        LF,
        CF,
        RF,
    }

    public enum PlayerPerk
    {
        None,
        Striker,
        Selfish,
        HotHead,
        Header,
        Sharpshooter,
    }

    public enum PlayerTactics
    {
        None,
        Captain,
        Offensive,
        Defensive,
        Distributor,
        Faker,
        Star,
    }

    [Space(10)]
    public PlayerPosition Position;


    [Space(10)]
    [Header("Technical Attributes")]
    [Space(5)]

    [Range(0, 100)]
    public int Goalkeeping = 50;

    [Range(0, 100)]
    public int Passing = 50;

    [Range(0, 100)]
    public int Tackling = 50;

    [Range(0, 100)]
    public int Shooting = 50;


    [Space(10)]
    [Header("Physical Attributes")]
    [Space(5)]

    [Range(0, 100)]
    public int Speed = 50;

    [Range(0, 100)]
    public int Strength = 50;

    [Range(0, 100)]
    public int Agility = 50;

    [Range(0, 100)]
    public int Stamina = 50;


    [Space(10)]
    [Header("Mental Attributes")]
    [Space(5)]

    [Range(0, 100)]
    public int Teamwork = 50;

    [Range(0, 100)]
    public int Vision = 50;

    public PlayerPerk Perk;
    public PlayerTactics Tactics;
    
    //Attributes that change during gameplay
    [HideInInspector]
    public int Fatigue = 0;

    [HideInInspector]
    public int Morale = 50;


    [HideInInspector]
    public PlayerPosition AssignedPosition;
}