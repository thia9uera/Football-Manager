using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using RotaryHeart.Lib.SerializableDictionary;

[CreateAssetMenu(fileName = "Player", menuName = "Player Data", order = 1)]
[System.Serializable]
public class PlayerData : ScriptableObject
{
    public PlayerAttributes Attributes;

    public string Id { get { return Attributes.Id; } set { Attributes.Id = value; } }

    public string FirstName { get { return Attributes.FirstName; } set { Attributes.FirstName = value; } }
    public string LastName { get { return Attributes.LastName; } set { Attributes.LastName = value; } }

    public MatchController.FieldZone Zone { get { return Attributes.Zone; } set { Attributes.Zone = value; } }
    public PlayerAttributes.PlayerPosition Position { get { return Attributes.Position; } set { Attributes.Position = value; } }
    public PlayerAttributes. PlayerStrategy Strategy { get { return Attributes.Strategy; } set { Attributes.Strategy = value; } }

    public int Goalkeeping { get { return Attributes.Goalkeeping; } set { Attributes.Goalkeeping = value; } } 
    public int Passing { get { return Attributes.Passing; } set { Attributes.Passing = value; } }
    public int Dribbling { get { return Attributes.Dribbling; } set { Attributes.Dribbling = value; } }
    public int Crossing { get { return Attributes.Crossing; } set { Attributes.Crossing = value; } }
    public int Tackling { get { return Attributes.Tackling; } set { Attributes.Tackling = value; } }
    public int Blocking { get { return Attributes.Blocking; } set { Attributes.Blocking = value; } }
    public int Shooting { get { return Attributes.Shooting; } set { Attributes.Shooting = value; } }
    public int Heading { get { return Attributes.Heading; } set { Attributes.Heading = value; } }
    public int Freekick { get { return Attributes.Freekick; } set { Attributes.Freekick = value; } }
    public int Penalty { get { return Attributes.Penalty; } set { Attributes.Penalty = value; } }

    public int Speed { get { return Attributes.Speed; } set { Attributes.Speed = value; } }
    public int Strength { get { return Attributes.Strength; } set { Attributes.Strength = value; } }
    public int Agility { get { return Attributes.Agility; } set { Attributes.Agility = value; } }
    public int Stamina { get { return Attributes.Stamina; } set { Attributes.Stamina = value; } }

    public int Teamwork { get { return Attributes.Teamwork; } set { Attributes.Teamwork = value; } }
    public int Vision { get { return Attributes.Vision; } set { Attributes.Vision = value; } }
    public int Stability { get { return Attributes.Stability; } set { Attributes.Stability = value; } }

    float fatigue;
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

    public PlayerStatistics MatchStats;

    public TeamData Team;

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

    public enum AttributeType
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
        int total = 0;

        //total += Goalkeeping;
        total += Passing;
        total += Dribbling;
        total += Crossing;
        total += Tackling;
        total += Blocking;
        total += Shooting;
        total += Heading;
        //total += Freekick;
        //total += Penalty;
        total += Speed;
        total += Strength;
        total += Agility;
        total += Stamina;
        total += Teamwork;
        total += Vision;
        total += Stability;

        switch (Position)
        {
            case PlayerAttributes.PlayerPosition.Goalkeeper:
                total += Goalkeeping;
                total += Agility;
                total = total / 16;
                break;
            case PlayerAttributes.PlayerPosition.Defender:
                total += Tackling;
                total += Blocking;
                total = total / 16;
                break;
            case PlayerAttributes.PlayerPosition.Midfielder:
                total += Dribbling;
                total += Passing;
                total += Crossing;
                total = total / 17;
                break;
            case PlayerAttributes.PlayerPosition.Forward:
                total += Dribbling;
                total += Passing;
                total += Shooting;
                total += Heading;
                total = total / 18;
                break;
            default:
                total = total / 14;
                break;
        }

