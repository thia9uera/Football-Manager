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
    private TackleChancePerZoneData tackleChancePerZone;

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

    private bool isGoal = false;
    private bool isGoalAnnounced = false;
    private bool isScorerAnnounced = false;
    private bool isHalfTime = false;
    private bool secondHalfStarted = false;

    public string DebugString;

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


        InvokeRepeating("DefineActions", 1f, 1f);  
    }

    private void DefineActions()
    {
        if(isGoal)
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
                isGoal = false;
                isGoalAnnounced = false;
                isScorerAnnounced = false;

                SwitchPossesion();
                currentZone = FieldZone.CM;
                attackingBonus = 1f;

                Narration.UpdateNarration("RECOMECA A PARTIDA", Color.gray);
                return;
            }
        }

        if (matchTime >= 45 && !isHalfTime)
        {
            isHalfTime = true;
            Narration.UpdateNarration("FIM DO PRIMEIRO TEMPO", Color.gray);
            return;
        }
        if (isHalfTime && !secondHalfStarted)
        {
            secondHalfStarted = true;
            Narration.UpdateNarration("COMECA SEGUNDO TEMPO", Color.gray);
            return;
        }
        else if (matchTime >= 90)
        {
            Narration.UpdateNarration("TERMINA A PARTIDA", Color.gray);
            CancelInvoke();
            startBtn.SetActive(false);
            return;
        }


        matchTime++;
        Score.UpdateTime(matchTime);
        

        Field.UpdateFieldArea((int)currentZone);

        if (!keepAttacker) attackingPlayer = GetAttackingPlayer(currentZone);
        if (keepDefender) attackingPlayer = defendingPlayer;
        defendingPlayer = GetDefendingPlayer(currentZone);

        keepAttacker = false;
        keepDefender = false;

        if (attackingPlayer == null && defendingPlayer == null)
        {
            Narration.UpdateNarration("BOLA SOBROU!", Color.gray);
        }
        else if(attackingPlayer == null)
        {
            Narration.UpdateNarration(attackingTeam.Name + " PERDE A POSSE DE BOLA", attackingTeam.PrimaryColor);
            keepDefender = true;
            SwitchPossesion();
        }
        else
        {
            if(defendingPlayer == null) Narration.UpdateNarration(attackingPlayer.FirstName + " SOZINHO NA JOGADA", attackingTeam.PrimaryColor);
            else DebugString += "\n<size=28>" + attackingPlayer.FirstName + " " + attackingPlayer.LastName + " VS " + defendingPlayer.FirstName + " " + defendingPlayer.LastName + " (" + currentZone + ")</size> \n";

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
                if(marking == MarkingType.Close) DebugString += "\nMARCACAO DE PERTO \n \n";
                else DebugString += "\nMARCACAO A DISTANCIA \n \n";
                offensiveAction = GetOffensiveAction(marking);
                if (IsActionSuccessful(marking))
                {
                    lastActionSuccessful = true;
                    if (marking == MarkingType.Close) attackingBonus += 0.1f;
                    else if (marking == MarkingType.Distance) attackingBonus += 0.05f;
                    else if (marking == MarkingType.None) attackingBonus += 0.01f;

                    switch (offensiveAction)
                    {
                        case PlayerData.PlayerAction.Pass:
                            DebugString += "PASSOU A BOLA! \n ________________________________\n";
                            Narration.UpdateNarration(attackingPlayer.FirstName + " PASSA A BOLA", attackingTeam.PrimaryColor);
                            currentZone = GetTargetZone();
                            break;
                        case PlayerData.PlayerAction.Dribble:
                            DebugString += "DRIBLOU! \n ________________________________\n";
                            Narration.UpdateNarration(attackingPlayer.FirstName + " DRIBLA " + defendingPlayer.FirstName, attackingTeam.PrimaryColor);
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

                    switch (offensiveAction)
                    {
                        case PlayerData.PlayerAction.Pass:
                            DebugString += "PASSE BLOQUEADO! \n ________________________________\n";
                            Narration.UpdateNarration(defendingPlayer.FirstName + " BLOQUEIA O PASSE", defendingTeam.PrimaryColor);
                            keepDefender = true;
                            break;
                        case PlayerData.PlayerAction.Dribble:
                            DebugString += "DRIBLE DESARMADO! \n ________________________________\n";
                            Narration.UpdateNarration(defendingPlayer.FirstName + " PARA O DRIBLE DE " + attackingPlayer.FirstName, defendingTeam.PrimaryColor);
                            keepDefender = true;
                            break;
                        case PlayerData.PlayerAction.Cross:
                            DebugString += "CRUZAMENTO BLOQUEADO! \n ________________________________\n";
                            Narration.UpdateNarration(defendingPlayer.FirstName + " IMPEDE O CRUZAMENTO", defendingTeam.PrimaryColor);
                            break;
                        case PlayerData.PlayerAction.Shot:
                            DebugString += "CHUTE BLOQUEADO! \n ________________________________\n";
                            Narration.UpdateNarration(defendingPlayer.FirstName + " BLOQUEIA O CHUTE DE " + attackingPlayer.FirstName, defendingTeam.PrimaryColor);
                            break;
                        case PlayerData.PlayerAction.Header:
                            DebugString += "JOGADA AEREA DESARMADA! \n ________________________________\n";
                            Narration.UpdateNarration(defendingPlayer.FirstName + " PULA MAIS ALTO QUE " + attackingPlayer.FirstName, defendingTeam.PrimaryColor);
                            break;
                    }

                    SwitchPossesion();
                }
            }
        }
    }

    private int RollDice(int _sides, int _amount = 1, RollType _rollType = RollType.None, int _bonus = 0, int _bonusChance = 101)
    {
        int n = 0;
        int roll;
        List<int> rolls = new List<int>();

        while (n < _amount)
        {
            roll = 1 + Random.Range(0, _sides);
            if (Random.Range(1, 100) < _bonusChance) roll += _bonus;
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
            if (Random.Range(1, 100) < _bonusChance) roll += _bonus;
            rolls.Add(roll);
            return rolls.Sum();
        }
        else return rolls.Sum();
    }

    private bool IsActionSuccessful(MarkingType _marking)
    {
        if (_marking == MarkingType.None) return true;

        bool success = false;   
        float attacking = 0f;
        float defending = 0f;

        switch(offensiveAction)
        {
            case PlayerData.PlayerAction.Pass:
                defensiveAction = PlayerData.PlayerAction.Block;
                attacking = ((float)attackingPlayer.Passing / 100) * ((float)attackingPlayer.Fatigue / 100) * attackingBonus;
                attacking += ((float)(attackingPlayer.Agility + attackingPlayer.Vision + attackingPlayer.Teamwork) / 300) * ((float)attackingPlayer.Fatigue / 100);
                if (_marking == MarkingType.Close) attacking = attacking * 0.75f;
                defending = ((float)defendingPlayer.Blocking / 100) * ((float)defendingPlayer.Fatigue / 100);
                defending += ((float)(defendingPlayer.Agility + defendingPlayer.Vision) / 200) * ((float)defendingPlayer.Fatigue / 100);
                break;

            case PlayerData.PlayerAction.Dribble:
                defensiveAction = PlayerData.PlayerAction.Tackle;
                attacking = ((float)attackingPlayer.Dribbling / 100) * ((float)attackingPlayer.Fatigue / 100) * attackingBonus;
                attacking += ((float)(attackingPlayer.Agility + attackingPlayer.Speed) / 200) * ((float)attackingPlayer.Fatigue / 100);
                if (_marking == MarkingType.Close) attacking = attacking * 0.5f;
                defending = ((float)defendingPlayer.Tackling / 100) * ((float)defendingPlayer.Fatigue / 100);
                defending += ((float)(defendingPlayer.Agility + defendingPlayer.Speed) / 200) * ((float)defendingPlayer.Fatigue / 100);
                break;

            case PlayerData.PlayerAction.Cross:
                defensiveAction = PlayerData.PlayerAction.Block;
                attacking = ((float)attackingPlayer.Crossing / 100) * ((float)attackingPlayer.Fatigue / 100) * attackingBonus;
                attacking += ((float)(attackingPlayer.Agility + attackingPlayer.Vision + attackingPlayer.Teamwork) / 300) * ((float)attackingPlayer.Fatigue / 100);
                if (_marking == MarkingType.Close) attacking = attacking * 0.5f;
                defending = ((float)defendingPlayer.Blocking / 100) * ((float)defendingPlayer.Fatigue / 100);
                defending += ((float)(defendingPlayer.Agility + defendingPlayer.Vision) / 200) * ((float)defendingPlayer.Fatigue / 100);
                break;

            case PlayerData.PlayerAction.Shot:
                defensiveAction = PlayerData.PlayerAction.Block;
                attacking = ((float)attackingPlayer.Shooting / 100) * ((float)attackingPlayer.Fatigue / 100) * attackingBonus;
                attacking += ((float)(attackingPlayer.Agility + attackingPlayer.Strength) / 200) * ((float)attackingPlayer.Fatigue / 100);
                if (_marking == MarkingType.Close) attacking = attacking * 0.5f;
                defending = ((float)defendingPlayer.Blocking / 100) * ((float)defendingPlayer.Fatigue / 100);
                defending += ((float)(defendingPlayer.Agility + defendingPlayer.Vision + defendingPlayer.Speed) / 200) * ((float)defendingPlayer.Fatigue / 100);
                break;

            case PlayerData.PlayerAction.Header:
                defensiveAction = PlayerData.PlayerAction.Block;
                attacking = ((float)attackingPlayer.Heading / 100) * ((float)attackingPlayer.Fatigue / 100) * attackingBonus;
                attacking += ((float)(attackingPlayer.Agility + attackingPlayer.Strength) / 200) * ((float)attackingPlayer.Fatigue / 100);
                if (_marking == MarkingType.Close) attacking = attacking * 0.5f;
                defending = ((float)(defendingPlayer.Heading + defendingPlayer.Blocking) / 200) * ((float)defendingPlayer.Fatigue / 100);
                defending += ((float)(defendingPlayer.Agility + defendingPlayer.Vision) / 200) * ((float)defendingPlayer.Fatigue / 100);
                break;
        }

        int attackRoll = RollDice(20, 1, RollType.None, Mathf.FloorToInt(attacking * 10));
        if (attackRoll == 20) attacking = attacking * 2;
        else if (attackRoll < 3) attacking = attacking / 2;

        int defenseRoll = RollDice(20, 1, RollType.None, Mathf.FloorToInt(defending * 10));
        if (defenseRoll == 20) defending = defending * 2;
        else if (defenseRoll < 3) defending = defending / 2;

        DebugString += "\nAtacante rolou " + attacking;
        DebugString += "\nDefensor rolou " + defending + "\n\n";

        if (attacking > defending) success = true;

        return success;
    }

    private void ResolveShot(MarkingType _marking)
    {
        float attacking = 0f;
        float defending = 0f;
        float distanceModifier = 1f;
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

        attacking = ((((float)attackingPlayer.Shooting / 100) + ((float)attackingPlayer.Strength / 100))) * ((float)attackingPlayer.Fatigue / 100) * attackingBonus * distanceModifier;

        if(offensiveAction == PlayerData.PlayerAction.Header)
        {
            attacking = ((((float)attackingPlayer.Heading / 100) + ((float)attackingPlayer.Strength / 100))) * ((float)attackingPlayer.Fatigue / 100) * attackingBonus * distanceModifier;
        }

        if (_marking == MarkingType.Close) attacking = attacking * 0.5f;

        if (RollDice(20, 1, RollType.None, (int)attacking) > 19)
        {
            attacking += attacking * 1.25f;
            DebugString += "\n ATACANTE GANHOU BONUS DE 25%";
        }

        defending = ((((float)defendingPlayer.Goalkeeping / 100) + ((float)defendingPlayer.Agility / 100))) * ((float)defendingPlayer.Fatigue / 100);
        if (RollDice(20, 1, RollType.None, (int)defending) > 18)
        {
            defending += defending * 1.50f;
            DebugString += "\n GOLEIRO GANHOU BONUS DE 50%";
        }

        DebugString += "\nAtacante: " + attacking + "  Goleiro: " + defending;
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

        if (attacking > defending) isGoal = true;
    }

    private PlayerData.PlayerAction GetOffensiveAction(MarkingType _marking)
    {
        FieldZone zone = currentZone;
        if (attackingTeam == AwayTeam) zone = GetAwayTeamZone();

        if (offensiveAction == PlayerData.PlayerAction.Cross && zone == FieldZone.Box)
        {
            return PlayerData.PlayerAction.Header;
        }

        PlayerData.PlayerAction action = PlayerData.PlayerAction.None;
 
        float higher = 0f;

        ActionChancePerZone zoneChance = actionChancePerZone.actionChancePerZones[(int)zone];

        float pass = zoneChance.Pass/100 + attackingPlayer.Prob_Pass;
        if(RollDice(20, 1, RollType.None, Mathf.FloorToInt(pass)) > 18) pass += 1;
        if (pass > higher)
        {
            higher = pass;
            action = PlayerData.PlayerAction.Pass;
        }

        float dribble = zoneChance.Dribble/100 + attackingPlayer.Prob_Dribble;
        if (attackingPlayer.Dribbling > 70) dribble += ((100 - (float)attackingPlayer.Dribbling) / 100) * ((float)attackingPlayer.Fatigue / 100);
        if (_marking == MarkingType.Close) dribble = dribble * 0.5f;
        else if (_marking == MarkingType.Distance) dribble = dribble * 2;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt(pass)) > 18) dribble += 1;
        if (dribble > higher)
        {
            higher = dribble;
            action = PlayerData.PlayerAction.Dribble;
        }

        float cross = zoneChance.Cross/100 + attackingPlayer.Prob_Crossing;
        if (attackingPlayer.Crossing > 70) cross += ((100 - (float)attackingPlayer.Crossing) / 100) * ((float)attackingPlayer.Fatigue / 100);
        if (_marking == MarkingType.Close) cross = cross * 0.5f;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt(cross)) > 18) cross += 1;
        if (cross > higher)
        {
            higher = cross;
            action = PlayerData.PlayerAction.Cross;
        }

        float shoot = zoneChance.Shot/100 + attackingPlayer.Prob_Shoot;
        if (attackingPlayer.Shooting > 70 && zoneChance.Shot > 0) shoot += ((100 - (float)attackingPlayer.Shooting) / 100) * ((float)attackingPlayer.Fatigue / 100);
        if (_marking == MarkingType.Close) shoot = shoot * 0.5f;
        else if (_marking == MarkingType.Distance) shoot = shoot * 1.5f;
        if (RollDice(20, 1, RollType.None, Mathf.FloorToInt(shoot)) > 18) shoot += 1;
        if (shoot > higher)
        {
            higher = shoot;
            action = PlayerData.PlayerAction.Shot;
        }


        DebugString += "Pass: " + pass + "\n";
        DebugString += "Dribble: " + dribble + "\n";
        DebugString += "Cross: " + cross + "\n";
        DebugString += "Shoot: " + shoot + "\n \n";

        return action;
    }

    private MarkingType GetMarkingType()
    {
        MarkingType type = MarkingType.None;
        if (defendingPlayer == null) return type;

        float totalChance = 0f;
        totalChance = defendingPlayer.Prob_Marking;

        if (defendingPlayer.Speed > 70) totalChance += ((100 - (float)defendingPlayer.Speed) / 100) * ((float)defendingPlayer.Fatigue/100);
        if (defendingPlayer.Vision > 70) totalChance += ((100 - (float)defendingPlayer.Vision) / 100) * (float)(defendingPlayer.Fatigue / 100);

        float r = RollDice(20, 1, RollType.None, Mathf.FloorToInt(totalChance));

        if(r >= 20)
        {
            type = MarkingType.Steal;
        }
        else if (r > 15)
        {
            type = MarkingType.Close;
        }
        else if(r > 3)
        {
            type = MarkingType.Distance;
        }

        return type;
    }



    private PlayerData GetAttackingPlayer(FieldZone _zone)
    {
        FieldZone zone = _zone;
        if (attackingTeam == AwayTeam) zone = GetAwayTeamZone();

        float chance = 0f;
        float higher = 0f;
        bool forcePlayer = false;
        if (offensiveAction == PlayerData.PlayerAction.Pass || offensiveAction == PlayerData.PlayerAction.Cross && lastActionSuccessful) forcePlayer = true;

        List<PlayerData> players = new List<PlayerData>();

        foreach (PlayerData player in attackingTeam.Squad)
        {
            chance = CalculatePresence(player, zone);
            if (forcePlayer)
            {
                if (chance > higher && player != attackingPlayer)
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
            if (chance >= 1f ) players.Add(player);
            else
            {
                if (chance > 0 && chance <= Random.Range(0f, 1f)) players.Add(player);
            }
        }

        if(forcePlayerOut)
        {
            if (players.Contains(defendingPlayer)) players.Remove(defendingPlayer);
        }

        return GetActivePlayer(players);
    }

    private PlayerData GetActivePlayer(List<PlayerData> list)
    {
        PlayerData activePlayer = null;
        float points = 0f;

        foreach(PlayerData player in list )
        {
            float stats = ((((float)player.Speed + (float)player.Vision) / 200) * (player.Fatigue / 100));

            int r = RollDice(20);

            if (r < 3) //se foi mto mal no dado já perde
            {
                
            }
            else if (r == 20) //o primeiro atleta que for bem ganha 
            {
                activePlayer = player;
            }
            else //se não for nem muito bem nem muito mal, soma o rolar do dado com os stats
            {
                float p = stats + (r/20);
                if (p > points)
                {
                    points = p;
                    activePlayer = player;
                }
            }
        }

        return activePlayer;
    }

    private float CalculatePresence(PlayerData _player, FieldZone _zone)
    {
        float chance = 0f;

        chance = _player.GetChancePerZone(_zone);
        if (chance < 1f && chance > 0f)
        {
            chance = _player.GetChancePerZone(_zone)  * ((((float)_player.Speed + (float)_player.Vision) / 200) * (_player.Fatigue / 100));
        }
        return chance;
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

        if (offensiveAction == PlayerData.PlayerAction.Pass || offensiveAction == PlayerData.PlayerAction.Dribble)
        { 
            switch (zone)
            {
                case FieldZone.OwnGoal:
                    target = GetRandomZone(1, 4);
                    break;

                case FieldZone.LD:
                case FieldZone.CD:
                case FieldZone.RD:
                    target = GetRandomZone(4, 7);
                    break;

                case FieldZone.LDM:
                case FieldZone.CDM:
                case FieldZone.RDM:
                    target = GetRandomZone(7, 10);
                    break;

                case FieldZone.LM:
                case FieldZone.CM:
                case FieldZone.RM:
                    target = GetRandomZone(10, 13);
                    break;

                case FieldZone.LAM:
                case FieldZone.CAM:
                case FieldZone.RAM:
                    target = GetRandomZone(13, 16);
                    break;

                case FieldZone.LF:
                case FieldZone.CF:
                case FieldZone.RF:
                    target = FieldZone.Box;
                    break;
            }
        }

        if (offensiveAction == PlayerData.PlayerAction.Cross)
        {
            switch (zone)
            {
                case FieldZone.OwnGoal:
                    target = GetRandomZone(4, 10);
                    break;

                case FieldZone.LD:
                case FieldZone.CD:
                case FieldZone.RD:
                    target = GetRandomZone(7, 13);
                    break;

                case FieldZone.LDM:
                case FieldZone.CDM:
                case FieldZone.RDM:
                    target = GetRandomZone(10, 16);
                    break;

                case FieldZone.LM:
                case FieldZone.CM:
                case FieldZone.RM:
                    target = GetRandomZone(13, totalZones);
                    break;

                case FieldZone.LAM:
                case FieldZone.CAM:
                case FieldZone.RAM:
                case FieldZone.LF:
                case FieldZone.CF:
                case FieldZone.RF:
                    target = FieldZone.Box;
                    break;
            }
        }

        if (attackingTeam == AwayTeam) target = (FieldZone)((totalZones - 1) - (int)target);
        return target;
    }

    private FieldZone GetRandomZone(int _minZone = 0, int _maxZone = 17)
    {
        int zone = Random.Range(_minZone, _maxZone);

        return (FieldZone)zone;
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
