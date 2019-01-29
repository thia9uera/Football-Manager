using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    //Names given by Home Team perspective
    public enum Zone
    {
        OwnGoal = 0,
        BLD, BRD,
        LD, LCD, CD, RCD, RD,
        LDM, LCDM, CDM, RCDM, RDM,
        LM, LCM, CM, RCM, RM,
        LAM, LCAM, CAM, RCAM, RAM,
        LF, LCF, CF, RCF, RF,
        ALF, ARF,
        Box,
    }
    const int totalZones = 31;
    public List<Vector2> Matrix;
    [HideInInspector]
    public Zone CurrentZone, LastZone;

    public PosChanceData[] TeamStrategies;

    [Space(5)]
    public FormationData[] TeamFormations;

    public Zone GetAwayTeamZone(Zone _zone)
    {
        int zone = (totalZones - 1) - (int)_zone;

        return (Zone)zone;
    }

    public float CalculatePresence(PlayerData _player, Zone _zone, TeamAttributes.TeamStrategy _teamStrategy)
    {
        float chance = _player.GetChancePerZone(_zone, GetTeamStrategyZones(_teamStrategy, _zone));

        if (chance < 1f && chance > 0f)
        {
            chance *= (float)(_player.Speed + _player.Vision) / 200;
            chance *= _player.FatigueModifier();
        }
        return chance;
    }

    public Zone GetTargetZone(Zone _currentZone, MatchControllerRefactor.MatchEvent _event, PlayerData.PlayerAction _action, TeamAttributes.TeamStrategy _teamStrategy)
    {
        Zone target = _currentZone;
        Zone zone = _currentZone;
        List<KeyValuePair<Zone, float>> list = new List<KeyValuePair<Zone, float>>();

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

        if (_event == MatchControllerRefactor.MatchEvent.Goalkick)
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

        else if (_action == PlayerData.PlayerAction.Pass || _action == PlayerData.PlayerAction.Dribble || _action == PlayerData.PlayerAction.Sprint)
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

        else if (_action == PlayerData.PlayerAction.Cross || _action == PlayerData.PlayerAction.LongPass)
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


        Team_Strategy strategy = MainController.Instance.TeamStrategyData.team_Strategys[(int)_teamStrategy];
        _OwnGoal *= strategy.Target_OwnGoal;
        _BLD *= strategy.Target_BLD;
        _BRD *= strategy.Target_BRD;

        _LD *= strategy.Target_LD;
        _LCD *= strategy.Target_LCD;
        _CD *= strategy.Target_CD;
        _RCD *= strategy.Target_RCD;
        _RD *= strategy.Target_RD;

        _LDM *= strategy.Target_LDM;
        _LCDM *= strategy.Target_LCDM;
        _CDM *= strategy.Target_CDM;
        _RCDM *= strategy.Target_RCDM;
        _RDM *= strategy.Target_RDM;

        _LM *= strategy.Target_LM;
        _LCM *= strategy.Target_LCM;
        _CM *= strategy.Target_CM;
        _RCM *= strategy.Target_RCM;
        _RM *= strategy.Target_RM;

        _LAM *= strategy.Target_LAM;
        _LCAM *= strategy.Target_LCAM;
        _CAM *= strategy.Target_CAM;
        _RCAM *= strategy.Target_RCAM;
        _RAM *= strategy.Target_RAM;

        _LF *= strategy.Target_LF;
        _LCF *= strategy.Target_LCF;
        _CF *= strategy.Target_CF;
        _RCF *= strategy.Target_RCF;
        _RF *= strategy.Target_RF;

        _ALF *= strategy.Target_ALF;
        _ARF *= strategy.Target_ARF;
        _Box *= strategy.Target_Box;



        float total =
            _OwnGoal + _BLD + _BRD +
            _LD + _LCD + +_CD + _RCD + _RD +
            _LDM + _LCDM + +_CDM + _RCDM + _RDM +
            _LM + _LCM + _CM + _RCM + _RM +
            _LAM + _LCAM + _CAM + _RCAM + _RAM +
            _LF + _LCF + _CF + _RCF + _RF +
            _ALF + _ARF + _Box;

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

        list.Add(new KeyValuePair<Zone, float>(Zone.OwnGoal, _OwnGoal));
        list.Add(new KeyValuePair<Zone, float>(Zone.BLD, _BLD));
        list.Add(new KeyValuePair<Zone, float>(Zone.BRD, _BRD));

        list.Add(new KeyValuePair<Zone, float>(Zone.LD, _LD));
        list.Add(new KeyValuePair<Zone, float>(Zone.LCD, _LCD));
        list.Add(new KeyValuePair<Zone, float>(Zone.CD, _CD));
        list.Add(new KeyValuePair<Zone, float>(Zone.RCD, _RCD));
        list.Add(new KeyValuePair<Zone, float>(Zone.RD, _RD));

        list.Add(new KeyValuePair<Zone, float>(Zone.LDM, _LDM));
        list.Add(new KeyValuePair<Zone, float>(Zone.LCDM, _LCDM));
        list.Add(new KeyValuePair<Zone, float>(Zone.CDM, _CDM));
        list.Add(new KeyValuePair<Zone, float>(Zone.RCDM, _RCDM));
        list.Add(new KeyValuePair<Zone, float>(Zone.RDM, _RDM));

        list.Add(new KeyValuePair<Zone, float>(Zone.LM, _LM));
        list.Add(new KeyValuePair<Zone, float>(Zone.LCM, _LCM));
        list.Add(new KeyValuePair<Zone, float>(Zone.CM, _CDM));
        list.Add(new KeyValuePair<Zone, float>(Zone.RCM, _RCM));
        list.Add(new KeyValuePair<Zone, float>(Zone.RM, _RM));

        list.Add(new KeyValuePair<Zone, float>(Zone.LAM, _LAM));
        list.Add(new KeyValuePair<Zone, float>(Zone.LCAM, _LCAM));
        list.Add(new KeyValuePair<Zone, float>(Zone.CAM, _CAM));
        list.Add(new KeyValuePair<Zone, float>(Zone.RCAM, _RCAM));
        list.Add(new KeyValuePair<Zone, float>(Zone.RAM, _RAM));

        list.Add(new KeyValuePair<Zone, float>(Zone.LF, _LF));
        list.Add(new KeyValuePair<Zone, float>(Zone.LCF, _LCF));
        list.Add(new KeyValuePair<Zone, float>(Zone.CF, _CF));
        list.Add(new KeyValuePair<Zone, float>(Zone.RCF, _RCF));
        list.Add(new KeyValuePair<Zone, float>(Zone.RF, _RF));

        list.Add(new KeyValuePair<Zone, float>(Zone.ALF, _ALF));
        list.Add(new KeyValuePair<Zone, float>(Zone.ARF, _ARF));
        list.Add(new KeyValuePair<Zone, float>(Zone.Box, _Box));

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

        //if (_isAwayTeam) target = (Zone)((totalZones - 1) - (int)target);
        return target;
    }

    Zones GetTeamStrategyZones(TeamAttributes.TeamStrategy _strategy, Zone _zone)
    {
        foreach(PosChanceData data in TeamStrategies)
        {
            if (data.Strategy == _strategy) return data.posChancePerZones[(int)_zone];
        }

        return null;
    }
}