        return total;
    }

    public float GetChancePerZone(MatchController.FieldZone _zone, TeamAttributes.TeamStrategy _teamStrategy)
    {
        float pct = 0f;

        Zones chancePerZone = MainController.Instance.Match.TeamStrategies[(int)_teamStrategy].PosChance.posChancePerZones[(int)Zone];

        switch(_zone)
        {
            case MatchController.FieldZone.OwnGoal: pct = chancePerZone.OwnGoal; break;
            case MatchController.FieldZone.BLD: pct = chancePerZone.BLD; break;
            case MatchController.FieldZone.BRD: pct = chancePerZone.BRD; break;
            case MatchController.FieldZone.LD: pct = chancePerZone.LD; break;
            case MatchController.FieldZone.LCD: pct = chancePerZone.LCD; break;
            case MatchController.FieldZone.CD: pct = chancePerZone.CD; break;
            case MatchController.FieldZone.RCD: pct = chancePerZone.RCD; break;
            case MatchController.FieldZone.RD: pct = chancePerZone.RD; break;
            case MatchController.FieldZone.LDM: pct = chancePerZone.LDM; break;
            case MatchController.FieldZone.LCDM: pct = chancePerZone.LCDM; break;
            case MatchController.FieldZone.CDM: pct = chancePerZone.CDM; break;
            case MatchController.FieldZone.RCDM: pct = chancePerZone.RCDM; break;
            case MatchController.FieldZone.RDM: pct = chancePerZone.RDM; break;
            case MatchController.FieldZone.LM: pct = chancePerZone.LM; break;
            case MatchController.FieldZone.LCM: pct = chancePerZone.LCM; break;
            case MatchController.FieldZone.CM: pct = chancePerZone.CM; break;
            case MatchController.FieldZone.RCM: pct = chancePerZone.RCM; break;
            case MatchController.FieldZone.RM: pct = chancePerZone.RM; break;
            case MatchController.FieldZone.LAM: pct = chancePerZone.LAM; break;
            case MatchController.FieldZone.LCAM: pct = chancePerZone.LCAM; break;
            case MatchController.FieldZone.CAM: pct = chancePerZone.CAM; break;
            case MatchController.FieldZone.RCAM: pct = chancePerZone.RCAM; break;
            case MatchController.FieldZone.RAM: pct = chancePerZone.RAM; break;
            case MatchController.FieldZone.LF: pct = chancePerZone.LF; break;
            case MatchController.FieldZone.LCF: pct = chancePerZone.LCF; break;
            case MatchController.FieldZone.CF: pct = chancePerZone.CF; break;
            case MatchController.FieldZone.RCF: pct = chancePerZone.RCF; break;
            case MatchController.FieldZone.RF: pct = chancePerZone.RF; break;
            case MatchController.FieldZone.ALF: pct = chancePerZone.ALF; break;
            case MatchController.FieldZone.ARF: pct = chancePerZone.ARF; break;
            case MatchController.FieldZone.Box: pct = chancePerZone.Box; break;
        }
        return pct;
    }

    AltPosition GetAltPosition(MatchController.FieldZone _zone)
    {
        AltPosition pos = AltPosition.None;
        Vector2 posMatrix = MainController.Instance.Match.FieldMatrix[(int)Zone];
        int posX = (int)posMatrix.x;
        int posY = (int)posMatrix.y;

        Vector2 altPosMatrix = MainController.Instance.Match.FieldMatrix[(int)_zone];
        int altPosX = (int)altPosMatrix.x;
        int altPosY = (int)altPosMatrix.y;

        if(altPosY == posY)
        {
            if (altPosX < posX) pos = AltPosition.Left;
            else if (altPosX > posX) pos = AltPosition.Right;
        }
        else if (altPosY > posY)
        {
            if (altPosX < posX) pos = AltPosition.LeftOffensive;
            else if (altPosX > posX) pos = AltPosition.RightOffensive;
            else if (altPosX == posX) pos = AltPosition.Offensive;
        }
        else if (altPosY < posY)
        {
            if (altPosX < posX) pos = AltPosition.LeftDefensive;
            else if (altPosX > posX) pos = AltPosition.RightDefensive;
            else if (altPosX == posX) pos = AltPosition.Defensive;
        }

        return pos;
    }

    public int GetPlayerAttribute(AttributeType _playerAttributes)
    {
        int value = 0;

        switch(_playerAttributes)
        {
            case AttributeType.Agility: value = Agility; break;
            case AttributeType.Blocking: value = Blocking; break;
            case AttributeType.Crossing: value = Crossing; break;
            case AttributeType.Dribbling: value = Dribbling; break;
            case AttributeType.Freekick: value = Freekick; break;
            case AttributeType.Goalkeeping: value = Goalkeeping; break;
            case AttributeType.Heading: value = Heading; break;
            case AttributeType.Passing: value = Passing; break;
            case AttributeType.Penalty: value = Penalty; break;
            case AttributeType.Shooting: value = Shooting; break;
            case AttributeType.Speed: value = Speed; break;
            case AttributeType.Stability: value = Stability; break;
            case AttributeType.Stamina: value = Stamina; break;
            case AttributeType.Strength: value = Strength; break;
            case AttributeType.Tackling: value = Tackling; break;
            case AttributeType.Teamwork: value = Teamwork; break;
            case AttributeType.Vision: value = Vision; break;
        }

        return value;
    }

    public void ResetStatistics(string _type, string _id="")
    {
        switch(_type)
        {
            default:
            case "Match" : MatchStats = new PlayerStatistics(); break;
            case "LifeTime" : Attributes.LifeTimeStats = new PlayerStatistics(); break;
            case "Tournament" :
                if (!Attributes.TournamentStatistics.ContainsKey(_id)) Attributes.TournamentStatistics.Add(_id, new PlayerStatistics());
                Attributes.TournamentStatistics[_id] = new PlayerStatistics();
                Attributes.TournamentStatistics[_id] = new PlayerStatistics();
                break;
        }
    }

    public void UpdateLifeTimeStats()
    {
        PlayerStatistics stats = MatchStats;

        UpdateStats(Attributes.LifeTimeStats, stats);

        if (MainController.Instance.CurrentTournament != null) UpdateTournamentStatistics(stats);

       ResetStatistics("Match");
    }

    void UpdateStats(PlayerStatistics _stats, PlayerStatistics _data)
    {
        _stats.TotalGoals += _data.TotalGoals;
        _stats.TotalPasses += _data.TotalPasses;
        _stats.TotalCrosses += _data.TotalCrosses;
        _stats.TotalFaults += _data.TotalFaults;
        _stats.TotalTackles += _data.TotalTackles;
        _stats.TotalDribbles += _data.TotalDribbles;
        _stats.TotalHeaders += _data.TotalHeaders;
        _stats.TotalSaves += _data.TotalSaves;
        _stats.TotalShots += _data.TotalShots;
        _stats.TotalCrossesMissed += _data.TotalCrossesMissed;
        _stats.TotalDribblesMissed += _data.TotalDribblesMissed;
        _stats.TotalHeadersMissed += _data.TotalHeadersMissed;
        _stats.TotalPassesMissed += _data.TotalPassesMissed;
        _stats.TotalShotsMissed += _data.TotalShotsMissed;
        _stats.TotalPresence += _data.TotalPresence;
    }

    public PlayerStatistics GetTournamentStatistics(string _key)
    {
        PlayerStatistics stats = new PlayerStatistics();

        if (Attributes.TournamentStatistics != null && Attributes.TournamentStatistics.ContainsKey(_key)) stats = Attributes.TournamentStatistics[_key];

        return stats;
    }

    void UpdateTournamentStatistics(PlayerStatistics _stats)
    {
        TournamentData currentTournament = MainController.Instance.CurrentTournament;
        if (Attributes.TournamentStatistics == null) Attributes.TournamentStatistics = new TournamentStats();

        if (!Attributes.TournamentStatistics.ContainsKey(currentTournament.Id))
        {
            Attributes.TournamentStatistics.Add(currentTournament.Id, new PlayerStatistics());
        }

        PlayerStatistics tStats = GetTournamentStatistics(currentTournament.Id);

        UpdateStats(tStats, _stats);
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
            case PlayerAttributes.PlayerPosition.Goalkeeper:
                if (zone != 0) value = true;
                break;

            case PlayerAttributes.PlayerPosition.Defender:
                if (zone < 1 || zone > 7) value = true;
                break;

            case PlayerAttributes.PlayerPosition.Midfielder:
                if (zone < 8 || zone > 22) value = true;
                break;

            case PlayerAttributes.PlayerPosition.Forward:
                if (zone < 23) value = true;
                break;
        }
        return value;
    }

    public void Reset()
    {
        ResetStatistics("LifeTime");
        Team = null;
        Attributes.TournamentStatistics = new TournamentStats();
    }
}