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

    private int matchTime = 0;
    private int homeTeamScore = 0;
    private int awayTeamScore = 0;

    private bool isGoal = false;
    private bool isGoalAnnounced = false;
    private bool isScorerAnnounced = false;
    private bool isHalfTime = false;

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

    private void Reset()
    {
        matchTime = 0;
        homeTeamScore = 0;
        awayTeamScore = 0;
        isHalfTime = false;
        Narration.Reset();
        Score.UpdateTime(matchTime);
        Score.UpdateScore(HomeTeam.Name, homeTeamScore, ColorUtility.ToHtmlStringRGB(HomeTeam.PrimaryColor), AwayTeam.Name, awayTeamScore, ColorUtility.ToHtmlStringRGB(AwayTeam.PrimaryColor));
    }

    private bool hasStarted = false;
    public void HandleStartButton()
    {
        hasStarted = !hasStarted;

        if(hasStarted)
        {
            KickOff();
        }
        else
        {
            CancelInvoke();
        }
    }

    public void KickOff()
    {
        Reset();
        //startBtn.SetActive(false);
        Narration.UpdateNarration("KICK OFF!", Color.gray);
        InvokeRepeating("DefineActions", 1f, 1f);

        DebugString = "KICK OFF! \n \n";
    }

    private void DefineActions()
    {
        attackingPlayer = GetAttackingPlayer(currentZone);
        defendingPlayer = GetDefendingPlayer(currentZone);

        if (attackingPlayer == null && defendingPlayer == null)
        {
            Narration.UpdateNarration("BOLA SOBROU!", Color.gray);
        }
        else if (defendingPlayer == null)
        {
            Narration.UpdateNarration(attackingPlayer.FirstName + " SOZINHO NA JOGADA", attackingTeam.PrimaryColor);
        }
        else if(attackingPlayer == null)
        {
            Narration.UpdateNarration(defendingTeam.Name + " PERDE A POSSE DE BOLA", defendingTeam.PrimaryColor);
        }
        else
        {
            //Narration.UpdateNarration(attackingPlayer.FirstName + " VS " + defendingPlayer.FirstName, Color.gray);

            MarkingType marking = GetMarkingType();
            if (marking == MarkingType.Steal)
            {
                DebugString += "\n ________________________________\n ROUBADA DE BOLA! \n ________________________________\n \n";
                Narration.UpdateNarration(defendingPlayer.FirstName + " ROUBA A BOLA DE " + attackingPlayer.FirstName, defendingTeam.PrimaryColor);

                SwitchPossesion();
                return;
            }
            else
            {
                if(marking == MarkingType.Close) DebugString += "\n MARCACAO DE PERTO. \n \n";
                else DebugString += "\n MARCACAO A DISTACIA. \n \n";
                offensiveAction = GetOffensiveAction(marking);
                if (IsActionSuccessful(marking))
                {
                    switch (offensiveAction)
                    {
                        case PlayerData.PlayerAction.Pass:
                            DebugString += "PASSOU A BOLA! \n ________________________________\n";
                            Narration.UpdateNarration(attackingPlayer.FirstName + " PASSA A BOLA", attackingTeam.PrimaryColor);
                            break;
                        case PlayerData.PlayerAction.Dribble:
                            DebugString += "DRIBLOU! \n ________________________________\n";
                            Narration.UpdateNarration(attackingPlayer.FirstName + " DRIBLA " + defendingPlayer.FirstName, attackingTeam.PrimaryColor);
                            break;
                        case PlayerData.PlayerAction.Cross:
                            DebugString += "CRUZOU! \n ________________________________\n";
                            Narration.UpdateNarration(attackingPlayer.FirstName + " CRUZA A BOLA", attackingTeam.PrimaryColor);
                            break;
                        case PlayerData.PlayerAction.Shot:
                            DebugString += "CHUTOU! \n ________________________________\n";
                            Narration.UpdateNarration(attackingPlayer.FirstName + " TENTA O CHUTE...", attackingTeam.PrimaryColor);
                            break;
                    }
                }

                else
                {
                    switch (offensiveAction)
                    {
                        case PlayerData.PlayerAction.Pass:
                            DebugString += "PASSE BLOQUEADO! \n ________________________________\n";
                            Narration.UpdateNarration(defendingPlayer.FirstName + " BLOQUEIA O PASSE", defendingTeam.PrimaryColor);
                            break;
                        case PlayerData.PlayerAction.Dribble:
                            DebugString += "DRIBLE DESARMADO! \n ________________________________\n";
                            Narration.UpdateNarration(defendingPlayer.FirstName + " PARA O DRIBLE DE " + attackingPlayer.FirstName, defendingTeam.PrimaryColor);
                            break;
                        case PlayerData.PlayerAction.Cross:
                            DebugString += "CRUZAMENTO BLOQUEADO! \n ________________________________\n";
                            Narration.UpdateNarration(defendingPlayer.FirstName + " IMPEDE O CRUZAMENTO", defendingTeam.PrimaryColor);
                            break;
                        case PlayerData.PlayerAction.Shot:
                            DebugString += "CHUTE BLOQUEADO! \n ________________________________\n";
                            Narration.UpdateNarration(defendingPlayer.FirstName + " BLOQUEIA O CHUTE DE " + attackingPlayer.FirstName, defendingTeam.PrimaryColor);
                            break;
                    }
                    SwitchPossesion();
                }
            }
        }
        

        // offensiveAction = GetOffensiveAction(attacking);
        //defensiveAction = GetDefensiveAction(defending);

        //TODO aquilo que tu sabe

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
        bool success = false;
        float attacking = 0f;
        float defending = 0f;


        switch(offensiveAction)
        {
            case PlayerData.PlayerAction.Pass:
                attacking = (attackingPlayer.Passing / 100) * (attackingPlayer.Fatigue / 100);
                if (_marking == MarkingType.Close) attacking = attacking * 0.75f;
                defending = (defendingPlayer.Blocking / 100) * (defendingPlayer.Fatigue / 100);
                break;

            case PlayerData.PlayerAction.Dribble:
                attacking = (attackingPlayer.Dribbling / 100) * (attackingPlayer.Fatigue / 100);
                if (_marking == MarkingType.Close) attacking = attacking * 0.5f;
                defending = (defendingPlayer.Tackling / 100) * (defendingPlayer.Fatigue / 100);
                break;

            case PlayerData.PlayerAction.Cross:
                attacking = (attackingPlayer.Crossing / 100) * (attackingPlayer.Fatigue / 100);
                if (_marking == MarkingType.Close) attacking = attacking * 0.5f;
                defending = (defendingPlayer.Blocking / 100) * (defendingPlayer.Fatigue / 100);
                break;

            case PlayerData.PlayerAction.Shot:
                attacking = (attackingPlayer.Shooting / 100) * (attackingPlayer.Fatigue / 100);
                if (_marking == MarkingType.Close) attacking = attacking * 0.5f;
                defending = (defendingPlayer.Blocking / 100) * (defendingPlayer.Fatigue / 100);
                break;
        }

        attacking = RollDice(20, 1, RollType.None, Mathf.FloorToInt(attacking));
        defending = RollDice(20, 1, RollType.None, Mathf.FloorToInt(defending));

        if (attacking < 3 || attacking <= defending || defending > 20 ) success = false;
        else success = true;

        return success;
    }

    private PlayerData.PlayerAction GetOffensiveAction(MarkingType _marking)
    {
        PlayerData.PlayerAction action = PlayerData.PlayerAction.None;
        FieldZone zone = currentZone;
        if (attackingTeam == AwayTeam) zone = GetAwayTeamZone();

        float higher = 0f;

        ActionChancePerZone zoneChance = actionChancePerZone.actionChancePerZones[(int)zone];

        float pass = zoneChance.Pass + attackingPlayer.Prob_Pass;
        if (attackingPlayer.Passing > 70) pass += ((100 - attackingPlayer.Passing) / 100) * (attackingPlayer.Fatigue / 100);
        if (_marking == MarkingType.Close) pass = pass * 2;
        //pass = RollDice(20, 1, RollType.None, Mathf.FloorToInt(pass));
        if (pass > higher) action = PlayerData.PlayerAction.Pass;

        float dribble = zoneChance.Dribble + attackingPlayer.Prob_Dribble;
        if (attackingPlayer.Dribbling > 70) dribble += ((100 - attackingPlayer.Dribbling) / 100) * (attackingPlayer.Fatigue / 100);
        if (_marking == MarkingType.Close) dribble = dribble * 0.5f;
        else if (_marking == MarkingType.Distance) dribble = dribble * 2;
        //dribble = RollDice(20, 1, RollType.None, Mathf.FloorToInt(dribble));
        if (dribble > higher) action = PlayerData.PlayerAction.Dribble;

        float cross = zoneChance.Cross + attackingPlayer.Prob_Crossing;
        if (attackingPlayer.Crossing > 70) cross += ((100 - attackingPlayer.Crossing) / 100) * (attackingPlayer.Fatigue / 100);
        if (_marking == MarkingType.Close) cross = cross * 0.5f;
        //cross = RollDice(20, 1, RollType.None, Mathf.FloorToInt(cross));
        if (cross > higher) action = PlayerData.PlayerAction.Cross;

        float shoot = zoneChance.Shot + attackingPlayer.Prob_Shoot;
        if (attackingPlayer.Shooting > 70 && zoneChance.Shot > 0) shoot += ((100 - attackingPlayer.Shooting) / 100) * (attackingPlayer.Fatigue / 100);
        if (_marking == MarkingType.Close) shoot = shoot * 0.5f;
        else if (_marking == MarkingType.Distance) shoot = shoot * 1.5f;
        //shoot = RollDice(20, 1, RollType.None, Mathf.FloorToInt(shoot));
        if (shoot > higher) action = PlayerData.PlayerAction.Shot;

        DebugString += "<size=28>" + attackingPlayer.FirstName + " " + attackingPlayer.LastName + "</size> \n \n";
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

        if (defendingPlayer.Speed > 70) totalChance += ((100 - defendingPlayer.Speed) / 100) * (defendingPlayer.Fatigue/100);
        if (defendingPlayer.Vision > 70) totalChance += ((100 - defendingPlayer.Vision) / 100) * (defendingPlayer.Fatigue / 100);

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

    private int GetRandomZone()
    {
        int zone = Random.Range(0, totalZones);

        return zone;
    }

    private PlayerData GetAttackingPlayer(FieldZone _zone)
    {
        FieldZone zone = _zone;
        if (attackingTeam == AwayTeam) zone = GetAwayTeamZone();

        float chance = 0f;

        List<PlayerData> players = new List<PlayerData>();

        foreach (PlayerData player in attackingTeam.Squad)
        {
            chance = CalculatePresence(player, zone);
            if(chance >= 1f) players.Add(player);
            else 
            {
                if(chance > 0 && chance <= Random.Range(0f, 1f)) players.Add(player);
            }
        }
        return GetActivePlayer(players);
    }

    private PlayerData GetDefendingPlayer(FieldZone _zone)
    {
        FieldZone zone = _zone;
        if (defendingTeam == AwayTeam) zone = GetAwayTeamZone();

        float chance = 0f;

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
   
    private void SwitchPossesion()
    {
        if(attackingTeam = HomeTeam)
        {
            attackingTeam = AwayTeam;
            defendingTeam = HomeTeam;
        }
        else
        {
            defendingTeam = AwayTeam;
            attackingTeam = HomeTeam;
        }
    }
}
