using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    public TeamData HomeTeam;
    public TeamData AwayTeam;

    public MatchScoreView Score;
    public MatchFieldView Field;
    public MatchTeamView HomeTeamSquad;
    public MatchTeamView AwayTeamSquad;
    public MatchNarration Narration;

    //Home = 0, Away = 1
    private int teamWithBall = 0;

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

    private int totalZones = 17;


    [SerializeField]
    private GameObject startBtn;

    [SerializeField]
    private FieldZone currentZone;

    private TeamData attackingTeam;
    private TeamData defendingTeam;
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

        HomeTeamSquad.Populate(_homeTeam.Squad);
        AwayTeamSquad.Populate(_awayTeam.Squad);
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
        PlayerData attacking = GetAttackingPlayer(currentZone);
        PlayerData defending = GetDefendingPlayer(currentZone);

        if(attacking == null && defending == null) Narration.UpdateNarration("BOLA SOBROU!", Color.gray);
        else if(attacking == null) Narration.UpdateNarration(defending.FirstName  + " DE BOAS" , defendingTeam.PrimaryColor);
        else if (defending == null) Narration.UpdateNarration(attacking.FirstName + " DE BOAS", attackingTeam.PrimaryColor);
        else Narration.UpdateNarration(attacking.FirstName + " VS " + defending.FirstName, Color.gray);
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
                if(chance <= Random.Range(0f, 1f) && chance > 0) players.Add(player);
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
                if (chance <= Random.Range(0f, 1f) && chance > 0) players.Add(player);
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
            int r = 1 + Random.Range(0, 20); //D20tão na mente
            if (r < 3) //se foi mto mal no dado já perde
            {
                print(player.FirstName + " MOSCOU FORTE!   -   " + r);
            }
            else if (r > 18) //o primeiro atleta que for bem ganha 
            {
                activePlayer = player;
                print(player.FirstName + " VINTOU!   -   " + r);
            }
            else //se não for nem muito bem nem muito mal, soma o rolar do dado com os stats
            {
                float p = stats + (r/20);
                print(player.FirstName + " TENTIOU!   -   " + p);
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
        float playerTacticsBonus = 0.5f;
        float teamTacticsBonus = 0.5f;

        chance = _player.GetChancePerZone(_zone);
        if (chance < 1f && chance > 0f)
        {
            chance = _player.GetChancePerZone(_zone) * (playerTacticsBonus + teamTacticsBonus) * ((((float)_player.Speed + (float)_player.Vision) / 200) * (_player.Fatigue / 100));
        }
        return chance;
    }

    private FieldZone GetAwayTeamZone()
    {
        int zone = (totalZones - 1) -  (int)currentZone;

        return (FieldZone)zone;
    }
   
 
}
