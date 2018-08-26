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
 
    //Attributes that change during gameplay
    [HideInInspector]
    public int Fatigue = 100;

    [HideInInspector]
    public int Morale = 50;

    public PlayerPosition AssignedPosition;

    [HideInInspector]
    public float Prob_DefPosition,
        Prob_OffPosition,
        Prob_LeftPos,
        Prob_RightPos,
        Prob_Pass,
        Prob_Shoot,
        Prob_Fault,
        Prob_Crossing,
        Prob_Dribble,
        Prob_Fall,
        Prob_OffsideLine,
        Prob_Marking = 0f,
        Prob_Tackling;

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
        LefPosChance,
        RightPosChance,
        PassingChance,
        ShootingChance,
        CrossingChance,
        DribblingChance,
        FaultChance,
        FallingChance,
        TeamMoraleBoost,
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
        Defensive,
        Ofensive,
        Left,
        Right,
        LeftDefensive,
        RightDefensive,
        LeftOffensive,
        RightOffensive,
    }

    public void ApplyBonus(Team_Strategy _teamStrategy)
    {
        Player_Strategy _playerStrategy = MainController.Instance.PlayerStrategyData.player_Strategys[(int)Strategy];

        Prob_DefPosition = _teamStrategy.DefPosChance * _playerStrategy.DefPosChance;
        Prob_OffPosition = _teamStrategy.OffPosChance * _playerStrategy.OffPosChance;
        Prob_LeftPos = _playerStrategy.LeftPosChance;
        Prob_RightPos = _playerStrategy.RightPosChance;
        Prob_Pass = _teamStrategy.PassingChance * _playerStrategy.PassingChance;
        Prob_Shoot = _teamStrategy.ShootingChance * _playerStrategy.ShootingChance;
        Prob_Crossing = _teamStrategy.CrossingChance * _playerStrategy.CrossingChance;
        Prob_Dribble = _teamStrategy.DribblingChance * _playerStrategy.DribblingChance;
        Prob_OffsideLine = _teamStrategy.OffsideTrickChance  * _playerStrategy.OffsideTrickChance;
        Prob_Marking = _teamStrategy.MarkingChance * _playerStrategy.MarkingChance;
        Prob_Tackling = _teamStrategy.TacklingChance * _playerStrategy.TacklingChance;

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
        float high_chance = 0.25f;
        float medium_chance = 0.15f;
        float low_chance = 0.05f;

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
                    case MatchController.FieldZone.CD: pct = high_chance * Prob_RightPos; break;
                    //Off
                    case MatchController.FieldZone.LDM: pct = medium_chance * Prob_OffPosition; break;
                    case MatchController.FieldZone.CDM: pct = low_chance * ((Prob_OffPosition + Prob_RightPos)/2); break;
                    //Def
                    case MatchController.FieldZone.OwnGoal: pct = medium_chance * ((Prob_DefPosition + Prob_RightPos)/2); break;
                }
                break;

            case PlayerPosition.CD:
                switch (_zone)
                {
                    case MatchController.FieldZone.CD: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.LD: pct = medium_chance * Prob_LeftPos; break;
                    case MatchController.FieldZone.RD: pct = medium_chance * Prob_RightPos; break;
                    //Off
                    case MatchController.FieldZone.LDM: pct = low_chance * ((Prob_OffPosition + Prob_LeftPos) / 2); break;
                    case MatchController.FieldZone.RDM: pct = low_chance * ((Prob_OffPosition + Prob_RightPos) / 2); break;
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
                    case MatchController.FieldZone.CD: pct = high_chance * Prob_LeftPos; break;
                    //Off
                    case MatchController.FieldZone.RDM: pct = medium_chance * Prob_OffPosition; break;
                    case MatchController.FieldZone.CDM: pct = low_chance * ((Prob_OffPosition + Prob_LeftPos) / 2); break;
                    //Def
                    case MatchController.FieldZone.OwnGoal: pct = medium_chance * Prob_DefPosition; break;
                }
                break;

            case PlayerPosition.LDM:
                switch (_zone)
                {
                    case MatchController.FieldZone.LDM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CDM: pct = medium_chance * Prob_RightPos; break;
                    //Off
                    case MatchController.FieldZone.LM: pct = medium_chance * Prob_OffPosition; break;
                    case MatchController.FieldZone.CM: pct = low_chance * ((Prob_OffPosition + Prob_RightPos) / 2); break;
                    //Def
                    case MatchController.FieldZone.LD: pct = medium_chance * Prob_DefPosition; break;
                    case MatchController.FieldZone.CD: pct = low_chance * ((Prob_DefPosition + Prob_RightPos) / 2); break;
                }
                break;

            case PlayerPosition.CDM:
                switch (_zone)
                {
                    case MatchController.FieldZone.CDM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.LDM: pct = medium_chance * Prob_LeftPos; break;
                    case MatchController.FieldZone.RDM: pct = medium_chance * Prob_RightPos; break;
                    //Off
                    case MatchController.FieldZone.CM: pct = medium_chance * Prob_OffPosition; break;
                    case MatchController.FieldZone.LM: pct = low_chance * ((Prob_OffPosition + Prob_LeftPos) / 2); break;
                    case MatchController.FieldZone.RM: pct = low_chance * ((Prob_OffPosition + Prob_RightPos) / 2); break;
                    //Def
                    case MatchController.FieldZone.LD: pct = low_chance * ((Prob_DefPosition + Prob_LeftPos) / 2); break;
                    case MatchController.FieldZone.RD: pct = low_chance * ((Prob_OffPosition + Prob_RightPos) / 2); break;
                    case MatchController.FieldZone.CD: pct = medium_chance * Prob_DefPosition; break;
                }
                break;

            case PlayerPosition.RDM:
                switch (_zone)
                {
                    case MatchController.FieldZone.RDM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CDM: pct = medium_chance * Prob_LeftPos; break;
                    //Off
                    case MatchController.FieldZone.CM: pct = low_chance * ((Prob_OffPosition + Prob_LeftPos) / 2); break;
                    case MatchController.FieldZone.RM: pct = medium_chance * Prob_OffPosition; break;
                    //Def
                    case MatchController.FieldZone.RD: pct = medium_chance * Prob_DefPosition; break;
                    case MatchController.FieldZone.CD: pct = low_chance * ((Prob_DefPosition + Prob_LeftPos) / 2); break;
                }
                break;

            case PlayerPosition.LM:
                switch (_zone)
                {
                    case MatchController.FieldZone.LM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CM: pct = medium_chance * Prob_RightPos; break;
                    //Off
                    case MatchController.FieldZone.CAM: pct = low_chance * ((Prob_OffPosition + Prob_RightPos) / 2); break;
                    case MatchController.FieldZone.LAM: pct = medium_chance * Prob_OffPosition; break;
                    //Def
                    case MatchController.FieldZone.LDM: pct = medium_chance * Prob_DefPosition; break;
                    case MatchController.FieldZone.CDM: pct = low_chance * ((Prob_DefPosition + Prob_RightPos) / 2); break;
                }
                break;

            case PlayerPosition.CM:
                switch (_zone)
                {
                    case MatchController.FieldZone.CM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.LM: pct = medium_chance * Prob_LeftPos; break;
                    case MatchController.FieldZone.RM: pct = medium_chance * Prob_RightPos; break;
                    //Off
                    case MatchController.FieldZone.CAM: pct = medium_chance * Prob_OffPosition; break;
                    case MatchController.FieldZone.LAM: pct = low_chance * ((Prob_OffPosition + Prob_LeftPos) / 2); break;
                    case MatchController.FieldZone.RAM: pct = low_chance * ((Prob_OffPosition + Prob_RightPos) / 2); break;
                    //Def
                    case MatchController.FieldZone.CDM: pct = medium_chance * Prob_DefPosition; break;
                    case MatchController.FieldZone.LDM: pct = low_chance * ((Prob_DefPosition + Prob_LeftPos) / 2); break;
                    case MatchController.FieldZone.RDM: pct = low_chance * ((Prob_DefPosition + Prob_RightPos) / 2); break;
                }
                break;

            case PlayerPosition.RM:
                switch (_zone)
                {
                    case MatchController.FieldZone.RM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CM: pct = medium_chance * Prob_LeftPos; break;
                    //Off
                    case MatchController.FieldZone.RAM: pct = medium_chance * Prob_OffPosition; break;
                    case MatchController.FieldZone.CAM: pct = low_chance * ((Prob_OffPosition + Prob_LeftPos) / 2); break;
                    //Def
                    case MatchController.FieldZone.RDM: pct = medium_chance * Prob_DefPosition; break;
                    case MatchController.FieldZone.CDM: pct = low_chance * ((Prob_DefPosition + Prob_LeftPos) / 2); break;
                }
                break;

            case PlayerPosition.LAM:
                switch (_zone)
                {
                    case MatchController.FieldZone.LAM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CAM: pct = high_chance * Prob_RightPos; break;
                    //Off
                    case MatchController.FieldZone.LF: pct = high_chance * Prob_OffPosition; break;
                    case MatchController.FieldZone.CF: pct = medium_chance * ((Prob_OffPosition + Prob_RightPos) / 2); break;
                    case MatchController.FieldZone.Box: pct = low_chance * ((Prob_OffPosition + Prob_RightPos) / 2); break;
                    //Def
                    case MatchController.FieldZone.LM: pct = medium_chance * Prob_DefPosition; break;
                    case MatchController.FieldZone.CM: pct = low_chance * ((Prob_DefPosition + Prob_RightPos) / 2); break;
                    
                }
                break;

            case PlayerPosition.CAM:
                switch (_zone)
                {
                    case MatchController.FieldZone.CAM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.LAM: pct = high_chance * Prob_LeftPos; break;
                    case MatchController.FieldZone.RAM: pct = high_chance * Prob_RightPos; break;
                    //Off
                    case MatchController.FieldZone.CF: pct = high_chance * Prob_OffPosition; break;
                    case MatchController.FieldZone.RF: pct = low_chance * ((Prob_OffPosition + Prob_RightPos) / 2); break;
                    case MatchController.FieldZone.LF: pct = low_chance * ((Prob_OffPosition + Prob_LeftPos) / 2); break;
                    case MatchController.FieldZone.Box: pct = medium_chance * Prob_OffPosition; break;
                    //Def
                    case MatchController.FieldZone.LM: pct = low_chance * ((Prob_DefPosition + Prob_LeftPos) / 2); break;
                    case MatchController.FieldZone.RM: pct = low_chance * ((Prob_DefPosition + Prob_RightPos) / 2); break;
                    case MatchController.FieldZone.CM: pct = medium_chance * Prob_DefPosition; break;
                }
                break;

            case PlayerPosition.RAM:
                switch (_zone)
                {
                    case MatchController.FieldZone.RAM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CAM: pct = high_chance * Prob_LeftPos; break;
                    //Off
                    case MatchController.FieldZone.RF: pct = high_chance * Prob_OffPosition; break;
                    case MatchController.FieldZone.CF: pct = medium_chance * ((Prob_OffPosition + Prob_LeftPos) / 2); break;
                    case MatchController.FieldZone.Box: pct = low_chance * ((Prob_OffPosition + Prob_LeftPos) / 2); break;
                    //Def
                    case MatchController.FieldZone.RM: pct = medium_chance * Prob_DefPosition; break;
                    case MatchController.FieldZone.CM: pct = low_chance * ((Prob_DefPosition + Prob_LeftPos) / 2); break;
                    
                }
                break;

            case PlayerPosition.LF:
                switch (_zone)
                {
                    case MatchController.FieldZone.LF: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CF: pct = high_chance * Prob_RightPos; break;
                    //Off
                    case MatchController.FieldZone.Box: pct = high_chance * ((Prob_OffPosition + Prob_RightPos) / 2); break;
                    //Def
                    case MatchController.FieldZone.LAM: pct = medium_chance * Prob_DefPosition; break;
                    case MatchController.FieldZone.CAM: pct = low_chance * ((Prob_DefPosition + Prob_RightPos) / 2); break;
                }
                break;

            case PlayerPosition.CF:
                switch (_zone)
                {
                    case MatchController.FieldZone.CF: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.LF: pct = low_chance * Prob_LeftPos; break;
                    case MatchController.FieldZone.RF: pct = low_chance * Prob_RightPos; break;
                    //Off
                    case MatchController.FieldZone.Box: pct = high_chance * Prob_OffPosition; break;
                    //Def
                    case MatchController.FieldZone.CAM: pct = medium_chance * Prob_DefPosition; break;
                    case MatchController.FieldZone.LAM: pct = low_chance * ((Prob_DefPosition + Prob_LeftPos) / 2); break;
                    case MatchController.FieldZone.RAM: pct = low_chance * ((Prob_DefPosition + Prob_RightPos) / 2); break;
                }
                break;

            case PlayerPosition.RF:
                switch (_zone)
                {
                    case MatchController.FieldZone.RF: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CF: pct = high_chance * Prob_LeftPos; break;
                    //Off
                    case MatchController.FieldZone.Box: pct = high_chance * ((Prob_OffPosition + Prob_LeftPos) / 2); break;
                    //Def
                    case MatchController.FieldZone.RAM: pct = medium_chance * Prob_DefPosition; break;
                    case MatchController.FieldZone.CAM: pct = low_chance * ((Prob_DefPosition + Prob_LeftPos) / 2); break;
                }
                break;
        }

        return pct;
    }

    public int GetPlayerAttribute(PlayerAttributes _playerAttributes)
    {
        int value = 0;

        switch(_playerAttributes)
        {
            case PlayerAttributes.Agility: value = Agility; break;
            case PlayerAttributes.Blocking: value = Blocking; break;
            case PlayerAttributes.Crossing: value = Crossing; break;
            case PlayerAttributes.Dribbling: value = Dribbling; break;
            case PlayerAttributes.Freekick: value = Freekick; break;
            case PlayerAttributes.Goalkeeping: value = Goalkeeping; break;
            case PlayerAttributes.Heading: value = Heading; break;
            case PlayerAttributes.Passing: value = Passing; break;
            case PlayerAttributes.Penalty: value = Penalty; break;
            case PlayerAttributes.Shooting: value = Shooting; break;
            case PlayerAttributes.Speed: value = Speed; break;
            case PlayerAttributes.Stability: value = Stability; break;
            case PlayerAttributes.Stamina: value = Stamina; break;
            case PlayerAttributes.Strength: value = Strength; break;
            case PlayerAttributes.Tackling: value = Tackling; break;
            case PlayerAttributes.Teamwork: value = Teamwork; break;
            case PlayerAttributes.Vision: value = Vision; break;
        }

        return value;
    }
}