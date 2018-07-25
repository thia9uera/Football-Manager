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
    public PlayerTacticsData Tactics;

    [Space(10)]
    [Header("Technical Attributes")]
    [Space(5)]

    [Range(0, 100)]
    public int Goalkeeping = 50;

    [Range(0, 100)]
    public int Passing = 50;

    [Range(0, 100)]
    public int Crossing = 50;

    [Range(0, 100)]
    public int Tackling = 50;

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


    [SerializeField]
    private float positionDebuf = 0.75f;
 
    //Attributes that change during gameplay
    [HideInInspector]
    public int Fatigue = 100;

    [HideInInspector]
    public int Morale = 50;

    [HideInInspector]
    public PlayerPosition AssignedPosition;

    [HideInInspector]
    public float DefPosChance, OffPosChance, ParPosChance, PassingChance, ShootingChance, FaultChance, CrossingChance, DribblingChance, FallingChance = 1f;


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
    }

    public void ApplyBonus()
    {
        DefPosChance = OffPosChance = ParPosChance = PassingChance = ShootingChance = FaultChance =  CrossingChance = DribblingChance = FallingChance = 1f;

        if (Tactics != null)
        {
            foreach (PlayerTacticsData.Effect effect in Tactics.AttributesAffected)
            {
                float bonus = effect.Bonus;
                switch(effect.Attribute)
                {
                    case PlayerAttributes.CrossingChance: CrossingChance = bonus; break;
                    case PlayerAttributes.DefPosChance: DefPosChance = bonus; break;
                    case PlayerAttributes.DribblingChance: DribblingChance = bonus; break;
                    case PlayerAttributes.FallingChance: FallingChance = bonus; break;
                    case PlayerAttributes.FaultChance: FaultChance = bonus; break;
                    case PlayerAttributes.OffPosChance: OffPosChance = bonus; break;
                    case PlayerAttributes.ParPosChance: ParPosChance = bonus; break;
                    case PlayerAttributes.PassingChance: PassingChance = bonus; break;
                    case PlayerAttributes.ShootingChance: ShootingChance = bonus; break;
                }
            }
        }
    }

    public float GetChancePerZone(MatchController.FieldZone _zone)
    {
        float pct = 0f;
        float high_chance = 0.5f;
        float medium_chance = 0.25f;
        float low_chance = 0.1f;

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
                    case MatchController.FieldZone.CD: pct = high_chance * ParPosChance; break;
                    //Off
                    case MatchController.FieldZone.LDM: pct = medium_chance * OffPosChance; break;
                    case MatchController.FieldZone.CDM: pct = low_chance * OffPosChance; break;
                    //Def
                    case MatchController.FieldZone.OwnGoal: pct = medium_chance * DefPosChance; break;
                }
                break;

            case PlayerPosition.CD:
                switch (_zone)
                {
                    case MatchController.FieldZone.CD: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.LD: pct = medium_chance * ParPosChance; break;
                    case MatchController.FieldZone.RD: pct = medium_chance * ParPosChance; break;
                    //Off
                    case MatchController.FieldZone.LDM: pct = low_chance * OffPosChance; break;
                    case MatchController.FieldZone.RDM: pct = low_chance * OffPosChance; break;
                    case MatchController.FieldZone.CDM: pct = medium_chance * OffPosChance; break;
                    //Def
                    case MatchController.FieldZone.OwnGoal: pct = high_chance * DefPosChance; break;
                }
                break;

            case PlayerPosition.RD:
                switch (_zone)
                {
                    case MatchController.FieldZone.RD: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CD: pct = high_chance * ParPosChance; break;
                    //Off
                    case MatchController.FieldZone.RDM: pct = medium_chance * OffPosChance; break;
                    case MatchController.FieldZone.CDM: pct = low_chance * OffPosChance; break;
                    //Def
                    case MatchController.FieldZone.OwnGoal: pct = medium_chance * DefPosChance; break;
                }
                break;

            case PlayerPosition.LDM:
                switch (_zone)
                {
                    case MatchController.FieldZone.LDM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CDM: pct = medium_chance * ParPosChance; break;
                    //Off
                    case MatchController.FieldZone.LM: pct = medium_chance * OffPosChance; break;
                    case MatchController.FieldZone.CM: pct = low_chance * OffPosChance; break;
                    //Def
                    case MatchController.FieldZone.LD: pct = medium_chance * DefPosChance; break;
                    case MatchController.FieldZone.CD: pct = low_chance * DefPosChance; break;
                }
                break;

            case PlayerPosition.CDM:
                switch (_zone)
                {
                    case MatchController.FieldZone.CDM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.LDM: pct = medium_chance * ParPosChance; break;
                    case MatchController.FieldZone.RDM: pct = medium_chance * ParPosChance; break;
                    //Off
                    case MatchController.FieldZone.CM: pct = medium_chance * OffPosChance; break;
                    case MatchController.FieldZone.LM: pct = low_chance * OffPosChance; break;
                    case MatchController.FieldZone.RM: pct = low_chance * OffPosChance; break;
                    //Def
                    case MatchController.FieldZone.LD: pct = low_chance * DefPosChance; break;
                    case MatchController.FieldZone.RD: pct = low_chance * DefPosChance; break;
                    case MatchController.FieldZone.CD: pct = medium_chance * DefPosChance; break;
                }
                break;

            case PlayerPosition.RDM:
                switch (_zone)
                {
                    case MatchController.FieldZone.RDM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CDM: pct = medium_chance * ParPosChance; break;
                    //Off
                    case MatchController.FieldZone.CM: pct = low_chance * OffPosChance; break;
                    case MatchController.FieldZone.RM: pct = medium_chance * OffPosChance; break;
                    //Def
                    case MatchController.FieldZone.RD: pct = medium_chance * DefPosChance; break;
                    case MatchController.FieldZone.CD: pct = low_chance * DefPosChance; break;
                }
                break;

            case PlayerPosition.LM:
                switch (_zone)
                {
                    case MatchController.FieldZone.LM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CM: pct = medium_chance * ParPosChance; break;
                    //Off
                    case MatchController.FieldZone.CAM: pct = low_chance * OffPosChance; break;
                    case MatchController.FieldZone.LAM: pct = medium_chance * OffPosChance; break;
                    //Def
                    case MatchController.FieldZone.LDM: pct = medium_chance * DefPosChance; break;
                    case MatchController.FieldZone.CDM: pct = low_chance * DefPosChance; break;
                }
                break;

            case PlayerPosition.CM:
                switch (_zone)
                {
                    case MatchController.FieldZone.CM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.LM: pct = medium_chance * ParPosChance; break;
                    case MatchController.FieldZone.RM: pct = medium_chance * ParPosChance; break;
                    //Off
                    case MatchController.FieldZone.CAM: pct = medium_chance * OffPosChance; break;
                    case MatchController.FieldZone.LAM: pct = low_chance * OffPosChance; break;
                    case MatchController.FieldZone.RAM: pct = low_chance * OffPosChance; break;
                    //Def
                    case MatchController.FieldZone.CDM: pct = medium_chance * DefPosChance; break;
                    case MatchController.FieldZone.LDM: pct = low_chance * DefPosChance; break;
                    case MatchController.FieldZone.RDM: pct = low_chance * DefPosChance; break;
                }
                break;

            case PlayerPosition.RM:
                switch (_zone)
                {
                    case MatchController.FieldZone.RM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CM: pct = medium_chance * ParPosChance; break;
                    //Off
                    case MatchController.FieldZone.RAM: pct = medium_chance * OffPosChance; break;
                    case MatchController.FieldZone.CAM: pct = low_chance * OffPosChance; break;
                    //Def
                    case MatchController.FieldZone.RDM: pct = medium_chance * DefPosChance; break;
                    case MatchController.FieldZone.CDM: pct = low_chance * DefPosChance; break;
                }
                break;

            case PlayerPosition.LAM:
                switch (_zone)
                {
                    case MatchController.FieldZone.LAM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CAM: pct = high_chance * ParPosChance; break;
                    //Off
                    case MatchController.FieldZone.LF: pct = high_chance * OffPosChance; break;
                    case MatchController.FieldZone.CF: pct = medium_chance * OffPosChance; break;
                    case MatchController.FieldZone.Box: pct = low_chance * OffPosChance; break;
                    //Def
                    case MatchController.FieldZone.LM: pct = medium_chance * DefPosChance; break;
                    case MatchController.FieldZone.CM: pct = low_chance * DefPosChance; break;
                    
                }
                break;

            case PlayerPosition.CAM:
                switch (_zone)
                {
                    case MatchController.FieldZone.CAM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.LAM: pct = high_chance * ParPosChance; break;
                    case MatchController.FieldZone.RAM: pct = high_chance * ParPosChance; break;
                    case MatchController.FieldZone.Box: pct = medium_chance * ParPosChance; break;
                    //Off
                    case MatchController.FieldZone.CF: pct = high_chance * OffPosChance; break;
                    case MatchController.FieldZone.RF: pct = low_chance * OffPosChance; break;
                    case MatchController.FieldZone.LF: pct = low_chance * OffPosChance; break;
                    //Def
                    case MatchController.FieldZone.LM: pct = low_chance * DefPosChance; break;
                    case MatchController.FieldZone.RM: pct = low_chance * DefPosChance; break;
                    case MatchController.FieldZone.CM: pct = medium_chance * DefPosChance; break;
                }
                break;

            case PlayerPosition.RAM:
                switch (_zone)
                {
                    case MatchController.FieldZone.RAM: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CAM: pct = high_chance * ParPosChance; break;
                    //Off
                    case MatchController.FieldZone.RF: pct = high_chance * OffPosChance; break;
                    case MatchController.FieldZone.CF: pct = medium_chance * OffPosChance; break;
                    case MatchController.FieldZone.Box: pct = low_chance * OffPosChance; break;
                    //Def
                    case MatchController.FieldZone.RM: pct = medium_chance * DefPosChance; break;
                    case MatchController.FieldZone.CM: pct = low_chance * DefPosChance; break;
                    
                }
                break;

            case PlayerPosition.LF:
                switch (_zone)
                {
                    case MatchController.FieldZone.LF: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CF: pct = high_chance * ParPosChance; break;
                    //Off
                    case MatchController.FieldZone.Box: pct = high_chance * OffPosChance; break;
                    //Def
                    case MatchController.FieldZone.LAM: pct = medium_chance * DefPosChance; break;
                    case MatchController.FieldZone.LM: pct = low_chance * DefPosChance; break;
                    case MatchController.FieldZone.CAM: pct = low_chance * DefPosChance; break;
                }
                break;

            case PlayerPosition.CF:
                switch (_zone)
                {
                    case MatchController.FieldZone.CF: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.LF: pct = low_chance * ParPosChance; break;
                    case MatchController.FieldZone.RF: pct = low_chance * ParPosChance; break;
                    //Off
                    case MatchController.FieldZone.Box: pct = high_chance * OffPosChance; break;
                    //Def
                    case MatchController.FieldZone.CAM: pct = medium_chance * DefPosChance; break;
                    case MatchController.FieldZone.LAM: pct = low_chance * DefPosChance; break;
                    case MatchController.FieldZone.RAM: pct = low_chance * DefPosChance; break;
                }
                break;

            case PlayerPosition.RF:
                switch (_zone)
                {
                    case MatchController.FieldZone.RF: pct = 1f; break;
                    //Parallel
                    case MatchController.FieldZone.CF: pct = high_chance; break;
                    //Off
                    case MatchController.FieldZone.Box: pct = high_chance; break;
                    //Def
                    case MatchController.FieldZone.RAM: pct = medium_chance; break;
                    case MatchController.FieldZone.CAM: pct = low_chance; break;
                    case MatchController.FieldZone.RM: pct = low_chance; break;
                }
                break;
        }

        if (AssignedPosition != Position) pct = pct * positionDebuf;

        return pct;
    }
}