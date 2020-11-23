using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
	public static Field Instance;
	
	private const int totalZones = 31;
	public List<Vector2> Matrix;
	
	private float _OwnGoal = 0;
	private float _BLD = 0;
	private float _BRD = 0;

	private float _LD = 0; //3
	private float _LCD = 0;
	private float _CD = 0;
	private float _RCD = 0;
	private float _RD = 0;

	private float _LDM = 0; //8
	private float _LCDM = 0;
	private float _CDM = 0;
	private float _RCDM = 0;
	private float _RDM = 0;

	private float _LM = 0; //13
	private float _LCM = 0;
	private float _CM = 0;
	private float _RCM = 0;
	private float _RM = 0;

	private float _LAM = 0; //18
	private float _LCAM = 0;
	private float _CAM = 0;
	private float _RCAM = 0;
	private float _RAM = 0;

	private float _LF = 0; //23
	private float _LCF = 0;
	private float _CF = 0;
	private float _RCF = 0;
	private float _RF = 0;

	private float _ALF = 0; //28
	private float _ARF = 0;
	private float _Box = 0;
    
	private void Awake()
	{
		if(Instance == null) Instance = this;
	}

	public Zone GetTeamZone(Zone _zone, bool _isAwayTeam)
	{
		Zone zone = _zone;
		if (_isAwayTeam)
		{
			zone = GetAwayTeamZone(_zone);
		}
		return zone;
	}
	
    public Zone GetAwayTeamZone(Zone _zone)
    {
        int zone = (totalZones - 1) - (int)_zone;

        return (Zone)zone;
    }

    public float CalculatePresence(PlayerData _player, Zone _zone)
    {
        float chance = _player.GetChancePerZone(_zone);
        if (chance < 1f && chance > 0f)
        {
            chance *= (float)(_player.Speed + _player.Vision) / 200;
            chance *= _player.FatigueModifier();
        }	    
        return chance;
    }

	public Zone GetTargetZone(PlayInfo _playInfo)
	{
		Zone zone = _playInfo.AttackingTeam.GetTeamZone(_playInfo.Zone);
		MatchEvent _event = _playInfo.Event;
		PlayerAction _action = _playInfo.OffensiveAction;
		TeamStrategy _teamStrategy = _playInfo.AttackingTeam.Strategy;
		bool _isAwayTeam = _playInfo.AttackingTeam.IsAwayTeam;
    	
        List<KeyValuePair<Zone, float>> list = new List<KeyValuePair<Zone, float>>();

        _OwnGoal = 0;
        _BLD = 0;
        _BRD = 0;

        _LD = 0;
        _LCD = 0;
        _CD = 0;
        _RCD = 0;
        _RD = 0;

        _LDM = 0;
        _LCDM = 0;
        _CDM = 0;
        _RCDM = 0;
        _RDM = 0;

        _LM = 0;
        _LCM = 0;
        _CM = 0;
        _RCM = 0;
        _RM = 0;

        _LAM = 0;
        _LCAM = 0;
        _CAM = 0;
        _RCAM = 0;
        _RAM = 0;

        _LF = 0;
        _LCF = 0;
        _CF = 0;
        _RCF = 0;
        _RF = 0;

        _ALF = 0;
        _ARF = 0;
        _Box = 0;

        if (_event == MatchEvent.Goalkick)
        {
	        _LDM = 0.15f;
	        _LCDM = 0.15f;
	        _CDM = 015f;
	        _RCDM = 0.15f;
	        _RDM = 0.15f;
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

        else if (_action == PlayerAction.Pass || _action == PlayerAction.Dribble || _action == PlayerAction.Sprint)
        {
	        TargetPassPerZone data = GameData.Instance.TargetPassPerZone[(int)zone];

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

        else if (_action == PlayerAction.Cross || _action == PlayerAction.LongPass)
        {
	        TargetCrossPerZone data = GameData.Instance.TargetCrossPerZone[(int)zone];

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

		if(_event != MatchEvent.Goalkick)
		{
		    Team_Strategy strategy = GameData.Instance.TeamStrategies[(int)_teamStrategy];
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
		}


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

		Zone target = zone;	    
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
	    if (_isAwayTeam) return GetAwayTeamZone(target);
	    else return target;
    }
}
