using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Player", menuName = "Player Data", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Personal")]
    public string FirstName;
    public string LastName;

    
    [Space(10)]
    [HideInInspector]
    public MatchController.FieldZone Zone;
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
    private float fatigue = 100f;
    [HideInInspector]
    public float Fatigue
    {
        get
        {
            return fatigue;
        }
        set
        {
            fatigue = value;
            if (fatigue <= 0) fatigue = 0.01f;
        }
    }

    //[HideInInspector]
   // public MatchController.FieldZone Zone;

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
        Prob_Tackling,
        Prob_LongPass;

    public enum PlayerPosition
    {
        Goalkeeper,
        Defender,
        Midfielder,
        Forward,
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
        Sprint,
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

    public void ApplyBonus()
    {
        Player_Strategy _playerStrategy = MainController.Instance.PlayerStrategyData.player_Strategys[(int)Strategy];

        Prob_DefPosition = _playerStrategy.DefPosChance;
        Prob_OffPosition = _playerStrategy.OffPosChance;
        Prob_LeftPos = _playerStrategy.LeftPosChance;
        Prob_RightPos = _playerStrategy.RightPosChance;
        Prob_Pass = _playerStrategy.PassingChance;
        Prob_LongPass = _playerStrategy.LongPassChance;
        Prob_Shoot = _playerStrategy.ShootingChance;
        Prob_Crossing = _playerStrategy.CrossingChance;
        Prob_Dribble = _playerStrategy.DribblingChance;
        Prob_OffsideLine = _playerStrategy.OffsideTrickChance;
        Prob_Marking = _playerStrategy.MarkingChance;
        Prob_Tackling = _playerStrategy.TacklingChance;

    }

    public int GetOverall()
    {
        int total = 17;

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

        if(Position == PlayerPosition.Goalkeeper)
        {
            total += Goalkeeping;
            total += Agility;
            total = total / 19;
        }
        else if (Position == PlayerPosition.Defender)
        {
            total += Tackling;
            total += Blocking;
            total = total / 19;
        }
        else if(Position == PlayerPosition.Midfielder)
        {
            total += Dribbling;
            total += Passing;
            total += Crossing;
            total = total / 20;
        }
        else if (Position == PlayerPosition.Forward)
        {
            total += Dribbling;
            total += Passing;
            total += Shooting;
            total += Heading;
            total = total / 21;
        }
        else
        {
            total = total / 17;
        }

        return total;
    }

    public float GetChancePerZone(MatchController.FieldZone _zone, bool _isTeamStrategyApplicable = false, Team_Strategy _teamStrategy = null)
    {
        float pct = 0f;

        PosChancePerZone chancePerZone = MainController.Instance.PosChancePerZone.posChancePerZones[(int)Zone];

        switch(_zone)
        {
            case MatchController.FieldZone.OwnGoal: pct = chancePerZone.OwnGoal; break;
            case MatchController.FieldZone.BLD: pct = chancePerZone.BLD; break;
            case MatchController.FieldZone.BRD: pct = chancePerZone.BRD; break;
            case MatchController.FieldZone.LD: pct = chancePerZone.LD; break;
            case MatchController.FieldZone.LCD: pct = chancePerZone.LCD; break;
            case MatchController.FieldZone.RCD: pct = chancePerZone.RCD; break;
            case MatchController.FieldZone.RD: pct = chancePerZone.RD; break;
            case MatchController.FieldZone.LDM: pct = chancePerZone.LDM; break;
            case MatchController.FieldZone.LCDM: pct = chancePerZone.LCDM; break;
            case MatchController.FieldZone.RCDM: pct = chancePerZone.RCDM; break;
            case MatchController.FieldZone.RDM: pct = chancePerZone.RDM; break;
            case MatchController.FieldZone.LM: pct = chancePerZone.LM; break;
            case MatchController.FieldZone.LCM: pct = chancePerZone.LCM; break;
            case MatchController.FieldZone.RCM: pct = chancePerZone.RCM; break;
            case MatchController.FieldZone.RM: pct = chancePerZone.RM; break;
            case MatchController.FieldZone.LAM: pct = chancePerZone.LAM; break;
            case MatchController.FieldZone.LCAM: pct = chancePerZone.LCAM; break;
            case MatchController.FieldZone.RCAM: pct = chancePerZone.RCAM; break;
            case MatchController.FieldZone.RAM: pct = chancePerZone.RAM; break;
            case MatchController.FieldZone.LF: pct = chancePerZone.LF; break;
            case MatchController.FieldZone.LCF: pct = chancePerZone.LCF; break;
            case MatchController.FieldZone.RCF: pct = chancePerZone.RCF; break;
            case MatchController.FieldZone.RF: pct = chancePerZone.RF; break;
            case MatchController.FieldZone.ALF: pct = chancePerZone.ALF; break;
            case MatchController.FieldZone.ARF: pct = chancePerZone.ARF; break;
            case MatchController.FieldZone.Box: pct = chancePerZone.Box; break;
        }

        AltPosition altPos = GetAltPosition(_zone);

        float teamDefPos = 1f;
        float teamOffPos = 1f;
        float teamLeftPos = 1f;
        float teamRightPos = 1f;
        if(_isTeamStrategyApplicable)
        {
            teamDefPos = _teamStrategy.DefPosChance;
            teamOffPos = _teamStrategy.OffPosChance;
            teamLeftPos = _teamStrategy.LeftPosChance;
            teamRightPos = _teamStrategy.RighPosChance;
        }

        if (altPos == AltPosition.Defensive) pct *= Prob_DefPosition * teamDefPos;
        else if (altPos == AltPosition.Offensive) pct *= Prob_OffPosition * teamOffPos;
        else if (altPos == AltPosition.Left) pct *= Prob_LeftPos * teamLeftPos;
        else if (altPos == AltPosition.Right) pct *= Prob_RightPos * teamRightPos;
        else if (altPos == AltPosition.LeftDefensive) pct *= ((Prob_LeftPos + Prob_DefPosition)/2) * ((teamLeftPos + teamDefPos)/2);
        else if (altPos == AltPosition.RightDefensive) pct *= ((Prob_RightPos + Prob_DefPosition)/2) * ((teamRightPos + teamDefPos)/2);
        else if (altPos == AltPosition.LeftOffensive) pct *= ((Prob_LeftPos + Prob_OffPosition)/2) * ((teamLeftPos + teamOffPos)/2);

        return pct;
    }

    private AltPosition GetAltPosition(MatchController.FieldZone _zone)
    {
        AltPosition pos = AltPosition.None;

        switch(_zone)
        {
            case MatchController.FieldZone.OwnGoal:
                if (Zone == MatchController.FieldZone.LD) pos = AltPosition.RightDefensive;
                else if (Zone == MatchController.FieldZone.LCD) pos = AltPosition.Defensive;
                else if (Zone == MatchController.FieldZone.RCD) pos = AltPosition.Defensive;
                else if (Zone == MatchController.FieldZone.RD) pos = AltPosition.LeftDefensive;
                else if (Zone == MatchController.FieldZone.BLD) pos = AltPosition.Right;
                else if (Zone == MatchController.FieldZone.BRD) pos = AltPosition.Left;
                break;

            case MatchController.FieldZone.LD:
                if (Zone == MatchController.FieldZone.LCD) pos = AltPosition.Left;
                else if (Zone == MatchController.FieldZone.LDM) pos = AltPosition.Defensive;
                else if (Zone == MatchController.FieldZone.LCDM) pos = AltPosition.LeftDefensive;
                else if (Zone == MatchController.FieldZone.BLD) pos = AltPosition.Offensive;
                break;

            case MatchController.FieldZone.RCD:
            case MatchController.FieldZone.LCD:
                if (Zone == MatchController.FieldZone.LD) pos = AltPosition.Right;
                else if (Zone == MatchController.FieldZone.RCD) pos = AltPosition.Left;
                else if (Zone == MatchController.FieldZone.LDM) pos = AltPosition.RightDefensive;
                else if (Zone == MatchController.FieldZone.LCDM) pos = AltPosition.Defensive;
                else if (Zone == MatchController.FieldZone.RCDM) pos = AltPosition.LeftDefensive;
                break;

            case MatchController.FieldZone.RD:
                if (Zone == MatchController.FieldZone.RCD) pos = AltPosition.Right;
                else if (Zone == MatchController.FieldZone.RCDM) pos = AltPosition.RightDefensive;
                else if (Zone == MatchController.FieldZone.RDM) pos = AltPosition.Defensive;
                break;

            case MatchController.FieldZone.LDM:
                if (Zone == MatchController.FieldZone.LD) pos = AltPosition.Offensive;
                else if (Zone == MatchController.FieldZone.LCD) pos = AltPosition.LeftOffensive;
                else if (Zone == MatchController.FieldZone.LCDM) pos = AltPosition.Left;
                else if (Zone == MatchController.FieldZone.LCM) pos = AltPosition.LeftDefensive;
                else if (Zone == MatchController.FieldZone.LM) pos = AltPosition.Defensive;
                break;

            case MatchController.FieldZone.RCDM:
            case MatchController.FieldZone.LCDM:
                if (Zone == MatchController.FieldZone.LD) pos = AltPosition.RightOffensive;
                else if (Zone == MatchController.FieldZone.LCD) pos = AltPosition.Offensive;
                else if (Zone == MatchController.FieldZone.RD) pos = AltPosition.LeftOffensive;
                else if (Zone == MatchController.FieldZone.LDM) pos = AltPosition.Right;
                else if (Zone == MatchController.FieldZone.RDM) pos = AltPosition.Left;
                else if (Zone == MatchController.FieldZone.LM) pos = AltPosition.RightDefensive;
                else if (Zone == MatchController.FieldZone.LCM) pos = AltPosition.Defensive;
                else if (Zone == MatchController.FieldZone.RM) pos = AltPosition.LeftDefensive;
                break;

            case MatchController.FieldZone.RDM:
                if (Zone == MatchController.FieldZone.RCD) pos = AltPosition.RightOffensive;
                else if (Zone == MatchController.FieldZone.RD) pos = AltPosition.Offensive;
                else if (Zone == MatchController.FieldZone.LCDM) pos = AltPosition.Right;
                else if (Zone == MatchController.FieldZone.RCM) pos = AltPosition.RightDefensive;
                else if (Zone == MatchController.FieldZone.RM) pos = AltPosition.Defensive;
                break;

            case MatchController.FieldZone.LM:
                if (Zone == MatchController.FieldZone.LDM) pos = AltPosition.Offensive;
                else if (Zone == MatchController.FieldZone.LCDM) pos = AltPosition.LeftOffensive;
                else if (Zone == MatchController.FieldZone.LCM) pos = AltPosition.Left;
                else if (Zone == MatchController.FieldZone.RAM) pos = AltPosition.Defensive;
                else if (Zone == MatchController.FieldZone.RCAM) pos = AltPosition.LeftDefensive;
                break;

            case MatchController.FieldZone.RCM:
            case MatchController.FieldZone.LCM:
                if (Zone == MatchController.FieldZone.LDM) pos = AltPosition.RightOffensive;
                else if (Zone == MatchController.FieldZone.LCDM) pos = AltPosition.Offensive;
                else if (Zone == MatchController.FieldZone.RDM) pos = AltPosition.LeftOffensive;
                else if (Zone == MatchController.FieldZone.LM) pos = AltPosition.Right;
                else if (Zone == MatchController.FieldZone.RM) pos = AltPosition.Left;
                else if (Zone == MatchController.FieldZone.LAM) pos = AltPosition.RightDefensive;
                else if (Zone == MatchController.FieldZone.LCAM) pos = AltPosition.Defensive;
                else if (Zone == MatchController.FieldZone.RAM) pos = AltPosition.LeftDefensive;
                break;

            case MatchController.FieldZone.RM:
                if (Zone == MatchController.FieldZone.RDM) pos = AltPosition.Offensive;
                else if (Zone == MatchController.FieldZone.RCDM) pos = AltPosition.RightOffensive;
                else if (Zone == MatchController.FieldZone.RCM) pos = AltPosition.Right;
                else if (Zone == MatchController.FieldZone.RAM) pos = AltPosition.Defensive;
                else if (Zone == MatchController.FieldZone.RCAM) pos = AltPosition.RightDefensive;
                break;

            case MatchController.FieldZone.LAM:
                if (Zone == MatchController.FieldZone.LM) pos = AltPosition.Offensive;
                else if (Zone == MatchController.FieldZone.LCM) pos = AltPosition.LeftOffensive;
                else if (Zone == MatchController.FieldZone.LCAM) pos = AltPosition.Left;
                else if (Zone == MatchController.FieldZone.LF) pos = AltPosition.Defensive;
                else if (Zone == MatchController.FieldZone.LCF) pos = AltPosition.LeftDefensive;
                break;

            case MatchController.FieldZone.RCAM:
            case MatchController.FieldZone.LCAM:
                if (Zone == MatchController.FieldZone.LM) pos = AltPosition.RightOffensive;
                else if (Zone == MatchController.FieldZone.RCM) pos = AltPosition.Offensive;
                else if (Zone == MatchController.FieldZone.RM) pos = AltPosition.LeftOffensive;
                else if (Zone == MatchController.FieldZone.LAM) pos = AltPosition.Right;
                else if (Zone == MatchController.FieldZone.RAM) pos = AltPosition.Left;
                else if (Zone == MatchController.FieldZone.LF) pos = AltPosition.RightDefensive;
                else if (Zone == MatchController.FieldZone.RCF) pos = AltPosition.Defensive;
                else if (Zone == MatchController.FieldZone.RF) pos = AltPosition.LeftDefensive;
                break;

            case MatchController.FieldZone.RAM:
                if (Zone == MatchController.FieldZone.RM) pos = AltPosition.Offensive;
                else if (Zone == MatchController.FieldZone.RCM) pos = AltPosition.RightOffensive;
                else if (Zone == MatchController.FieldZone.RCAM) pos = AltPosition.Right;
                else if (Zone == MatchController.FieldZone.RF) pos = AltPosition.Defensive;
                else if (Zone == MatchController.FieldZone.RCF) pos = AltPosition.RightDefensive;
                break;

            case MatchController.FieldZone.LF:
                if (Zone == MatchController.FieldZone.LAM) pos = AltPosition.Offensive;
                else if (Zone == MatchController.FieldZone.LCAM) pos = AltPosition.LeftOffensive;
                else if (Zone == MatchController.FieldZone.LCF) pos = AltPosition.Left;
                else if (Zone == MatchController.FieldZone.ALF) pos = AltPosition.Defensive;
                break;

            case MatchController.FieldZone.LCF:
            case MatchController.FieldZone.RCF:
                if (Zone == MatchController.FieldZone.LAM) pos = AltPosition.RightOffensive;
                else if (Zone == MatchController.FieldZone.RCAM) pos = AltPosition.Offensive;
                else if (Zone == MatchController.FieldZone.RAM) pos = AltPosition.LeftOffensive;
                else if (Zone == MatchController.FieldZone.LF) pos = AltPosition.Right;
                else if (Zone == MatchController.FieldZone.RF) pos = AltPosition.Left;
                break;

            case MatchController.FieldZone.RF:
                if (Zone == MatchController.FieldZone.RAM) pos = AltPosition.Offensive;
                else if (Zone == MatchController.FieldZone.RCAM) pos = AltPosition.RightOffensive;
                else if (Zone == MatchController.FieldZone.RCF) pos = AltPosition.Right;
                else if (Zone == MatchController.FieldZone.ARF) pos = AltPosition.Defensive;
                break;

            case MatchController.FieldZone.Box:
                if (Zone == MatchController.FieldZone.LF) pos = AltPosition.RightOffensive;
                else if (Zone == MatchController.FieldZone.RCF) pos = AltPosition.Offensive;
                else if (Zone == MatchController.FieldZone.RF) pos = AltPosition.LeftOffensive;
                else if (Zone == MatchController.FieldZone.ALF) pos = AltPosition.Right;
                else if (Zone == MatchController.FieldZone.ARF) pos = AltPosition.Left;
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

    public string GetFullName()
    {
        return FirstName + " " + LastName;
    }

    public bool IsWronglyAssigned()
    {
        bool value = false;
        int zone = (int)Zone;

        switch (Position)
        {
            case PlayerPosition.Goalkeeper:
                if (zone != 0) value = true;
                break;

            case PlayerPosition.Defender:
                if (zone < 1 || zone > 6) value = true;
                break;

            case PlayerPosition.Midfielder:
                if (zone < 7 || zone > 18) value = true;
                break;

            case PlayerPosition.Forward:
                if (zone < 19) value = true;
                break;
        }
        return value;
    }
}