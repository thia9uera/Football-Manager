﻿using System.Collections;
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
    private int currentZone = (int)FieldZone.CM;
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

    [SerializeField]
    private GameObject startBtn;


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
        InvokeRepeating("DefineActions", 1f, 1f);
    }

    private void UpdateMatch()
    {
        //LAST RESOLVE ENDED UP IN A GOAL
        if (isGoal)
        {
            if (isScorerAnnounced)
            {
                currentZone = 2;
                Field.UpdateFieldArea(currentZone);
                if (teamWithBall == 0) Narration.UpdateNarration(HomeTeam.Name + " kick off again.", HomeTeam.PrimaryColor);
                else Narration.UpdateNarration(AwayTeam.Name + " kick off again.", AwayTeam.PrimaryColor);
                isGoal = false;
                isGoalAnnounced = false;
                isScorerAnnounced = false;
                return;
            }
            else
            {
                if (teamWithBall == 0)
                {
                    Narration.UpdateNarration(GetAreaNarration(), HomeTeam.PrimaryColor);
                    teamWithBall = 1;
                }
                else
                {
                    Narration.UpdateNarration(GetAreaNarration(), AwayTeam.PrimaryColor);
                    teamWithBall = 0;
                }
                return;
            }
        }

        //HALF TIME
        if(matchTime == 45 && !isHalfTime)
        {
            isHalfTime = true;
            Narration.UpdateNarration("And it's half time.", Color.gray);
            teamWithBall = 1;
            currentZone = 2;
            Field.UpdateFieldArea(currentZone);
            return;
        }

        if(matchTime == 90)
        {
            CancelInvoke("UpdateMatch");
            string winner = "No winners today.";
            if (homeTeamScore > awayTeamScore) winner =  HomeTeam.Name + " won the match";
            else if (awayTeamScore > homeTeamScore) winner = AwayTeam.Name + " won the match";
            Narration.UpdateNarration("And it's the final whistle. " + winner, Color.gray);
            startBtn.SetActive(true);
            return;
        }

        matchTime++;
        Score.UpdateTime(matchTime);
        Field.UpdateFieldArea(currentZone);

        if (Resolve() == 0)
        {
            if (isGoal)
            {
                Narration.UpdateNarration(GetAreaNarration(), HomeTeam.PrimaryColor);
            }
            else Narration.UpdateNarration(matchTime + "' " + HomeTeam.Name + GetAreaNarration(), HomeTeam.PrimaryColor);
        }
        else
        {
            if (isGoal)
            {
                Narration.UpdateNarration(GetAreaNarration(), AwayTeam.PrimaryColor);
            }
            else Narration.UpdateNarration(matchTime + "' " + AwayTeam.Name + GetAreaNarration(), AwayTeam.PrimaryColor);
        }

        if (teamWithBall == 0) currentZone++;
        else currentZone--;
    }

    private void DefineActions()
    {
        PlayerData attacking = GetAttackingPlayer((FieldZone)currentZone);
        PlayerData defending= GetDefendingPlayer((FieldZone)currentZone);

        print("ATTACKING: " + attacking.FirstName + "   DEFENDING: " + defending.FirstName);
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
                if(chance <= Random.Range(0f, 1f)) players.Add(player);
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

        foreach (PlayerData player in attackingTeam.Squad)
        {
            chance = CalculatePresence(player, zone);

            if (chance >= 1f) players.Add(player);
            else
            {
                if (chance <= Random.Range(0f, 1f)) players.Add(player);
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
            float p = Random.Range(0f, stats);

            if(p > points)
            {
                points = p;
                activePlayer = player;
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
        if (chance < 1f) chance = _player.GetChancePerZone(_zone) * (playerTacticsBonus + teamTacticsBonus) * ((((float)_player.Speed + (float)_player.Vision) / 200) * (_player.Fatigue / 100));

        return chance;
    }

    private FieldZone GetAwayTeamZone()
    {
        int zone = (totalZones - 1) -  currentZone;

        return (FieldZone)zone;
    }
   
    private string GetAreaNarration()
    {
        string str = "";

        //HOME TEAM ATTACKING
        if (teamWithBall == 0)
        {
            if (isGoal && !isGoalAnnounced)
            {
                str = "<size=60>GOOOOAAAL!!!</size>";
                isGoalAnnounced = true;
            }
            else if (isGoal && isGoalAnnounced)
            {
                str = matchTime + "' " + HomeTeam.Squad[3].FirstName + " scored for " + HomeTeam.Name;
                isScorerAnnounced = true;
            }
            else if (currentZone == (int)FieldZone.CM) str = " has the ball in the Midfield.";
            else if (currentZone == (int)FieldZone.CAM) str = " is pressuring on the attack.";
            else if (currentZone == (int)FieldZone.CF) str = " is going for the goal...";
            else if (currentZone == (int)FieldZone.CDM) str = " managed to stop the attack.";
            else if (currentZone == (int)FieldZone.CD) str = " keeper saved the day.";
        }
        //AWAY TEAM ATTACKING
        else
        {
            if (isGoal && !isGoalAnnounced)
            {
                str = "<size=60>GOOOOAAAL!!!</size>";
                isGoalAnnounced = true;
            }
            else if (isGoal && isGoalAnnounced)
            {
                str = matchTime + "' " + AwayTeam.Squad[3].FirstName + " scored for " + AwayTeam.Name;
                isScorerAnnounced = true;
            }
            else if (currentZone == (int)FieldZone.CM) str = " has the ball in the Midfield.";
            else if (currentZone == (int)FieldZone.CDM) str = " is pressuring on the attack.";
            else if (currentZone == (int)FieldZone.CD) str = " is going for the goal...";
            else if (currentZone == (int)FieldZone.CAM) str = " managed to stop the attack.";
            else if (currentZone == (int)FieldZone.CF) str = " keeper saved the day.";
        }

        return str;
    }


    public int Resolve()
    {
        int home = 0;
        int away = 0;
            /*
        if (currentZone == (int)FieldZone.Midfield)
        {
            //HOME ATTACKING
            if (teamWithBall == 0)
            {
                home = Random.Range(0, HomeTeamSquad.CenterAtt);
                away = Random.Range(0, AwayTeamSquad.CenterDef);
                if (home > away) teamWithBall = 0;
                else teamWithBall = 1;
            }
            //AWAY ATTACKING
            else
            {
                home = Random.Range(0, HomeTeamSquad.CenterDef);
                away = Random.Range(0, AwayTeamSquad.CenterAtt);
                if (away > home) teamWithBall = 1;
                else teamWithBall = 0;
            }
        }

        if (currentZone == (int)FieldZone.AwayDefense)
        {
            //HOME ATTACKING
            if (teamWithBall == 0)
            {
                home = Random.Range(0, HomeTeamSquad.ForwardAtt);
                away = Random.Range(0, AwayTeamSquad.BackDef);
                if (home > away) teamWithBall = 0;
                else teamWithBall = 1;
            }
            //AWAY ATTACKING
            else
            {
                home = Random.Range(0, HomeTeamSquad.BackAtt);
                away = Random.Range(0, AwayTeamSquad.ForwardDef);
                if (away > home) teamWithBall = 1;
                else teamWithBall = 0;
            }
        }

        if (currentZone == (int)FieldZone.HomeDefense)
        {
            //HOME ATTACKING
            if (teamWithBall == 0)
            {
                home = Random.Range(0, HomeTeamSquad.BackDef);
                away = Random.Range(0, AwayTeamSquad.ForwardAtt);
                if (home > away) teamWithBall = 0;
                else teamWithBall = 1;
            }
            //AWAY ATTACKING
            else
            {
                home = Random.Range(0, HomeTeamSquad.BackDef);
                away = Random.Range(0, AwayTeamSquad.ForwardAtt);
                if (away > home) teamWithBall = 1;
                else teamWithBall = 0;
            }
        }

        if (currentZone == (int)FieldZone.AwayGoal)
        {
            //HOME ATTACKING
            if (teamWithBall == 0)
            {
                home = Random.Range(0, HomeTeamSquad.ForwardAtt);
                away = Random.Range(0, AwayTeamSquad.Keeper);
                if (home > away)
                {
                    isGoal = true;
                    homeTeamScore++;
                    Score.UpdateScore(HomeTeam.Name, homeTeamScore, ColorUtility.ToHtmlStringRGB(HomeTeam.PrimaryColor), AwayTeam.Name, awayTeamScore, ColorUtility.ToHtmlStringRGB(AwayTeam.PrimaryColor));
                }
                else teamWithBall = 1;
            }
            //AWAY ATTACKING
            else
            {
                home = Random.Range(0, HomeTeamSquad.ForwardDef);
                away = Random.Range(0, AwayTeamSquad.BackAtt + AwayTeamSquad.Keeper);
                if (away > home) teamWithBall = 1;
                else teamWithBall = 0;
            }
        }

        if (currentZone == (int)FieldZone.HomeGoal)
        {
            //HOME ATTACKING
            if (teamWithBall == 0)
            {
                home = Random.Range(0, HomeTeamSquad.BackAtt + HomeTeamSquad.Keeper);
                away = Random.Range(0, AwayTeamSquad.ForwardDef);
                if (home > away) teamWithBall = 0;
                else teamWithBall = 1;
            }
            //AWAY ATTACKING
            else
            {
                home = Random.Range(0, HomeTeamSquad.Keeper);
                away = Random.Range(0, AwayTeamSquad.ForwardAtt);
                if (away > home)
                {
                    isGoal = true;
                    awayTeamScore++;
                    Score.UpdateScore(HomeTeam.Name, homeTeamScore, ColorUtility.ToHtmlStringRGB(HomeTeam.PrimaryColor), AwayTeam.Name, awayTeamScore, ColorUtility.ToHtmlStringRGB(AwayTeam.PrimaryColor));
                }
                else teamWithBall = 0;
            }
        }
        */
        return teamWithBall;
           
}
 
}
