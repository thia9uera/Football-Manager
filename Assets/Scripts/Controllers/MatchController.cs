using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    private int matchTime = 0;
    private int homeTeamScore = 0;
    private int awayTeamScore = 0;

    private bool isGoalAnnounced = false;
    private bool isScorerAnnounced = false;
    private bool isHalfTime = false;
    private bool secondHalfStarted = false;
    private bool isFreekickTaken = false;

    [HideInInspector]
    public string DebugString;

    [SerializeField]
    private float positionDebuff = 0.85f;

    public void Populate(TeamData _homeTeam, TeamData _awayTeam)
    {
        HomeTeam = _homeTeam;
        AwayTeam = _awayTeam;

        attackingTeam = HomeTeam;
        defendingTeam = AwayTeam;

        HomeTeamSquad.Populate(_homeTeam);
        AwayTeamSquad.Populate(_awayTeam);
        Score.UpdateTime(matchTime);
        Score.UpdateScore(
            HomeTeam.Name,
            homeTeamScore,
            ColorUtility.ToHtmlStringRGB(HomeTeam.PrimaryColor), 
            AwayTeam.Name, 
            awayTeamScore, 
            ColorUtility.ToHtmlStringRGB(AwayTeam.PrimaryColor));
    }

    public void UpdateTeams()
    {
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
        Narration.Reset();
        Score.UpdateTime(matchTime);
        Score.UpdateScore(HomeTeam.Name, homeTeamScore, ColorUtility.ToHtmlStringRGB(HomeTeam.PrimaryColor), AwayTeam.Name, awayTeamScore, ColorUtility.ToHtmlStringRGB(AwayTeam.PrimaryColor));
    }

    public void HandleStartButton()
    {
        Reset();
        KickOff();
    }

    public void KickOff()
    {
        startBtn.SetActive(false);

        Narration.UpdateNarration("KICK OFF!", Color.gray);
        DebugString = "KICK OFF! \n \n";
        currentZone = FieldZone.CM;
        Field.UpdateFieldArea((int)currentZone);

        InvokeRepeating("DefineActions", 1f, 1f);  
    }

    //MAIN CONTROLLING FUNCTION
    private void DefineActions()
    {
        Field.UpdateFieldArea((int)currentZone);

        //IF LAST ACTION RESULTED IN A GOAL
        if (matchEvent == MatchEvent.Goal)
        {
            if (!isGoalAnnounced)
            {
                isGoalAnnounced = true;
                Narration.UpdateNarration("<size=60>GOOOOOOAAAAALLLL!!!", attackingTeam.PrimaryColor);
                DebugString += "\n\n<size=40>GOL de " + attackingPlayer.FirstName + " " + attackingPlayer.LastName + "</size>\n ________________________________\n \n";
                if (attackingTeam == HomeTeam) homeTeamScore++;
                else awayTeamScore++;
                Score.UpdateScore(HomeTeam.Name, homeTeamScore, ColorUtility.ToHtmlStringRGB(HomeTeam.PrimaryColor), AwayTeam.Name, awayTeamScore, ColorUtility.ToHtmlStringRGB(AwayTeam.PrimaryColor));

                return;
            }
            if(!isScorerAnnounced)
            {
                isScorerAnnounced = true;
                Narration.UpdateNarration(attackingPlayer.FirstName + " " + attackingPlayer.LastName + " marca para " + attackingTeam.Name + "!", attackingTeam.PrimaryColor);
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
        }

        //IF LAST ACTION RESULTED IN A FREEKICK
        if (matchEvent == MatchEvent.Freekick)
        {
            if(!isFreekickTaken)
            {
                Narration.UpdateNarration(attackingPlayer.FirstName + " VAI PARA A COBRANCA...", attackingTeam.PrimaryColor);    
                isFreekickTaken = true;
                return;
            }
            else
            {
                ResolveFreeKick();
                isFreekickTaken = false;
                return;
            }
        }

        if (matchTime >= 45 && !isHalfTime)
        {
            isHalfTime = true;
            Narration.UpdateNarration("FIM DO PRIMEIRO TEMPO", Color.gray);
            currentZone = FieldZone.CM;
            attackingTeam = AwayTeam;
            defendingTeam = HomeTeam;
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
            MarkingType marking = GetMarkingType();
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
                    DebugString += "\n<size=28>" + attackingPlayer.FirstName + " " + attackingPlayer.LastName + " VS " + defendingPlayer.FirstName + " " + defendingPlayer.LastName + " (" + currentZone + ")</size> \n";
                    DebugString += "\nMARCACAO DE PERTO \n \n";
                }
                else if (marking == MarkingType.Distance)
                {
                    DebugString += "\n<size=28>" + attackingPlayer.FirstName + " " + attackingPlayer.LastName + " VS " + defendingPlayer.FirstName + " " + defendingPlayer.LastName + " (" + currentZone + ")</size> \n";
                    DebugString += "\nMARCACAO A DISTANCIA \n \n";
                }
                else
                {
                    //Narration.UpdateNarration(attackingPlayer.FirstName + " SOZINHO NA JOGADA", attackingTeam.PrimaryColor);
                    DebugString += "\n<size=28>" + attackingPlayer.FirstName + " " + attackingPlayer.LastName + " (" + currentZone + ")</size> \n";
                    DebugString += "\nSEM MARCACAO \n\n";
                }

                //Step 3: Get type of offensive play
                offensiveAction = GetOffensiveAction(marking);

                //Step 4: Test action against defender (if there is one)
                if (IsActionSuccessful(marking))
                {
                    lastActionSuccessful = true;

                    //Give bonus based on type of marking
                    if (marking == MarkingType.Close) attackingBonus *= 1.3f;
                    else if (marking == MarkingType.Distance) attackingBonus *= 1.2f;
                    else if (marking == MarkingType.None) attackingBonus *= 1.1f;

                    switch (offensiveAction)
                    {
                        case PlayerData.PlayerAction.Pass:
                            DebugString += "PASSOU A BOLA! \n ________________________________\n";
                            Narration.UpdateNarration(attackingPlayer.FirstName + " PASSA A BOLA", attackingTeam.PrimaryColor);
                            currentZone = GetTargetZone();
                            break;

                        case PlayerData.PlayerAction.Dribble:
                            DebugString += "DRIBLOU! \n ________________________________\n";
                            if (defendingPlayer != null) Narration.UpdateNarration(attackingPlayer.FirstName + " DRIBLA " + defendingPlayer.FirstName, attackingTeam.PrimaryColor);
                            currentZone = GetTargetZone();
                            keepAttacker = true;
                            break;

                        case PlayerData.PlayerAction.Cross:
                            DebugString += "CRUZOU! \n ________________________________\n";
                            Narration.UpdateNarration(attackingPlayer.FirstName + " CRUZA A BOLA", attackingTeam.PrimaryColor);
                            currentZone = GetTargetZone();
                            break;

                        case PlayerData.PlayerAction.Shot:
                            DebugString += "CHUTOU! \n";
                            Narration.UpdateNarration(attackingPlayer.FirstName + " TENTA O CHUTE...", attackingTeam.PrimaryColor);
                            ResolveShot(marking);
                            break;

                        case PlayerData.PlayerAction.Header:
                            DebugString += "CABECEOU! \n";
                            Narration.UpdateNarration(attackingPlayer.FirstName + " TENTA O LANCE DE CABECA...", attackingTeam.PrimaryColor);
                            ResolveShot(marking);
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
                        attackingPlayer = GetTopPlayerByAttribute(attackingTeam, PlayerData.PlayerAttributes.Freekick);
                        defendingPlayer = defendingTeam.Squad[0];
                        return;
                    }

                    switch (offensiveAction)
                    {
                        case PlayerData.PlayerAction.None:
                            DebugString += "RATIOU FEIO E PERDEU A BOLA! \n ________________________________\n";
                            Narration.UpdateNarration(attackingPlayer.FirstName + " DEU BOBEIRA E PERDEU A BOLA", attackingTeam.PrimaryColor);
                            break;

                        case PlayerData.PlayerAction.Pass:
                            if(defensiveAction == PlayerData.PlayerAction.None)
                            {
                                DebugString += "ERROU O PASSE! \n ________________________________\n";
                                Narration.UpdateNarration(attackingPlayer.FirstName + " ERRA O PASSE", Color.gray);
                                currentZone = GetTargetZone();
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
                                DebugString += "ERROU O CRUZAMENTO! \n ________________________________\n";
                                Narration.UpdateNarration(attackingPlayer.FirstName + " ERRA O CRUZAMENTO", Color.gray);
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
                                currentZone = GetTargetZone();
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
                                currentZone = GetTargetZone();
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
        }
        //debugController.UpdateDebug();
    }

    private PlayerData GetAttackingPlayer(FieldZone _zone)
    {
        FieldZone zone = _zone;
        if (attackingTeam == AwayTeam) zone = GetAwayTeamZone();

        float chance = 0f;
        bool forcePlayer = false;
        if (offensiveAction == PlayerData.PlayerAction.Pass || offensiveAction == PlayerData.PlayerAction.Cross)
        {
            if (lastActionSuccessful) forcePlayer = true;
        }

        List<PlayerData> players = new List<PlayerData>();

        foreach (PlayerData player in attackingTeam.Squad)
        {
            chance = CalculatePresence(player, zone);
            if (player.Position != player.AssignedPosition) chance *= positionDebuff;
            if (forcePlayer)
            {
                if (player != attackingPlayer && chance > 0f)
                {
                    players.Clear();
                    players.Add(player);
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
                    if (chance > 0 && chance <= Random.Range(0f, 1f)) players.Add(player);
                }
            }
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
                if (chance > 0 && chance <= Random.Range(0f, 1f))
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
            chance *= ((float)(_player.Speed + _player.Vision) / 200) * (_player.Fatigue / 100);
        }
        return chance;
    }

    private MarkingType GetMarkingType()
    {
        MarkingType type = MarkingType.None;
        if (defendingPlayer == null) return type;

        float totalChance = 0f;
        totalChance = defendingPlayer.Prob_Marking;

        if (defendingPlayer.Speed > 70) totalChance += ((100 - (float)defendingPlayer.Speed) / 100) * ((float)defendingPlayer.Fatigue / 100);
        if (defendingPlayer.Vision > 70) totalChance += ((100 - (float)defendingPlayer.Vision) / 100) * (float)(defendingPlayer.Fatigue / 100);

        float r = RollDice(20, 1, RollType.None, Mathf.FloorToInt(totalChance));

        if (r >= 20)
        {
            type = MarkingType.Steal;
        }
        else if (r > 15)
        {
            type = MarkingType.Close;
        }
        else if (r > 3)
        {
            type = MarkingType.Distance;
        }

        return type;
    }

    private PlayerData.PlayerAction GetOffensiveAction(MarkingType _marking)
    {
        FieldZone zone = currentZone;
        if (attackingTeam == AwayTeam) zone = GetAwayTeamZone();
        int bonus = 0;

        ActionChancePerZone zoneChance = actionChancePerZone.actionChancePerZones[(int)zone];

        float pass = zoneChance.Pass * attackingPlayer.Prob_Pass;
        bonus = GetAttributeBonus(attackingPlayer.Passing);
        if (_marking == MarkingType.Close) pass *= 2f;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt(pass * 5) + bonus/10) > 18) pass *= 2f;

        float dribble = zoneChance.Dribble * attackingPlayer.Prob_Dribble;
        bonus = GetAttributeBonus(attackingPlayer.Dribbling);
        if (_marking == MarkingType.Close) dribble *= 0.5f;
        else if (_marking == MarkingType.Distance) dribble *= 1.5f;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt(dribble*5) + bonus / 10) > 18) dribble *= 2f;

        float cross = zoneChance.Cross * attackingPlayer.Prob_Crossing;
        bonus = GetAttributeBonus(attackingPlayer.Crossing);
        if (_marking == MarkingType.Close) cross *= 0.5f;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt(cross * 5), bonus) > 18) cross *= 2f;

        float shoot = zoneChance.Shot * attackingPlayer.Prob_Shoot;
        bonus = GetAttributeBonus(attackingPlayer.Shooting);
        if (_marking == MarkingType.Close) shoot *= 0.5f;
        else if (_marking == MarkingType.None) shoot *= 3f;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt(shoot * 5) + bonus / 10) > 18) shoot *= 2f;

        float header = 0f;
        if (offensiveAction == PlayerData.PlayerAction.Cross && zone == FieldZone.Box && lastActionSuccessful)
        {
            header = (zoneChance.Shot + attackingPlayer.Prob_Shoot) * 1.5f;
            bonus = GetAttributeBonus(attackingPlayer.Heading);
            if (_marking == MarkingType.Distance) header *= 2f;
            else if (_marking == MarkingType.None) header *= 3f;
            if (RollDice(20, 1, RollType.None, Mathf.FloorToInt(shoot * 5) + bonus / 10) > 18) header *= 2f;
        }

        float total = pass + dribble + cross + shoot + header;
        pass = pass / total;
        dribble = dribble / total;
        cross = cross / total;
        shoot = shoot / total;
        header = header / total;

        List<KeyValuePair<PlayerData.PlayerAction, float>> list = new List<KeyValuePair<PlayerData.PlayerAction, float>>();
        list.Add(new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Pass, pass));
        list.Add(new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Dribble, dribble));
        list.Add(new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Cross, cross));
        list.Add(new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Shot, shoot));
        list.Add(new KeyValuePair<PlayerData.PlayerAction, float>(PlayerData.PlayerAction.Header, header));

        float random = Random.Range(0f, 1f);
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
        float faultChance = 0f;
        float agilityBonus;

        FieldZone zone = currentZone;
        if (defendingTeam == AwayTeam) zone = GetAwayTeamZone();

        switch (offensiveAction)
        {
            case PlayerData.PlayerAction.None: return false;
            
            case PlayerData.PlayerAction.Pass:
                defensiveAction = PlayerData.PlayerAction.Block;
                attacking = (float)(attackingPlayer.Passing + attackingPlayer.Agility + attackingPlayer.Vision + attackingPlayer.Teamwork)/400;
                attackBonusChance = GetAttributeBonus(attackingPlayer.Passing);
                if (_marking == MarkingType.Close)
                {
                    attacking = attacking * 0.75f;
                }
                if (_marking != MarkingType.None)
                {
                    defending = (float)(defendingPlayer.Blocking + defendingPlayer.Agility + defendingPlayer.Vision) / 300;
                    defenseBonusChance = GetAttributeBonus(defendingPlayer.Blocking);
                }
                break;

            case PlayerData.PlayerAction.Dribble:
                defensiveAction = PlayerData.PlayerAction.Tackle;
                attacking = (float)(attackingPlayer.Dribbling + attackingPlayer.Agility + attackingPlayer.Speed)/300;
                attackBonusChance = GetAttributeBonus(attackingPlayer.Tackling);
                if (_marking == MarkingType.Close)
                {
                    attacking = attacking * 0.5f;
                }
                if (_marking != MarkingType.None)
                {
                    defending = (float)(defendingPlayer.Tackling + defendingPlayer.Agility + defendingPlayer.Speed) / 300;
                    defenseBonusChance = GetAttributeBonus(defendingPlayer.Tackling);
                }
                break;

            case PlayerData.PlayerAction.Cross:
                defensiveAction = PlayerData.PlayerAction.Block;
                attacking = (float)(attackingPlayer.Crossing + attackingPlayer.Agility + attackingPlayer.Vision + attackingPlayer.Teamwork) / 400;
                attackBonusChance = GetAttributeBonus(attackingPlayer.Crossing);
                if (_marking == MarkingType.Close)
                {
                    attacking = attacking * 0.5f;
                }
                if (_marking != MarkingType.None)
                {
                    defending = (float)(defendingPlayer.Blocking + defendingPlayer.Agility + defendingPlayer.Vision) / 300;
                    defenseBonusChance = GetAttributeBonus(defendingPlayer.Blocking);
                }
                break;

            case PlayerData.PlayerAction.Shot:
                defensiveAction = PlayerData.PlayerAction.Block;
                attacking = (float)(attackingPlayer.Shooting + attackingPlayer.Agility + attackingPlayer.Strength) / 300;
                attackBonusChance = GetAttributeBonus(attackingPlayer.Shooting);
                if (_marking == MarkingType.Close)
                {
                    attacking = attacking * 0.5f;
                }
                if (_marking != MarkingType.None)
                {
                    defending = (float)(defendingPlayer.Blocking + defendingPlayer.Agility + defendingPlayer.Vision + defendingPlayer.Speed) / 400;
                    defenseBonusChance = GetAttributeBonus(defendingPlayer.Blocking);
                }
                break;

            case PlayerData.PlayerAction.Header:
                defensiveAction = PlayerData.PlayerAction.Block;
                attacking = (float)(attackingPlayer.Heading + attackingPlayer.Agility + attackingPlayer.Strength) / 300;
                attackBonusChance = GetAttributeBonus(attackingPlayer.Heading);
                if (_marking == MarkingType.Close) attacking = attacking * 0.5f;
                if (_marking != MarkingType.None)
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
        if (attackRoll == 20)
        {
            DebugString += "\nAtacante ganhou bonus de 100% \n";
            attacking = attacking * 2;
        }
        else if (attackRoll == 2)
        {
            DebugString += "\nAtacante meia-bomba \n";
            attacking = attacking * 0.5f;
        }
        else if (attackRoll == 1)
        {
            DebugString += "\nAtacante ratiou \n";
            attacking = attacking * 0.25f;
        }

        //Check if tackling is really happening  
        if (_marking == MarkingType.None) isTackling = false;
        else
        {     
            float tackleChance = 0.5f * actionChancePerZone.actionChancePerZones[(int)zone].Tackle * defendingPlayer.Prob_Tackling;
            if (_marking == MarkingType.Close) tackleChance *= 1.25f;
            if (tackleChance > Random.Range(0f, 1f)) isTackling = true;
        }

        if (isTackling)
        {
            defending *= (float)(defendingPlayer.Fatigue / 100);
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


            faultChance = 0.25f;
            agilityBonus = GetAttributeBonus(defendingPlayer.Agility)/100;
            agilityBonus *= defendingPlayer.Fatigue / 100;
            faultChance *= (1f - agilityBonus);

            if(Random.Range(0f, 1f) <  faultChance)
            {
                matchEvent = MatchEvent.Freekick;
                success = false;
            }

            else if (attacking > defending)
            {
                DebugString += "\nAtacante rolou " + attacking;
                DebugString += "\nDefensor rolou " + defending + "\n\n";
                success = true;
            }
        }
        else
        {
            DebugString += "\nAtacante tem espaco pra jogar\n";
            defensiveAction = PlayerData.PlayerAction.None;
            if (attacking > Random.Range(0f, 1f)) success = true;
        }

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
        PlayerData defendingPlayer = defendingTeam.Squad[0];
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
            attacking = (float)(attackingPlayer.Shooting + attackingPlayer.Strength) / 200;
            bonusChance = GetAttributeBonus(attackingPlayer.Shooting);
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

        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt(attacking*5), bonusChance) > 19)
        {
            attacking += attacking * 1.25f;
            DebugString += "\n ATACANTE GANHOU BONUS DE 25%";
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
            keepDefender = true;
           
            if (offensiveAction == PlayerData.PlayerAction.Header)
            {
                DebugString += "\n\n" + defendingPlayer.FirstName + " " + defendingPlayer.LastName + " defende a cabecada de " + attackingPlayer.FirstName + " " + attackingPlayer.LastName + "\n\n_____________________________________\n\n";
                Narration.UpdateNarration(defendingPlayer.FirstName + " " + defendingPlayer.LastName + " defende a cabecada de " + attackingPlayer.FirstName + " " + attackingPlayer.LastName, defendingTeam.PrimaryColor);
            }
            else
            {
                DebugString += "\n\n" + defendingPlayer.FirstName + " " + defendingPlayer.LastName + " defende o chute de " + attackingPlayer.FirstName + " " + attackingPlayer.LastName + "\n\n_____________________________________\n\n";
                Narration.UpdateNarration(defendingPlayer.FirstName + " " + defendingPlayer.LastName + " defende o chute de " + attackingPlayer.FirstName + " " + attackingPlayer.LastName, defendingTeam.PrimaryColor);
            }

            SwitchPossesion();
        }

        currentZone = FieldZone.OwnGoal;
        if (defendingTeam == AwayTeam) GetAwayTeamZone();

        if (attacking > defending) matchEvent = MatchEvent.Goal;
    }

    private void ResolveFreeKick()
    {        
        float attacking = (float)(attackingPlayer.Freekick + attackingPlayer.Strength) / 200;
        float defending = 0f;
        int bonusChance = GetAttributeBonus(attackingPlayer.Freekick);
        int roll = RollDice(20, 1, RollType.None, Mathf.FloorToInt(attacking * 5), bonusChance);      

        if (roll > 19)
        {
            attacking += attacking * 1.25f;
            DebugString += "\n ATACANTE GANHOU BONUS DE 25%";
        }
        else if(roll < 3)
        {
            Narration.UpdateNarration(attackingPlayer.FirstName + " " + attackingPlayer.LastName + " chuta na barreira",Color.gray);
            DebugString += "\n\nChutou na barreira\n\n_____________________________________\n\n";
            SwitchPossesion();
            keepDefender = true;
            matchEvent = MatchEvent.None;
        }
        else
        {
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
                keepDefender = true;

                DebugString += "\n\n" + defendingPlayer.FirstName + " " + defendingPlayer.LastName + " defende a crobanca de falta" + "\n\n_____________________________________\n\n";
                Narration.UpdateNarration(defendingPlayer.FirstName + " " + defendingPlayer.LastName + " defende a crobanca de falta!", defendingTeam.PrimaryColor);
                matchEvent = MatchEvent.None;
                keepDefender = true;
                SwitchPossesion();
            }

            if (attacking > defending)
            {
                matchEvent = MatchEvent.Goal;
            }
        }
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
        if (offensiveAction == PlayerData.PlayerAction.Pass || offensiveAction == PlayerData.PlayerAction.Dribble)
        { 
            switch (zone)
            {
                case FieldZone.OwnGoal:
                    zones.InsertRange(0, new int[] { 1, 2, 3 });
                    break;

                case FieldZone.LD:
                    zones.InsertRange(0, new int[] { 4, 5, 2 });
                    break;
                case FieldZone.CD:
                    zones.InsertRange(0, new int[] { 4, 5, 6 });
                    break;
                case FieldZone.RD:
                    zones.InsertRange(0, new int[] { 2, 5, 6 });
                    break;

                case FieldZone.LDM:
                    zones.InsertRange(0, new int[] { 5, 7, 8 });
                    break;
                case FieldZone.CDM:
                    zones.InsertRange(0, new int[] { 7, 8, 9 });
                    break;
                case FieldZone.RDM:
                    zones.InsertRange(0, new int[] { 5, 8, 9 });
                    break;

                case FieldZone.LM:
                    zones.InsertRange(0, new int[] { 8, 10, 11 });
                    break;
                case FieldZone.CM:
                    zones.InsertRange(0, new int[] { 10, 11, 12 });
                    break;
                case FieldZone.RM:
                    zones.InsertRange(0, new int[] { 8, 11, 12 });
                    break;

                case FieldZone.LAM:
                    zones.InsertRange(0, new int[] { 11, 13, 14 });
                    break;
                case FieldZone.CAM:
                    zones.InsertRange(0, new int[] { 13, 14, 15 });
                    break;
                case FieldZone.RAM:
                    zones.InsertRange(0, new int[] { 11, 14, 15 });
                    break;

                case FieldZone.LF:
                    zones.InsertRange(0, new int[] { 14, 16 });
                    break;
                case FieldZone.CF:
                    zones.InsertRange(0, new int[] { 13, 16, 15 });
                    break;
                case FieldZone.RF:
                    zones.InsertRange(0, new int[] { 14, 16 });
                    break;
                case FieldZone.Box:
                    zones.InsertRange(0, new int[] { 13, 14, 15, 16, 16, 16 });
                    break;
            } 
        }

        if (offensiveAction == PlayerData.PlayerAction.Cross)
        {
            switch (zone)
            {
                case FieldZone.OwnGoal:
                    zones.InsertRange(0, new int[] { 4, 5, 6, 7, 8, 9 });
                    break;

                case FieldZone.LD:
                    zones.InsertRange(0, new int[] { 7, 8, 10 });
                    break;
                case FieldZone.CD:
                    zones.InsertRange(0, new int[] { 7, 8, 9, 11 });
                    break;
                case FieldZone.RD:
                    zones.InsertRange(0, new int[] { 8, 9, 12 });
                    break;

                case FieldZone.LDM:
                    zones.InsertRange(0, new int[] { 10, 11, 13 });
                    break;
                case FieldZone.CDM:
                    zones.InsertRange(0, new int[] { 10, 11, 12, 14 });
                    break;
                case FieldZone.RDM:
                    zones.InsertRange(0, new int[] { 11, 12, 15 });
                    break;

                case FieldZone.LM:
                    zones.InsertRange(0, new int[] { 13, 14 });
                    break;
                case FieldZone.CM:
                    zones.InsertRange(0, new int[] { 13, 14, 15 });
                    break;
                case FieldZone.RM:
                    zones.InsertRange(0, new int[] { 14, 15 });
                    break;

                case FieldZone.LAM:
                case FieldZone.CAM:
                case FieldZone.RAM:
                    zones.InsertRange(0, new int[] { 13, 15, 16, 16, 16 });
                    break;
                case FieldZone.LF:
                case FieldZone.CF:
                case FieldZone.RF:
                case FieldZone.Box:
                    zones.InsertRange(0, new int[] { 16 });
                    break;
            }
        }

        int random = Random.Range(0, zones.Count);
        target = (FieldZone)zones[random];
        
        if (attackingTeam == AwayTeam) target = (FieldZone)((totalZones - 1) - (int)target);
        return target;
    }

    private PlayerData GetTopPlayerByAttribute(TeamData _team, PlayerData.PlayerAttributes _attribute)
    {
        PlayerData best = null;
        int higher = 0;
        foreach(PlayerData player in _team.Squad)
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
