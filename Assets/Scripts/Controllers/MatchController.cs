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
        OwnGoal = 0,               //                       AWAY GOAL 
        LD = 1,                    //               LF         CF        RF
        CD = 2,                    //               LAM        CAM       RAM                                                
        RD = 3,                    //               LM         CM        RM
        LDM = 4,                   //               LDM        CDM       RDM
        CDM = 5,                   //               LD         CD        RD
        RDM = 6,                   //                       HOME GOAL
        LM = 7,                    
        CM = 8,
        RM = 9,
        LAM = 10,
        CAM = 11,
        RAM = 12,
        LF = 13,
        CF = 14,
        RF = 15,
        Box = 16,
    }

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
    }

    public enum MarkingType
    {
        None,
        Distance,
        Close,
        Steal
    }

    private enum RollType
    {
        None,
        GetMax,
        DropMin,
    }

    private int totalZones = 17;

    private PlayerData.PlayerAction defensiveAction = PlayerData.PlayerAction.None;
    private PlayerData.PlayerAction offensiveAction = PlayerData.PlayerAction.None;
    private MatchEvent matchEvent = MatchEvent.None;
    private bool lastActionSuccessful = false;

    [SerializeField]
    private GameObject startBtn;

    [SerializeField]
    private FieldZone currentZone;

    [SerializeField]
    private ActionChancePerZoneData actionChancePerZone;

    [SerializeField]
    private DebugController debugController;

    private TeamData attackingTeam;
    private TeamData defendingTeam;
    private PlayerData attackingPlayer;
    private PlayerData defendingPlayer;
    private PlayerData playerWithBall;
    private float attackingBonus = 1f;
    private bool keepAttacker = false;
    private bool keepDefender= false;
    private MarkingType marking;

    private int matchTime = 0;
    private int homeTeamScore = 0;
    private int awayTeamScore = 0;
    private bool isGameOn = false;

    private bool isGoalAnnounced = false;
    private bool isScorerAnnounced = false;
    private bool isHalfTime = false;
    private bool secondHalfStarted = false;
    private bool isFreekickTaken = false;
    private bool shotMissed = false;
    private bool shotSaved = false;

    [HideInInspector]
    public string DebugString;

    private float positionDebuff;
    private float attackingBonusLow;
    private float attackingBonusMedium;
    private float attackingBonusHigh;
    private float faultChance;
    private float offsideChance;

    private int fatigueLow;
    private int fatigueMedium;
    private int fatigueHigh;
    private float fatigueRecoverHalfTime;

    [SerializeField]
    private TextMeshProUGUI version;

    [SerializeField]
    [Range(1, 100)]
    private int matchSpeed = 1;

    private void Awake()
    {
        Game_Modifier modifiers = MainController.Instance.Modifiers.game_Modifiers[0];

        positionDebuff = modifiers.PositionDebuff;
        attackingBonusLow = modifiers.AttackBonusLow;
        attackingBonusMedium = modifiers.AttackBonusMediun;
        attackingBonusHigh = modifiers.AttackBonusHigh;
        faultChance = modifiers.FaultChance;
        offsideChance = modifiers.OffsideChance;
        fatigueLow = (int)modifiers.FatigueLow;
        fatigueMedium = (int)modifiers.FatigueMedium;
        fatigueHigh = (int)modifiers.FatigueHigh;
        fatigueRecoverHalfTime = modifiers.FatigueRecoverHalfTime;

        version.text = "v." + Application.version;
    }

    public void Populate(TeamData _homeTeam, TeamData _awayTeam)
    {
        HomeTeam = _homeTeam;
        AwayTeam = _awayTeam;

        attackingTeam = HomeTeam;
        defendingTeam = AwayTeam;

        HomeTeamSquad.Populate(_homeTeam, true);
        AwayTeamSquad.Populate(_awayTeam, true);
        Score.UpdateTime(matchTime);
        Score.UpdateScore(
            HomeTeam.Name,
            homeTeamScore,
            ColorUtility.ToHtmlStringRGB(HomeTeam.PrimaryColor), 
            AwayTeam.Name, 
            awayTeamScore, 
            ColorUtility.ToHtmlStringRGB(AwayTeam.PrimaryColor));
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
                Narration.UpdateNarration("SAI: " + playersOut, Color.gray);
                Narration.UpdateNarration("ENTRA: " + playersIn, Color.gray);
            }
        }

        if(matchTime > 0 && matchTime < 90) PauseGame(false);

        HomeTeamSquad.Populate(HomeTeam);
        AwayTeamSquad.Populate(AwayTeam);
    }

    private void Reset()
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
        HomeTeamSquad.ResetFatigue();
        AwayTeamSquad.ResetFatigue();
        matchEvent = MatchEvent.None;
        Narration.Reset();
        Score.UpdateTime(matchTime);
        Score.UpdateScore(HomeTeam.Name, homeTeamScore, ColorUtility.ToHtmlStringRGB(HomeTeam.PrimaryColor), AwayTeam.Name, awayTeamScore, ColorUtility.ToHtmlStringRGB(AwayTeam.PrimaryColor));
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
            Reset();
            KickOff();
        }
        else
        {
            PauseGame(isGameOn);
        }
    }

    public void KickOff()
    {
        //startBtn.SetActive(false);

        Narration.UpdateNarration("KICK OFF!", Color.gray);
        DebugString = "KICK OFF! \n \n";
        currentZone = FieldZone.CM;
        Field.UpdateFieldArea((int)currentZone);

        isGameOn = true;
        StartCoroutine("GameLoop");
    }

    IEnumerator GameLoop()
    {
        yield return new WaitForSeconds(1f / matchSpeed);
        while (isGameOn == true)
        {
            DefineActions();
            yield return new WaitForSeconds(1f/matchSpeed);
        }
    }

    //MAIN CONTROLLING FUNCTION
    private void DefineActions()
    {
        Field.UpdateFieldArea((int)currentZone);
        HomeTeamSquad.UpdateFatigue();
        AwayTeamSquad.UpdateFatigue();

        //IF LAST ACTION RESULTED IN A GOAL
        switch (matchEvent)
        {
            case MatchEvent.Goal:
                if (!isGoalAnnounced)
                {
                    isGoalAnnounced = true;
                    Narration.UpdateNarration("<size=60>GOOOOOOAAAAALLLL!!!", attackingTeam.PrimaryColor);
                    DebugString += "\n\n<size=40>GOL de " + attackingPlayer.GetFullName() + "</size>\n ________________________________\n \n";
                    if (attackingTeam == HomeTeam) homeTeamScore++;
                    else awayTeamScore++;
                    Score.UpdateScore(HomeTeam.Name, homeTeamScore, ColorUtility.ToHtmlStringRGB(HomeTeam.PrimaryColor), AwayTeam.Name, awayTeamScore, ColorUtility.ToHtmlStringRGB(AwayTeam.PrimaryColor));
                    attackingPlayer.TotalGoals++;
                    return;
                }
                if (!isScorerAnnounced)
                {
                    isScorerAnnounced = true;
                    Narration.UpdateNarration(attackingPlayer.GetFullName() + " marca para " + attackingTeam.Name + "!", attackingTeam.PrimaryColor);
                    return;
                }
                else
                {
                    matchEvent = MatchEvent.None;
                    isGoalAnnounced = false;
                    isScorerAnnounced = false;

                    SwitchPossesion();
                    currentZone = FieldZone.CM;
                    attackingBonus = 1f;
                    Field.UpdateFieldArea((int)currentZone);

                    Narration.UpdateNarration("RECOMECA A PARTIDA", Color.gray);
                    return;
                }

            case MatchEvent.Offside:
            case MatchEvent.Freekick:
                if (!isFreekickTaken)
                {
                    isFreekickTaken = true;

                    attackingPlayer = GetAttackingPlayer(currentZone);
                    offensiveAction = GetFreeKickAction();

                    if (offensiveAction == PlayerData.PlayerAction.Shot)
                    {
                        defendingPlayer = defendingTeam.Squad[0];
                        Narration.UpdateNarration(attackingPlayer.GetFullName() + " VAI PARA A COBRANCA...", attackingTeam.PrimaryColor);
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
                    attackingPlayer = GetTopPlayerByAttribute(attackingTeam.Squad, PlayerData.PlayerAttributes.Penalty);
                    defendingPlayer = defendingTeam.Squad[0];
                    offensiveAction = PlayerData.PlayerAction.Shot;
                    Narration.UpdateNarration(attackingPlayer.GetFullName() + " VAI PARA A COBRANCA...", attackingTeam.PrimaryColor);
                }
                else
                {
                    matchEvent = MatchEvent.None;
                    ResolveShot(MarkingType.None);
                    isFreekickTaken = false;
                }
                return;

            case MatchEvent.Goalkick:
                attackingPlayer = attackingTeam.Squad[0];
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
                Narration.UpdateNarration(attackingPlayer.GetFullName() + " chuta na barreira", Color.gray);
                DebugString += "\n\nChutou na barreira\n\n_____________________________________\n\n";
            }
            else if (matchEvent == MatchEvent.Penalty)
            {
                Narration.UpdateNarration(attackingPlayer.GetFullName() + " manda a bola pra fora do estadio!", Color.gray);
                DebugString += "\n\nChutou pra fora\n\n_____________________________________\n\n";
                matchEvent = MatchEvent.Goalkick;
            }
            else if (offensiveAction == PlayerData.PlayerAction.Shot)
            {
                Narration.UpdateNarration(attackingPlayer.GetFullName() + " chuta pra fora!", Color.gray);
                DebugString += "\n\nChutou pra fora\n\n_____________________________________\n\n";
                matchEvent = MatchEvent.Goalkick;
            }
            else if (offensiveAction == PlayerData.PlayerAction.Header)
            {
                Narration.UpdateNarration(attackingPlayer.GetFullName() + " cabeceia por cima do gol!", Color.gray);
                DebugString += "\n\nCabeceou pra fora\n\n_____________________________________\n\n";
                matchEvent = MatchEvent.Goalkick;
            }

            attackingPlayer.TotalShotsMissed++;
            SwitchPossesion();
            shotMissed = false;
            return;
        }

        //IF KEEPER SAVED LAST SHOT
        if(shotSaved)
        {
            if (offensiveAction == PlayerData.PlayerAction.Header)
            {
                DebugString += "\n\n" + defendingPlayer.GetFullName() + " defende a cabecada de " + attackingPlayer.GetFullName() + "\n\n_____________________________________\n\n";
                Narration.UpdateNarration(defendingPlayer.GetFullName() + " defende a cabecada de " + attackingPlayer.GetFullName(), defendingTeam.PrimaryColor);
            }
            else
            {
                if (matchEvent == MatchEvent.Freekick)
                {
                    DebugString += "\n\n" + defendingPlayer.GetFullName() + " defende a cobranca de falta" + "\n\n_____________________________________\n\n";
                    Narration.UpdateNarration(defendingPlayer.GetFullName() + " defende a cobranca de falta!", defendingTeam.PrimaryColor);
                    matchEvent = MatchEvent.None;
                    keepDefender = true;
                }
                else if (matchEvent == MatchEvent.Penalty)
                {
                    DebugString += "\n\n" + defendingPlayer.GetFullName() + " defende a cobranca de penalty" + "\n\n_____________________________________\n\n";
                    Narration.UpdateNarration(defendingPlayer.GetFullName() + " defende a cobranca de penalty!", defendingTeam.PrimaryColor);
                    matchEvent = MatchEvent.None;
                    keepDefender = true;
                }
                else
                {
                    DebugString += "\n\n" + defendingPlayer.GetFullName() + " defende o chute de " + attackingPlayer.FirstName + " " + attackingPlayer.LastName + "\n\n_____________________________________\n\n";
                    Narration.UpdateNarration(defendingPlayer.GetFullName() + " defende o chute de " + attackingPlayer.FirstName + " " + attackingPlayer.LastName, defendingTeam.PrimaryColor);
                    keepDefender = true;
                }
            }

            currentZone = FieldZone.OwnGoal;
            if (defendingTeam == AwayTeam) currentZone = GetAwayTeamZone();

            defendingPlayer.TotalSaves++;
            SwitchPossesion();
            keepDefender = true;
            shotSaved = false;
            return;
        }

        //HALF TIME
        if (matchTime >= 45 && !isHalfTime)
        {
            isHalfTime = true;
            Narration.UpdateNarration("FIM DO PRIMEIRO TEMPO", Color.gray);
            currentZone = FieldZone.CM;
            attackingTeam = AwayTeam;
            defendingTeam = HomeTeam;
            keepAttacker = false;
            keepDefender = false;
            shotMissed = false;
            shotSaved = false;
            HomeTeamSquad.ModifyFatigue(fatigueRecoverHalfTime);
            AwayTeamSquad.ModifyFatigue(fatigueRecoverHalfTime);
            matchEvent = MatchEvent.None;
            return;
        }
        if (isHalfTime && !secondHalfStarted)
        {
            secondHalfStarted = true;
            Narration.UpdateNarration("COMECA SEGUNDO TEMPO", Color.gray);
            DebugString = "SEGUNDO TEMPO\n\n";
            return;
        }
        else if (matchTime >= 90)
        {
            Narration.UpdateNarration("TERMINA A PARTIDA", Color.gray);
            CancelInvoke();
            isGameOn = false;

            HomeTeam.TotalGoals += homeTeamScore;
            HomeTeam.TotalGoalsAgainst += awayTeamScore;

            AwayTeam.TotalGoals += awayTeamScore;
            AwayTeam.TotalGoalsAgainst += homeTeamScore;

            if (homeTeamScore > awayTeamScore)
            {
                HomeTeam.TotalWins++;
                AwayTeam.TotalLosts++;
            }
            else if (awayTeamScore > homeTeamScore)
            {
                AwayTeam.TotalWins++;
                HomeTeam.TotalLosts++;
            }
            else
            {
                HomeTeam.TotalDraws++;
                AwayTeam.TotalDraws++;
            }

            startBtn.SetActive(true);
            return;
        }


        matchTime++;
        Score.UpdateTime(matchTime);


        //Step 1: Get players involved in the dispute
        if (!keepAttacker) attackingPlayer = GetAttackingPlayer(currentZone);
        if (keepDefender) attackingPlayer = defendingPlayer;
        defendingPlayer = GetDefendingPlayer(currentZone);

        keepAttacker = false;
        keepDefender = false;

        //No players in the dispute
        if (attackingPlayer == null && defendingPlayer == null)
        {
            Narration.UpdateNarration("BOLA SOBROU!", Color.gray);
            DebugString += "\nSOBROU NA " + currentZone.ToString() + " ! \n ________________________________\n \n";
        }
        //No players form team in possesion in the dispute
        else if(attackingPlayer == null)
        {
            Narration.UpdateNarration(attackingTeam.Name + " PERDE A POSSE DE BOLA", attackingTeam.PrimaryColor);
            keepDefender = true;
            SwitchPossesion();
        }
        //Player from team in possesion in the dispute
        else
        {
            //Step 2: Get type of marking
            marking = GetMarkingType();
            if (marking == MarkingType.Steal)
            {
                attackingBonus = 1f;

                DebugString += "\nROUBADA DE BOLA! \n ________________________________\n \n";
                Narration.UpdateNarration(defendingPlayer.FirstName + " ROUBA A BOLA DE " + attackingPlayer.FirstName, defendingTeam.PrimaryColor);
                keepDefender = true;

                SwitchPossesion();
                return;
            }
            else
            {
                if (marking == MarkingType.Close)
                {
                    DebugString += "\n<size=28>" + attackingPlayer.GetFullName() + " VS " + defendingPlayer.GetFullName() + " (" + currentZone + ")</size> \n";
                    DebugString += "\nMARCACAO DE PERTO \n \n";
                }
                else if (marking == MarkingType.Distance)
                {
                    DebugString += "\n<size=28>" + attackingPlayer.GetFullName() + " VS " + defendingPlayer.GetFullName() + " (" + currentZone + ")</size> \n";
                    DebugString += "\nMARCACAO A DISTANCIA \n \n";
                }
                else
                {
                    //Narration.UpdateNarration(attackingPlayer.FirstName + " SOZINHO NA JOGADA", attackingTeam.PrimaryColor);
                    DebugString += "\n<size=28>" + attackingPlayer.GetFullName() + " (" + currentZone + ")</size> \n";
                    DebugString += "\nSEM MARCACAO \n\n";
                }

                //Step 3: Get type of offensive play
                offensiveAction = GetOffensiveAction(marking);

                //Step 4: Test action against defender (if there is one)
                ResolveAction();
            }
        }
    }

    private void ResolveAction()
    {
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
                    Narration.UpdateNarration(attackingPlayer.FirstName + " PASSA A BOLA", attackingTeam.PrimaryColor);
                    currentZone = GetTargetZone();
                    attackingPlayer.TotalPasses++;
                    break;

                case PlayerData.PlayerAction.Dribble:
                    DebugString += "DRIBLOU! \n ________________________________\n";
                    if (defendingPlayer != null) Narration.UpdateNarration(attackingPlayer.FirstName + " DRIBLA " + defendingPlayer.FirstName, attackingTeam.PrimaryColor);
                    currentZone = GetTargetZone();
                    keepAttacker = true;
                    attackingPlayer.TotalDribbles++;
                    break;

                case PlayerData.PlayerAction.Cross:
                    DebugString += "CRUZOU! \n ________________________________\n";
                    if(matchEvent == MatchEvent.Goalkick)
                    {
                        Narration.UpdateNarration(attackingPlayer.FirstName + " COBRA O TIRO-DE-META", attackingTeam.PrimaryColor);
                        matchEvent = MatchEvent.None;
                    }
                    else
                    {
                        Narration.UpdateNarration(attackingPlayer.FirstName + " CRUZA A BOLA", attackingTeam.PrimaryColor);
                    }
                    currentZone = GetTargetZone();
                    attackingPlayer.TotalCrosses++;
                    break;

                case PlayerData.PlayerAction.Shot:
                    DebugString += "CHUTOU! \n";
                    Narration.UpdateNarration(attackingPlayer.FirstName + " TENTA O CHUTE...", attackingTeam.PrimaryColor);
                    ResolveShot(marking);
                    attackingPlayer.TotalShots++;
                    break;

                case PlayerData.PlayerAction.Header:
                    DebugString += "CABECEOU! \n";
                    Narration.UpdateNarration(attackingPlayer.FirstName + " TENTA O LANCE DE CABECA...", attackingTeam.PrimaryColor);
                    ResolveShot(marking);
                    attackingPlayer.TotalHeaders++;
                    break;
            }
        }
        else
        {
            lastActionSuccessful = false;
            attackingBonus = 1f;

            if (matchEvent == MatchEvent.Freekick)
            {
                DebugString += "\n\n" + defendingPlayer.FirstName + " " + defendingPlayer.LastName + " faz falta.\n\n";
                Narration.UpdateNarration(defendingPlayer.FirstName + " FAZ FALTA EM " + attackingPlayer.FirstName, Color.gray);
                return;
            }
            else if (matchEvent == MatchEvent.Penalty)
            {
                DebugString += "\n\n" + defendingPlayer.FirstName + " " + defendingPlayer.LastName + " faz penalty.\n\n";
                Narration.UpdateNarration("PENALIDADE MAXIMA!!!", Color.gray);
                return;
            }
            else if (matchEvent == MatchEvent.Offside)
            {
                DebugString += "\n\n" + attackingPlayer.FirstName + " " + attackingPlayer.LastName + " impedido no lance.\n\n";
                Narration.UpdateNarration("IMPEDIMENTO MARCADO", Color.gray);
                return;
            }

            switch (offensiveAction)
            {
                case PlayerData.PlayerAction.None:
                    DebugString += "RATIOU FEIO E PERDEU A BOLA! \n ________________________________\n";
                    Narration.UpdateNarration(attackingPlayer.FirstName + " DEU BOBEIRA E PERDEU A BOLA", attackingTeam.PrimaryColor);
                    break;

                case PlayerData.PlayerAction.Pass:
                    if (defensiveAction == PlayerData.PlayerAction.None)
                    {
                        DebugString += "ERROU O PASSE! \n ________________________________\n";
                        Narration.UpdateNarration(attackingPlayer.FirstName + " ERRA O PASSE", Color.gray);
                        currentZone = GetTargetZone();
                        attackingPlayer.TotalPassesMissed++;
                    }
                    else
                    {
                        DebugString += "PASSE BLOQUEADO! \n ________________________________\n";
                        Narration.UpdateNarration(defendingPlayer.FirstName + " BLOQUEIA O PASSE", defendingTeam.PrimaryColor);
                        keepDefender = true;
                    }
                    break;

                case PlayerData.PlayerAction.Dribble:
                    if (defensiveAction == PlayerData.PlayerAction.None)
                    {
                        DebugString += "ERROU O DRIBLE! \n ________________________________\n";
                        Narration.UpdateNarration(attackingPlayer.FirstName + " SE ATRAPALHA NO DRIBLE", Color.gray);
                        currentZone = GetTargetZone();
                        attackingPlayer.TotalDribblesMissed++;
                    }
                    else
                    {
                        DebugString += "DRIBLE DESARMADO! \n ________________________________\n";
                        Narration.UpdateNarration(defendingPlayer.FirstName + " PARA O DRIBLE DE " + attackingPlayer.FirstName, defendingTeam.PrimaryColor);
                        keepDefender = true;
                    }
                    break;

                case PlayerData.PlayerAction.Cross:
                    if (defensiveAction == PlayerData.PlayerAction.None)
                    {
                        if (matchEvent == MatchEvent.Goalkick)
                        {
                            DebugString += "CAGOU O TIRO-DE-META! \n ________________________________\n";
                            Narration.UpdateNarration(attackingPlayer.FirstName + " FURA O TIRO-DE-META", Color.gray);
                        }
                        else
                        {
                            DebugString += "ERROU O CRUZAMENTO! \n ________________________________\n";
                            Narration.UpdateNarration(attackingPlayer.FirstName + " ERRA O CRUZAMENTO", Color.gray);
                            attackingPlayer.TotalCrossesMissed++;
                        }

                        currentZone = GetTargetZone();
                    }
                    else
                    {
                        DebugString += "CRUZAMENTO BLOQUEADO! \n ________________________________\n";
                        Narration.UpdateNarration(defendingPlayer.FirstName + " IMPEDE O CRUZAMENTO", defendingTeam.PrimaryColor);
                    }
                    break;

                case PlayerData.PlayerAction.Shot:
                    if (defensiveAction == PlayerData.PlayerAction.None)
                    {
                        DebugString += "ERROU O CHUTE! \n ________________________________\n";
                        Narration.UpdateNarration(attackingPlayer.FirstName + " TEM QUE BOTAR O PÉ NA FORMA", Color.gray);
                        attackingPlayer.TotalShotsMissed++;
                    }
                    else
                    {
                        DebugString += "CHUTE BLOQUEADO! \n ________________________________\n";
                        Narration.UpdateNarration(defendingPlayer.FirstName + " BLOQUEIA O CHUTE DE " + attackingPlayer.FirstName, defendingTeam.PrimaryColor);
                    }
                    break;

                case PlayerData.PlayerAction.Header:
                    if (defensiveAction == PlayerData.PlayerAction.None)
                    {
                        DebugString += "ERROU A CABECADA! \n ________________________________\n";
                        Narration.UpdateNarration(attackingPlayer.FirstName + " CABECEIA O AR", Color.gray);
                        attackingPlayer.TotalHeadersMissed++;
                    }
                    else
                    {
                        DebugString += "JOGADA AEREA DESARMADA! \n ________________________________\n";
                        Narration.UpdateNarration(defendingPlayer.FirstName + " PULA MAIS ALTO QUE " + attackingPlayer.FirstName, defendingTeam.PrimaryColor);
                    }
                    break;
            }
            SwitchPossesion();
        }
    }

    private PlayerData GetAttackingPlayer(FieldZone _zone)
    {
        FieldZone zone = _zone;
        if (attackingTeam == AwayTeam) zone = GetAwayTeamZone();

        float chance = 0f;
        bool forcePlayer = false;
        bool excludeLastPlayer = false;

        if (matchEvent == MatchEvent.Freekick || matchEvent == MatchEvent.Offside || attackingPlayer == null)
        {
            forcePlayer = true;
        }
        else if (offensiveAction == PlayerData.PlayerAction.Pass || offensiveAction == PlayerData.PlayerAction.Cross)
        {
            if (lastActionSuccessful)
            {
                forcePlayer = true;
                excludeLastPlayer = true;
            }
        }

        List<PlayerData> players = new List<PlayerData>();

        foreach (PlayerData player in attackingTeam.Squad)
        {
            chance = CalculatePresence(player, zone);
            
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
                    players.Add(player);
                }
                else
                {
                    if (chance > 0 && chance >= Random.Range(0f, 1f)) players.Add(player);
                }
            }
        }

        if (matchEvent == MatchEvent.Freekick)
        {
            return GetTopPlayerByAttribute(players.ToArray(), PlayerData.PlayerAttributes.Freekick);
        }

        return GetActivePlayer(players);
    }

    private PlayerData GetDefendingPlayer(FieldZone _zone)
    {
        FieldZone zone = _zone;
        if (defendingTeam == AwayTeam) zone = GetAwayTeamZone();

        float chance = 0f;
        bool forcePlayerOut = false;
        if (offensiveAction == PlayerData.PlayerAction.Dribble && lastActionSuccessful) forcePlayerOut = true;

        List<PlayerData> players = new List<PlayerData>();
        foreach (PlayerData player in defendingTeam.Squad)
        {
            chance = CalculatePresence(player, zone);
            if (player.Position != player.AssignedPosition) chance *= positionDebuff;
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

    private PlayerData GetActivePlayer(List<PlayerData> _list)
    {
        PlayerData activePlayer = null;
        List<KeyValuePair<PlayerData, float>> compareList = new List<KeyValuePair<PlayerData, float>>();
        int bonus = 0;

        foreach (PlayerData player in _list)
        {
            float stats = (float)(player.Speed + player.Vision) / 200;
            stats *= (float)player.Fatigue / 100;
            bonus = GetAttributeBonus((player.Vision + player.Speed)/2);
            if (player.Position != player.AssignedPosition) stats *= positionDebuff;

            int r = RollDice(20, 1, RollType.None, Mathf.FloorToInt(stats*5) + bonus/10);

            if (r > 18)
            {
                stats *= 1.5f;
            }
            else if (r < 3)
            {
                stats *= 0.25f;
            }
            
            compareList.Add(new KeyValuePair<PlayerData, float>(player, stats));
        }

        float random = Random.Range(0f, 1f);
        float cumulative = 0f;
        for (int i = 0; i < compareList.Count; i++)
        {
            cumulative += compareList[i].Value;
            if (random < cumulative)
            {
                activePlayer = compareList[i].Key;
                break;
            }
        }

        return activePlayer;
    }

    private float CalculatePresence(PlayerData _player, FieldZone _zone)
    {
        float chance = _player.GetChancePerZone(_zone);

        if (chance < 1f && chance > 0f)
        {
            chance *= ((float)(_player.Speed + _player.Vision) / 200) * ((float)_player.Fatigue / 100);
        }
        return chance;
    }

    private MarkingType GetMarkingType()
    {
        MarkingType type = MarkingType.None;
        if (defendingPlayer == null || attackingPlayer.AssignedPosition == PlayerData.PlayerPosition.GK) return type;

        float totalChance = 0f;
        totalChance = defendingPlayer.Prob_Marking;

        if (defendingPlayer.Speed > 70) totalChance += (float)GetAttributeBonus(defendingPlayer.Speed)/100;
        if (defendingPlayer.Vision > 70) totalChance += (float)GetAttributeBonus(defendingPlayer.Vision)/100;

        float r = RollDice(20, 1, RollType.None, Mathf.FloorToInt(totalChance));

        if (r >= 20)
        {
            type = MarkingType.Steal;
            defendingPlayer.Fatigue -= Mathf.FloorToInt(fatigueHigh * (0.5f * ((float)defendingPlayer.Stamina / 100)));
        }
        else if (r > 15)
        {
            type = MarkingType.Close;
            defendingPlayer.Fatigue -= Mathf.FloorToInt(fatigueMedium * (0.5f * ((float)defendingPlayer.Stamina / 100)));
        }
        else if (r > 3)
        {
            type = MarkingType.Distance;
            defendingPlayer.Fatigue -= Mathf.FloorToInt(fatigueLow * (0.5f * ((float)defendingPlayer.Stamina / 100)));
        }

        return type;
    }

    private PlayerData.PlayerAction GetOffensiveAction(MarkingType _marking)
    {
        FieldZone zone = currentZone;
        if (attackingTeam == AwayTeam) zone = GetAwayTeamZone();
        float bonus = 0;

        ActionChancePerZone zoneChance = actionChancePerZone.actionChancePerZones[(int)zone];

        float pass = zoneChance.Pass * attackingPlayer.Prob_Pass;
        bonus = GetAttributeBonus(attackingPlayer.Passing);
        if (_marking == MarkingType.Close) pass *= 2f;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((pass * 5) + (bonus/10))) > 18) pass *= 2f;

        float dribble = zoneChance.Dribble * attackingPlayer.Prob_Dribble;
        bonus = GetAttributeBonus(attackingPlayer.Dribbling);
        if (_marking == MarkingType.Close) dribble *= 0.5f;
        else if (_marking == MarkingType.Distance) dribble *= 1.5f;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((pass * 5) + (bonus / 10))) > 18) dribble *= 2f;

        float cross = zoneChance.Cross * attackingPlayer.Prob_Crossing;
        bonus = GetAttributeBonus(attackingPlayer.Crossing);
        if (_marking == MarkingType.Close) cross *= 0.5f;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((pass * 5) + (bonus / 10))) > 18) cross *= 2f;

        float shoot = zoneChance.Shot * attackingPlayer.Prob_Shoot;
        bonus = GetAttributeBonus(attackingPlayer.Shooting);
        if (_marking == MarkingType.Close) shoot *= 0.5f;
        else if (_marking == MarkingType.None) shoot *= 3f;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((pass * 5) + (bonus / 10))) > 18) shoot *= 2f;

        float header = 0f;
        if (offensiveAction == PlayerData.PlayerAction.Cross && zone == FieldZone.Box && lastActionSuccessful)
        {
            header = (zoneChance.Shot + attackingPlayer.Prob_Shoot) * 1.5f;
            bonus = GetAttributeBonus(attackingPlayer.Heading);
            if (_marking == MarkingType.Distance) header *= 2f;
            else if (_marking == MarkingType.None) header *= 3f;
            if (RollDice(20, 1, RollType.None, Mathf.FloorToInt((pass * 5) + (bonus / 10))) > 18) header *= 2f;
        }

        if(attackingPlayer.AssignedPosition == PlayerData.PlayerPosition.GK)
        {
            dribble = 0;
            shoot = 0;
            header = 0;
        }

        float total = pass + dribble + cross + shoot + header;
        pass /= total;
        dribble /= total;
        cross /= total;
        shoot /= total;
        header /= total;

        List<KeyValuePair<PlayerData.PlayerAction, float>> list = new List<KeyValuePair<PlayerData.PlayerAction, float>>();
        list.Add(new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Pass, pass));
        list.Add(new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Dribble, dribble));
        list.Add(new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Cross, cross));
        list.Add(new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Shot, shoot));
        list.Add(new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Header, header));

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

        DebugString += "Pass: " + pass + "\n";
        DebugString += "Dribble: " + dribble + "\n";
        DebugString += "Cross: " + cross + "\n";
        DebugString += "Shoot: " + shoot + "\n";
        DebugString += "Header: " + header + "\n\n";

        return action;
    }

    private bool IsActionSuccessful(MarkingType _marking)
    {
        bool success = false;
        float attacking = 0f;
        float defending = 0f;
        bool isTackling = false;
        int attackBonusChance = 0;
        int defenseBonusChance = 0;
        float fault = faultChance;
        float agilityBonus;
        int fatigueRate = 0;

        FieldZone zone = currentZone;
        if (defendingTeam == AwayTeam) zone = GetAwayTeamZone();

        if ((int)zone > 12)
        {
            float offside = offsideChance;
            if (defendingPlayer == null) offside *= MainController.Instance.TeamStrategyData.team_Strategys[(int)defendingTeam.Strategy].OffsideTrickChance;
            else offside *= defendingPlayer.Prob_OffsideLine;

            if (offside >= Random.Range(0f, 1f))
            {
                matchEvent = MatchEvent.Offside;
                return false;
            }
        }

            switch (offensiveAction)
        {
            case PlayerData.PlayerAction.None: return false;
            
            case PlayerData.PlayerAction.Pass:
                defensiveAction = PlayerData.PlayerAction.Block;
                attacking = (float)(attackingPlayer.Passing + attackingPlayer.Agility + attackingPlayer.Vision + attackingPlayer.Teamwork)/400;
                attackBonusChance = GetAttributeBonus(attackingPlayer.Passing);
                fatigueRate = fatigueLow;
                if (_marking == MarkingType.Close)
                {
                    attacking = attacking * 0.75f;
                }
                else if (_marking == MarkingType.Distance)
                {
                    defending = (float)(defendingPlayer.Blocking + defendingPlayer.Agility + defendingPlayer.Vision) / 300;
                    defenseBonusChance = GetAttributeBonus(defendingPlayer.Blocking);
                }
                break;

            case PlayerData.PlayerAction.Dribble:
                defensiveAction = PlayerData.PlayerAction.Tackle;
                attacking = (float)(attackingPlayer.Dribbling + attackingPlayer.Agility + attackingPlayer.Speed)/300;
                attackBonusChance = GetAttributeBonus(attackingPlayer.Tackling);
                fatigueLow = fatigueHigh;
                if (_marking == MarkingType.Close)
                {
                    attacking = attacking * 0.5f;
                }
                else if (_marking == MarkingType.Distance)
                {
                    defending = (float)(defendingPlayer.Tackling + defendingPlayer.Agility + defendingPlayer.Speed) / 300;
                    defenseBonusChance = GetAttributeBonus(defendingPlayer.Tackling);
                }
                break;

            case PlayerData.PlayerAction.Cross:
                defensiveAction = PlayerData.PlayerAction.Block;
                attacking = (float)(attackingPlayer.Crossing + attackingPlayer.Agility + attackingPlayer.Vision + attackingPlayer.Teamwork) / 400;
                attackBonusChance = GetAttributeBonus(attackingPlayer.Crossing);
                fatigueRate = fatigueMedium;
                if (_marking == MarkingType.Close)
                {
                    attacking = attacking * 0.5f;
                }
                else if (_marking == MarkingType.Distance)
                {
                    defending = (float)(defendingPlayer.Blocking + defendingPlayer.Agility + defendingPlayer.Vision) / 300;
                    defenseBonusChance = GetAttributeBonus(defendingPlayer.Blocking);
                }
                break;

            case PlayerData.PlayerAction.Shot:
                defensiveAction = PlayerData.PlayerAction.Block;
                attacking = (float)(attackingPlayer.Shooting + attackingPlayer.Agility + attackingPlayer.Strength) / 300;
                attackBonusChance = GetAttributeBonus(attackingPlayer.Shooting);
                fatigueRate = fatigueMedium;
                if (_marking == MarkingType.Close)
                {
                    attacking = attacking * 0.5f;
                }
                else if (_marking == MarkingType.Distance)
                {
                    defending = (float)(defendingPlayer.Blocking + defendingPlayer.Agility + defendingPlayer.Vision + defendingPlayer.Speed) / 400;
                    defenseBonusChance = GetAttributeBonus(defendingPlayer.Blocking);
                }
                break;

            case PlayerData.PlayerAction.Header:
                defensiveAction = PlayerData.PlayerAction.Block;
                attacking = (float)(attackingPlayer.Heading + attackingPlayer.Agility + attackingPlayer.Strength) / 300;
                attackBonusChance = GetAttributeBonus(attackingPlayer.Heading);
                fatigueRate = fatigueMedium;
                if (_marking == MarkingType.Close)
                {
                    attacking = attacking * 0.5f;
                }
                else if (_marking == MarkingType.Distance)
                {
                    defending = (float)(defendingPlayer.Heading + defendingPlayer.Blocking + defendingPlayer.Agility + defendingPlayer.Vision) / 400;
                    defenseBonusChance = GetAttributeBonus(defendingPlayer.Blocking);
                }
                break;
        }

        attacking *= ((float)attackingPlayer.Fatigue / 100);
        attacking *= attackingBonus;
        if (attackingPlayer.Position != attackingPlayer.AssignedPosition) attacking *= positionDebuff;

        int attackRoll = RollDice(20, 1, RollType.None, Mathf.FloorToInt(attacking * 5), attackBonusChance);
        switch (attackRoll)
        {
            case 20:
                DebugString += "\nAtacante ganhou bonus de 100% \n";
                attacking = attacking * 2;
                break;
            case 2:
                DebugString += "\nAtacante meia-bomba \n";
                attacking = attacking * 0.5f;
                break;
            case 1:
                DebugString += "\nAtacante ratiou \n";
                attacking = attacking * 0.25f;
                break;
        }

        //Check if tackling is really happening  
        if (_marking == MarkingType.None)
        {
            isTackling = false;
        }
        else
        {     
            float tackleChance = 0.5f * actionChancePerZone.actionChancePerZones[(int)zone].Tackle * defendingPlayer.Prob_Tackling;
            if (_marking == MarkingType.Close)
            {
                tackleChance *= 1.25f;
            }

            isTackling |= tackleChance >= Random.Range(0f, 1f);
        }

        if (isTackling)
        {  
            defending *= (float)defendingPlayer.Fatigue / 100;
            defendingPlayer.Fatigue -= Mathf.FloorToInt(fatigueMedium * (0.5f * ((float)defendingPlayer.Stamina / 100)));
            int defenseRoll = RollDice(20, 1, RollType.None, Mathf.FloorToInt(defending * 5), defenseBonusChance);
            if (defenseRoll == 20)
            {
                DebugString += "\nDefensor ganhou bonus de 50% \n";
                defending = defending * 1.5f;
            }
            else if (defenseRoll == 2)
            {
                DebugString += "\nDefensor meia-bomba \n";
                defending = defending * 0.5f;
            }
            else if (defenseRoll == 1)
            {
                DebugString += "\nDefensor ratiou \n";
                defending = 0.25f;
            }


            agilityBonus = (float)GetAttributeBonus(defendingPlayer.Agility)/100;
            agilityBonus *= (float)defendingPlayer.Fatigue / 100;
            fault *= (1f - agilityBonus);

            if(fault >= Random.Range(0f, 1f))
            {
                if(attackingTeam == HomeTeam && currentZone == FieldZone.Box)
                {
                    matchEvent = MatchEvent.Penalty;
                }
                else if (attackingTeam == AwayTeam && currentZone == FieldZone.OwnGoal)
                {
                    matchEvent = MatchEvent.Penalty;
                }
                else 
                {
                    matchEvent = MatchEvent.Freekick;
                }
                success = false;
                defendingPlayer.TotalFaults++;
            }

            else if (attacking > defending)
            {
                DebugString += "\nAtacante rolou " + attacking;
                DebugString += "\nDefensor rolou " + defending + "\n\n";
                success = true;
            }

            defendingPlayer.TotalTackles++;
        }
        else
        {
            DebugString += "\nAtacante tem espaco pra jogar\n";
            defensiveAction = PlayerData.PlayerAction.None;
            success |= attacking >= Random.Range(0f, 1f);
        }

        float fatigueLost = (fatigueRate * (0.5f * ((float)attackingPlayer.Stamina / 100)));
        attackingPlayer.Fatigue -= Mathf.FloorToInt(fatigueLost);

        return success;
    }

    private int GetAttributeBonus(int _attribute)
    {
        int bonusChance = 0;

        if(_attribute > 70)
        {
            bonusChance = _attribute - 70;
        }

        return bonusChance;
    }

    private void ResolveShot(MarkingType _marking)
    {
        float attacking = 0f;
        float defending = 0f;
        float distanceModifier = 1f;
        int bonusChance = 0;
        defendingPlayer = defendingTeam.Squad[0];
        FieldZone zone = currentZone;

        if (attackingPlayer == AwayTeam) currentZone = GetAwayTeamZone();


        switch(zone)
        {
            case FieldZone.LAM:
            case FieldZone.RAM:
                distanceModifier = 0.5f;
                break;

            case FieldZone.CAM:
                distanceModifier = 0.65f;
                break;

            case FieldZone.LF:
            case FieldZone.RF:
                distanceModifier = 0.75f;
                break;

            case FieldZone.CF:
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
            attacking = (float)(attackingPlayer.Heading + attackingPlayer.Strength) / 100;
            bonusChance = GetAttributeBonus(attackingPlayer.Heading);
        }

        attacking *= (float)attackingPlayer.Fatigue / 100;
        attacking *= attackingBonus;
        attacking *= distanceModifier;

        if (_marking == MarkingType.Close) attacking = attacking * 0.5f;

        int roll = RollDice(20, 1, RollType.None, Mathf.FloorToInt(attacking * 5), bonusChance);
        if  (roll > 19)
        {
            attacking += attacking * 1.25f;
            DebugString += "\n ATACANTE GANHOU BONUS DE 25%";
        }
        else if (roll < 4)
        {
            shotMissed = true;
            return;
        }

        defending = ((float)defendingPlayer.Goalkeeping + defendingPlayer.Agility) / 200;
        defending *= (float)defendingPlayer.Fatigue / 100;
        if (RollDice(20, 1, RollType.None, (int)defending) > 18)
        {
            defending *= 1.50f;
            DebugString += "\n GOLEIRO GANHOU BONUS DE 50%";
        }

        DebugString += "\nAtacante rolou: " + attacking + "\nGoleiro rolou: " + defending;
        if (attacking <= defending)
        {  
            shotSaved = true;
        }

        else if (attacking > defending) matchEvent = MatchEvent.Goal;
    }

    private PlayerData.PlayerAction GetFreeKickAction()
    {
        PlayerData.PlayerAction action = PlayerData.PlayerAction.Pass;
        FieldZone zone = currentZone;
        if (attackingTeam == AwayTeam) zone = GetAwayTeamZone();
        int bonus = 0;
        ActionChancePerZone zoneChance = actionChancePerZone.actionChancePerZones[(int)zone];

        float pass = zoneChance.Pass * attackingPlayer.Prob_Pass;
        bonus = GetAttributeBonus(attackingPlayer.Passing);
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt(pass * 5) + bonus / 10) > 18) pass *= 2f;

        float cross = zoneChance.Cross * attackingPlayer.Prob_Crossing;
        bonus = GetAttributeBonus(attackingPlayer.Crossing);
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt(cross * 5), bonus) > 18) cross *= 2f;

        float shoot = zoneChance.Shot * attackingPlayer.Prob_Shoot;
        bonus = GetAttributeBonus(attackingPlayer.Shooting);
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt(shoot * 5) + bonus / 10) > 18) shoot *= 2f;

        float total = pass + cross + shoot;
        pass = pass / total;
        cross = cross / total;
        shoot = shoot / total;

        List<KeyValuePair<PlayerData.PlayerAction, float>> list = new List<KeyValuePair<PlayerData.PlayerAction, float>>();
        list.Add(new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Pass, pass));
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

        DebugString += "Pass: " + pass + "\n";
        DebugString += "Cross: " + cross + "\n";
        DebugString += "Shoot: " + shoot + "\n";

        return action;
    }

    private int RollDice(int _sides, int _amount = 1, RollType _rollType = RollType.None, int _bonus = 0, int _bonusChance = 100)
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
    private FieldZone GetAwayTeamZone()
    {
        int zone = (totalZones - 1) -  (int)currentZone;

        return (FieldZone)zone;
    }

    private FieldZone GetTargetZone()
    {
        FieldZone target = currentZone;
        FieldZone zone = currentZone;
        if (attackingTeam == AwayTeam) zone = GetAwayTeamZone();
        List<int> zones = new List<int>();
        List<KeyValuePair<FieldZone, float>> list = new List<KeyValuePair<FieldZone, float>>();

        float _OwnGoal = 0;
        float _LD = 0;
        float _CD = 0;
        float _RD = 0;
        float _LDM = 0;
        float _CDM = 0;
        float _RDM = 0;
        float _LM = 0;
        float _CM = 0;
        float _RM = 0;
        float _LAM = 0;
        float _CAM = 0;
        float _RAM = 0;
        float _LF = 0;
        float _CF = 0;
        float _RF = 0;
        float _Box = 0;

        if (matchEvent == MatchEvent.Goalkick)
        {
            _LM = 1f;
            _CM = 1f;
            _RM = 1f;
            _LAM = 1.5f;
            _CAM = 1.5f;
            _RAM = 1.5f;
        }

        else if (offensiveAction == PlayerData.PlayerAction.Pass || offensiveAction == PlayerData.PlayerAction.Dribble)
        { 
            switch (zone)
            {
                case FieldZone.OwnGoal:
                    _LD = 1f;
                    _CD = 1f;
                    _RD = 1f;
                    break;

                case FieldZone.LD:
                    _LDM = 1f;
                    _CDM = 1f;
                    _CD = 1f;
                    break;
                case FieldZone.CD:
                    _LDM = 1f;
                    _CDM = 1f;
                    _RDM = 1f;
                    break;
                case FieldZone.RD:
                    _CD = 1f;
                    _CDM = 1f;
                    _RDM = 1f;
                    break;

                case FieldZone.LDM:
                    _CDM = 1f;
                    _LM = 1f;
                    _CM = 1f;
                    break;
                case FieldZone.CDM:
                    _LM = 1f;
                    _CM = 1f;
                    _RM = 1f;
                    break;
                case FieldZone.RDM:
                    _CDM = 1f;
                    _CM = 1f;
                    _RM = 1f;
                    break;

                case FieldZone.LM:
                    _CM = 1f;
                    _LAM = 1f;
                    _CAM = 1f;
                    break;
                case FieldZone.CM:
                    _LAM = 1f;
                    _CAM = 1f;
                    _RAM = 1f;
                    break;
                case FieldZone.RM:
                    _CM = 1f;
                    _CAM = 1f;
                    _RAM = 1f;
                    break;

                case FieldZone.LAM:
                    _CAM = 1f;
                    _LF = 1f;
                    _CF = 1f;
                    break;
                case FieldZone.CAM:
                    _LF = 1f;
                    _CF = 1f;
                    _RF = 1f;
                    break;
                case FieldZone.RAM:
                    _CAM = 1f;
                    _CF = 1f;
                    _RF = 1f;
                    break;

                case FieldZone.LF:
                    _CF = 1f;
                    _Box = 2f;
                    break;
                case FieldZone.CF:
                    _LF = 1f;
                    _RF = 1f;
                    _Box = 2.5f;
                    break;
                case FieldZone.RF:
                    _CF = 1f;
                    _Box = 2f;
                    break;
                case FieldZone.Box:
                    zones.InsertRange(0, new int[] { 13, 14, 15, 16, 16, 16 });
                    _LF = 0.5f;
                    _CF = 1;
                    _RF = 0.5f;
                    _Box = 3f;
                    break;
            } 
        }

        else if (offensiveAction == PlayerData.PlayerAction.Cross)
        {
            switch (zone)
            {
                case FieldZone.OwnGoal:
                    _LDM = 1f;
                    _CDM = 1f;
                    _RDM = 1f;
                    _LM = 1f;
                    _CM = 1f;
                    _RM = 1f;
                    break;

                case FieldZone.LD:
                    _LM = 1f;
                    _CM = 1f;
                    _LAM = 1f;
                    break;
                case FieldZone.CD:
                    _LM = 1f;
                    _CM = 1f;
                    _RM = 1f;
                    _CAM = 1f;
                    break;
                case FieldZone.RD:
                    _RM = 1f;
                    _CM = 1f;
                    _RAM = 1f;
                    break;

                case FieldZone.LDM:
                    _LM = 1f;
                    _CAM = 1f;
                    _LF = 1f;
                    break;
                case FieldZone.CDM:
                    _LAM = 1f;
                    _CAM = 1f;
                    _RAM = 1f;
                    _CF = 1f;
                    break;
                case FieldZone.RDM:
                    _CAM = 1f;
                    _RAM = 1f;
                    _RF = 1f;
                    break;

                case FieldZone.LM:
                    _LF = 1f;
                    _CF = 1f;
                    break;
                case FieldZone.CM:
                    _LF = 1f;
                    _CF = 1f;
                    _RF = 1f;
                    break;
                case FieldZone.RM:
                    _CF = 1f;
                    _RF = 1f;
                    break;

                case FieldZone.LAM:
                case FieldZone.CAM:
                case FieldZone.RAM:
                    _LF = 1f;
                    _RF = 1f;
                    _Box = 3f;
                    break;
                case FieldZone.LF:
                case FieldZone.CF:
                case FieldZone.RF:
                case FieldZone.Box:
                    _Box = 1;
                    break;
            }
        }


        Team_Strategy strategy = MainController.Instance.TeamStrategyData.team_Strategys[(int)attackingTeam.Strategy];
        _OwnGoal *= strategy.Target_OwnGoal;
        _LD *= strategy.Target_LD;
        _CD *= strategy.Target_CD;
        _RD *= strategy.Target_RD;
        _LDM *= strategy.Target_LDM;
        _CDM *= strategy.Target_CDM;
        _RDM *= strategy.Target_RDM;
        _LM *= strategy.Target_LM;
        _CM *= strategy.Target_CM;
        _RM *= strategy.Target_RM;
        _LAM *= strategy.Target_LAM;
        _CAM *= strategy.Target_CAM;
        _RAM *= strategy.Target_RAM;
        _LF *= strategy.Target_LF;
        _CF *= strategy.Target_CF;
        _RF *= strategy.Target_RF;
        _Box *= strategy.Target_Box;



        float total = _OwnGoal + _LD + _CD + _RD + _LDM + _CDM + _RDM + _LM + _CM + _RM + _LAM + _CAM + _RAM + _LF + _CF + _RF + _Box;
        _OwnGoal /= total;
        _LD /= total;
        _CD /= total;
        _RD /= total;
        _LDM /= total;
        _CDM /= total;
        _RDM /= total;
        _LM /= total;
        _CM /= total;
        _RM /= total;
        _LAM /= total;
        _CAM /= total;
        _RAM /= total;
        _LF /= total;
        _CF /= total;
         _RF /= total;
        _Box /= total;

        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.OwnGoal, _OwnGoal));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.LD, _LD));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.CD, _CD));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.RD, _RD));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.LDM, _LDM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.CDM, _CDM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.RDM, _RDM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.LM, _LM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.CM, _CM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.RM, _RM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.LAM, _LAM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.CAM, _CAM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.RAM, _RAM));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.LF, _LF));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.CF, _CF));
        list.Add(new KeyValuePair<FieldZone, float>(FieldZone.RF, _RF));
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

        //int random = Random.Range(0, zones.Count);
        //target = (FieldZone)zones[random];
        
        if (attackingTeam == AwayTeam) target = (FieldZone)((totalZones - 1) - (int)target);
        return target;
    }

    private PlayerData GetTopPlayerByAttribute(PlayerData[] _players, PlayerData.PlayerAttributes _attribute)
    {
        if (_players.Length == 0) print("PALHA");
        PlayerData best = null;
        int higher = 0;
        foreach(PlayerData player in _players)
        {
            if(player.GetPlayerAttribute(_attribute) > higher) best = player;
        }

        return best;
    }

    private void SwitchPossesion()
    {
        if(attackingTeam == HomeTeam)
        {
            attackingTeam = AwayTeam;
            defendingTeam = HomeTeam;
        }
        else if(attackingTeam == AwayTeam)
        {
            defendingTeam = AwayTeam;
            attackingTeam = HomeTeam;
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
}
