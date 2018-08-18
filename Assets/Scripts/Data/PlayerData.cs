using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Player", menuName = "Player Data", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Personal")]
    public string FirstName;
    public string LastName;

    
    [Space(10)]
    public PlayerPosition Position;

    public PerkData Perk;
    public PlayerStrategy Strategy;

    [Space(10)]
    [Header("Technical Attributes")]
    [Space(5)]

    [Range(0, 100)]
    public int Goalkeeping = 50;

    [Range(0, 100)]
    public int Passing = 50;

    [Range(0, 100)]
    public int Dribbling = 50;

    [Range(0, 100)]
    public int Crossing = 50;

    [Range(0, 100)]
    public int Tackling = 50;

    [Range(0, 100)]
    public int Blocking = 50;

    [Range(0, 100)]
    public int Shooting = 50;

    [Range(0, 100)]
    public int Heading = 50;

    [Range(0, 100)]
    public int Freekick = 50;

    [Range(0, 100)]
    public int Penalty = 50;


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

    [Range(0, 100)]
    public int Stability = 50;


    public float positionDebuf = 0.75f;
 
    //Attributes that change during gameplay
    [HideInInspector]
    public int Fatigue = 100;

    [HideInInspector]
    public int Morale = 50;

    public PlayerPosition AssignedPosition;

    [HideInInspector]
    public float Prob_DefPosition,
        Prob_OffPosition,
        Prob_ParPosChance,
        Prob_Pass,
        Prob_Shoot,
        Prob_Fault,
        Prob_Crossing,
        Prob_Dribble,
        Prob_Fall,
        Prob_OffsideLine,
        Prob_Marking = 0f,
        Prob_Tackling;

    [HideInInspector]
    public float Prob_OwnGoal, 
        Prob_LD,
        Prob_CD, 
        Prob_RD,
        Prob_LDM, 
        Prob_CDM, 
        Prob_RDM,
        Prob_LM, 
        Prob_CM, 
        Prob_RM, 
        Prob_LAM, 
        Prob_CAM, 
        Prob_RAM,
        Prob_LF,
        Prob_CF,
        Prob_RF,
        Prob_Box = 0f;

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

    public enum PlayerAttributes
    {
        Goalkeeping,
        Passing,
        Crossing,
        Tackling,
        Shooting,
        Heading,
        Freekick,
        Penalty,
        Speed,
        Strength,
        Agility,
        Stamina,
        Fatigue,
        Teamwork,
        Morale,
        Vision,
        Stability,
        DefPosChance,
        OffPosChance,
        PassingChance,
        ShootingChance,
        CrossingChance,
        DribblingChance,
        FaultChance,
        FallingChance,
        TeamMoraleBoost,
        ParPosChance,
        LongShotChance,
        Blocking,
        Dribbling,
    }

    public enum PlayerAction
    {
        None,
        Dribble,
        Pass,
        LongPass,
        Cross,
        ThroughPass,
        Shot,
        LongShot,
        Tackle,
        Fault,
        Block,
        Save,
        Header,
    }

    public enum PlayerStrategy
    {
        Neutral,
        Guard,
        Infiltrator,
        Omnipresent,
        Producer,
        Winger
    }

    public void ApplyBonus(Team_Strategy _teamStrategy)
    {
        Player_Strategy _playerStrategy = MainController.Instance.PlayerStrategyData.player_Strategys[(int)Strategy];

        Prob_DefPosition = _teamStrategy.DefPosChance * _playerStrategy.DefPosChance;
        Prob_OffPosition = _teamStrategy.OffPosChance * _playerStrategy.OffPosChance;
        Prob_ParPosChance = _playerStrategy.ParPosChance;
        Prob_Pass = _teamStrategy.PassingChance * _playerStrategy.PassingChance;
        Prob_Shoot = _teamStrategy.ShootingChance * _playerStrategy.ShootingChance;
        //Prob_Fault = 
        Prob_Crossing = _teamStrategy.CrossingChance * _playerStrategy.CrossingChance;
        Prob_Dribble = _teamStrategy.DribblingChance * _playerStrategy.DribblingChance;
        Prob_OffsideLine = _teamStrategy.OffsideTrickChance  * _playerStrategy.OffsideTrickChance;
        Prob_Marking = _teamStrategy.MarkingChance * _playerStrategy.MarkingChance;
        Prob_Tackling = _teamStrategy.TacklingChance * _playerStrategy.TacklingChance;

        Prob_OwnGoal = _teamStrategy.OwnGoal;
        Prob_LD = _teamStrategy.LD;
        Prob_CD = _teamStrategy.CD;
        Prob_RD = _teamStrategy.RD;
        Prob_LDM = _teamStrategy.LDM;
        Prob_CDM = _teamStrategy.CDM;
        Prob_RDM = _teamStrategy.RDM;
        Prob_LM= _teamStrategy.LM;
        Prob_CM = _teamStrategy.CM;
        Prob_RM = _teamStrategy.RM;
        Prob_LAM = _teamStrategy.LAM;
        Prob_CAM= _teamStrategy.CAM;
        Prob_RAM = _teamStrategy.RAM;
        Prob_LF = _teamStrategy.LF;
        Prob_CF = _teamStrategy.CF;
        Prob_RF = _teamStrategy.RD;
        Prob_Box = _teamStrategy.Box;
    }

    public int GetOverall()
    {
        int total = 0;

        total += Goalkeeping;
        total += Passing;
        total += Dribbling;
        total += Crossing;
        total += Tackling;
        total += Blocking;
        total += Shooting;
        total += Heading;
        total += Freekick;
        total += Penalty;
        total += Speed;
        total += Strength;
        total += Agility;
        total += Stamina;
        total += Teamwork;
        total += Vision;
        total += Stability;

        total = total / 17;

        return total;
    }

    public float GetChancePerZone(MatchController.FieldZone _zone)
    {
        float pct = 0f;
        float high_chance = 1.5f;
        float medium_chance = 1.25f;
        float low_chance = 1.1f;

        switch(AssignedPosition)
        {
            case PlayerPosition.GK:
                if (_zone == MatchController.FieldZone.OwnGoal) pct = 1f;
                break;

            case PlayerPosition.LD:
                switch(_zone)
                {
                    case MatchController.FieldZone.LD: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CD: pct = high_chance * Prob_ParPosChance * Prob_CD; break;
                    //Off
                    case MatchController.FieldZone.LDM: pct = medium_chance * Prob_OffPosition * Prob_LDM; break;
                    case MatchController.FieldZone.CDM: pct = low_chance * Prob_OffPosition * Prob_CDM; break;
                    //Def
                    case MatchController.FieldZone.OwnGoal: pct = medium_chance * Prob_DefPosition * Prob_OwnGoal; break;
                }
                break;

            case PlayerPosition.CD:
                switch (_zone)
                {
                    case MatchController.FieldZone.CD: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.LD: pct = medium_chance * Prob_ParPosChance * Prob_LD; break;
                    case MatchController.FieldZone.RD: pct = medium_chance * Prob_ParPosChance * Prob_RD; break;
                    //Off
                    case MatchController.FieldZone.LDM: pct = low_chance * Prob_OffPosition * Prob_LDM; break;
                    case MatchController.FieldZone.RDM: pct = low_chance * Prob_OffPosition; break;
                    case MatchController.FieldZone.CDM: pct = medium_chance * Prob_OffPosition; break;
                    //Def
                    case MatchController.FieldZone.OwnGoal: pct = high_chance * Prob_DefPosition; break;
                }
                break;

            case PlayerPosition.RD:
                switch (_zone)
                {
                    case MatchController.FieldZone.RD: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CD: pct = high_chance * Prob_ParPosChance * Prob_CD; break;
                    //Off
                    case MatchController.FieldZone.RDM: pct = medium_chance * Prob_OffPosition * Prob_RDM; break;
                    case MatchController.FieldZone.CDM: pct = low_chance * Prob_OffPosition * Prob_CDM; break;
                    //Def
                    case MatchController.FieldZone.OwnGoal: pct = medium_chance * Prob_DefPosition * Prob_OwnGoal; break;
                }
                break;

            case PlayerPosition.LDM:
                switch (_zone)
                {
                    case MatchController.FieldZone.LDM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CDM: pct = medium_chance * Prob_ParPosChance * Prob_CDM; break;
                    //Off
                    case MatchController.FieldZone.LM: pct = medium_chance * Prob_OffPosition * Prob_LM; break;
                    case MatchController.FieldZone.CM: pct = low_chance * Prob_OffPosition * Prob_CM; break;
                    //Def
                    case MatchController.FieldZone.LD: pct = medium_chance * Prob_DefPosition * Prob_LD; break;
                    case MatchController.FieldZone.CD: pct = low_chance * Prob_DefPosition * Prob_CD; break;
                }
                break;

            case PlayerPosition.CDM:
                switch (_zone)
                {
                    case MatchController.FieldZone.CDM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.LDM: pct = medium_chance * Prob_ParPosChance * Prob_LDM; break;
                    case MatchController.FieldZone.RDM: pct = medium_chance * Prob_ParPosChance * Prob_RDM; break;
                    //Off
                    case MatchController.FieldZone.CM: pct = medium_chance * Prob_OffPosition * Prob_CM; break;
                    case MatchController.FieldZone.LM: pct = low_chance * Prob_OffPosition * Prob_LM; break;
                    case MatchController.FieldZone.RM: pct = low_chance * Prob_OffPosition * Prob_RM; break;
                    //Def
                    case MatchController.FieldZone.LD: pct = low_chance * Prob_DefPosition * Prob_LD; break;
                    case MatchController.FieldZone.RD: pct = low_chance * Prob_DefPosition * Prob_RD; break;
                    case MatchController.FieldZone.CD: pct = medium_chance * Prob_DefPosition * Prob_CD; break;
                }
                break;

            case PlayerPosition.RDM:
                switch (_zone)
                {
                    case MatchController.FieldZone.RDM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CDM: pct = medium_chance * Prob_ParPosChance * Prob_CDM; break;
                    //Off
                    case MatchController.FieldZone.CM: pct = low_chance * Prob_OffPosition * Prob_CM; break;
                    case MatchController.FieldZone.RM: pct = medium_chance * Prob_OffPosition * Prob_RM; break;
                    //Def
                    case MatchController.FieldZone.RD: pct = medium_chance * Prob_DefPosition * Prob_RD; break;
                    case MatchController.FieldZone.CD: pct = low_chance * Prob_DefPosition * Prob_CD; break;
                }
                break;

            case PlayerPosition.LM:
                switch (_zone)
                {
                    case MatchController.FieldZone.LM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CM: pct = medium_chance * Prob_ParPosChance * Prob_CM; break;
                    //Off
                    case MatchController.FieldZone.CAM: pct = low_chance * Prob_OffPosition * Prob_CAM; break;
                    case MatchController.FieldZone.LAM: pct = medium_chance * Prob_OffPosition * Prob_LAM; break;
                    //Def
                    case MatchController.FieldZone.LDM: pct = medium_chance * Prob_DefPosition * Prob_LDM; break;
                    case MatchController.FieldZone.CDM: pct = low_chance * Prob_DefPosition * Prob_CDM; break;
                }
                break;

            case PlayerPosition.CM:
                switch (_zone)
                {
                    case MatchController.FieldZone.CM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.LM: pct = medium_chance * Prob_ParPosChance * Prob_LM; break;
                    case MatchController.FieldZone.RM: pct = medium_chance * Prob_ParPosChance * Prob_RM; break;
                    //Off
                    case MatchController.FieldZone.CAM: pct = medium_chance * Prob_OffPosition * Prob_CAM; break;
                    case MatchController.FieldZone.LAM: pct = low_chance * Prob_OffPosition * Prob_LAM; break;
                    case MatchController.FieldZone.RAM: pct = low_chance * Prob_OffPosition * Prob_RAM; break;
                    //Def
                    case MatchController.FieldZone.CDM: pct = medium_chance * Prob_DefPosition * Prob_CDM; break;
                    case MatchController.FieldZone.LDM: pct = low_chance * Prob_DefPosition * Prob_LDM; break;
                    case MatchController.FieldZone.RDM: pct = low_chance * Prob_DefPosition * Prob_RDM; break;
                }
                break;

            case PlayerPosition.RM:
                switch (_zone)
                {
                    case MatchController.FieldZone.RM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CM: pct = medium_chance * Prob_ParPosChance * Prob_CM; break;
                    //Off
                    case MatchController.FieldZone.RAM: pct = medium_chance * Prob_OffPosition * Prob_RAM; break;
                    case MatchController.FieldZone.CAM: pct = low_chance * Prob_OffPosition * Prob_CAM; break;
                    //Def
                    case MatchController.FieldZone.RDM: pct = medium_chance * Prob_DefPosition * Prob_RDM; break;
                    case MatchController.FieldZone.CDM: pct = low_chance * Prob_DefPosition * Prob_CDM; break;
                }
                break;

            case PlayerPosition.LAM:
                switch (_zone)
                {
                    case MatchController.FieldZone.LAM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CAM: pct = high_chance * Prob_ParPosChance * Prob_CAM; break;
                    //Off
                    case MatchController.FieldZone.LF: pct = high_chance * Prob_OffPosition * Prob_LF; break;
                    case MatchController.FieldZone.CF: pct = medium_chance * Prob_OffPosition * Prob_CF; break;
                    case MatchController.FieldZone.Box: pct = low_chance * Prob_OffPosition * Prob_Box; break;
                    //Def
                    case MatchController.FieldZone.LM: pct = medium_chance * Prob_DefPosition * Prob_LM; break;
                    case MatchController.FieldZone.CM: pct = low_chance * Prob_DefPosition * Prob_CM; break;
                    
                }
                break;

            case PlayerPosition.CAM:
                switch (_zone)
                {
                    case MatchController.FieldZone.CAM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.LAM: pct = high_chance * Prob_ParPosChance * Prob_LAM; break;
                    case MatchController.FieldZone.RAM: pct = high_chance * Prob_ParPosChance * Prob_RAM; break;
                    case MatchController.FieldZone.Box: pct = medium_chance * Prob_ParPosChance; break;
                    //Off
                    case MatchController.FieldZone.CF: pct = high_chance * Prob_OffPosition * Prob_CF; break;
                    case MatchController.FieldZone.RF: pct = low_chance * Prob_OffPosition * Prob_RF; break;
                    case MatchController.FieldZone.LF: pct = low_chance * Prob_OffPosition * Prob_LF; break;
                    //Def
                    case MatchController.FieldZone.LM: pct = low_chance * Prob_DefPosition * Prob_LM; break;
                    case MatchController.FieldZone.RM: pct = low_chance * Prob_DefPosition * Prob_RM; break;
                    case MatchController.FieldZone.CM: pct = medium_chance * Prob_DefPosition * Prob_CM; break;
                }
                break;

            case PlayerPosition.RAM:
                switch (_zone)
                {
                    case MatchController.FieldZone.RAM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CAM: pct = high_chance * Prob_ParPosChance * Prob_CAM; break;
                    //Off
                    case MatchController.FieldZone.RF: pct = high_chance * Prob_OffPosition * Prob_RF; break;
                    case MatchController.FieldZone.CF: pct = medium_chance * Prob_OffPosition * Prob_CF; break;
                    case MatchController.FieldZone.Box: pct = low_chance * Prob_OffPosition * Prob_Box; break;
                    //Def
                    case MatchController.FieldZone.RM: pct = medium_chance * Prob_DefPosition * Prob_RM; break;
                    case MatchController.FieldZone.CM: pct = low_chance * Prob_DefPosition * Prob_CM; break;
                    
                }
                break;

            case PlayerPosition.LF:
                switch (_zone)
                {
                    case MatchController.FieldZone.LF: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CF: pct = high_chance * Prob_ParPosChance * Prob_CF; break;
                    //Off
                    case MatchController.FieldZone.Box: pct = high_chance * Prob_OffPosition * Prob_Box; break;
                    //Def
                    case MatchController.FieldZone.LAM: pct = medium_chance * Prob_DefPosition * Prob_LAM; break;
                    case MatchController.FieldZone.LM: pct = low_chance * Prob_DefPosition * Prob_LM; break;
                    case MatchController.FieldZone.CAM: pct = low_chance * Prob_DefPosition * Prob_CAM; break;
                }
                break;

            case PlayerPosition.CF:
                switch (_zone)
                {
                    case MatchController.FieldZone.CF: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.LF: pct = low_chance * Prob_ParPosChance * Prob_LF; break;
                    case MatchController.FieldZone.RF: pct = low_chance * Prob_ParPosChance * Prob_RF; break;
                    //Off
                    case MatchController.FieldZone.Box: pct = high_chance * Prob_OffPosition * Prob_Box; break;
                    //Def
                    case MatchController.FieldZone.CAM: pct = medium_chance * Prob_DefPosition * Prob_CAM; break;
                    case MatchController.FieldZone.LAM: pct = low_chance * Prob_DefPosition * Prob_LAM; break;
                    case MatchController.FieldZone.RAM: pct = low_chance * Prob_DefPosition * Prob_RAM; break;
                }
                break;

            case PlayerPosition.RF:
                switch (_zone)
                {
                    case MatchController.FieldZone.RF: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CF: pct = high_chance * Prob_CF; break;
                    //Off
                    case MatchController.FieldZone.Box: pct = high_chance * Prob_Box; break;
                    //Def
                    case MatchController.FieldZone.RAM: pct = medium_chance * Prob_RAM; break;
                    case MatchController.FieldZone.CAM: pct = low_chance * Prob_CAM; break;
                    case MatchController.FieldZone.RM: pct = low_chance * Prob_RM; break;
                }
                break;
        }

        if (AssignedPosition != Position) pct *= positionDebuf;

        return pct;
    }
}