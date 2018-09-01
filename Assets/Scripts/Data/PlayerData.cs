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
    public PlayerStrategy Strategy;
    public PerkData Perk;


    [Space(10)]
    [Header("Technical Attributes")]
    [Space(5)]

    [Range(0, 100)]
    public int Goalkeeping = 50;

    [Space(10)]
    [Range(0, 100)]
    public int Passing = 50;

    [Range(0, 100)]
    public int Dribbling = 50;

    [Range(0, 100)]
    public int Crossing = 50;

    [Space(10)]
    [Range(0, 100)]
    public int Tackling = 50;

    [Range(0, 100)]
    public int Blocking = 50;

    [Space(10)]
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
    public PlayerPosition AssignedPosition;

    private enum AltPosition
    {
        None,
        Offensive,
        Defensive,
        Left,
        Right,
        LeftDefensive,
        RightDefensive,
        LeftOffensive,
        RightOffensive
    }

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
        Prob_Marking,
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
        Teamwork,
        Vision,
        Stability,
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

    public int TotalPasses,
        TotalCrosses,
        TotalShots,
        TotalHeaders,
        TotalFaults,
        TotalTackles,
        TotalDribbles,
        TotalGoals,
        TotalSaves,
        TotalPassesMissed,
        TotalShotsMissed,
        TotalHeadersMissed,
        TotalDribblesMissed,
        TotalCrossesMissed;

    public void ApplyBonus(Team_Strategy _teamStrategy)
    {
        Player_Strategy _playerStrategy = MainController.Instance.PlayerStrategyData.player_Strategys[(int)Strategy];

        Prob_DefPosition = _teamStrategy.DefPosChance * _playerStrategy.DefPosChance;
        Prob_OffPosition = _teamStrategy.OffPosChance * _playerStrategy.OffPosChance;
        Prob_LeftPos = _playerStrategy.LeftPosChance * _playerStrategy.LeftPosChance;
        Prob_RightPos = _playerStrategy.RightPosChance * _playerStrategy.RightPosChance;
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

        PosChancePerZone chancePerZone = MainController.Instance.PosChancePerZone.posChancePerZones[(int)AssignedPosition];

        switch(_zone)
        {
            case MatchController.FieldZone.OwnGoal: pct = chancePerZone.OwnGoal; break;
            case MatchController.FieldZone.LD: pct = chancePerZone.LD; break;
            case MatchController.FieldZone.CD: pct = chancePerZone.CD; break;
            case MatchController.FieldZone.RD: pct = chancePerZone.RD; break;
            case MatchController.FieldZone.LDM: pct = chancePerZone.LDM; break;
            case MatchController.FieldZone.CDM: pct = chancePerZone.CDM; break;
            case MatchController.FieldZone.RDM: pct = chancePerZone.RDM; break;
            case MatchController.FieldZone.LM: pct = chancePerZone.LM; break;
            case MatchController.FieldZone.CM: pct = chancePerZone.CM; break;
            case MatchController.FieldZone.RM: pct = chancePerZone.RM; break;
            case MatchController.FieldZone.LAM: pct = chancePerZone.LAM; break;
            case MatchController.FieldZone.CAM: pct = chancePerZone.CAM; break;
            case MatchController.FieldZone.RAM: pct = chancePerZone.RAM; break;
            case MatchController.FieldZone.LF: pct = chancePerZone.LF; break;
            case MatchController.FieldZone.CF: pct = chancePerZone.CF; break;
            case MatchController.FieldZone.RF: pct = chancePerZone.RF; break;
            case MatchController.FieldZone.Box: pct = chancePerZone.Box; break;
        }

        AltPosition altPos = GetAltPosition(_zone);

        if (altPos == AltPosition.Defensive) pct *= Prob_DefPosition;
        else if (altPos == AltPosition.Offensive) pct *= Prob_OffPosition;
        else if (altPos == AltPosition.Left) pct *= Prob_LeftPos;
        else if (altPos == AltPosition.Right) pct *= Prob_RightPos;
        else if (altPos == AltPosition.LeftDefensive) pct *= (Prob_LeftPos + Prob_DefPosition)/2;
        else if (altPos == AltPosition.RightDefensive) pct *= (Prob_RightPos + Prob_DefPosition) / 2;
        else if (altPos == AltPosition.LeftOffensive) pct *= (Prob_LeftPos + Prob_OffPosition) / 2;
        else if (altPos == AltPosition.RightOffensive) pct *= (Prob_RightPos + Prob_OffPosition) / 2;

        return pct;
    }

    private AltPosition GetAltPosition(MatchController.FieldZone _zone)
    {
        AltPosition pos = AltPosition.None;

        switch(_zone)
        {
            case MatchController.FieldZone.OwnGoal:
                if (AssignedPosition == PlayerPosition.LD) pos = AltPosition.RightDefensive;
                else if (AssignedPosition == PlayerPosition.CD) pos = AltPosition.Defensive;
                else if (AssignedPosition == PlayerPosition.RD) pos = AltPosition.LeftDefensive;
                break;

            case MatchController.FieldZone.LD:
                if (AssignedPosition == PlayerPosition.CD) pos = AltPosition.Left;
                else if (AssignedPosition == PlayerPosition.LDM) pos = AltPosition.Defensive;
                else if (AssignedPosition == PlayerPosition.CDM) pos = AltPosition.LeftDefensive;
                break;

            case MatchController.FieldZone.CD:
                if (AssignedPosition == PlayerPosition.LD) pos = AltPosition.Right;
                else if (AssignedPosition == PlayerPosition.RD) pos = AltPosition.Left;
                else if (AssignedPosition == PlayerPosition.LDM) pos = AltPosition.RightDefensive;
                else if (AssignedPosition == PlayerPosition.CDM) pos = AltPosition.Defensive;
                else if (AssignedPosition == PlayerPosition.RDM) pos = AltPosition.LeftDefensive;
                break;

            case MatchController.FieldZone.RD:
                if (AssignedPosition == PlayerPosition.CD) pos = AltPosition.Right;
                else if (AssignedPosition == PlayerPosition.CDM) pos = AltPosition.RightDefensive;
                else if (AssignedPosition == PlayerPosition.RDM) pos = AltPosition.Defensive;
                break;

            case MatchController.FieldZone.LDM:
                if (AssignedPosition == PlayerPosition.LD) pos = AltPosition.Offensive;
                else if (AssignedPosition == PlayerPosition.CD) pos = AltPosition.LeftOffensive;
                else if (AssignedPosition == PlayerPosition.CDM) pos = AltPosition.Left;
                else if (AssignedPosition == PlayerPosition.CM) pos = AltPosition.LeftDefensive;
                else if (AssignedPosition == PlayerPosition.LM) pos = AltPosition.Defensive;
                break;

            case MatchController.FieldZone.CDM:
                if (AssignedPosition == PlayerPosition.LD) pos = AltPosition.RightOffensive;
                else if (AssignedPosition == PlayerPosition.CD) pos = AltPosition.Offensive;
                else if (AssignedPosition == PlayerPosition.RD) pos = AltPosition.LeftOffensive;
                else if (AssignedPosition == PlayerPosition.LDM) pos = AltPosition.Right;
                else if (AssignedPosition == PlayerPosition.RDM) pos = AltPosition.Left;
                else if (AssignedPosition == PlayerPosition.LM) pos = AltPosition.RightDefensive;
                else if (AssignedPosition == PlayerPosition.CM) pos = AltPosition.Defensive;
                else if (AssignedPosition == PlayerPosition.RM) pos = AltPosition.LeftDefensive;
                break;

            case MatchController.FieldZone.RDM:
                if (AssignedPosition == PlayerPosition.CD) pos = AltPosition.RightOffensive;
                else if (AssignedPosition == PlayerPosition.RD) pos = AltPosition.Offensive;
                else if (AssignedPosition == PlayerPosition.CDM) pos = AltPosition.Right;
                else if (AssignedPosition == PlayerPosition.CM) pos = AltPosition.RightDefensive;
                else if (AssignedPosition == PlayerPosition.RM) pos = AltPosition.Defensive;
                break;

            case MatchController.FieldZone.LM:
                if (AssignedPosition == PlayerPosition.LDM) pos = AltPosition.Offensive;
                else if (AssignedPosition == PlayerPosition.CDM) pos = AltPosition.LeftOffensive;
                else if (AssignedPosition == PlayerPosition.CM) pos = AltPosition.Left;
                else if (AssignedPosition == PlayerPosition.LAM) pos = AltPosition.Defensive;
                else if (AssignedPosition == PlayerPosition.CAM) pos = AltPosition.LeftDefensive;
                break;

            case MatchController.FieldZone.CM:
                if (AssignedPosition == PlayerPosition.LDM) pos = AltPosition.RightOffensive;
                else if (AssignedPosition == PlayerPosition.CDM) pos = AltPosition.Offensive;
                else if (AssignedPosition == PlayerPosition.RDM) pos = AltPosition.LeftOffensive;
                else if (AssignedPosition == PlayerPosition.LM) pos = AltPosition.Right;
                else if (AssignedPosition == PlayerPosition.RM) pos = AltPosition.Left;
                else if (AssignedPosition == PlayerPosition.LAM) pos = AltPosition.RightDefensive;
                else if (AssignedPosition == PlayerPosition.CAM) pos = AltPosition.Defensive;
                else if (AssignedPosition == PlayerPosition.RAM) pos = AltPosition.LeftDefensive;
                break;

            case MatchController.FieldZone.RM:
                if (AssignedPosition == PlayerPosition.RDM) pos = AltPosition.Offensive;
                else if (AssignedPosition == PlayerPosition.CDM) pos = AltPosition.RightOffensive;
                else if (AssignedPosition == PlayerPosition.CM) pos = AltPosition.Right;
                else if (AssignedPosition == PlayerPosition.RAM) pos = AltPosition.Defensive;
                else if (AssignedPosition == PlayerPosition.CAM) pos = AltPosition.RightDefensive;
                break;

            case MatchController.FieldZone.LAM:
                if (AssignedPosition == PlayerPosition.LM) pos = AltPosition.Offensive;
                else if (AssignedPosition == PlayerPosition.CM) pos = AltPosition.LeftOffensive;
                else if (AssignedPosition == PlayerPosition.CAM) pos = AltPosition.Left;
                else if (AssignedPosition == PlayerPosition.LF) pos = AltPosition.Defensive;
                else if (AssignedPosition == PlayerPosition.CF) pos = AltPosition.LeftDefensive;
                break;

            case MatchController.FieldZone.CAM:
                if (AssignedPosition == PlayerPosition.LM) pos = AltPosition.RightOffensive;
                else if (AssignedPosition == PlayerPosition.CM) pos = AltPosition.Offensive;
                else if (AssignedPosition == PlayerPosition.RM) pos = AltPosition.LeftOffensive;
                else if (AssignedPosition == PlayerPosition.LAM) pos = AltPosition.Right;
                else if (AssignedPosition == PlayerPosition.RAM) pos = AltPosition.Left;
                else if (AssignedPosition == PlayerPosition.LF) pos = AltPosition.RightDefensive;
                else if (AssignedPosition == PlayerPosition.CF) pos = AltPosition.Defensive;
                else if (AssignedPosition == PlayerPosition.RF) pos = AltPosition.LeftDefensive;
                break;

            case MatchController.FieldZone.RAM:
                if (AssignedPosition == PlayerPosition.RM) pos = AltPosition.Offensive;
                else if (AssignedPosition == PlayerPosition.CM) pos = AltPosition.RightOffensive;
                else if (AssignedPosition == PlayerPosition.CAM) pos = AltPosition.Right;
                else if (AssignedPosition == PlayerPosition.RF) pos = AltPosition.Defensive;
                else if (AssignedPosition == PlayerPosition.CF) pos = AltPosition.RightDefensive;
                break;

            case MatchController.FieldZone.LF:
                if (AssignedPosition == PlayerPosition.LAM) pos = AltPosition.Offensive;
                else if (AssignedPosition == PlayerPosition.CAM) pos = AltPosition.LeftOffensive;
                else if (AssignedPosition == PlayerPosition.CF) pos = AltPosition.Left;
                break;

            case MatchController.FieldZone.CF:
                if (AssignedPosition == PlayerPosition.LAM) pos = AltPosition.RightOffensive;
                else if (AssignedPosition == PlayerPosition.CAM) pos = AltPosition.Offensive;
                else if (AssignedPosition == PlayerPosition.RAM) pos = AltPosition.LeftOffensive;
                else if (AssignedPosition == PlayerPosition.LF) pos = AltPosition.Right;
                else if (AssignedPosition == PlayerPosition.RF) pos = AltPosition.Left;
                break;

            case MatchController.FieldZone.RF:
                if (AssignedPosition == PlayerPosition.RAM) pos = AltPosition.Offensive;
                else if (AssignedPosition == PlayerPosition.CAM) pos = AltPosition.RightOffensive;
                else if (AssignedPosition == PlayerPosition.CF) pos = AltPosition.Right;
                break;

            case MatchController.FieldZone.Box:
                if (AssignedPosition == PlayerPosition.LF) pos = AltPosition.RightOffensive;
                else if (AssignedPosition == PlayerPosition.CF) pos = AltPosition.Offensive;
                else if (AssignedPosition == PlayerPosition.RF) pos = AltPosition.LeftOffensive;
                break;
        }

        return pos;
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

    public void ResetStatistics()
    {
        TotalGoals = 0;
        TotalPasses = 0;
        TotalCrosses = 0;
        TotalFaults = 0;
        TotalTackles = 0;
        TotalDribbles = 0;
        TotalHeaders = 0;
        TotalSaves = 0;
        TotalShots = 0;
        TotalCrossesMissed = 0;
        TotalDribblesMissed = 0;
        TotalHeadersMissed = 0;
        TotalPassesMissed = 0;
        TotalShotsMissed = 0;
    }
}