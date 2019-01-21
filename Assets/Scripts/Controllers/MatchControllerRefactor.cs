using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchControllerRefactor : MonoBehaviour
{
    public enum MatchEvent
    {
        None,
        KickOff,
        Penalty,
        Freekick,
        Offside,
        ThrowIn,
        Goal,
        Goalkick,
        CornerKick,
    }

    public enum MarkingType
    {
        None,
        Distance,
        Close,
        Steal
    }

    [SerializeField]
    ActionChancePerZoneData actionChancePerZone;

    public Field Field;

    public TeamData HomeTeam;
    public TeamData AwayTeam;

    TeamData attackingTeam;
    TeamData defendingTeam;

    public class PlayInfo
    {
        public PlayerData Attacker;
        public float AttackerRoll;
        public PlayerData.PlayerAction OffensiveAction;

        public PlayerData Defender;
        public float DefenderRoll;
        public PlayerData.PlayerAction DefensiveAction;

        public MarkingType Marking;
        public MatchEvent Event;
        public int Excitment;
    }
    public List<PlayInfo> PlayList;

    float attackingBonus = 1f;
    bool keepAttacker;
    bool keepDefender;

    float positionDebuff;
    float attackingBonusLow;
    float attackingBonusMedium;
    float attackingBonusHigh;
    float faultChance;
    float offsideChance;
    float counterAttackChance;

    int fatigueLow;
    int fatigueMedium;
    int fatigueHigh;
    float fatigueRecoverHalfTime;

    int matchTime = 0;
    bool isGameOn;
    bool isSimulatingMatch;
    bool isSimulatingTournament;

    bool isHalfTime;
    bool secondHalfStarted;
    bool isFreekickTaken;
    bool isGoalAnnounced;
    bool isScorerAnnounced;

    MatchScreen screen;

    void Awake()
    {
        Game_Modifier modifiers = MainController.Instance.Modifiers.game_Modifiers[0];

        screen = GetComponent<MatchScreen>();

        positionDebuff = modifiers.PositionDebuff;
        attackingBonusLow = modifiers.AttackBonusLow;
        attackingBonusMedium = modifiers.AttackBonusMediun;
        attackingBonusHigh = modifiers.AttackBonusHigh;
        faultChance = modifiers.FaultChance;
        offsideChance = modifiers.OffsideChance;
        counterAttackChance = modifiers.CounterAttackChance;

        fatigueLow = modifiers.FatigueLow;
        fatigueMedium = modifiers.FatigueMedium;
        fatigueHigh = modifiers.FatigueHigh;
        fatigueRecoverHalfTime = modifiers.FatigueRecoverHalfTime;
    }

    public void Populate(MatchData _data, bool _simulateTournament = false)
    {
        Reset();

        HomeTeam = MainController.Instance.GetTeamById(_data.HomeTeam.TeamAttributes.Id);
        AwayTeam = MainController.Instance.GetTeamById(_data.AwayTeam.TeamAttributes.Id);

        attackingTeam = HomeTeam;
        defendingTeam = AwayTeam;

        screen.HomeTeamSquad.Populate(HomeTeam, true);
        screen.AwayTeamSquad.Populate(AwayTeam, true);
        screen.Score.UpdateTime(matchTime);
        screen.Score.UpdateScore(0, 0);
        screen.Score.Populate(HomeTeam.Name, 0, HomeTeam.PrimaryColor, AwayTeam.Name, 0, AwayTeam.PrimaryColor);


        isSimulatingTournament = _simulateTournament;
        if (_simulateTournament) StartSimulation(true);
    }

    public void StartSimulation(bool _hideMain)
    {
        screen.Narration.Reset();

        isSimulatingMatch = true;

        Reset();
        KickOff();

        if (_hideMain)
        {
            if (screen == null) screen = GetComponent<MatchScreen>();
            screen.ShowMain(false);
        }
    }

    public void KickOff()
    {
        Field.CurrentZone = Field.LastZone = Field.Zone.CM;
        screen.Field.UpdateFieldArea((int)Field.CurrentZone);

        isGameOn = true;

        HomeTeam.ResetMatchData();
        AwayTeam.ResetMatchData();

        if (!isSimulatingMatch)
        {
            //UpdateNarration("nar_KickOff_");
            StartCoroutine("GameLoop");
        }
    }

    public void Reset()
    {
        keepAttacker = false;
        keepDefender = false;
        matchTime = 0;
        isHalfTime = false;
        secondHalfStarted = false;
        isFreekickTaken = false;
        isGoalAnnounced = false;
        isScorerAnnounced = false;
        if (screen.HomeTeamSquad != null)
        {
            screen.HomeTeamSquad.ResetFatigue();
            screen.AwayTeamSquad.ResetFatigue();
        }

        if (!isSimulatingMatch) screen.Narration.Reset();
    }
}
