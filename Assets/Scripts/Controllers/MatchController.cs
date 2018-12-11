using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class MatchController : MonoBehaviour
{
    public TeamData HomeTeam;
    public TeamData AwayTeam;

    public MatchScoreView Score;
    public MatchFieldView Field;
    public MatchTeamView HomeTeamSquad;
    public MatchTeamView AwayTeamSquad;
    public MatchNarration Narration;

    //Names given by Home Team perspective
    public enum FieldZone
    { 
        OwnGoal = 0, 
        BLD, BRD,
        LD, LCD, CD, RCD, RD,                   
        LDM, LCDM, CDM, RCDM, RDM,                   
        LM, LCM, CM, RCM,RM,
        LAM, LCAM, CAM, RCAM, RAM,
        LF, LCF, CF, RCF, RF,
        ALF, ARF,
        Box,
    }
    const int totalZones = 31;
    public List<Vector2> FieldMatrix;

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

    enum RollType
    {
        None,
        GetMax,
        DropMin,
    }

    [SerializeField]
    public FieldZone CurrentZone;

    [SerializeField]
    ActionChancePerZoneData actionChancePerZone;

    [SerializeField]
    DebugController debugController;

    PlayerData.PlayerAction defensiveAction = PlayerData.PlayerAction.None;
    PlayerData.PlayerAction offensiveAction = PlayerData.PlayerAction.None;
    PlayerData.PlayerAction lastAction = PlayerData.PlayerAction.None;
    bool lastActionSuccessful;
    MatchEvent matchEvent = MatchEvent.None;

    public TeamData AttackingTeam;
    public TeamData DefendingTeam;
    PlayerData attackingPlayer;
    PlayerData defendingPlayer;
    PlayerData playerWithBall;
    float attackingBonus = 1f;
    bool keepAttacker;
    bool keepDefender;
    MarkingType marking;
    string passer;
    int counterAttack = 0;

    int matchTime = 0;
    int homeTeamScore = 0;
    int awayTeamScore = 0;
    bool isGameOn;

    bool isGoalAnnounced ;
    bool isScorerAnnounced;
    bool isHalfTime;
    bool secondHalfStarted;
    bool isFreekickTaken;
    bool shotMissed;
    bool shotSaved;
    int attackExcitment = 0;
    int defenseExcitement = 0;

    [HideInInspector]
    public string DebugString;

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

    LocalizationData localization;

    [SerializeField]
    TextMeshProUGUI version;

    [Range(1, 100)]
    public int MatchSpeed = 1;

    int totalMatches = 1;
    int matchesPlayed;
    bool isSimulating;

    [SerializeField]
    GameObject startBtn;

    [SerializeField]
    GameObject simulateBtn;

    [SerializeField]
    TMP_InputField totalMatchesInput;

    [System.Serializable]
    public class TeamStrategy
    {
        public TeamData.TeamStrategy Strategy;
        public PosChanceData PosChance;
    }

    public TeamStrategy[] TeamStrategies;

    void Awake()
    {
        Game_Modifier modifiers = MainController.Instance.Modifiers.game_Modifiers[0];

        localization = MainController.Instance.Localization;

        positionDebuff = modifiers.PositionDebuff;
        attackingBonusLow = modifiers.AttackBonusLow;
        attackingBonusMedium = modifiers.AttackBonusMediun;
        attackingBonusHigh = modifiers.AttackBonusHigh;
        faultChance = modifiers.FaultChance;
        offsideChance = modifiers.OffsideChance;
        fatigueLow = modifiers.FatigueLow;
        fatigueMedium = modifiers.FatigueMedium;
        fatigueHigh = modifiers.FatigueHigh;
        fatigueRecoverHalfTime = modifiers.FatigueRecoverHalfTime;
        counterAttackChance = modifiers.CounterAttackChance;

        version.text = "v." + Application.version;
    }

    public void Populate(TeamData _homeTeam, TeamData _awayTeam)
    {
        HomeTeam = _homeTeam;
        AwayTeam = _awayTeam;

        AttackingTeam = HomeTeam;
        DefendingTeam = AwayTeam;

        HomeTeamSquad.Populate(_homeTeam, true);
        AwayTeamSquad.Populate(_awayTeam, true);
        Score.UpdateTime(matchTime);
        Score.UpdateScore(homeTeamScore, awayTeamScore);
        Score.Populate(_homeTeam.Name, homeTeamScore, _homeTeam.PrimaryColor, _awayTeam.Name, awayTeamScore, _awayTeam.PrimaryColor);
    }

    public void UpdateTeams(List<PlayerData> _in, List<PlayerData> _out)
    {
        if(_in.Count > 0)
        {
            string playersIn = "";
            string playersOut = "";
            PlayerData player;
            for (int i = 0; i < _in.Count; i++)
            {
                player = _in[i];
                if(i == 0) playersIn += player.FirstName + " " + player.LastName;
                else playersIn += ", " + player.FirstName + " " + player.LastName;
            }

            for (int i = 0; i < _out.Count; i++)
            {
                player = _out[i];
                if (i == 0) playersOut += player.FirstName + " " + player.LastName;
                else playersOut += ", " + player.FirstName + " " + player.LastName;
            }

            if (matchTime > 0 && matchTime < 90)
            {
                localization.EXTRA_1 = playersOut;
                UpdateNarration("nar_SubsOut_");
                localization.EXTRA_1 = playersIn;
                UpdateNarration("nar_SubsIn_");
            }
        }

        if(matchTime > 0 && matchTime < 90) PauseGame(false);

        HomeTeamSquad.Populate(HomeTeam);
        AwayTeamSquad.Populate(AwayTeam);
    }

    void UpdateNarration(string _text, int _variations = 1, TeamData _team = null)
    {
        if (isSimulating) return;

        if(_team == null) Narration.UpdateNarration(_text, _variations, null, CurrentZone);
        else
        {
            FieldZone zone = GetTeamZone(AttackingTeam);
            Narration.UpdateNarration(_text, _variations,_team, zone);
        }
    }

    void Reset()
    {
        matchTime = 0;
        homeTeamScore = 0;
        awayTeamScore = 0;
        isHalfTime = false;
        secondHalfStarted = false;
        keepAttacker = false;
        keepDefender = false;
        shotMissed = false;
        shotSaved = false;
        isFreekickTaken = false;
        isGoalAnnounced = false;
        isScorerAnnounced = false;
        HomeTeamSquad.ResetFatigue();
        AwayTeamSquad.ResetFatigue();
        matchEvent = MatchEvent.None;
        if(!isSimulating) Narration.Reset();
        Score.UpdateTime(matchTime);
        Score.Populate(HomeTeam.Name, homeTeamScore, HomeTeam.PrimaryColor, AwayTeam.Name, awayTeamScore, AwayTeam.PrimaryColor);
    }

    public void PauseGame(bool _isPaused)
    {
        isGameOn = !_isPaused;
        if (!_isPaused)
        {
            StartCoroutine("GameLoop");
        }
        else StopAllCoroutines();
    }

    public void HandleStartButton()
    {
        if(matchTime == 0 || matchTime >= 90)
        {
            totalMatches = int.Parse(totalMatchesInput.text);
            isSimulating = false;
            Reset();
            KickOff();

            simulateBtn.SetActive(false);
        }
        else
        {
            PauseGame(isGameOn);
        }
    }

    public void HandleSimulateButton()
    {
        Narration.Reset();
        totalMatches = int.Parse(totalMatchesInput.text);
        isSimulating = true;

        startBtn.SetActive(false);
        simulateBtn.SetActive(false);
        totalMatchesInput.interactable = false;

        Reset();
        KickOff();
    }

    public void EndSimulation()
    {
        startBtn.SetActive(true);
        simulateBtn.SetActive(true);
        totalMatchesInput.interactable = true;
        matchesPlayed = 0;
        isSimulating = false;

        Narration.UpdateNarration(totalMatches + " PARTIDAS SIMULADAS.", Color.gray);
    }

    public void KickOff()
    {
        //startBtn.SetActive(false);
        
        CurrentZone = FieldZone.CM;
        Field.UpdateFieldArea((int)CurrentZone);

        isGameOn = true;

        if (!isSimulating)
        {
            UpdateNarration("nar_KickOff_");
            DebugString = "KICK OFF! \n \n";
            StartCoroutine("GameLoop");
        }
        else
        {
            totalMatchesInput.text = totalMatches - matchesPlayed + "";
            StartCoroutine("SimulateLoop");
        }

    }

    private void EndMatch()
    {
        HomeTeam.MatchStats.TotalGoals += homeTeamScore;
        HomeTeam.MatchStats.TotalGoalsAgainst += awayTeamScore;

        AwayTeam.MatchStats.TotalGoals += awayTeamScore;
        AwayTeam.MatchStats.TotalGoalsAgainst += homeTeamScore;

        if (homeTeamScore > awayTeamScore)
        {
            HomeTeam.LifeTimeStats.TotalWins++;
            AwayTeam.LifeTimeStats.TotalLosts++;
        }
        else if (awayTeamScore > homeTeamScore)
        {
            AwayTeam.LifeTimeStats.TotalWins++;
            HomeTeam.LifeTimeStats.TotalLosts++;
        }
        else
        {
            HomeTeam.LifeTimeStats.TotalDraws++;
            AwayTeam.LifeTimeStats.TotalDraws++;
        }

        foreach (PlayerData player in HomeTeam.Squad) player.UpdateLifeTimeStats();
        foreach (PlayerData player in HomeTeam.Substitutes) player.UpdateLifeTimeStats();
        foreach (PlayerData player in AwayTeam.Squad) player.UpdateLifeTimeStats();
        foreach (PlayerData player in AwayTeam.Substitutes) player.UpdateLifeTimeStats();

        HomeTeam.UpdateLifeTimeStats();
        AwayTeam.UpdateLifeTimeStats();

        UpdateNarration("nar_TimeUp_");
        CancelInvoke();
        isGameOn = false;

        if (isSimulating)
        {
            Color color = Color.gray;
            if (awayTeamScore > homeTeamScore) color = AwayTeam.PrimaryColor;
            else if (homeTeamScore > awayTeamScore) color = HomeTeam.PrimaryColor;

            Narration.UpdateNarration(HomeTeam.Name + "  " + homeTeamScore + "  X  " + awayTeamScore + "  " + AwayTeam.Name, color);

            matchesPlayed++;
            if (matchesPlayed == totalMatches)
            {
                EndSimulation();
            }
            else
            {
                Reset();
                KickOff();
            }
        }
        else
        {
            simulateBtn.SetActive(true);
        }
    }

    IEnumerator GameLoop()
    {
        yield return new WaitForSeconds(1f / MatchSpeed);
        while (isGameOn == true)
        {
            DefineActions();
            yield return new WaitForSeconds(1f/MatchSpeed);
        }
    }

    IEnumerator SimulateLoop()
    {
        while(isGameOn)
        {
            DefineActions();
            yield return null;
        }
    }

    //MAIN CONTROLLING FUNCTION
    void DefineActions()
    {
        Field.UpdateFieldArea((int)CurrentZone);

        if(!isSimulating)
        {
            HomeTeamSquad.UpdateFatigue();
            AwayTeamSquad.UpdateFatigue();
        }


        //IF LAST ACTION RESULTED IN A GOAL
        switch (matchEvent)
        {
            case MatchEvent.Goal:
                if (!isGoalAnnounced)
                {
                    isGoalAnnounced = true;
                    UpdateNarration("nar_GoalScream_", 1, AttackingTeam);
                    DebugString += "\n\n<size=40>GOL de " + attackingPlayer.GetFullName() + "</size>\n ________________________________\n \n";
                    if (AttackingTeam == HomeTeam) homeTeamScore++;
                    else awayTeamScore++;
                    if(!isSimulating) Score.UpdateScore(homeTeamScore, awayTeamScore);
                    attackingPlayer.MatchStats.TotalGoals++;
                    return;
                }
                if (!isScorerAnnounced)
                {
                    isScorerAnnounced = true;
                    UpdateNarration("nar_Goal_", 8, AttackingTeam);
                    return;
                }
                else
                {
                    matchEvent = MatchEvent.None;
                    isGoalAnnounced = false;
                    isScorerAnnounced = false;

                    SwitchPossesion();
                    CurrentZone = FieldZone.CM;
                    attackingBonus = 1f;
                    Field.UpdateFieldArea((int)CurrentZone);

                    UpdateNarration("nar_MatchRestart_");
                    return;
                }

            case MatchEvent.Offside:
            case MatchEvent.Freekick:
                if (!isFreekickTaken)
                {
                    if (matchEvent == MatchEvent.Offside) SwitchPossesion();

                    isFreekickTaken = true;

                    FieldZone zone = GetTeamZone(AttackingTeam);

                    if ((int)zone >= (int)FieldZone.LF) attackingPlayer = GetTopPlayerByAttribute(AttackingTeam.Squad, PlayerData.PlayerAttributes.Freekick);
                    attackingPlayer = GetAttackingPlayer(CurrentZone);
                    localization.PLAYER_1 = attackingPlayer.FirstName;
                    offensiveAction = GetFreeKickAction();

                    if (offensiveAction == PlayerData.PlayerAction.Shot)
                    {
                        defendingPlayer = DefendingTeam.Squad[0];
                        UpdateNarration("nar_FreekickTake_", 1, AttackingTeam);
                        attackingPlayer.MatchStats.TotalShots++;
                    }                    
                    else
                    {
                        isFreekickTaken = false;
                        marking = MarkingType.None;
                        matchEvent = MatchEvent.None;
                        ResolveAction();
                    }
                }
                else
                {
                    isFreekickTaken = false;
                    matchEvent = MatchEvent.None;
                    ResolveShot(MarkingType.None);
                }
                return;

            case MatchEvent.Penalty:
                if (!isFreekickTaken)
                {
                    isFreekickTaken = true;
                    attackingPlayer = GetTopPlayerByAttribute(AttackingTeam.Squad, PlayerData.PlayerAttributes.Penalty);
                    localization.PLAYER_1 = attackingPlayer.FirstName;
                    defendingPlayer = DefendingTeam.Squad[0];
                    localization.PLAYER_2 = defendingPlayer.FirstName;
                    offensiveAction = PlayerData.PlayerAction.Shot;
                    UpdateNarration("nar_PenaltyTake_", 1, AttackingTeam);
                    attackingPlayer.MatchStats.TotalShots++;

                    DebugString += "__________________________________________________________________________________________\n\n";
                    DebugString += "<size=30>PENALTY - " + attackingPlayer.GetFullName() + " (" + attackingPlayer.GetOverall() + ")</size>\n\n";
                }
                else
                {
                    attackingBonus *= attackingBonusHigh;
                    matchEvent = MatchEvent.None;
                    ResolveShot(MarkingType.None);
                    isFreekickTaken = false;
                }
                return;

            case MatchEvent.Goalkick:
                attackingPlayer = AttackingTeam.Squad[0];
                localization.PLAYER_1 = attackingPlayer.FirstName;
                marking = MarkingType.None;
                offensiveAction = PlayerData.PlayerAction.Cross;
                ResolveAction();
                break;
        }

        //IF LAST SHOT WAS A MISS
        if (shotMissed)
        {
            if (matchEvent == MatchEvent.Freekick)
            {
                UpdateNarration("nar_WrongFreekick_", 1);
                DebugString += "\n\nChutou na barreira\n\n_____________________________________\n\n";
            }
            else if (matchEvent == MatchEvent.Penalty)
            {
                UpdateNarration("nar_MissedShot_", 2);
                DebugString += "\n\nChutou pra fora\n\n_____________________________________\n\n";
                matchEvent = MatchEvent.Goalkick;
            }
            else if (offensiveAction == PlayerData.PlayerAction.Shot)
            {
                UpdateNarration("nar_WrongShot_", 3);
                matchEvent = MatchEvent.Goalkick;
            }
            else if (offensiveAction == PlayerData.PlayerAction.Header)
            {
                UpdateNarration("nar_WrongHeader_");
                DebugString += "\n\nCabeceou pra fora\n\n_____________________________________\n\n";
                matchEvent = MatchEvent.Goalkick;
            }

            attackingPlayer.MatchStats.TotalShotsMissed++;
            SwitchPossesion();
            shotMissed = false;
            return;
        }
        
        //IF KEEPER CONCEDED A CORNER KICK
        if(matchEvent == MatchEvent.CornerKick)
        {
            if (!isFreekickTaken)
            {
                shotSaved = false;

                offensiveAction = PlayerData.PlayerAction.Cross;
                CurrentZone = FieldZone.LF;

                if (AttackingTeam == AwayTeam)
                {
                    CurrentZone = FieldZone.BLD;
                }

                defendingPlayer = null;
                marking = MarkingType.None;
                attackingPlayer = GetTopPlayerByAttribute(AttackingTeam.Squad, PlayerData.PlayerAttributes.Crossing);

                UpdateNarration("nar_CornerKick_", 1, AttackingTeam);

                isFreekickTaken = true;
            }
            else
            {
                attackingBonus *= attackingBonusMedium;
                CurrentZone = FieldZone.Box;
                attackingPlayer = GetDefendingPlayer(FieldZone.Box);
                defendingPlayer = GetDefendingPlayer(FieldZone.Box);
                if(AttackingTeam == AwayTeam)
                {
                    CurrentZone = FieldZone.OwnGoal;
                    defendingPlayer = GetDefendingPlayer(FieldZone.OwnGoal);
                    attackingPlayer = GetDefendingPlayer(FieldZone.OwnGoal);
                }

                matchEvent = MatchEvent.None;
                ResolveAction();
            }
            return;
        }

        //IF KEEPER SAVED LAST SHOT
        if(shotSaved)
        {
            if (offensiveAction == PlayerData.PlayerAction.Header)
            {
                DebugString += "\n\n" + defendingPlayer.GetFullName() + " defende a cabecada de " + attackingPlayer.GetFullName() + "\n\n_____________________________________\n\n";
                UpdateNarration("nar_SaveHeader_", 1, DefendingTeam);
            }
            else
            {
                if (matchEvent == MatchEvent.Freekick)
                {
                    DebugString += "\n\n" + defendingPlayer.GetFullName() + " defende a cobranca de falta" + "\n\n_____________________________________\n\n";
                    UpdateNarration("nar_SaveFreekick_",1, DefendingTeam);
                    matchEvent = MatchEvent.None;
                    keepDefender = true;
                }
                else if (matchEvent == MatchEvent.Penalty)
                {
                    DebugString += "\n\n" + defendingPlayer.GetFullName() + " defende a cobranca de penalty" + "\n\n_____________________________________\n\n";
                    UpdateNarration("nar_SavePenalty_", 1, DefendingTeam);
                    matchEvent = MatchEvent.None;
                    keepDefender = true;
                }
                else
                {
                    DebugString += "\n\n" + defendingPlayer.GetFullName() + " defende o chute de " + attackingPlayer.FirstName + " " + attackingPlayer.LastName + "\n\n_____________________________________\n\n";
                    if(defenseExcitement == -1) UpdateNarration("nar_WorstSaveShot_", 1, DefendingTeam);
                    else if (defenseExcitement == 0) UpdateNarration("nar_SaveShot_", 1, DefendingTeam);
                    else if (defenseExcitement == 1) UpdateNarration("nar_BestSaveShot_", 1, DefendingTeam);
                    keepDefender = true;
                }
            }

            CurrentZone = FieldZone.OwnGoal;
            if (DefendingTeam == AwayTeam) CurrentZone = GetAwayTeamZone();

            defendingPlayer.MatchStats.TotalSaves++;
            SwitchPossesion();
            keepDefender = true;
            shotSaved = false;
            return;
        }

        //HALF TIME
        if (matchTime >= 45 && !isHalfTime)
        {
            isHalfTime = true;
            UpdateNarration("nar_FirstHalfEnd_");
            CurrentZone = FieldZone.CM;
            AttackingTeam = AwayTeam;
            DefendingTeam = HomeTeam;
            keepAttacker = false;
            keepDefender = false;
            shotMissed = false;
            shotSaved = false;
            if (!isSimulating)
            {
                HomeTeamSquad.ModifyFatigue(fatigueRecoverHalfTime);
                AwayTeamSquad.ModifyFatigue(fatigueRecoverHalfTime);
            }
            matchEvent = MatchEvent.None;
            return;
        }
        if (isHalfTime && !secondHalfStarted)
        {
            secondHalfStarted = true;
            UpdateNarration("nar_SecondHalfStart_");
            DebugString = "SEGUNDO TEMPO\n\n";
            return;
        }
        else if (matchTime >= 90)
        {
            EndMatch();
             
            return;
        }

        matchTime++;
        if(!isSimulating) Score.UpdateTime(matchTime);

        //Step 1: Get players involved in the dispute
        if (!keepAttacker) attackingPlayer = GetAttackingPlayer(CurrentZone);
        if (keepDefender) attackingPlayer = defendingPlayer;
        defendingPlayer = GetDefendingPlayer(CurrentZone);

        keepAttacker = false;
        keepDefender = false;

        //Update global localization strings
        if (attackingPlayer != null) localization.PLAYER_1 = attackingPlayer.FirstName;
        if (defendingPlayer != null) localization.PLAYER_2 = defendingPlayer.FirstName;
        localization.TEAM_1 = AttackingTeam.Name;
        localization.TEAM_2 = DefendingTeam.Name;
        localization.ZONE = localization.GetZoneString(CurrentZone);
        if (AttackingTeam == AwayTeam) localization.ZONE = localization.GetZoneString(GetAwayTeamZone());

        //No players in the dispute
        if (attackingPlayer == null && defendingPlayer == null)
        {
            UpdateNarration("nar_EmptyPlay_", 3);
            DebugString += "\nSOBROU NA " + CurrentZone.ToString() + " ! \n ________________________________\n \n";
        }
        //No players from attacking team in the dispute
        else if(attackingPlayer == null)
        {
            UpdateNarration("nar_SwitchPossession_", 1, AttackingTeam);
            DebugString += "\n________________________________\n" + AttackingTeam.name + " PERDEU A POSSE DE BOLA" + " ! \n ________________________________\n \n";
            keepDefender = true;
            SwitchPossesion();
        }
        //Player from attacking team in the dispute
        else
        {
            //Step 2: Get type of marking
            marking = GetMarkingType();
            if (marking == MarkingType.Steal)
            {
                attackingBonus = 1f;

                DebugString += "\n" + defendingPlayer.FirstName + " ROUBA A BOLA DE " + attackingPlayer.FirstName + "\n ________________________________\n \n";
                UpdateNarration("nar_Steal_", 1, DefendingTeam);
                keepDefender = true;

                DefendingTeam.MatchStats.TotalSteals++;

                SwitchPossesion();
                return;
            }
            else
            {
                if (marking == MarkingType.Close)
                {
                    DebugString += "\n<size=30>" + attackingPlayer.GetFullName() + " VS " + defendingPlayer.GetFullName() + " (" + CurrentZone + ")</size> \n";
                    DebugString += "\nMARCACAO DE PERTO \n \n";
                }
                else if (marking == MarkingType.Distance)
                {
                    DebugString += "\n<size=30>" + attackingPlayer.GetFullName() + " VS " + defendingPlayer.GetFullName() + " (" + CurrentZone + ")</size> \n";
                    DebugString += "\nMARCACAO A DISTANCIA \n \n";
                }
                else
                {
                    //UpdateNarration(attackingPlayer.FirstName + " SOZINHO NA JOGADA", attackingTeam.PrimaryColor);
                    DebugString += "\n<size=30>" + attackingPlayer.GetFullName() + " (" + CurrentZone + ")</size> \n";
                    DebugString += "\nSEM MARCACAO \n\n";
                }

                //Step 3: Get type of offensive play
                offensiveAction = GetOffensiveAction(marking);

                //Step 4: Test action against defender (if there is one)
                ResolveAction();
            }
        }
    }

    void ResolveAction()
    {
        string narration = "";
        int var = 1;
        if (IsActionSuccessful(marking))
        {
            lastActionSuccessful = true;

            //Give bonus based on type of marking
            if (marking == MarkingType.Close) attackingBonus *= attackingBonusHigh;
            else if (marking == MarkingType.Distance) attackingBonus *= attackingBonusMedium;
            else if (marking == MarkingType.None) attackingBonus *= attackingBonusLow;

            switch (offensiveAction)
            {
                case PlayerData.PlayerAction.Pass:
                    DebugString += "PASSOU A BOLA! \n ________________________________\n";

                    localization.PLAYER_1 = passer;
                    localization.PLAYER_2 = attackingPlayer.FirstName;

                    switch(attackExcitment)
                    {
                                            case 0 :
                            narration = "nar_Pass_";
                            break;
                        case 1:
                            narration = "nar_BestPass_";
                            var = 2;
                            break;
                        case -1:
                            narration = "nar_WorstPass_";
                            break;
                    }
                    
                    UpdateNarration(narration, var, AttackingTeam);                   
                    break;

                case PlayerData.PlayerAction.LongPass:
                    DebugString += "PASSE LONGO! \n ________________________________\n";

                    localization.PLAYER_1 = passer;
                    localization.PLAYER_2 = attackingPlayer.FirstName;

                    var = 1;
                    switch (attackExcitment)
                    {
                        case 0:
                            narration = "nar_LongPass_";
                            break;
                        case 1:
                            narration = "nar_BestLongPass_";
                            break;
                        case -1:
                            narration = "nar_WorstLongPass_";
                            break;
                    }

                    AttackingTeam.MatchStats.TotalLongPasses++;
                    UpdateNarration(narration, var, AttackingTeam);
                    break;

                case PlayerData.PlayerAction.Dribble:
                    DebugString += "DRIBLOU! \n ________________________________\n";
                    if (defendingPlayer != null)
                    {
                        switch (attackExcitment)
                        {
                            case 0:
                                narration = "nar_Dribble_";
                                var = 2;
                                break;
                            case 1:
                                narration = "nar_BestDribble_";
                                var = 3;
                                break;
                            case -1:
                                narration = "nar_WorstDribble_";
                                break;
                        }
                        UpdateNarration(narration, var, AttackingTeam);
                    }
                    CurrentZone = GetTargetZone();
                    keepAttacker = true;
                    attackingPlayer.MatchStats.TotalDribbles++;
                    break;

                case PlayerData.PlayerAction.Cross:
                    DebugString += "CRUZOU! \n ________________________________\n";
                    if(matchEvent == MatchEvent.Goalkick)
                    {
                        UpdateNarration("nar_Goalkick_", 1, AttackingTeam);
                        matchEvent = MatchEvent.None;
                    }
                    else
                    {
                        switch (attackExcitment)
                        {
                            case 0:
                                narration = "nar_Cross_";
                                var = 1;
                                break;
                            case 1:
                                narration = "nar_BestCross_";
                                var = 3;
                                break;
                            case -1:
                                narration = "nar_WorstCross_";
                                var = 1;
                                break;
                        }
                        UpdateNarration(narration, var, AttackingTeam);
                    }
                    CurrentZone = GetTargetZone();
                    attackingPlayer.MatchStats.TotalCrosses++;
                    if (GetTeamZone(AttackingTeam) == FieldZone.Box) AttackingTeam.MatchStats.TotalBoxCrosses++;
                    break;

                case PlayerData.PlayerAction.Shot:
                    DebugString += "CHUTOU! \n";
                    UpdateNarration("nar_Shot_", 3, AttackingTeam);
                    ResolveShot(marking);
                    attackingPlayer.MatchStats.TotalShots++;
                    AttackingTeam.MatchStats.TotalShots++;
                    break;

                case PlayerData.PlayerAction.Header:
                    DebugString += "CABECEOU! \n";
                    UpdateNarration("nar_Header_", 1, AttackingTeam);
                    ResolveShot(marking);
                    attackingPlayer.MatchStats.TotalHeaders++;
                    AttackingTeam.MatchStats.TotalHeaders++;
                    break;

                case PlayerData.PlayerAction.Sprint:
                    DebugString += "SPRINTOU!  \n ________________________________\n";
                    UpdateNarration("nar_Sprint_", 1, AttackingTeam);
                    keepAttacker = true;
                    CurrentZone = GetTargetZone();
                    break;
            }
        }
        else
        {
            lastActionSuccessful = false;
            attackingBonus = 1f;
            if (counterAttack > 0) counterAttack--;

            switch (matchEvent)
            {
                case MatchEvent.Freekick:
                    DebugString += "\n\n" + defendingPlayer.FirstName + " " + defendingPlayer.LastName + " faz falta.\n\n";
                    UpdateNarration("nar_Fault_", 5);
                    if (counterAttack > 0) counterAttack = 0;
                    return;

                case MatchEvent.Penalty:
                    DebugString += "\n\n" + defendingPlayer.FirstName + " " + defendingPlayer.LastName + " faz penalty.\n\n";
                    UpdateNarration("nar_Penalty_");
                    if (counterAttack > 0) counterAttack = 0;
                    return;

                case MatchEvent.Offside:
                    DebugString += "\n\n" + attackingPlayer.FirstName + " " + attackingPlayer.LastName + " impedido no lance.\n\n";
                    UpdateNarration("nar_Offside_", 3);
                    if (counterAttack > 0) counterAttack = 0;
                    return;
            }

            switch (offensiveAction)
            {
                case PlayerData.PlayerAction.None:
                    DebugString += "RATIOU FEIO E PERDEU A BOLA! \n ________________________________\n";
                    UpdateNarration("nar_LostPossession_", 4, AttackingTeam);
                    if (counterAttack > 0) counterAttack = 0;
                    break;

                case PlayerData.PlayerAction.LongPass:
                case PlayerData.PlayerAction.Pass:
                    if (defensiveAction == PlayerData.PlayerAction.None)
                    {
                        DebugString += "ERROU O PASSE! \n ________________________________\n";
                        UpdateNarration("nar_WrongPass_", 1);
                        //currentZone = GetTargetZone();
                        attackingPlayer.MatchStats.TotalPassesMissed++;
                        AttackingTeam.MatchStats.TotalPassesMissed++;
                    }
                    else
                    {
                        DebugString += "PASSE BLOQUEADO! \n ________________________________\n";
                        switch (defenseExcitement)
                        {
                            case -1:
                            case 0:
                                narration = "nar_BlockPass_";
                                var = 1;
                                break;
                            case 1:
                                narration = "nar_BestBlockPass_";
                                var = 1;
                                break;
                        }
                        UpdateNarration(narration, var, DefendingTeam);
                        keepDefender = true;
                    }
                    break;

                case PlayerData.PlayerAction.Dribble:
                    if (defensiveAction == PlayerData.PlayerAction.None)
                    {
                        DebugString += "ERROU O DRIBLE! \n ________________________________\n";
                        UpdateNarration("nar_WrongDribble_");
                        CurrentZone = GetTargetZone();
                        attackingPlayer.MatchStats.TotalDribblesMissed++;
                    }
                    else
                    {
                        DebugString += "DRIBLE DESARMADO! \n ________________________________\n";
                        UpdateNarration("nar_BlockDribble_", 1, DefendingTeam);
                        keepDefender = true;
                    }
                    break;

                case PlayerData.PlayerAction.Cross:
                    if (defensiveAction == PlayerData.PlayerAction.None)
                    {
                        if (matchEvent == MatchEvent.Goalkick)
                        {
                            DebugString += "CAGOU O TIRO-DE-META! \n ________________________________\n";
                            UpdateNarration("nar_WrongGoalkick_");
                        }
                        else
                        {
                            DebugString += "ERROU O CRUZAMENTO! \n ________________________________\n";
                            UpdateNarration("nar_WrongCross_");
                            attackingPlayer.MatchStats.TotalCrossesMissed++;
                        }

                        CurrentZone = GetTargetZone();
                    }
                    else
                    {
                        DebugString += "CRUZAMENTO BLOQUEADO! \n ________________________________\n";
                        switch (defenseExcitement)
                        {
                            case 0:
                            case 1:
                                narration = "nar_BlockCross_";
                                var = 1;
                                break;

                            case -1:
                                narration = "nar_WorstBlockCross_";
                                var = 1;
                                break;
                        }
                        UpdateNarration(narration, var, DefendingTeam);
                    }
                    break;

                case PlayerData.PlayerAction.Shot:
                    if (defensiveAction == PlayerData.PlayerAction.None)
                    {
                        DebugString += "ERROU O CHUTE! \n ________________________________\n";
                        UpdateNarration("nar_WrongShot_", 3);
                        attackingPlayer.MatchStats.TotalShotsMissed++;
                    }
                    else
                    {
                        DebugString += "CHUTE BLOQUEADO! \n ________________________________\n";
                        UpdateNarration("nar_BlockShot_", 3, DefendingTeam);
                    }
                    break;

                case PlayerData.PlayerAction.Header:
                    if (defensiveAction == PlayerData.PlayerAction.None)
                    {
                        DebugString += "ERROU A CABECADA! \n ________________________________\n";
                        UpdateNarration("nar_WrongHeader_");
                        attackingPlayer.MatchStats.TotalShotsMissed++;
                    }
                    else
                    {
                        DebugString += "JOGADA AEREA DESARMADA! \n ________________________________\n";
                        UpdateNarration("nar_BlockHeader_", 2, DefendingTeam);
                    }
                    break;

                case PlayerData.PlayerAction.Sprint:
                    DebugString += "CAIU NA CORRIDA! \n";
                    UpdateNarration("nar_WrongSprint_");
                    break;
            }
            SwitchPossesion();
        }
    }

    bool IsActionSuccessful(MarkingType _marking)
    {
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
        attackExcitment = 0;
        defenseExcitement = 0;

        FieldZone zone = CurrentZone;
        if (DefendingTeam == AwayTeam) zone = GetAwayTeamZone();

        if ((int)zone < 8)
        {
            float offside = offsideChance;
            if (defendingPlayer != null)
            {
                offside *= defendingPlayer.Prob_OffsideLine;
               
            }
            if(IsTeamStrategyApplicable(DefendingTeam.Strategy, zone))
            {
                offside *= MainController.Instance.TeamStrategyData.team_Strategys[(int)DefendingTeam.Strategy].OffsideTrickChance;
            }

            if (offside >= Random.Range(0f, 1f))
            {
                matchEvent = MatchEvent.Offside;
                AttackingTeam.MatchStats.TotalOffsides++;
                return false;
            }

        }

        string defenseDebug = "";
        DebugString += "<size=30>" + attackingPlayer.GetFullName() + " (" + attackingPlayer.GetOverall() + ")</size>\n";
        if (defendingPlayer != null) defenseDebug += "<size=30>" + defendingPlayer.GetFullName() + " (" + defendingPlayer.GetOverall() + ")</size>\n";
        switch (offensiveAction)
        {
            case PlayerData.PlayerAction.None: return false;

            case PlayerData.PlayerAction.Pass:
                if (lastAction == PlayerData.PlayerAction.Cross && lastActionSuccessful)
                {
                    attacking = (float)(attackingPlayer.Passing + attackingPlayer.Agility + attackingPlayer.Vision + attackingPlayer.Teamwork + attackingPlayer.Heading) / 500;
                    if (defendingPlayer != null)
                    {
                        defensiveAction = PlayerData.PlayerAction.Block;
                        defending = (float)(defendingPlayer.Blocking + defendingPlayer.Agility + defendingPlayer.Vision + defendingPlayer.Heading) / 400;
                    }
                }
                else
                {
                    attacking = (float)(attackingPlayer.Passing + attackingPlayer.Agility + attackingPlayer.Vision + attackingPlayer.Teamwork) / 400;
                    if (defendingPlayer != null)
                    {
                        defensiveAction = PlayerData.PlayerAction.Block;
                        defending = (float)(defendingPlayer.Blocking + defendingPlayer.Agility + defendingPlayer.Vision) / 300;
                    }
                }

                DebugString += "PASS " + attackingPlayer.Passing;
                DebugString += "  |  AGILITY " + attackingPlayer.Agility;
                DebugString += "  |  VISION " + attackingPlayer.Vision;
                DebugString += "  |  TEAMWORK " + attackingPlayer.Teamwork;
                DebugString += "\nATTACKING: " + attacking + "\n\n";

                attackBonusChance = GetAttributeBonus(attackingPlayer.Passing);

                attFatigueRate = fatigueLow;
                if (_marking == MarkingType.Close)
                {
                    attacking *= 0.9f;
                    DebugString += "<color=#ff0000>- Close Marking (-10%): </color>" + attacking + "\n";
                }

                if (defendingPlayer != null)
                {
                    defenseBonusChance = GetAttributeBonus(defendingPlayer.Blocking);
                    defenseDebug += "BLOCK " + defendingPlayer.Blocking;
                    defenseDebug += "  |  AGILITY " + defendingPlayer.Agility;
                    defenseDebug += "  |  VISION " + defendingPlayer.Vision;
                    defenseDebug += "\nDEFENDING: " + defending + "\n\n";
                }
                break;

            case PlayerData.PlayerAction.LongPass:
                attacking = (float)(attackingPlayer.Passing + attackingPlayer.Agility + attackingPlayer.Vision + attackingPlayer.Teamwork + attackingPlayer.Strength) / 500;
                if (defendingPlayer != null)
                {
                    defensiveAction = PlayerData.PlayerAction.Block;
                    defending = (float)(defendingPlayer.Blocking + defendingPlayer.Agility + defendingPlayer.Vision) / 300;
                }
                
                DebugString += "PASS " + attackingPlayer.Passing;
                DebugString += "  |  AGILITY " + attackingPlayer.Agility;
                DebugString += "  |  VISION " + attackingPlayer.Vision;
                DebugString += "  |  TEAMWORK " + attackingPlayer.Teamwork;
                DebugString += "  |  STRENGTH " + attackingPlayer.Strength;
                DebugString += "\nATTACKING: " + attacking + "\n\n";

                attackBonusChance = GetAttributeBonus(attackingPlayer.Passing);

                attFatigueRate = fatigueMedium;
                if (_marking == MarkingType.Close)
                {
                    attacking *= 0.9f;
                    DebugString += "<color=#ff0000>- Close Marking (-10%): </color>" + attacking + "\n";
                }

                if (defendingPlayer != null)
                {
                    defenseBonusChance = GetAttributeBonus(defendingPlayer.Blocking);
                    defenseDebug += "BLOCK " + defendingPlayer.Blocking;
                    defenseDebug += "  |  AGILITY " + defendingPlayer.Agility;
                    defenseDebug += "  |  VISION " + defendingPlayer.Vision;
                    defenseDebug += "\nDEFENDING: " + defending + "\n\n";
                }
                break;

            case PlayerData.PlayerAction.Dribble:
                if (defendingPlayer != null)
                {
                    defensiveAction = PlayerData.PlayerAction.Tackle;
                    defending = (float)(defendingPlayer.Tackling + defendingPlayer.Agility + defendingPlayer.Speed) / 300;
                    defenseBonusChance = GetAttributeBonus(defendingPlayer.Tackling);
                }

                attacking = (float)(attackingPlayer.Dribbling + attackingPlayer.Agility + attackingPlayer.Speed) / 300;
                attackBonusChance = GetAttributeBonus(attackingPlayer.Tackling);
                attFatigueRate = fatigueHigh;

                DebugString += "DRIBLING " + attackingPlayer.Dribbling;
                DebugString += "  |  AGILITY " + attackingPlayer.Agility;
                DebugString += "  |  SPEED " + attackingPlayer.Speed;
                DebugString += "\nATTACKING: " + attacking + "\n\n";

                if (_marking == MarkingType.Close)
                {
                    attacking = attacking * 0.75f;
                    DebugString += "<color=#ff0000>- Close Marking (-25%): </color>" + attacking + "\n";
                }

                if (defendingPlayer != null)
                {
                    defenseDebug += "TACKLING " + defendingPlayer.Tackling;
                    defenseDebug += "  |  AGILITY " + defendingPlayer.Agility;
                    defenseDebug += "  |  SPEED " + defendingPlayer.Speed;
                    defenseDebug += "\nDEFENDING: " + defending + "\n\n";
                }
                break;

            case PlayerData.PlayerAction.Sprint:
                attacking = (float)(attackingPlayer.Agility + attackingPlayer.Speed) / 200;
                attackBonusChance = 100;
                attacking *= 2f;
                attFatigueRate = fatigueMedium;

                DebugString += "AGILITY " + attackingPlayer.Agility;
                DebugString += "  |  SPEED " + attackingPlayer.Speed;
                DebugString += "\nATTACKING: " + attacking + "   (2x Bonus Sprint)\n\n";
                break;

            case PlayerData.PlayerAction.Cross:
                if (defendingPlayer != null)
                {
                    defensiveAction = PlayerData.PlayerAction.Block;
                    defending = (float)(defendingPlayer.Blocking + defendingPlayer.Agility + defendingPlayer.Vision) / 300;
                    defenseBonusChance = GetAttributeBonus(defendingPlayer.Blocking);
                }

                attacking = (float)(attackingPlayer.Crossing + attackingPlayer.Agility + attackingPlayer.Vision + attackingPlayer.Teamwork) / 400;
                attackBonusChance = GetAttributeBonus(attackingPlayer.Crossing);
                attFatigueRate = fatigueLow;

                DebugString += "CROSS " + attackingPlayer.Crossing;
                DebugString += "  |  AGILITY " + attackingPlayer.Agility;
                DebugString += "  |  VISION " + attackingPlayer.Vision;
                DebugString += "  |  TEAMWORK " + attackingPlayer.Teamwork;
                DebugString += "\nATTACKING: " + attacking + "\n\n";

                if (_marking == MarkingType.Close)
                {
                    attacking = attacking * 0.75f;
                    DebugString += "<color=#ff0000>- Close Marking (-25%): </color>" + attacking + "\n";
                }

                if (defendingPlayer != null)
                {
                    defenseDebug += "BLOCK " + defendingPlayer.Blocking;
                    defenseDebug += "  |  AGILITY " + defendingPlayer.Vision;
                    defenseDebug += "\nDEFENDING: " + defending + "\n\n";
                }
                break;

            case PlayerData.PlayerAction.Shot:
                if (defendingPlayer != null)
                {
                    defensiveAction = PlayerData.PlayerAction.Block;
                    defending = (float)(defendingPlayer.Blocking + defendingPlayer.Agility + defendingPlayer.Vision + defendingPlayer.Speed) / 400;
                    defenseBonusChance = GetAttributeBonus(defendingPlayer.Blocking);
                }

                attacking = (float)(attackingPlayer.Shooting + attackingPlayer.Agility + attackingPlayer.Strength) / 300;
                attackBonusChance = GetAttributeBonus(attackingPlayer.Shooting);
                attFatigueRate = fatigueLow;

                DebugString += "SHOOTING " + attackingPlayer.Shooting;
                DebugString += "  |  AGILITY " + attackingPlayer.Agility;
                DebugString += "  |  STRENGTH " + attackingPlayer.Strength;
                DebugString += "\nATTACKING: " + attacking + "\n\n";

                if (_marking == MarkingType.Close)
                {
                    attacking = attacking * 0.75f;
                    DebugString += "<color=#ff0000>- Close Marking (-25%): </color>" + attacking + "\n";
                }

                if (defendingPlayer != null)
                {
                    defenseDebug += "BLOCK " + defendingPlayer.Blocking;
                    defenseDebug += "  |  AGILITY " + defendingPlayer.Agility;
                    defenseDebug += "  |  VISION " + defendingPlayer.Vision;
                    defenseDebug += "  |  SPEED " + defendingPlayer.Speed;
                    defenseDebug += "\nDEFENDING: " + defending + "\n\n";
                }
                break;

            case PlayerData.PlayerAction.Header:
                if (defendingPlayer != null)
                {
                    defensiveAction = PlayerData.PlayerAction.Block;
                    defending = (float)(defendingPlayer.Heading + defendingPlayer.Blocking + defendingPlayer.Agility + defendingPlayer.Vision) / 400;
                    defenseBonusChance = GetAttributeBonus(defendingPlayer.Blocking);
                }

                attacking = (float)(attackingPlayer.Heading + attackingPlayer.Agility + attackingPlayer.Strength) / 300;
                attackBonusChance = GetAttributeBonus(attackingPlayer.Heading);
                attFatigueRate = fatigueMedium;

                DebugString += "HEADING " + attackingPlayer.Heading;
                DebugString += "  |  AGILITY " + attackingPlayer.Agility;
                DebugString += "  |  STRENGTH " + attackingPlayer.Strength;
                DebugString += "\nATTACKING: " + attacking + "\n\n";

                if (_marking == MarkingType.Close)
                {
                    attacking = attacking * 0.75f;
                    DebugString += "<color=#ff0000>- Close Marking (-25%): </color>" + attacking + "\n";
                }

                if (defendingPlayer != null)
                {
                    defenseDebug += "HEADING " + defendingPlayer.Heading;
                    defenseDebug += "  |  BLOCK " + defendingPlayer.Blocking;
                    defenseDebug += "  |  AGILITY " + defendingPlayer.Agility;
                    defenseDebug += "  |  VISION " + defendingPlayer.Vision;
                    defenseDebug += "\nDEFENDING: " + defending + "\n\n";
                }
                break;
        }

        if (defendingPlayer != null && defendingPlayer.Zone == FieldZone.OwnGoal)
        {
            defending = (float)(defendingPlayer.Goalkeeping + defendingPlayer.Agility + defendingPlayer.Vision + defendingPlayer.Speed) / 400;
            defenseBonusChance = GetAttributeBonus(defendingPlayer.Goalkeeping);

            defenseDebug = "";
            defenseDebug += "GOALKEEPING " + defendingPlayer.Goalkeeping;
            defenseDebug += "  |  AGILITY " + defendingPlayer.Agility;
            defenseDebug += "  |  VISION " + defendingPlayer.Vision;
            defenseDebug += "  |  SPEED " + defendingPlayer.Speed;
            defenseDebug += "\nDEFENDING: " + defending + "\n\n";
        }

        attacking *= FatigueModifier(attackingPlayer.Fatigue);
        DebugString += "<color=#ff0000>- Fatigue: </color>" + attacking + "\n";

        if (attackingPlayer.IsWronglyAssigned())
        {
            attacking *= positionDebuff;
            DebugString += "<color=#ff0000>- Position Debuff (-" + ((1 - positionDebuff) * 100) + "%): </color>" + attacking + "\n";
        }

        attacking *= attackingBonus;
        if (attackingBonus > 0) DebugString += "<color=#00ff00>+ Attacking Bonus (" + ((1 - attackingBonus) * 100) + "%): </color>" + attacking;

        int attackRoll = RollDice(20, 1, RollType.None, Mathf.FloorToInt(attacking * 5), attackBonusChance);

        if (attackRoll >= 20)
        {
            attacking *= 2;
            DebugString += "\n<color=#00ff00>+ Bonus (100%): </color>" + attacking + "\n";
            attackExcitment = 1;
        }
        else if (attackRoll >= 10)
        {
            attacking *= 1 + (float)(attackRoll - 9) / 100;
            DebugString += "<color=#00ff00>\n+ Bonus " + (float)(attackRoll - 9) + "%: </color>" + attacking + "\n";
            attackExcitment = 0;
        }
        else if (attackRoll <= 1)
        {
            if (defendingPlayer == null)
            {
                DebugString += "\nAtacante perdeu a bola sozinho\n";
                success = false;
                return success;
            }
            else
            {
                attacking = attacking * 0.75f;
                DebugString += "\n<color=#ff0000>- Debuff (75%): </color>" + attacking + "\n";
                attackExcitment = -1;
            }
        }

        //Check if tackling is really happening  
        if (_marking == MarkingType.None)
        {
            isTackling = false;
            defensiveAction = PlayerData.PlayerAction.None;
        }
        else
        {
            float tackleChance = 0.75f * actionChancePerZone.actionChancePerZones[(int)zone].Tackle * defendingPlayer.Prob_Tackling;
            if (_marking == MarkingType.Close) tackleChance *= 1.25f;

            if (IsTeamStrategyApplicable(DefendingTeam.Strategy, GetTeamZone(DefendingTeam)))
            {
                tackleChance *= MainController.Instance.TeamStrategyData.team_Strategys[(int)DefendingTeam.Strategy].TacklingChance;
            }

            isTackling |= tackleChance >= Random.Range(0f, 1f);
        }

        if (isTackling)
        {
            defending *= FatigueModifier(defendingPlayer.Fatigue);
            defenseDebug += "<color=#ff0000>- Fatigue: </color>" + defending;
            defFatigueRate = fatigueMedium;
            int defenseRoll = RollDice(20, 1, RollType.None, Mathf.FloorToInt(defending * 5), defenseBonusChance);

            if (defenseRoll >= 20)
            {
                defenseDebug += "\n <color=#00ff00>+ Bonus 100%: </color>" + defending + "\n";
                defending *= 2f;
                defenseExcitement = 1;
            }
            else if (defenseRoll >= 10)
            {
                defending *= 1 + (float)(defenseRoll - 9) / 100;
                defenseDebug += "\n <color=#00ff00>+ Bonus " + ((float)defenseRoll - 9) + "%: </color>" + defending + "\n";
                defenseExcitement = 0;
            }
            else if (defenseRoll <= 1)
            {
                defending *= 0.75f;
                defenseDebug += "\n <color=#ff0000>- Debuff (75%): </color>" + defending + "\n";
                defenseExcitement = -1;
            }

            agilityBonus = (float)GetAttributeBonus(defendingPlayer.Agility) / 100;
            agilityBonus *= FatigueModifier(defendingPlayer.Fatigue);
            fault *= (1f - agilityBonus);

            if (fault >= Random.Range(0f, 1f))
            {
                if (AttackingTeam == HomeTeam && CurrentZone == FieldZone.Box)
                {
                    matchEvent = MatchEvent.Penalty;
                }
                else if (AttackingTeam == AwayTeam && CurrentZone == FieldZone.OwnGoal)
                {
                    matchEvent = MatchEvent.Penalty;
                }
                else
                {
                    matchEvent = MatchEvent.Freekick;
                }
                success = false;
                defendingPlayer.MatchStats.TotalFaults++;
                DefendingTeam.MatchStats.TotalFaults++;
            }

            else
            {
                DebugString += "\n\n" + defenseDebug + "\n";
                DebugString += "\nAtacante: " + attacking;
                DebugString += "\nDefensor: " + defending + "\n\n";

                success |= attacking > defending;
            }

            defendingPlayer.MatchStats.TotalTackles++;
        }

        else
        {

            float difficulty = Random.Range(0f, 1f);
            float bonus = (float)attackingPlayer.GetOverall() / 100;
            if(bonus > 0)
            {
                difficulty -= bonus;
                DebugString += "\n\n<color=#00ff00>Difficulty reduced by " + bonus + "</color>\n";
            }
            DebugString += "\n\nAttacking: " + attacking;
            DebugString += "\nDifficulty: " + difficulty + "\n\n";

            success |= attacking > difficulty;
        }

        attackingPlayer.Fatigue -= attFatigueRate * (25 / (float)attackingPlayer.Stamina);

        if (defendingPlayer == null) defensiveAction = PlayerData.PlayerAction.None;
        else defendingPlayer.Fatigue -= defFatigueRate * (25 / (float)defendingPlayer.Stamina);

        //RESOLVE PASS
        if ((offensiveAction == PlayerData.PlayerAction.Pass || offensiveAction == PlayerData.PlayerAction.LongPass) && success)
        {
            CurrentZone = GetTargetZone();
            passer = attackingPlayer.FirstName;
            PlayerData passerData = attackingPlayer;
            PlayerData receiverData = GetAttackingPlayer(CurrentZone, true);

            if (receiverData == null)
            {
                success = false;
                keepAttacker = false;
                attackingPlayer = passerData;
            }
            else
            {
                keepAttacker = true;
                attackingPlayer = receiverData;
                passerData.MatchStats.TotalPasses++;
                AttackingTeam.MatchStats.TotalPasses++;
            }
        }

        //CHECK COUNTER ATTACK
        if(!success)
        {
            if (counterAttack > 0) counterAttack = 0;
            counterAttackChance *= MainController.Instance.TeamStrategyData.team_Strategys[(int)DefendingTeam.Strategy].CounterAttackChance;
            float counterRoll = Random.Range(0, 1f);

            if ((int)zone < 17 && counterAttackChance > counterRoll)
            {
                counterAttack = 4;
                DefendingTeam.MatchStats.TotalCounterAttacks++;
            }
        }

        return success;
    }

    PlayerData GetAttackingPlayer(FieldZone _zone, bool _excludeLastPlayer = false, bool _forcePlayer = false)
    {
        FieldZone zone = GetTeamZone(AttackingTeam);

        float chance = 0f;
        bool forcePlayer = _forcePlayer;
        bool excludeLastPlayer = _excludeLastPlayer;

        if (matchEvent == MatchEvent.Freekick || matchEvent == MatchEvent.Offside || attackingPlayer == null)
        {
            forcePlayer = true;
        }
        else if (offensiveAction == PlayerData.PlayerAction.Pass || offensiveAction == PlayerData.PlayerAction.LongPass)
        {
            forcePlayer = true;
            excludeLastPlayer = true;
        }
        else if (offensiveAction == PlayerData.PlayerAction.Cross)
        {
            excludeLastPlayer = true;
        }

            List<PlayerData> players = new List<PlayerData>();

        foreach (PlayerData player in AttackingTeam.Squad)
        {
            chance = CalculatePresence(player, zone, AttackingTeam.Strategy);
            
            if (forcePlayer)
            {
                if (chance > 0f)
                { 
                    if (excludeLastPlayer)
                    {
                        if(player != attackingPlayer) players.Add(player);
                    }
                    else
                    {
                        players.Add(player);
                    }
                }
            }
            else
            {
                if (chance >= 1f)
                {
                    if (excludeLastPlayer)
                    {
                        if (player != attackingPlayer) players.Add(player);
                    }
                    else
                    {
                        players.Add(player);
                    }
                }
                else
                {
                    if (chance > 0 && chance >= Random.Range(0f, 1f))
                    {
                        if (excludeLastPlayer)
                        {
                            if (player != attackingPlayer) players.Add(player);
                        }
                        else
                        {
                            players.Add(player);
                        }
                    }
                }
            }
        }

        if (matchEvent == MatchEvent.Freekick)
        {
            return GetTopPlayerByAttribute(players.ToArray(), PlayerData.PlayerAttributes.Freekick);
        }

        return GetActivePlayer(players);
    }

    PlayerData GetDefendingPlayer(FieldZone _zone)
    {
        FieldZone zone = _zone;
        if (DefendingTeam == AwayTeam) zone = GetAwayTeamZone();

        float chance = 0f;
        bool forcePlayerOut = false || (offensiveAction == PlayerData.PlayerAction.Dribble && lastActionSuccessful);

        List<PlayerData> players = new List<PlayerData>();
        foreach (PlayerData player in DefendingTeam.Squad)
        {
            chance = CalculatePresence(player, zone, DefendingTeam.Strategy);
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
        if (forcePlayerOut)
        {
            if (players.Contains(defendingPlayer)) players.Remove(defendingPlayer);
        }

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
            stats *= FatigueModifier(player.Fatigue);
            bonus = GetAttributeBonus((player.Vision + player.Speed)/2);
            if (player.IsWronglyAssigned()) stats *= positionDebuff;

            int r = RollDice(20, 1, RollType.None, Mathf.FloorToInt(stats*5) + bonus/10);

            if (r >= 20)
            {
                stats *= 1.5f;
            }
            else if (r <= 1)
            {
                stats *= 0.75f;
            }

            total += stats;
            compareList.Add(new KeyValuePair<PlayerData, float>(player, stats));
        }

        float random = Random.Range(0f, 1f);
        float cumulative = 0f;

        for (int i = 0; i < compareList.Count; i++)
        {
            float value = compareList[i].Value/total;

            cumulative += value;
            if (random <= cumulative)
            {
                activePlayer = compareList[i].Key;
                break;
            }
        }

        if (activePlayer != null) activePlayer.MatchStats.TotalPresence++;
        return activePlayer;
    }

    float CalculatePresence(PlayerData _player, FieldZone _zone, TeamData.TeamStrategy _teamStrategy)
    {
        float chance = _player.GetChancePerZone(_zone, _teamStrategy);

        if (chance < 1f && chance > 0f)
        {
            chance *= (float)(_player.Speed + _player.Vision) / 200;
            chance *=  FatigueModifier(_player.Fatigue);
        }
        return chance;
    }

    MarkingType GetMarkingType()
    {
        MarkingType type = MarkingType.None;
        if (defendingPlayer == null || attackingPlayer.Zone == FieldZone.OwnGoal) return type;

        float totalChance = 0f;
        totalChance = defendingPlayer.Prob_Marking;

        totalChance += (float)GetAttributeBonus(defendingPlayer.Speed)/100;
        totalChance += (float)GetAttributeBonus(defendingPlayer.Vision)/100;

        FieldZone zone = GetTeamZone(DefendingTeam);

        if(IsTeamStrategyApplicable(DefendingTeam.Strategy, zone))
        {
            totalChance *= DefendingTeam.GetStrategy().MarkingChance;
        }

        float r = RollDice(20, 1, RollType.None, Mathf.FloorToInt(totalChance));

        if (r >= 20)
        {
            type = MarkingType.Steal;
            defendingPlayer.Fatigue -= fatigueHigh * (25 / (float)defendingPlayer.Stamina);
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

    PlayerData.PlayerAction GetOffensiveAction(MarkingType _marking)
    {
        FieldZone zone = GetTeamZone(AttackingTeam);
        float bonus = 0;

        ActionChancePerZone zoneChance = actionChancePerZone.actionChancePerZones[(int)zone];
       
        float pass = zoneChance.Pass * attackingPlayer.Prob_Pass;
        bonus = GetAttributeBonus(attackingPlayer.Passing);
        if (_marking == MarkingType.Close) pass *= 2f;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((pass * 5) + (bonus/10))) >= 20) pass *= 2f;

        float longPass = zoneChance.LongPass * attackingPlayer.Prob_LongPass;
        bonus = GetAttributeBonus(Mathf.FloorToInt((float)(attackingPlayer.Passing + attackingPlayer.Strength)/2));
        if (_marking == MarkingType.Close) longPass *= 1.75f;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((longPass * 5) + (bonus / 10))) >= 20) longPass *= 2f;
        if (lastAction == PlayerData.PlayerAction.Cross && lastActionSuccessful) longPass = 0;

        float dribble = zoneChance.Dribble * attackingPlayer.Prob_Dribble;
        bonus = GetAttributeBonus(attackingPlayer.Dribbling);
        if (_marking == MarkingType.Close) dribble *= 0.5f;
        else if (_marking == MarkingType.Distance) dribble *= 1.5f;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((dribble * 5) + (bonus / 10))) >= 20) dribble *= 2f;

        float cross = zoneChance.Cross * attackingPlayer.Prob_Crossing;
        bonus = GetAttributeBonus(attackingPlayer.Crossing);
        if (_marking == MarkingType.Close) cross *= 0.5f;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((cross * 5) + (bonus / 10))) >= 20) cross *= 2f;

        float shoot = zoneChance.Shot * attackingPlayer.Prob_Shoot;
        bonus = GetAttributeBonus(attackingPlayer.Shooting);
        if (_marking == MarkingType.Close) shoot *= 0.5f;
        else if (_marking == MarkingType.None) shoot *= 3f;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((shoot * 5) + (bonus / 10))) >= 20) shoot *= 2f;

        float header = 0f;
        if (offensiveAction == PlayerData.PlayerAction.Cross && zone == FieldZone.Box && lastActionSuccessful)
        {
            header = (zoneChance.Shot + attackingPlayer.Prob_Shoot) * 1.5f;
            bonus = GetAttributeBonus(attackingPlayer.Heading);
            if (_marking == MarkingType.Distance) header *= 2f;
            else if (_marking == MarkingType.None) header *= 3f;
            if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((header * 5) + (bonus / 10))) >= 20) header *= 2f;
        }

        if(IsTeamStrategyApplicable(AttackingTeam.Strategy, zone))
        {
            Team_Strategy teamStrategy = AttackingTeam.GetStrategy();
            pass *= teamStrategy.PassingChance;
            longPass *= teamStrategy.LongPassChance;
            dribble *= teamStrategy.DribblingChance;
            cross *= teamStrategy.CrossingChance;
            shoot *= teamStrategy.ShootingChance;
            header *= teamStrategy.ShootingChance;
        }

        if(attackingPlayer.Zone == FieldZone.OwnGoal)
        {
            dribble = 0;
            shoot = 0;
            header = 0;
        }

        float sprint = 0f;
        if (marking == MarkingType.None)
        {
            sprint = dribble * 1.5f; ;
            dribble = 0f;

            bonus = GetAttributeBonus(attackingPlayer.Speed);
            if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((sprint * 5) + (bonus / 10))) >= 20) sprint *= 2f;
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

        DebugString += "<size=22>Pass: " + (pass * 100).ToString("F2") + "%\n";
        DebugString += "Long Pass: " + (longPass * 100).ToString("F2") + "%\n";
        DebugString += "Dribble: " + (dribble * 100).ToString("F2") + "%\n";
        DebugString += "Cross: " + (cross * 100).ToString("F2") + "%\n";
        DebugString += "Shoot: " + (shoot * 100).ToString("F2") + "%\n";
        DebugString += "Header: " + (header * 100).ToString("F2") + "%</size>\n";
        DebugString += "Sprint: " + (sprint * 100).ToString("F2") + "%</size>\n\n";

        lastAction = offensiveAction;
        return action;
    } 

    int GetAttributeBonus(int _attribute)
    {
        int bonus = 0;
        if(_attribute > 70)
        {
            bonus = _attribute - 70;
        }

        return bonus;
    }

    void ResolveShot(MarkingType _marking)
    {
        float attacking = 0f;
        float defending = 0f;
        float distanceModifier = 1f;
        int bonusChance = 0;
        defendingPlayer = DefendingTeam.Squad[0];
        localization.PLAYER_2 = defendingPlayer.FirstName;
        FieldZone zone = CurrentZone;

        if (attackingPlayer == AwayTeam) CurrentZone = GetAwayTeamZone();


        switch(zone)
        {
            case FieldZone.LAM:
            case FieldZone.RAM:
                distanceModifier = 0.5f;
                break;

            case FieldZone.LCAM:
            case FieldZone.CAM:
            case FieldZone.RCAM:
                distanceModifier = 0.65f;
                break;

            case FieldZone.LF:
            case FieldZone.RF:
                distanceModifier = 0.75f;
                break;

            case FieldZone.ALF:
            case FieldZone.ARF:
                distanceModifier = 0.35f;
                break;

            case FieldZone.LCF:
            case FieldZone.CF:
            case FieldZone.RCF:
                distanceModifier = 0.8f;
                break;
        }

        if (offensiveAction == PlayerData.PlayerAction.Shot)
        {
            
            if (matchEvent == MatchEvent.Freekick)
            {
                attacking = (float)(attackingPlayer.Freekick + attackingPlayer.Strength) / 200;
                bonusChance = GetAttributeBonus(attackingPlayer.Freekick);
            }
            else if (matchEvent == MatchEvent.Penalty)
            {
                attacking = (float)(attackingPlayer.Penalty + attackingPlayer.Strength) / 200;
                bonusChance = GetAttributeBonus(attackingPlayer.Penalty);
            }
            else
            {
                attacking = (float)(attackingPlayer.Shooting + attackingPlayer.Strength) / 200;
                bonusChance = GetAttributeBonus(attackingPlayer.Shooting);
            }
        }
        else if (offensiveAction == PlayerData.PlayerAction.Header)
        {
            attacking = (float)(attackingPlayer.Heading + attackingPlayer.Strength) / 200;
            bonusChance = GetAttributeBonus(attackingPlayer.Heading);
        }

        DebugString += "\n\n<size=30> RESOLVE SHOT </size>\n";
        DebugString += "Attacking: " + attacking + "\n";
        attacking *= FatigueModifier(attackingPlayer.Fatigue);
        DebugString += "\n<color=#ff0000>- Fatigue: </color>" + attacking;
        attacking *= distanceModifier;
        if (distanceModifier < 1) DebugString += "\n<color=#ff0000>- Distance modifier (-" + distanceModifier * 100 + "%):</color>" + attacking;
        if (_marking == MarkingType.Close)
        {
            attacking *= 0.5f;
            DebugString += "\n<color=#ff0000>- Close Marking (-50%): </color>" + attacking;
        }
        attacking *= attackingBonus;
        DebugString += "\n<color=#00ff00>+ Attacking Bonus (" + (1-attackingBonus)*100 + "%): </color>" + attacking + "\n";
       

        int attackRoll = RollDice(20, 1, RollType.None, Mathf.FloorToInt(attacking * 5), bonusChance);

        if (attackRoll >= 20)
        {
            attacking *= 1.5f;
            DebugString += "\n<color=#00ff00>+ Bonus 50%: </color>" + attacking + "\n";
            attackExcitment = 1;
        }
        else if (attackRoll >= 10)
        {
            attacking *= 1 + (float)(attackRoll - 9) / 100;
            DebugString += "<color=#00ff00>\n+ Bonus " + (float)(attackRoll - 9) + "%: </color>" + attacking + "\n";
            attackExcitment = 0;
        }
        else if (attackRoll <= 4)
        {
            shotMissed = true;
            DebugString += "\n\nErrou o chute!\n";
            DebugString += "\n____________________________________________\n\n";
            return;
        }

        defending = ((float)defendingPlayer.Goalkeeping + defendingPlayer.Agility) / 200;
        DebugString += "\n\nKeeper: " + defending + "\n";
        defending *= FatigueModifier(defendingPlayer.Fatigue);
        DebugString += "\n<color=#ff0000>- Fatigue: </color>" + defending;
        float defenseRoll = RollDice(20, 1, RollType.None, Mathf.FloorToInt(defending * 5), GetAttributeBonus(defendingPlayer.Goalkeeping));

        //defendingPlayer.Fatigue -= fatigueLow * (25 / (float)defendingPlayer.Stamina);

        if (defenseRoll >= 20)
        {
            DebugString += "\n <color=#00ff00>+ Bonus 100%: </color>" + defending + "\n";
            defending *= 2f;
            defenseExcitement = 1;            
        }
        else if (defenseRoll >= 10)
        {
            defending *= 1 + (float)(defenseRoll - 9) / 100;
            DebugString += "\n <color=#00ff00>+ Bonus " + ((float)defenseRoll - 9) + "%: </color>" + defending + "\n";
            defenseExcitement = 0;
        }
        else if (defenseRoll <= 1)
        {
            defending *= 0.5f;
            DebugString += "\n <color=#ff0000>- Debuff 50%: </color>" + defending + "\n";
            defenseExcitement = -1;
        }

        DebugString += "\n\nAtacante: " + attacking;
        DebugString += "\nGoleiro: " + defending;
        if (attacking <= defending)
        {
            shotSaved = true;
            if(defenseExcitement == -1) matchEvent = MatchEvent.CornerKick;
            else if (defenseExcitement == 0)
            {
                int roll = RollDice(20);
                if (roll < 5) matchEvent = MatchEvent.CornerKick;
            }
        }
        else
        {
            matchEvent = MatchEvent.Goal;
        }
    }

    PlayerData.PlayerAction GetFreeKickAction()
    {
        PlayerData.PlayerAction action = PlayerData.PlayerAction.Pass;
        FieldZone zone = GetTeamZone(AttackingTeam);
        int bonus = 0;
        ActionChancePerZone zoneChance = actionChancePerZone.actionChancePerZones[(int)zone];

        float pass = zoneChance.Pass * attackingPlayer.Prob_Pass;
        bonus = GetAttributeBonus(attackingPlayer.Passing);
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((pass * 5) + (bonus / 10))) >= 20) pass *= 2f;

        float longPass = zoneChance.LongPass * attackingPlayer.Prob_LongPass;
        bonus = GetAttributeBonus(Mathf.FloorToInt((float)(attackingPlayer.Passing + attackingPlayer.Strength) / 2));
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((longPass * 5) + (bonus / 10))) >= 20) longPass *= 2f;

        float cross = zoneChance.Cross * attackingPlayer.Prob_Crossing;
        bonus = GetAttributeBonus(attackingPlayer.Crossing);
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((cross * 5) + (bonus / 10))) >= 20) cross *= 2f;

        float shoot = zoneChance.Shot * attackingPlayer.Prob_Shoot;
        bonus = GetAttributeBonus(attackingPlayer.Shooting);
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((shoot * 5) + (bonus / 10))) >= 20) shoot *= 2f;

        if (IsTeamStrategyApplicable(AttackingTeam.Strategy, zone))
        {
            Team_Strategy teamStrategy = MainController.Instance.TeamStrategyData.team_Strategys[(int)AttackingTeam.Strategy];
            pass *= teamStrategy.PassingChance;
            cross *= teamStrategy.CrossingChance;
            shoot *= teamStrategy.ShootingChance;
        }

        float total = pass + longPass + cross + shoot;
        pass = pass / total;
        longPass /= total;
        cross = cross / total;
        shoot = shoot / total;

        List<KeyValuePair<PlayerData.PlayerAction, float>> list = new List<KeyValuePair<PlayerData.PlayerAction, float>>();
        list.Add(new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Pass, pass));
        list.Add(new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.LongPass, longPass));
        list.Add(new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Cross, cross));
        list.Add(new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Shot, shoot));

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

        DebugString += "__________________________________________________________________________________________\n\n";
        DebugString += "<size=30>FREE KICK - " + CurrentZone + " - "  + attackingPlayer.GetFullName() + " (" + attackingPlayer.GetOverall() + ")</size>\n\n";
        DebugString += "<size=22>Pass: " + (pass * 100).ToString("F2") + "%\n";
        DebugString += "Long Pass: " + (longPass * 100).ToString("F2") + "%\n";
        DebugString += "Cross: " + (cross * 100).ToString("F2") + "%\n";
        DebugString += "Shoot: " + (shoot * 100).ToString("F2") + "%\n";

        return action;
    }

    int RollDice(int _sides, int _amount = 1, RollType _rollType = RollType.None, int _bonus = 0, int _bonusChance = 100)
    {
        int n = 0;
        int roll;
        List<int> rolls = new List<int>();

        while (n < _amount)
        {
            roll = 1 + Random.Range(0, _sides);
            if (1 + Random.Range(0, 100) < _bonusChance) roll += _bonus;
            rolls.Add(roll);
            n++;
        }

        if (_rollType == RollType.GetMax)
        {
            return rolls.Max();
        }
        else if (_rollType == RollType.DropMin)
        {
            rolls.Remove(rolls.Min());
            roll = 1 + Random.Range(0, _sides);
            if (1 + Random.Range(0, 100) < _bonusChance) roll += _bonus;
            rolls.Add(roll);
            return rolls.Sum();
        }
        else return rolls.Sum();
    }

    //Inverts the field for away team perspective
    FieldZone GetAwayTeamZone()
    {
        int zone = (totalZones - 1) -  (int)CurrentZone;

        return (FieldZone)zone;
    }

    FieldZone GetTeamZone(TeamData _team)
    {
        FieldZone zone = CurrentZone;
        if (_team == AwayTeam) zone = GetAwayTeamZone();
        return zone;
    }

    FieldZone GetTargetZone()
    {
        FieldZone target = CurrentZone;
        FieldZone zone = GetTeamZone(AttackingTeam);
        List<KeyValuePair<FieldZone, float>> list = new List<KeyValuePair<FieldZone, float>>();

        float _OwnGoal = 0;
        float _BLD = 0;
        float _BRD = 0;

        float _LD = 0;
        float _LCD = 0;
        float _CD = 0;
        float _RCD = 0;
        float _RD = 0;

        float _LDM = 0;
        float _LCDM = 0;
        float _CDM = 0;
        float _RCDM = 0;
        float _RDM = 0;

        float _LM = 0;
        float _LCM = 0;
        float _CM = 0;
        float _RCM = 0;
        float _RM = 0;

        float _LAM = 0;
        float _LCAM = 0;
        float _CAM = 0;
        float _RCAM = 0;
        float _RAM = 0;

        float _LF = 0;
        float _LCF = 0;
        float _CF = 0;
        float _RCF = 0;
        float _RF = 0;

        float _ALF = 0;
        float _ARF = 0;
        float _Box = 0;

        if (matchEvent == MatchEvent.Goalkick)
        {
            _LDM = 0.75f;
            _LCDM = 0.75f;
            _CDM = 075f;
            _RCDM = 0.75f;
            _RDM = 0.75f;
            _LM = 1f;
            _LCM = 1f;
            _CM = 1f;
            _RCM = 1f;
            _RM = 1f;
            _LAM = 0.5f;
            _LCAM = 0.5f;
            _CAM = 0.5f;
            _RCAM = 0.5f;
            _RAM = 0.5f;
        }

        else if (offensiveAction == PlayerData.PlayerAction.Pass || offensiveAction == PlayerData.PlayerAction.Dribble || offensiveAction == PlayerData.PlayerAction.Sprint)
        {
            TargetPassPerZone data = MainController.Instance.TargetPassPerZone.targetPassPerZones[(int)zone];

            _OwnGoal = data.OwnGoal;
            _BLD = data.BLD;
            _BRD = data.BRD;

            _LD = data.LD;
            _LCD = data.LCD;
            _CD = data.CD;
            _RCD = data.RCD;
            _RD = data.RD;

            _LDM = data.LDM;
            _LCDM = data.LCDM;
            _CDM = data.CDM;
            _RCDM = data.RCDM;
            _RDM = data.RDM;

            _LM = data.LM;
            _LCM = data.LCM;
            _CM = data.CM;
            _RCM = data.RCM;
            _RM = data.RM;

            _LAM = data.LAM;
            _LCAM = data.LCAM;
            _CAM = data.CAM;
            _RCAM = data.RCAM;
            _RAM = data.RAM;

            _LF = data.LF;
            _LCF = data.LCF;
            _CF = data.CF;
            _RCF = data.RCF;
            _RF = data.RF;

            _ALF = data.ALF;
            _ARF = data.ARF;
            _Box = data.Box;
        }

        else if (offensiveAction == PlayerData.PlayerAction.Cross || offensiveAction == PlayerData.PlayerAction.LongPass)
        {
            TargetCrossPerZone data = MainController.Instance.TargetCrossPerZone.targetCrossPerZones[(int)zone];

            _OwnGoal = data.OwnGoal;
            _BLD = data.BLD;
            _BRD = data.BRD;

            _LD = data.LD;
            _LCD = data.LCD;
            _CD = data.CD;
            _RCD = data.RCD;
            _RD = data.RD;

            _LDM = data.LDM;
            _LCDM = data.LCDM;
            _CDM = data.CDM;
            _RCDM = data.RCDM;
            _RDM = data.RDM;

            _LM = data.LM;
            _LCM = data.LCM;
            _CM = data.CM;
            _RCM = data.RCM;
            _RM = data.RM;

            _LAM = data.LAM;
            _LCAM = data.LCAM;
            _CAM = data.CAM;
            _RCAM = data.RCAM;
            _RAM = data.RAM;

            _LF = data.LF;
            _LCF = data.LCF;
            _CF = data.CF;
            _RCF = data.RCF;
            _RF = data.RF;

            _ALF = data.ALF;
            _ARF = data.ARF;
            _Box = data.Box;
        }


        Team_Strategy strategy = MainController.Instance.TeamStrategyData.team_Strategys[(int)AttackingTeam.Strategy];
        _OwnGoal += strategy.Target_OwnGoal;
        _BLD += strategy.Target_BLD;
        _BRD += strategy.Target_BRD;

        _LD += strategy.Target_LD;
        _LCD += strategy.Target_LCD;
        _CD += strategy.Target_CD;
        _RCD += strategy.Target_RCD;
        _RD += strategy.Target_RD;

        _LDM += strategy.Target_LDM;
        _LCDM += strategy.Target_LCDM;
        _CDM += strategy.Target_CDM;
        _RCDM += strategy.Target_RCDM;
        _RDM += strategy.Target_RDM;

        _LM += strategy.Target_LM;
        _LCM += strategy.Target_LCM;
        _CM += strategy.Target_CM;
        _RCM += strategy.Target_RCM;
        _RM += strategy.Target_RM;

        _LAM += strategy.Target_LAM;
        _LCAM += strategy.Target_LCAM;
        _CAM += strategy.Target_CAM;
        _RCAM += strategy.Target_RCAM;
        _RAM += strategy.Target_RAM;

        _LF += strategy.Target_LF;
        _LCF += strategy.Target_LCF;
        _CF += strategy.Target_CF;
        _RCF += strategy.Target_RCF;
        _RF += strategy.Target_RF;

        _ALF += strategy.Target_ALF;
        _ARF += strategy.Target_ARF;
        _Box += strategy.Target_Box;



        float total = 
            _OwnGoal + _BLD + _BRD + 
            _LD + _LCD + + _CD +_RCD + _RD + 
            _LDM + _LCDM + + _CDM + _RCDM + _RDM + 
            _LM + _LCM + _CM +_RCM + _RM + 
            _LAM + _LCAM + _CAM + _RCAM + _RAM +
            _LF + _LCF + _CF + _RCF + _RF +
            _ALF + _ARF +  _Box;

        _OwnGoal /= total;
        _BLD /= total;
        _BRD /= total;

        _LD /= total;
        _LCD /= total;
        _CD /= total;
        _RCD /= total;
        _RD /= total;

        _LDM /= total;
        _LCDM /= total;
        _CDM /= total;
        _RCDM /= total;
        _RDM /= total;

        _LM /= total;
        _LCM /= total;
        _CM /= total;
        _RCM /= total;
        _RM /= total;

        _LAM /= total;
        _LCAM /= total;
        _CAM /= total;
        _RCAM /= total;
        _RAM /= total;

        _LF /= total;
        _LCF /= total;
        _CF /= total;
        _RCF /= total;
         _RF /= total;

        _ALF /= total;
        _ARF /= total;
        _Box /= total;

        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.OwnGoal, _OwnGoal));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.BLD, _BLD));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.BRD, _BRD));

        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.LD, _LD));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.LCD, _LCD));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.CD, _CD));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.RCD, _RCD));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.RD, _RD));

        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.LDM, _LDM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.LCDM, _LCDM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.CDM, _CDM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.RCDM, _RCDM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.RDM, _RDM));

        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.LM, _LM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.LCM, _LCM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.CM, _CDM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.RCM, _RCM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.RM, _RM));

        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.LAM, _LAM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.LCAM, _LCAM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.CAM, _CAM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.RCAM, _RCAM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.RAM, _RAM));

        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.LF, _LF));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.LCF, _LCF));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.CF, _CF));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.RCF, _RCF));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.RF, _RF));

        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.ALF, _ALF));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.ARF, _ARF));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.Box, _Box));

        float random = Random.Range(0.00001f, 1f);
        float cumulative = 0;
        for (int i = 0; i < list.Count; i++)
        {
            cumulative += list[i].Value;
            if (random < cumulative)
            {
                target = list[i].Key;
                break;
            }
        }

        if (AttackingTeam == AwayTeam) target = (FieldZone)((totalZones - 1) - (int)target);
        return target;
    }

    PlayerData GetTopPlayerByAttribute(PlayerData[] _players, PlayerData.PlayerAttributes _attribute)
    {
        PlayerData best = null;
        int higher = 0;
        foreach(PlayerData player in _players)
        {
            if(player.GetPlayerAttribute(_attribute) > higher) best = player;
        }

        return best;
    }

    bool IsTeamStrategyApplicable(TeamData.TeamStrategy _strategy, FieldZone _zone)
    {
        bool value = false;
        Team_Strategy teamStrategy = MainController.Instance.TeamStrategyData.team_Strategys[(int)_strategy];

        switch (_zone)
        {
            case FieldZone.OwnGoal: value = teamStrategy.OwnGoal; break;
            case FieldZone.BLD: value = teamStrategy.BLD; break;
            case FieldZone.BRD: value = teamStrategy.BRD; break;

            case FieldZone.LD: value = teamStrategy.LD; break;
            case FieldZone.LCD: value = teamStrategy.LCD; break;
            case FieldZone.CD: value = teamStrategy.CD; break;
            case FieldZone.RCD: value = teamStrategy.RCD; break;
            case FieldZone.RD: value = teamStrategy.RD; break;

            case FieldZone.LDM: value = teamStrategy.LDM; break;
            case FieldZone.LCDM: value = teamStrategy.LCDM; break;
            case FieldZone.CDM: value = teamStrategy.CDM; break;
            case FieldZone.RCDM: value = teamStrategy.RCDM; break;
            case FieldZone.RDM: value = teamStrategy.RDM; break;

            case FieldZone.LM: value = teamStrategy.LM; break;
            case FieldZone.LCM: value = teamStrategy.LCM; break;
            case FieldZone.CM: value = teamStrategy.CM; break;
            case FieldZone.RCM: value = teamStrategy.RCM; break;
            case FieldZone.RM: value = teamStrategy.RM; break;

            case FieldZone.LAM: value = teamStrategy.LAM; break;
            case FieldZone.LCAM: value = teamStrategy.LCAM; break;
            case FieldZone.CAM: value = teamStrategy.CAM; break;
            case FieldZone.RCAM: value = teamStrategy.RCAM; break;
            case FieldZone.RAM: value = teamStrategy.RAM; break;

            case FieldZone.LF: value = teamStrategy.LF; break;
            case FieldZone.LCF: value = teamStrategy.LCF; break;
            case FieldZone.CF: value = teamStrategy.CF; break;
            case FieldZone.RCF: value = teamStrategy.RCF; break;
            case FieldZone.RF: value = teamStrategy.RF; break;

            case FieldZone.ALF: value = teamStrategy.ALF; break;
            case FieldZone.ARF: value = teamStrategy.ARF; break;
            case FieldZone.Box: value = teamStrategy.Box; break;
        }
        return value;
    }

    void SwitchPossesion()
    {
        if(AttackingTeam == HomeTeam)
        {
            AttackingTeam = AwayTeam;
            DefendingTeam = HomeTeam;
        }
        else if(AttackingTeam == AwayTeam)
        {
            DefendingTeam = AwayTeam;
            AttackingTeam = HomeTeam;
        }
    }

    public void EditHomeTeam()
    {
        MainController.Instance.EditSquad(HomeTeam);
    }

    public void EditAwayTeam()
    {
        MainController.Instance.EditSquad(AwayTeam);
    }

    float FatigueModifier(float _fatigue)
    {
        float value = 0.5f + (0.5f *(_fatigue/100));
        //float value = 1f;
        return value;
    }
}
