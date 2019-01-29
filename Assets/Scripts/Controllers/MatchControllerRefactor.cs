﻿using System.Collections;
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
        ShotSaved,
        ShotMissed,
        GoalAnnounced,
        ScorerAnnounced,
        FreekickTaken,
        ShotOnGoal,
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
    MatchScreen screen;

    #region Game Modifiers

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

    #endregion

    public class PlayInfo
    {
        public PlayerData Attacker;
        public float AttackerRoll;
        public PlayerData.PlayerAction OffensiveAction;
        public bool IsActionSuccesful;

        public PlayerData Defender;
        public float DefenderRoll;
        public PlayerData.PlayerAction DefensiveAction;

        public MarkingType Marking;
        public MatchEvent Event;
        public PlayerData Assister;
        public int Excitment;
        public Field.Zone Zone;
        public Field.Zone TargetZone;
    }
    List<PlayInfo> playList;

    TeamData homeTeam;
    TeamData awayTeam;

    TeamData attackingTeam;
    TeamData defendingTeam;

    float attackingBonus = 1f;
    int counterAttack = 0;
    bool keepAttacker;
    bool keepDefender;

    int matchTime = 0;
    bool isGameOn;
    bool isSimulatingMatch;
    bool isSimulatingTournament;
    int turn = 0;

    bool isHalfTime;
    bool secondHalfStarted;
    bool isFreekickTaken;
    bool isGoalAnnounced;
    bool isScorerAnnounced;

    int matchSpeed = 1;

    void Awake()
    {
        Game_Modifier modifiers = MainController.Instance.Modifiers.game_Modifiers[0];

        screen = GetComponent<MatchScreen>();
        Field = GetComponent<Field>();

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

        homeTeam = MainController.Instance.GetTeamById(_data.HomeTeam.TeamAttributes.Id);
        awayTeam = MainController.Instance.GetTeamById(_data.AwayTeam.TeamAttributes.Id);

        attackingTeam = homeTeam;
        defendingTeam = awayTeam;

        homeTeam.ResetMatchData();
        awayTeam.ResetMatchData();

        screen.HomeTeamSquad.Populate(homeTeam, true);
        screen.AwayTeamSquad.Populate(awayTeam, true);
        screen.Score.UpdateTime(matchTime);
        screen.Score.UpdateScore(0, 0);
        screen.Score.Populate(homeTeam.Name, 0, homeTeam.PrimaryColor, awayTeam.Name, 0, awayTeam.PrimaryColor);


        isSimulatingTournament = _simulateTournament;
        if (_simulateTournament) StartSimulation(true);
    }

    public void UpdateTeams(List<PlayerData> _in, List<PlayerData> _out)
    {
        if (_in.Count > 0)
        {
            string playersIn = "";
            string playersOut = "";
            PlayerData player;
            for (int i = 0; i < _in.Count; i++)
            {
                player = _in[i];
                if (i == 0) playersIn += player.FirstName + " " + player.LastName;
                else playersIn += ", " + player.FirstName + " " + player.LastName;
            }

            for (int i = 0; i < _out.Count; i++)
            {
                player = _out[i];
                if (i == 0) playersOut += player.FirstName + " " + player.LastName;
                else playersOut += ", " + player.FirstName + " " + player.LastName;
            }
        }

        //if(matchTime > 0 && matchTime < 90) PauseGame(false);


        screen.HomeTeamSquad.Populate(homeTeam);
        screen.AwayTeamSquad.Populate(awayTeam);
    }

    void StartSimulation(bool _hideMain)
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
        if (!isSimulatingMatch)
        {
            //UpdateNarration("nar_KickOff_");
            StartCoroutine("GameLoop");
        }
    }

    IEnumerator GameLoop()
    {
        while (isGameOn == true)
        {
            bool evt = false;
            if(turn == 0)
            {
                evt = true;
                PlayInfo play = new PlayInfo();
                play.Event = MatchEvent.None;
                play.Marking = MarkingType.None;
                playList.Insert(0, play);
                ResolveKickOff(attackingTeam, turn);
            }

            else if(turn > 0)
            {
               UpdateNarration(turn - 1);
               evt =  ResolveEvents(attackingTeam, defendingTeam, turn);
            }
            

            if (!evt) DefineActions(attackingTeam, defendingTeam, Field.CurrentZone, turn);            
            turn++;

            yield return new WaitForSeconds(2f / matchSpeed);
        }
    }

    IEnumerator SimulateLoop()
    {
        while (isGameOn)
        {
            bool evt = false;
            if (turn > 0)
            {
                evt = ResolveEvents(attackingTeam, defendingTeam, turn);
            }


            if (!evt) DefineActions(attackingTeam, defendingTeam, Field.CurrentZone, turn);
            turn++;
            yield return new WaitForEndOfFrame();
        }
    }

    public void PauseGame(bool _isPaused)
    {

    }

    #region Resolve Match Events

    bool ResolveEvents(TeamData _attackingTeam , TeamData _defendingTeam, int _turn)
    {
        int lastTurn = _turn - 1;
        MatchEvent lastEvent = playList[lastTurn].Event;
        if (lastEvent == MatchEvent.None) return false;
        Field.Zone _zone = playList[lastTurn].TargetZone;

        PlayInfo playInfo = new PlayInfo();
        playList.Insert(_turn, playInfo);
        playInfo.Marking = MarkingType.None;
        playInfo.Zone = _zone;
        switch (lastEvent)
        {
            case MatchEvent.ShotOnGoal: ResolveShotOnGoal(_attackingTeam, _defendingTeam, _zone, _turn); break;
            case MatchEvent.Goal: ResolveGoal(_turn); break;
            case MatchEvent.Offside:
            case MatchEvent.Freekick: ResolveFreekick(_attackingTeam, _defendingTeam,  _turn); break;
            case MatchEvent.Penalty: ResolvePenalty(_attackingTeam, _defendingTeam, _turn); break;
            case MatchEvent.Goalkick: ResolveGoalkick(_attackingTeam, _turn); break;
            case MatchEvent.ShotMissed: ResolveShotMissed(_attackingTeam, _defendingTeam, _turn); break;
            case MatchEvent.ShotSaved: ResolveShotSaved(_attackingTeam, _turn); break;
            case MatchEvent.CornerKick: ResolveCornerKick(_attackingTeam, _turn); break;
        }

        return true;
    }

    void ResolveShotOnGoal(TeamData _attackingTeam, TeamData _defendingTeam, Field.Zone _zone, int _turn)
    {
        PlayInfo lastPlay = playList[_turn - 1];
        PlayInfo play = playList[_turn];
        CopyPlay(play, lastPlay);
        play.Defender = _defendingTeam.Squad[0];
        play.DefensiveAction = PlayerData.PlayerAction.Save;
        play.Event = MatchEvent.None;
        play.Zone = _zone;
        bool success = IsShotSuccessful(_turn);
        play.IsActionSuccesful = success;
        if (success) play.Event = MatchEvent.Goal;
        else
        {
            int roll = Dice.Roll(20);
            if (play.Excitment == -1) play.Event = MatchEvent.CornerKick;
            else if (play.Excitment == 0 && roll < 5) play.Event = MatchEvent.CornerKick;
            else play.Event = MatchEvent.ShotSaved;
        }
    }

    bool IsShotSuccessful(int _turn)
    {
        PlayInfo play = playList[_turn];
        PlayInfo lastPlay = playList[_turn - 1];
        PlayerData attacker = play.Attacker;
        PlayerData defender = play.Defender;

        float attacking = 0f;
        float defending = 0f;
        float distanceModifier = 1f;
        int bonusChance = 0;

        Field.Zone zone = GetTeamZone(attacker.Team, play.Zone);

        switch (zone)
        {
            case Field.Zone.LAM:
            case Field.Zone.RAM:
                distanceModifier = 0.5f;
                break;

            case Field.Zone.LCAM:
            case Field.Zone.CAM:
            case Field.Zone.RCAM:
                distanceModifier = 0.65f;
                break;

            case Field.Zone.LF:
            case Field.Zone.RF:
                distanceModifier = 0.75f;
                break;

            case Field.Zone.ALF:
            case Field.Zone.ARF:
                distanceModifier = 0.35f;
                break;

            case Field.Zone.LCF:
            case Field.Zone.CF:
            case Field.Zone.RCF:
                distanceModifier = 0.8f;
                break;
        }

        if (play.OffensiveAction == PlayerData.PlayerAction.Shot)
        {

            if (lastPlay.Event == MatchEvent.Freekick)
            {
                attacking = (float)(attacker.Freekick + attacker.Strength) / 200;
                bonusChance = GetPlayerAttributeBonus(attacker.Freekick);
            }
            else if (lastPlay.Event == MatchEvent.Penalty)
            {
                attacking = (float)(attacker.Penalty + attacker.Strength) / 200;
                bonusChance = GetPlayerAttributeBonus(attacker.Penalty);
            }
            else
            {
                attacking = (float)(attacker.Shooting + attacker.Strength) / 200;
                bonusChance = GetPlayerAttributeBonus(attacker.Shooting);
            }
        }
        else if (play.OffensiveAction == PlayerData.PlayerAction.Header)
        {
            attacking = (float)(attacker.Heading + attacker.Strength) / 200;
            bonusChance = GetPlayerAttributeBonus(attacker.Heading);
        }

        attacking *= attacker.FatigueModifier();
        attacking *= distanceModifier;

        if (play.Marking == MarkingType.Close) attacking *= 0.5f;
        attacking *= attackingBonus;

        int attackRoll = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(attacking * 5), bonusChance);

        if (attackRoll >= 20)
        {
            attacking *= 1.5f;
            play.Excitment = 1;
        }
        else if (attackRoll >= 10)
        {
            attacking *= 1 + (float)(attackRoll - 9) / 100;
            play.Excitment = 0;
        }
        else if (attackRoll <= 4)
        {
            play.Event = MatchEvent.Goalkick;
            return false;
        }

        defending = ((float)defender.Goalkeeping + defender.Agility) / 200;
        defending *= defender.FatigueModifier();
        float defenseRoll = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(defending * 5), GetPlayerAttributeBonus(defender.Goalkeeping));

        int defenseExcitement = 0;
        if (defenseRoll >= 20)
        {
            defending *= 2f;
            defenseExcitement = 1;
        }
        else if (defenseRoll >= 10)
        {
            defending *= 1 + (float)(defenseRoll - 9) / 100;
            defenseExcitement = 0;
        }
        else if (defenseRoll <= 1)
        {
            defending *= 0.5f;
            defenseExcitement = -1;
        }

        //CHECK ATTACKING X DEFENDING
        if (attacking <= defending)
        {
            play.Excitment = defenseExcitement;
            return false;
        }
        else
        {
            return true;
        }
    }

    void ResolveGoal(int _turn)
    {
        PlayInfo lastPlay = playList[_turn - 1];
        PlayerData shooter = lastPlay.Attacker;
        PlayerData assister = lastPlay.Assister;

        shooter.MatchStats.Goals++;
        shooter.Team.MatchStats.TotalGoals++;
        shooter.Team.MatchData.Scorers.Add(shooter);
        if (assister != null) assister.MatchStats.Assists++;

        PlayInfo playInfo = playList[_turn];
        CopyPlay(playInfo, lastPlay);
        playInfo.Event = MatchEvent.GoalAnnounced;
        playList.Insert(_turn, playInfo);
    }

    void ResolveGoalAnnounced(int _turn)
    {
        PlayInfo lastPlay = playList[_turn - 1];
        PlayInfo playInfo = playList[_turn];
        CopyPlay(playInfo, lastPlay);
        playInfo.Event = MatchEvent.ScorerAnnounced;
        playList.Insert(_turn, playInfo);
    }

    void ResolveScorerAnnounced(int _turn)
    {
        PlayInfo lastPlay = playList[_turn - 1];
        PlayInfo playInfo = playList[_turn];
        CopyPlay(playInfo, lastPlay);
        playInfo.Event = MatchEvent.KickOff;
        playList.Insert(_turn, playInfo);

        SwitchPossesion();
    }

    void ResolveFreekick(TeamData _attackingTeam, TeamData _defendingTeam, int _turn)
    {
        PlayInfo playInfo = playList[_turn];
        playInfo.Attacker = GetBestPlayerInArea(_attackingTeam, playInfo.Zone, PlayerData.AttributeType.Freekick);
        playInfo.OffensiveAction = GetFreeKickAction(playInfo.Attacker, playInfo.Zone);
        if (playInfo.OffensiveAction == PlayerData.PlayerAction.Shot)
        {
            playInfo.Event = MatchEvent.ShotOnGoal;
        }
        else
        {
            playInfo.Event = MatchEvent.None;
            ResolveAction(_turn);           
        }
    }

    void ResolvePenalty(TeamData _attackingTeam, TeamData _defendingTeam, int _turn)
    {
        PlayInfo playInfo = playList[_turn];
        playInfo.Attacker = _attackingTeam.GetTopPlayerByAttribute(PlayerData.AttributeType.Penalty, _attackingTeam.Squad);
        playInfo.Defender = _defendingTeam.Squad[0];
        playInfo.OffensiveAction = PlayerData.PlayerAction.Shot;
        playInfo.Event = MatchEvent.ShotOnGoal;
        ResolveShotOnGoal(_attackingTeam, _defendingTeam, playInfo.Zone, _turn);
    }

    void ResolveGoalkick(TeamData _attackingTeam, int _turn)
    {
        PlayInfo playInfo = playList[_turn];
        playInfo.Attacker = _attackingTeam.Squad[0];
        playInfo.Zone = Field.Zone.OwnGoal;
        if (_attackingTeam == awayTeam) playInfo.Zone = Field.Zone.Box;
        playInfo.Marking = MarkingType.None;
        playInfo.OffensiveAction = PlayerData.PlayerAction.Cross;
        ResolveAction(_turn);
    }

    void ResolveShotMissed(TeamData _attackingTeam, TeamData _defendingTeam, int _turn)
    {
        PlayInfo playInfo = playList[_turn];
        PlayInfo lastplay = playList[_turn - 1];
        if (lastplay.Defender != null) playInfo.Attacker = lastplay.Defender;
        else playInfo.Attacker = GetAttackingPlayer(_attackingTeam, lastplay.Zone, null, true);
        playInfo.Defender = GetDefendingPlayer(_defendingTeam, lastplay.Zone, lastplay.Attacker);
        playInfo.Marking = GetMarkingType(playInfo.Defender, playInfo.Zone);
        playInfo.OffensiveAction = GetOffensiveAction(playInfo.Marking, playInfo.Attacker, playInfo.Zone, false);
        return;
    }

    void ResolveCornerKick(TeamData _attackingTeam, int _turn)
    {
        PlayInfo playInfo = playList[_turn];
        playInfo.Attacker = _attackingTeam.GetTopPlayerByAttribute(PlayerData.AttributeType.Crossing, _attackingTeam.Squad, false);
        playInfo.Zone = Field.Zone.ARF;
        if (playInfo.Attacker.Team == awayTeam) playInfo.Zone = Field.Zone.BLD;
        playInfo.OffensiveAction = GetFreeKickAction(playInfo.Attacker, playInfo.Zone);
        if (playInfo.OffensiveAction == PlayerData.PlayerAction.Shot)
        {
            playInfo.Event = MatchEvent.ShotOnGoal;
        }
        else
        {
            //TODO Resolve Action
            playInfo.Event = MatchEvent.None;
        }
    }

    void ResolveShotSaved(TeamData _attackingTeam, int _turn)
    {
        PlayInfo play = playList[_turn];
        play.Attacker = _attackingTeam.Squad[0];
        play.Zone = Field.Zone.OwnGoal;
        if (play.Attacker.Team == awayTeam) play.Zone = Field.Zone.Box;
        play.OffensiveAction = GetOffensiveAction(MarkingType.None, play.Attacker, GetTeamZone(_attackingTeam, play.Zone), false);
        play.IsActionSuccesful = IsActionSuccessful(_turn);
    }

    void ResolveKickOff(TeamData _attackingTeam, int _turn)
    {
        PlayInfo playInfo = playList[_turn];
        playInfo.Zone = Field.Zone.CM;
        playInfo.Attacker = GetAttackingPlayer(_attackingTeam, playInfo.Zone, null, true);
        playInfo.OffensiveAction = PlayerData.PlayerAction.Pass;
        attackingBonus = 1f;
        ResolveAction(_turn);
    }

    #endregion

    #region Match Actions

    void DefineActions(TeamData _attackingTeam, TeamData _defendingTeam, Field.Zone _zone, int _turn)
    {
        PlayInfo playInfo = new PlayInfo();
        playList.Insert(_turn, playInfo);

        screen.HomeTeamSquad.UpdateFatigue();
        screen.AwayTeamSquad.UpdateFatigue();

        keepAttacker = false;
        keepDefender = false;
        PlayerData exludePlayer = null;
        bool forcePlayer = false;
        if (_turn > 0)
        {
            PlayInfo lastPlay = playList[_turn-1];

            playInfo.Zone = lastPlay.TargetZone;

            if (lastPlay.OffensiveAction == PlayerData.PlayerAction.Pass || lastPlay.OffensiveAction == PlayerData.PlayerAction.LongPass || lastPlay.OffensiveAction == PlayerData.PlayerAction.Cross)
            {
                playInfo.Assister = exludePlayer = lastPlay.Attacker;
                if (lastPlay.IsActionSuccesful) forcePlayer = true;
            }
            else if (lastPlay.OffensiveAction == PlayerData.PlayerAction.Dribble || lastPlay.OffensiveAction == PlayerData.PlayerAction.Sprint) keepAttacker = true;

            if (!lastPlay.IsActionSuccesful && lastPlay.Defender != null) keepDefender = true;
        }

        //Step 1: Get players involved in the dispute
        if (keepAttacker)
        {
            playInfo.Attacker = playList[_turn - 1].Attacker;
        }
        else
        {
            playInfo.Attacker = GetAttackingPlayer(_attackingTeam, _zone, exludePlayer, forcePlayer);
        }

        if (keepDefender)
        {
            playInfo.Attacker = playList[_turn - 1].Defender;
        }

        playInfo.Defender = GetDefendingPlayer(_defendingTeam, _zone, exludePlayer);
     

        //HALF TIME
        //UpdateMatchTime();

        //No players from attacking team in the dispute
        if (playInfo.Attacker == null)
        {
            keepDefender = true;
            //SwitchPossesion();
        }
        //Player from attacking team in the dispute
        else
        {
            //Step 2: Get type of marking
            if (playInfo.Defender == null || playInfo.Attacker.Zone == Field.Zone.OwnGoal) playInfo.Marking = MarkingType.None;
            else playInfo.Marking = GetMarkingType(playInfo.Defender, _zone);

            if (playInfo.Marking == MarkingType.Steal)
            {
                attackingBonus = 1f;

                keepDefender = true;

                defendingTeam.MatchStats.TotalSteals++;

                //SwitchPossesion();
            }
            else
            {
                //Step 3: Get type of offensive play
                bool header = false;
                if (playList[_turn - 1].IsActionSuccesful && playList[_turn - 1].OffensiveAction == PlayerData.PlayerAction.Cross) header = true;
                playInfo.OffensiveAction = GetOffensiveAction(playInfo.Marking, playInfo.Attacker, _zone, header);

                //Step 4: Test action against defender (if there is one)
                ResolveAction(_turn);
            }
        }
    }

    void ResolveAction(int _turn)
    {
        PlayInfo play = playList[_turn];
        MarkingType marking = play.Marking;
        PlayerData.PlayerAction offensiveAction = play.OffensiveAction;
        play.TargetZone = play.Zone;
        play.IsActionSuccesful = IsActionSuccessful(_turn);

        if (play.IsActionSuccesful)
        {
            //Give bonus based on type of marking
            if (marking == MarkingType.Close) attackingBonus *= attackingBonusHigh;
            else if (marking == MarkingType.Distance) attackingBonus *= attackingBonusMedium;
            else if (marking == MarkingType.None) attackingBonus *= attackingBonusLow;

            if (offensiveAction == PlayerData.PlayerAction.Shot || offensiveAction == PlayerData.PlayerAction.Header) play.Event = MatchEvent.ShotOnGoal;

            else play.TargetZone = GetTeamZone(play.Attacker.Team, Field.GetTargetZone(GetTeamZone(play.Attacker.Team, play.Zone), play.Event, play.OffensiveAction, play.Attacker.Team.Strategy));
        }
        else
        {
            attackingBonus = 1f;
            if (play.Event != MatchEvent.Freekick || play.Event != MatchEvent.Penalty) SwitchPossesion();
            if (offensiveAction == PlayerData.PlayerAction.Shot || offensiveAction == PlayerData.PlayerAction.Header) play.Event = MatchEvent.ShotMissed;
        }
    }

    bool IsActionSuccessful(int _turn)
    {
        PlayInfo play = playList[_turn];
        PlayerData attacker = play.Attacker;
        PlayerData defender = play.Defender;
        Field.Zone zone = play.Zone;
        PlayInfo lastPlay = null;
        PlayerData.PlayerAction lastAction = PlayerData.PlayerAction.None;
        bool isLastActionSuccessful = false;
        MarkingType marking = play.Marking;
        if (_turn > 0)
        {
            lastPlay = playList[_turn - 1];
            lastAction = lastPlay.OffensiveAction;
            isLastActionSuccessful = lastPlay.IsActionSuccesful;
        }
        bool success = false;
        float attacking = 0f;
        float defending = 0f;
        bool isTackling = false;
        int attackBonusChance = 0;
        int defenseBonusChance = 0;
        float fault = faultChance;
        float agilityBonus;
        int attFatigueRate = fatigueLow;
        int defFatigueRate = fatigueLow;
        int attackExcitment = 0;
        int defenseExcitement = 0;

        if ((int)GetTeamZone(attacker.Team, zone) < 8)
        {
            float offside = offsideChance;
            if (defender != null)
            {
                offside *= defender.Prob_OffsideLine;
                if (defender.Team.IsStrategyApplicable(GetTeamZone(defender.Team, zone))) offside *= MainController.Instance.TeamStrategyData.team_Strategys[(int)defender.Team.Strategy].OffsideTrickChance;
            }
            
            if (offside >= Random.Range(0f, 1f))
            {
                play.Event = MatchEvent.Offside;
                return false;
            }
        }

        switch (play.OffensiveAction)
        {
            case PlayerData.PlayerAction.None: return false;

            case PlayerData.PlayerAction.Pass:
                if (lastAction == PlayerData.PlayerAction.Cross && isLastActionSuccessful)
                {
                    attacking = (float)(attacker.Passing + attacker.Agility + attacker.Vision + attacker.Teamwork + attacker.Heading) / 500;
                    if (defender != null)
                    {
                        play.DefensiveAction = PlayerData.PlayerAction.Block;
                        defending = (float)(defender.Blocking + defender.Agility + defender.Vision + defender.Heading) / 400;
                    }
                }
                else
                {
                    attacking = (float)(attacker.Passing + attacker.Agility + attacker.Vision + attacker.Teamwork) / 400;
                    if (defender != null)
                    {
                        play.DefensiveAction = PlayerData.PlayerAction.Block;
                        defending = (float)(defender.Blocking + defender.Agility + defender.Vision) / 300;
                    }
                }

                attackBonusChance = GetPlayerAttributeBonus(attacker.Passing);

                attFatigueRate = fatigueLow;
                if (marking == MarkingType.Close) attacking *= 0.9f;
                if (defender != null) defenseBonusChance = GetPlayerAttributeBonus(defender.Blocking);

                break;

            case PlayerData.PlayerAction.LongPass:
                attacking = (float)(attacker.Passing + attacker.Agility + attacker.Vision + attacker.Teamwork + attacker.Strength) / 500;
                if (defender != null)
                {
                    play.DefensiveAction = PlayerData.PlayerAction.Block;
                    defending = (float)(defender.Blocking + defender.Agility + defender.Vision) / 300;
                }

                attackBonusChance = GetPlayerAttributeBonus(attacker.Passing);

                attFatigueRate = fatigueMedium;
                if (marking == MarkingType.Close) attacking *= 0.9f;
                if (defender != null) defenseBonusChance = GetPlayerAttributeBonus(defender.Blocking);
                break;

            case PlayerData.PlayerAction.Dribble:
                if (defender != null)
                {
                    play.DefensiveAction = PlayerData.PlayerAction.Tackle;
                    defending = (float)(defender.Tackling + defender.Agility + defender.Speed) / 300;
                    defenseBonusChance = GetPlayerAttributeBonus(defender.Tackling);
                }

                attacking = (float)(attacker.Dribbling + attacker.Agility + attacker.Speed) / 300;
                attackBonusChance = GetPlayerAttributeBonus(attacker.Tackling);
                attFatigueRate = fatigueHigh;

                if (marking == MarkingType.Close) attacking = attacking * 0.75f;
                break;

            case PlayerData.PlayerAction.Sprint:
                attacking = (float)(attacker.Agility + attacker.Speed) / 200;
                attackBonusChance = 100;
                attacking *= 2f;
                attFatigueRate = fatigueMedium;
                break;

            case PlayerData.PlayerAction.Cross:
                if (defender != null)
                {
                    play.DefensiveAction = PlayerData.PlayerAction.Block;
                    defending = (float)(defender.Blocking + defender.Agility + defender.Vision) / 300;
                    defenseBonusChance = GetPlayerAttributeBonus(defender.Blocking);
                }

                attacking = (float)(attacker.Crossing + attacker.Agility + attacker.Vision + attacker.Teamwork) / 400;
                attackBonusChance = GetPlayerAttributeBonus(attacker.Crossing);
                attFatigueRate = fatigueLow;

                if (marking == MarkingType.Close) attacking = attacking * 0.75f;
                break;

            case PlayerData.PlayerAction.Shot:
                if (defender != null)
                {
                    play.DefensiveAction = PlayerData.PlayerAction.Block;
                    defending = (float)(defender.Blocking + defender.Agility + defender.Vision + defender.Speed) / 400;
                    defenseBonusChance = GetPlayerAttributeBonus(defender.Blocking);
                }

                attacking = (float)(attacker.Shooting + attacker.Agility + attacker.Strength) / 300;
                attackBonusChance = GetPlayerAttributeBonus(attacker.Shooting);
                attFatigueRate = fatigueLow;

                if (marking == MarkingType.Close) attacking = attacking * 0.75f;

                break;

            case PlayerData.PlayerAction.Header:
                if (defender != null)
                {
                    play.DefensiveAction = PlayerData.PlayerAction.Block;
                    defending = (float)(defender.Heading + defender.Blocking + defender.Agility + defender.Vision) / 400;
                    defenseBonusChance = GetPlayerAttributeBonus(defender.Blocking);
                }

                attacking = (float)(attacker.Heading + attacker.Agility + attacker.Strength) / 300;
                attackBonusChance = GetPlayerAttributeBonus(attacker.Heading);
                attFatigueRate = fatigueMedium;

                if (marking == MarkingType.Close) attacking = attacking * 0.75f;
                break;
        }

        if (defender != null && defender.Zone == Field.Zone.OwnGoal)
        {
            defending = (float)(defender.Goalkeeping + defender.Agility + defender.Vision + defender.Speed) / 400;
            defenseBonusChance = GetPlayerAttributeBonus(defender.Goalkeeping);
        }

        attacking *= attacker.FatigueModifier();

        if (attacker.IsWronglyAssigned()) attacking *= positionDebuff;

        attacking *= attackingBonus;

        int attackRoll = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(attacking * 5), attackBonusChance);

        if (attackRoll >= 20)
        {
            attacking *= 2;
            attackExcitment = 1;
        }
        else if (attackRoll >= 10)
        {
            attacking *= 1 + (float)(attackRoll - 9) / 100;
            attackExcitment = 0;
        }
        else if (attackRoll <= 1)
        {
            if (defender == null)
            {
                success = false;
                return success;
            }
            else
            {
                attacking = attacking * 0.75f;
                attackExcitment = -1;
            }
        }

        //Check if tackling is really happening  
        if (defender == null)
        {
            isTackling = false;
            play.DefensiveAction = PlayerData.PlayerAction.None;
        }
        else
        {
            float tackleChance = 0.75f * actionChancePerZone.actionChancePerZones[(int)zone].Tackle * defender.Prob_Tackling;
            if (marking == MarkingType.Close) tackleChance *= 1.25f;

            if (defender.Team.IsStrategyApplicable(GetTeamZone(defender.Team, zone)))
            {
                tackleChance *= MainController.Instance.TeamStrategyData.team_Strategys[(int)defender.Team.Strategy].TacklingChance;
            }

            isTackling |= tackleChance >= Random.Range(0f, 1f);
        }

        if (isTackling)
        {
            defending *= defender.FatigueModifier();
            defFatigueRate = fatigueMedium;
            int defenseRoll = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(defending * 5), defenseBonusChance);

            if (defenseRoll >= 20)
            {
                defending *= 2f;
                defenseExcitement = 1;
            }
            else if (defenseRoll >= 10)
            {
                defending *= 1 + (float)(defenseRoll - 9) / 100;
                defenseExcitement = 0;
            }
            else if (defenseRoll <= 1)
            {
                defending *= 0.75f;
                defenseExcitement = -1;
            }

            agilityBonus = (float)GetPlayerAttributeBonus(defender.Agility) / 100;
            agilityBonus *= defender.FatigueModifier();
            fault *= (1f - agilityBonus);

            //Check if tackle resulted in a fault
            if (fault >= Random.Range(0f, 1f))
            {
                if (GetTeamZone(attacker.Team, zone) == Field.Zone.Box) play.Event = MatchEvent.Penalty;
                else play.Event = MatchEvent.Freekick;

                success = false;
            }

            else success |= attacking > defending;

            defender.MatchStats.Tackles++;
        }

        else
        {
            float difficulty = Random.Range(0f, 1f);
            float bonus = (float)attacker.GetOverall() / 100;
            if (bonus > 0) difficulty -= bonus;

            success |= attacking > difficulty;
        }

        attacker.Fatigue -= attFatigueRate * (25 / (float)attacker.Stamina);

        if (defender == null) play.DefensiveAction = PlayerData.PlayerAction.None;
        else defender.Fatigue -= defFatigueRate * (25 / (float)defender.Stamina);

        if (success) play.Excitment = attackExcitment;
        else play.Excitment = defenseExcitement;

        return success;
    }

    #endregion

    #region GetPlayers

    PlayerData GetAttackingPlayer(TeamData _team, Field.Zone _zone, PlayerData _excludePlayer = null, bool _forcePlayer = false)
    {
        Field.Zone zone = GetTeamZone(_team, _zone);

        float chance = 0f;
        bool forcePlayer = _forcePlayer;

        List<PlayerData> players = new List<PlayerData>();

        foreach (PlayerData player in _team.Squad)
        {
            chance = Field.CalculatePresence(player, zone, _team.Strategy);

            if (forcePlayer)
            {
                if (chance > 0f) players.Add(player);
            }
            else
            {
                if (chance >= 1f) players.Add(player);
                else if (chance > 0 && chance >= Random.Range(0f, 1f)) players.Add(player);
            }
        }

        if (_excludePlayer != null && players.Contains(_excludePlayer)) players.Remove(_excludePlayer);

        return GetActivePlayer(players);
    }

    PlayerData GetBestPlayerInArea(TeamData _team, Field.Zone _zone, PlayerData.AttributeType _attribute)
    {
        Field.Zone zone = GetTeamZone(_team, _zone);

        float chance = 0f;


        List<PlayerData> players = new List<PlayerData>();

        foreach (PlayerData player in _team.Squad)
        {
            chance = Field.CalculatePresence(player, zone, _team.Strategy);
            if (chance > 0f) players.Add(player);
              
        }

        return _team.GetTopPlayerByAttribute(_attribute, players.ToArray());
    }

    PlayerData GetDefendingPlayer(TeamData _team, Field.Zone _zone, PlayerData _excludePlayer = null)
    {
        Field.Zone zone = GetTeamZone(_team, _zone);

        float chance = 0f;

        List<PlayerData> players = new List<PlayerData>();
        foreach (PlayerData player in _team.Squad)
        {
            chance = Field.CalculatePresence(player, zone, _team.Strategy);
            if (counterAttack > 0)
            {
                chance *= 0.5f;
            }
            if (chance >= 1f)
            {
                players.Add(player);
            }
            else
            {
                if (chance > 0 && chance >= Random.Range(0f, 1f))
                {
                    players.Add(player);
                }
            }
        }

        if (_excludePlayer != null && players.Contains(_excludePlayer)) players.Remove(_excludePlayer);

        return GetActivePlayer(players);
    }

    PlayerData GetActivePlayer(List<PlayerData> _list)
    {
        PlayerData activePlayer = null;
        List<KeyValuePair<PlayerData, float>> compareList = new List<KeyValuePair<PlayerData, float>>();
        int bonus = 0;
        float total = 0f;

        foreach (PlayerData player in _list)
        {
            float stats = (float)(player.Speed + player.Vision) / 200;
            stats *= player.FatigueModifier();
            bonus = GetPlayerAttributeBonus((player.Vision + player.Speed) / 2);
            if (player.IsWronglyAssigned()) stats *= positionDebuff;

            int r = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(stats * 5) + bonus / 10);

            if (r >= 20) stats *= 1.5f;
            else if (r <= 1) stats *= 0.75f;

            total += stats;
            compareList.Add(new KeyValuePair<PlayerData, float>(player, stats));
        }

        float random = Random.Range(0f, 1f);
        float cumulative = 0f;

        for (int i = 0; i < compareList.Count; i++)
        {
            float value = compareList[i].Value / total;

            cumulative += value;
            if (random <= cumulative)
            {
                activePlayer = compareList[i].Key;
                break;
            }
        }

        if (activePlayer != null) activePlayer.MatchStats.Presence++;
        return activePlayer;
    }

    int GetPlayerAttributeBonus(int _attribute)
    {
        int bonus = 0;
        if (_attribute > 70)
        {
            bonus = _attribute - 70;
        }

        return bonus;
    }

    #endregion

    MarkingType GetMarkingType(PlayerData _defender, Field.Zone _zone)
    {
        MarkingType type = MarkingType.None;

        float totalChance = 0f;
        totalChance = _defender.Prob_Marking;

        totalChance += (float)GetPlayerAttributeBonus(_defender.Speed) / 100;
        totalChance += (float)GetPlayerAttributeBonus(_defender.Vision) / 100;

        Field.Zone zone = GetTeamZone(_defender.Team, _zone);

        if (_defender.Team.IsStrategyApplicable(zone))
        {
            totalChance *= _defender.Team.GetStrategy().MarkingChance;
        }

        float r = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(totalChance));

        if (r >= 20)
        {
            type = MarkingType.Steal;
            _defender.Fatigue -= fatigueHigh * (25 / (float)_defender.Stamina);
        }
        else if (r > 10)
        {
            type = MarkingType.Close;
        }
        else if (r > 3)
        {
            type = MarkingType.Distance;
        }

        return type;
    }

    PlayerData.PlayerAction GetOffensiveAction(MarkingType _marking, PlayerData _player, Field.Zone _zone, bool _headerAvailable)
    {
        Field.Zone zone = GetTeamZone(_player.Team, _zone);
        float bonus = 0;

        ActionChancePerZone zoneChance = actionChancePerZone.actionChancePerZones[(int)zone];

        float pass = _player.GetActionChance(PlayerData.PlayerAction.Pass, zoneChance, _marking, zone);
        float longPass = _player.GetActionChance(PlayerData.PlayerAction.LongPass, zoneChance, _marking, zone);
        float dribble = _player.GetActionChance(PlayerData.PlayerAction.Dribble, zoneChance, _marking, zone);
        float cross = _player.GetActionChance(PlayerData.PlayerAction.Cross, zoneChance, _marking, zone);
        float shoot = _player.GetActionChance(PlayerData.PlayerAction.Shot, zoneChance, _marking, zone);

        float header = 0f;
        if (_headerAvailable)
        {
            header = _player.GetActionChance(PlayerData.PlayerAction.Header, zoneChance, _marking, zone);
        }

        if (_player.Zone == Field.Zone.OwnGoal)
        {
            dribble = 0;
            shoot = 0;
            header = 0;
        }

        float sprint = 0f;
        if (_marking == MarkingType.None)
        {
            sprint = dribble * 1.5f; ;
            dribble = 0f;

            bonus = GetPlayerAttributeBonus(_player.Speed);
            if (Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt((sprint * 5) + (bonus / 10))) >= 20) sprint *= 2f;
        }

        float total = pass + longPass + dribble + cross + shoot + header + sprint;
        pass /= total;
        longPass /= total;
        dribble /= total;
        cross /= total;
        shoot /= total;
        header /= total;
        sprint /= total;

        List<KeyValuePair<PlayerData.PlayerAction, float>> list = new List<KeyValuePair<PlayerData.PlayerAction, float>>
        {
            new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Pass, pass),
            new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.LongPass, longPass),
            new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Dribble, dribble),
            new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Cross, cross),
            new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Shot, shoot),
            new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Header, header),
            new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Sprint, sprint)
        };

        float random = Random.Range(0.00001f, 1f);
        float cumulative = 0f;
        PlayerData.PlayerAction action = PlayerData.PlayerAction.None;
        for (int i = 0; i < list.Count; i++)
        {
            cumulative += list[i].Value;
            if (random < cumulative)
            {
                action = list[i].Key;
                break;
            }
        }

        return action;
    }

    PlayerData.PlayerAction GetFreeKickAction(PlayerData _player, Field.Zone _zone)
    {
        PlayerData.PlayerAction action = PlayerData.PlayerAction.Pass;
        MarkingType marking = MarkingType.None;
        Field.Zone zone = GetTeamZone(_player.Team, _zone);
        ActionChancePerZone zoneChance = actionChancePerZone.actionChancePerZones[(int)zone];

        float pass = _player.GetActionChance(PlayerData.PlayerAction.Pass, zoneChance, marking, zone);
        float longPass = _player.GetActionChance(PlayerData.PlayerAction.LongPass, zoneChance, marking, zone);
        float cross = _player.GetActionChance(PlayerData.PlayerAction.Cross, zoneChance, marking, zone);
        float shoot = _player.GetActionChance(PlayerData.PlayerAction.Shot, zoneChance, marking, zone);

        if (_player.Team.IsStrategyApplicable(zone))
        {
            Team_Strategy teamStrategy = MainController.Instance.TeamStrategyData.team_Strategys[(int)_player.Team.Strategy];
            pass *= teamStrategy.PassingChance;
            cross *= teamStrategy.CrossingChance;
            shoot *= teamStrategy.ShootingChance;
            longPass *= teamStrategy.LongPassChance;
        }

        float total = pass + longPass + cross + shoot;
        pass = pass / total;
        longPass /= total;
        cross = cross / total;
        shoot = shoot / total;

        List<KeyValuePair<PlayerData.PlayerAction, float>> list = new List<KeyValuePair<PlayerData.PlayerAction, float>>
        {
            new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Pass, pass),
            new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.LongPass, longPass),
            new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Cross, cross),
            new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Shot, shoot)
        };

        float random = Random.Range(0f, 1f);
        float cumulative = 0f;
        for (int i = 0; i < list.Count; i++)
        {
            cumulative += list[i].Value;
            if (random < cumulative)
            {
                action = list[i].Key;
                break;
            }
        }

        return action;
    }

    Field.Zone GetTeamZone(TeamData _team, Field.Zone _zone)
    {
        Field.Zone zone = _zone;
        if (_team == awayTeam)
        {
            zone = Field.GetAwayTeamZone(_zone);
        }
        return zone;
    }

    bool CheckCounterAttack(TeamData _defendingTeam, Field.Zone _zone)
    {
        if (counterAttack > 0) counterAttack = 0;
        counterAttackChance = MainController.Instance.Modifiers.game_Modifiers[0].CounterAttackChance;

        counterAttackChance *= MainController.Instance.TeamStrategyData.team_Strategys[(int)_defendingTeam.Strategy].CounterAttackChance;
        float counterRoll = Random.Range(0, 1f);

        if ((int)_zone < 17 && counterAttackChance > counterRoll)
        {
            counterAttack = 4;
            return true;
        }

        return false;
    }

    void SwitchPossesion()
    {
        if (attackingTeam == homeTeam)
        {
            attackingTeam = awayTeam;
            defendingTeam = homeTeam;
        }
        else if (attackingTeam == awayTeam)
        {
            defendingTeam = awayTeam;
            attackingTeam = homeTeam;
        }
    }

    void CopyPlay(PlayInfo _play, PlayInfo _original)
    {
        _play.Attacker = _original.Attacker;
        _play.AttackerRoll = _original.AttackerRoll;
        _play.OffensiveAction = _original.OffensiveAction;
        _play.Defender = _original.Defender;
        _play.DefenderRoll = _original.DefenderRoll;
        _play.DefensiveAction = _original.DefensiveAction;
        _play.Assister = _original.Assister;
        _play.Event = _original.Event;
        _play.Excitment = _original.Excitment;
        _play.Marking = _original.Marking;
        _play.IsActionSuccesful = _original.IsActionSuccesful;
    }

    void UpdateNarration(int _turn)
    {
        PlayInfo play = playList[_turn];
        string attacker = "None";
        Color color = Color.gray;
        if (play.Attacker != null)
        {
            attacker = play.Attacker.FirstName;
            color = play.Attacker.Team.PrimaryColor;
        }
        else if (play.Defender != null)
        {
            color = play.Defender.Team.PrimaryColor;
        }
        string defender = "None";
        if (play.Defender != null) defender = play.Defender.FirstName;
        string action = play.OffensiveAction.ToString();
        string evt = play.Event.ToString();
        string success = play.IsActionSuccesful.ToString();
        string zone = play.Zone.ToString();
        string target = play.TargetZone.ToString();

        //print(_turn + " - ZONE: " + zone + "  ATTACKER: " + attacker + "  DEFENDER: " + defender + "  ACTION: " + action + "  EVENT: " + evt + "  SUCCESS: " + success + "  TARGET: " + target);

        string str = "ATTACKER: " + attacker + " - Actions: " + action + " - Success: " + success + " - Event: " + evt + " - Defender: " + defender;
        screen.Narration.UpdateNarration(str, color, zone);

        screen.Field.UpdateFieldArea((int)play.Zone);
    }

    void Reset()
    {
        turn = 0;
        playList = new List<PlayInfo>();
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