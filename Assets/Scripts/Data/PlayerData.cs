using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using RotaryHeart.Lib.SerializableDictionary;

[CreateAssetMenu(fileName = "Player", menuName = "Player Data", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Personal")]
    public string FirstName;
    public string LastName;

    
    [Space(10)]
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

    [Space(10)]
    //Attributes that change during gameplay
    [SerializeField]
    private float fatigue = 100f;

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

    [System.Serializable]
    public class Statistics
    {
        public int TotalPasses = 0;
        public int TotalCrosses = 0;
        public int TotalShots = 0;
        public int TotalHeaders = 0;
        public int TotalFaults = 0;
        public int TotalTackles = 0;
        public int TotalDribbles = 0;
        public int TotalGoals = 0;
        public int TotalSaves = 0;
        public int TotalPassesMissed = 0;
        public int TotalShotsMissed = 0;
        public int TotalHeadersMissed = 0;
        public int TotalDribblesMissed = 0;
        public int TotalCrossesMissed  =0;
        public int TotalPresence = 0;
    }

    public Statistics LifeTimeStats;
    public Statistics MatchStats;

    [System.Serializable]
    public class TournamentStats : SerializableDictionaryBase<string, Statistics> { }
    public TournamentStats TournamentStatistics;

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
            case PlayerPosition.Goalkeeper:
                total += Goalkeeping;
                total += Agility;
                total = total / 16;
                break;
            case PlayerPosition.Defender:
                total += Tackling;
                total += Blocking;
                total = total / 16;
                break;
            case PlayerPosition.Midfielder:
                total += Dribbling;
                total += Passing;
                total += Crossing;
                total = total / 17;
                break;
            case PlayerPosition.Forward:
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

    public float GetChancePerZone(MatchController.FieldZone _zone, TeamData.TeamStrategy _teamStrategy)
    {
        float pct = 0f;

        //Zones chancePerZone = MainController.Instance.PosChancePerZone.posChancePerZones[(int)Zone];
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


    public void ResetStatistics(string _type, string _id="")
    {
        switch(_type)
        {
            default:
            case "Match" : MatchStats = new Statistics(); break;
            case "LifeTime" : LifeTimeStats = new Statistics(); break;
            case "Tournament" :
                if (!TournamentStatistics.ContainsKey(_id)) TournamentStatistics.Add(_id, new Statistics());
                TournamentStatistics[_id] = new Statistics();
                break;
        }
        Save();
    }

    public void UpdateLifeTimeStats()
    {
        Statistics stats = MatchStats;

        UpdateStats(LifeTimeStats, stats);

        if (MainController.Instance.CurrentTournament != null) UpdateTournamentStatistics(stats);

        ResetStatistics("Match");
    }

    void UpdateStats(Statistics _stats, Statistics _data)
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

    public Statistics GetTournamentStatistics(string _key)
    {
        Statistics stats = null;

        if (TournamentStatistics != null && TournamentStatistics.ContainsKey(_key)) stats = TournamentStatistics[_key];

        return stats;
    }

    void UpdateTournamentStatistics(Statistics _stats)
    {
        TournamentData currentTournament = MainController.Instance.CurrentTournament;
        if (TournamentStatistics == null) TournamentStatistics = new TournamentStats();

        if (!TournamentStatistics.ContainsKey(currentTournament.Id))
        {
            TournamentStatistics.Add(currentTournament.Id, new Statistics());
        }

        Statistics tStats = GetTournamentStatistics(currentTournament.Id);

        UpdateStats(tStats, _stats);

        Save();
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
                if (zone < 1 || zone > 7) value = true;
                break;

            case PlayerPosition.Midfielder:
                if (zone < 8 || zone > 22) value = true;
                break;

            case PlayerPosition.Forward:
                if (zone < 23) value = true;
                break;
        }
        return value;
    }

    void Save()
    {
        EditorUtility.SetDirty(this);
        //AssetDatabase.SaveAssets();
    }
}