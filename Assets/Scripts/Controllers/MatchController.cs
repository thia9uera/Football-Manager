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

    public void KickOff()
    {
        Reset();
        startBtn.SetActive(false);
        Narration.UpdateNarration("KICK OFF!", Color.gray);
        InvokeRepeating("DefineActions", 0.5f, 0.5f);
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
            Narration.UpdateNarration(attackingPlayer.FirstName + " VS " + defendingPlayer.FirstName, Color.gray);

            MarkingType marking = GetMarkingType();
            print("MARKING: " + marking.ToString());
            if (marking == MarkingType.Steal)
            {
                Narration.UpdateNarration(defendingPlayer.FirstName + " ROUBA A BOLA DE " + attackingPlayer.FirstName, defendingTeam.PrimaryColor);

                SwitchPossesion();
                return;
            }
            else
            {
                offensiveAction = GetOffensiveAction();
            }
        }
        

        // offensiveAction = GetOffensiveAction(attacking);
        //defensiveAction = GetDefensiveAction(defending);

        //TODO aquilo que tu sabe

    }

    private int RollDice(int _sides, int _amount = 1, RollType _rollType = RollType.None, int _bonus = 0, int _bonusChance = 0)
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

    private PlayerData.PlayerAction GetOffensiveAction()
    {
        PlayerData.PlayerAction action = PlayerData.PlayerAction.None;

        ActionChancePerZone zoneChance = actionChancePerZone.actionChancePerZones[(int)currentZone];

        float pass = zoneChance.Pass + attackingPlayer.Prob_Pass;
        float dribble = zoneChance.Dribble + attackingPlayer.Prob_Dribble;
        float crossing = zoneChance.Cross + attackingPlayer.Prob_Crossing;
        float shoot = zoneChance.Shot + attackingPlayer.Prob_Shoot;

        return action;
    }

    private MarkingType GetMarkingType()
    {
        MarkingType type = MarkingType.None;
        if (defendingPlayer == null) return type;

        float totalChance = 0f;
        totalChance = defendingPlayer.Prob_Marking;

        if (defendingPlayer.Speed > 70) totalChance += (100 - defendingPlayer.Speed) / 100;
        if (defendingPlayer.Vision > 70) totalChance += (100 - defendingPlayer.Vision) / 100;

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

    private PlayerData.PlayerAction GetDefensiveAction(PlayerData _player)
    {
        PlayerData.PlayerAction action = PlayerData.PlayerAction.None;

        //TODO decide what is player's action

        return action;
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
