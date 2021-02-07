
using UnityEngine;
using UnityEngine.UI;

public class MatchNarration : MonoBehaviour
{
	//[SerializeField] private MatchNarrationTextView narrationText = null;
	[SerializeField] private MatchNarrationVisual visualNarration = null;
	[SerializeField] private Transform content = null;
	[SerializeField] private ScrollRect scroll = null;
	[SerializeField] private Color grayFrame = Color.gray;
	[SerializeField] private Color grayText = Color.gray;
	
	private byte flowPasses = 0;
	private byte flowDribbles = 0;
	
	private Color homeColor;
	private Color awayColor;
	private TeamData awayTeam;
	
	private LocalizationController loc;
	
	private const byte PLAYER_1 = 1;
	private const byte PLAYER_2 = 2;
	
	public void Initialize(Color _homeColor, Color _awayColor, TeamData _awayTeam)
	{
		homeColor = _homeColor;
		awayColor = _awayColor;
		awayTeam = _awayTeam;
		loc = LocalizationController.Instance;
		visualNarration.SetNarrationText("");
		visualNarration.ShowGoalCelebration(false);
	}
	
	public void SetNarrationText(string _text)
	{
		visualNarration.SetNarrationText(loc.Localize(_text));
	}
	
	private void SetNarrationPlayers (NarrationData _data, PlayerData _player1 = null, PlayerData _player2 = null)
	{
		if(_player1) SetNarrationPlayer(PLAYER_1, _data, _player1);
		if(_player2) SetNarrationPlayer(PLAYER_2, _data, _player2);
	}
	
	private void SetNarrationPlayer(byte _index, NarrationData _data, PlayerData _player)
	{
		Color teamColor = _player.Team.IsAwayTeam ? awayColor : homeColor;
		
		if(_index == PLAYER_1)
		{
			_data.Player1 = GetFormatedName(_player);
			_data.Team1 = _player.Team.Name;
			_data.Color1 = "#" + ColorUtility.ToHtmlStringRGB(teamColor);
		}
		else
		{
			_data.Player2 = GetFormatedName(_player);
			_data.Team2 = _player.Team.Name;
			_data.Color2 = "#" + ColorUtility.ToHtmlStringRGB(teamColor);
		}
	}
	
	private string GetFormatedName(PlayerData _player)
	{
		int format = MainController.Instance.User.PlayerNameFormat;
		if(format == 0) return _player.FullName;
		else if(format == 1) return _player.ShortName;
		else if(format == 2) return _player.FirstName;
		else if(format == 3) return _player.LastName;
		return "BAD NAME";
	}
	
	public void UpdateNarration(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData(); 		
		
		if(!_lastPlay.IsActionSuccessful)
		{
			flowDribbles = 0;
			flowPasses = 0;
		}
		
		// NO EVENTS
		if(_lastPlay.Event == MatchEvent.None)
		{
			if(_lastPlay.Marking == MarkingType.Steal) 
				narData = GetStealNarration(_lastPlay);
			else if (_lastPlay.CounterAttack == 4) 
				narData = GetCounterAttackNarration(_lastPlay);
			else if (_lastPlay.Attacker == null)
				narData = GetEmptyPlayNarration(_lastPlay);
			else
			{
				switch(_lastPlay.OffensiveAction)
				{
					case PlayerAction.None: narData = GetLostBallNarration(_lastPlay); break;
					case PlayerAction.Pass: narData = GetPassNarration(_lastPlay, _currentPlay); break;	
					case PlayerAction.LongPass: narData = GetLongPassNarration(_lastPlay, _currentPlay) ; break;
					case PlayerAction.Cross: narData = GetCrossNarration(_lastPlay, _currentPlay); break;
					case PlayerAction.Dribble: narData = GetDribbleNarration(_lastPlay); break;
					case PlayerAction.Header: narData = GetHeaderNarration(_lastPlay) ; break;				
					case PlayerAction.Sprint: narData = GetSprintNarration(_lastPlay) ; break;
				}
			}
		}
		//EVENTS
		else
		{			
			switch(_lastPlay.Event)
			{
				
				case MatchEvent.KickOff: narData = GetKickOffNarration(_lastPlay); break;	
				case MatchEvent.HalfTime: narData = GetFirstHalfEndNarration(_lastPlay); break;
				case MatchEvent.SecondHalfKickOff: narData = GetSecondHalfStartNarration(_lastPlay); break;			
				case MatchEvent.FullTime: narData = GetFullTimeNarration(_lastPlay); break;
				case MatchEvent.ShotOnGoal: narData = GetShotOnGoalNarration(_lastPlay);  break;
				case MatchEvent.Goal: narData = GetGoalNarration(_lastPlay); break;		
				case MatchEvent.GoalAnnounced: narData = GetGoalAnnouncedNarration(_lastPlay); break;		
				case MatchEvent.ScorerAnnounced: narData = GetScorerAnnoucendNarration(_lastPlay); break;		
				case MatchEvent.Fault: narData = GetFaultNarration(_lastPlay); break;		
				case MatchEvent.CornerKick: narData = GetCornerKickNarration(_lastPlay); break;
				case MatchEvent.Penalty: narData = GetPenaltytNarration(_lastPlay); break;	
				case MatchEvent.PenaltyShot: narData = GetPenaltyShotNarration(_lastPlay); break;	
				case MatchEvent.ShotSaved: narData = GetShotSavedNarration(_lastPlay); break;	
				case MatchEvent.ShotMissed: narData = GetShotMissedNarration(_lastPlay); break;
				case MatchEvent.Offside: narData = GetOffsideNarration(_lastPlay); break;
				case MatchEvent.Goalkick: narData = GetGoalKickNarration(_lastPlay); break;
			}
		}
		
		narData.PlayInfo = _lastPlay;
		narData.Zone = loc.GetZoneString(_lastPlay.AttackingTeam.GetTeamZone(_lastPlay.Zone));
		UpdateNarration(narData, _currentPlay.Zone);
	}
	
	private void UpdateNarration(NarrationData _data, Zone _zone)
	{		
		if(_data.Text != "NARRATION MISSING")
		{
			
			if(_data.Text != "")
			{
				int r = Random.Range(1, _data.Variations);
				string narText = loc.Localize(_data.Text + r);
				if(narText == null)
				{
					Debug.LogFormat("MISSING LOC NARRATION!  ORIGINAL STRING: {0}   TURN: {1}", _data.Text, _data.PlayInfo.Turn);
				}
				narText = ReplaceNames(narText, _data);
				_data.Text = narText;
			}

			visualNarration.UpdateNarration(_data, _zone);
		}
		else
		{
			Debug.LogFormat("MISSING NARRATION!!! -   EVENT: {0}     OFF ACTION: {1}     IS SUCCESS: {2}     TURN: {3}", _data.PlayInfo.Event.ToString(), _data.PlayInfo.OffensiveAction, _data.PlayInfo.IsActionSuccessful, _data.PlayInfo.Turn);
		}

		
		/*
		bool isAway;
		if(_data.PlayInfo.IsActionSuccessful) isAway = _data.PlayInfo.AttackingTeam == awayTeam;
		else isAway = _data.PlayInfo.DefendingTeam == awayTeam;
		
		MatchNarrationTextView textObj =  Instantiate(narrationText, content);
		textObj.Populate(narText, isAway, _data.PlayInfo, homeColor, awayColor) ;
		UpdateLayout();
		*/
	}
	
	private NarrationData GetPassNarration(PlayInfo _lastPlay, PlayInfo _currentPlay)
	{
		NarrationData narData = new NarrationData();
		narData.PlayInfo = _lastPlay;

		if (_lastPlay.IsActionSuccessful)
		{
			flowPasses++;
			if (flowPasses == 3) 
			{
				narData.Text = "nar_FlowPasses_";
				narData.Variations = 1;
			}
			else 
			{
				switch(_lastPlay.OffenseExcitment)
				{
					case -1 : narData.Text = "nar_WorstPass_"; break;
					case 0 : narData.Text = "nar_Pass_"; break;
					case 1 : narData.Text = "nar_BestPass_"; break;
				}
				narData.Variations = 3;
			}		
			SetNarrationPlayers(narData, _lastPlay.Attacker, _currentPlay.Attacker);
		}
		else
		{
			if(_lastPlay.IsActionDefended) 
			{
				SetNarrationPlayers(narData, _lastPlay.Defender, _lastPlay.Attacker);
				narData.Text = "nar_BlockPass_";
			}
			else
			{
				SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
				narData.Text = "nar_WrongPass_";
			}
		}
		return narData;
	}
	
	private NarrationData GetLongPassNarration(PlayInfo _lastPlay, PlayInfo _currentPlay)
	{
		NarrationData narData = new NarrationData();
		narData.PlayInfo = _lastPlay;

		if (_lastPlay.IsActionSuccessful)
		{
			flowPasses++;

			switch(_lastPlay.OffenseExcitment) {
				case -1 : narData.Text = "nar_WorstLongPass_"; break;
				case 0 : narData.Text = "nar_LongPass_"; break;
				case 1 : narData.Text = "nar_BestLongPass_"; break;
			}
			narData.Variations = 1;

			SetNarrationPlayer(PLAYER_2, narData, _currentPlay.Attacker);
		}
		else
		{
			if(_lastPlay.IsActionDefended) 
			{
				SetNarrationPlayers(narData, _lastPlay.Defender, _lastPlay.Attacker);
				narData.Text = "nar_BlockPass_";
			}
			else
			{
				SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
				narData.Text = "nar_WrongPass_";
			}
		}
		return narData;
	}
	
	private NarrationData GetCrossNarration(PlayInfo _lastPlay, PlayInfo _currentPlay)
	{
		NarrationData narData = new NarrationData();
		narData.PlayInfo = _lastPlay;
		
		if (_lastPlay.IsActionSuccessful)
		{
			flowPasses++;
			narData.Text = "nar_Cross_";
			SetNarrationPlayers(narData, _lastPlay.Attacker, _currentPlay.Attacker);
		}
		else
		{
			if(_lastPlay.IsActionDefended) 
			{
				SetNarrationPlayers(narData, _lastPlay.Defender, _lastPlay.Attacker);
				narData.Text = "nar_BlockPass_";
			}
			else 
			{
				SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
				narData.Text = "nar_WrongCross_";
			}
		}
		
		return narData;
	}
	
	private NarrationData GetDribbleNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.PlayInfo = _lastPlay;
		
		if (_lastPlay.IsActionSuccessful)
		{
			flowDribbles++;
			if (flowDribbles == 3) narData.Text = "nar_FlowDribbles_";
			else narData.Text = "nar_Dribble_";
			SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
		}
		else
		{
			if(_lastPlay.IsActionDefended) 
			{
				SetNarrationPlayers(narData, _lastPlay.Defender, _lastPlay.Attacker);
				narData.Text = "nar_BlockDribble_";			
			}
			else 
			{
				SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
				narData.Text = "nar_WrongDribble_";			
			}
		}
		
		return narData;
	}
	
	private NarrationData GetSprintNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.PlayInfo = _lastPlay;
		
		if (_lastPlay.IsActionSuccessful)
		{
			narData.Text = "nar_Sprint_";
			SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
		}
		else
		{
			SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
			narData.Text = "nar_WrongSprint_1";			
		}
		
		return narData;
	}
	
	private NarrationData GetHeaderNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		
		if (_lastPlay.IsActionSuccessful)
		{
			SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
			narData.Text = "nar_Header_";
			narData.Variations = 1;
		}
		else
		{
			if (_lastPlay.IsActionDefended)
			{
				SetNarrationPlayers(narData, _lastPlay.Defender, _lastPlay.Attacker);
				narData.Text = "nar_BlockHeader_";
			}
			else
			{
				SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
				narData.Text = "nar_WrongHeader_";
			}                
		}
		return narData;
	}
	
	private NarrationData GetStealNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.Text = "nar_Steal_";
		narData.Variations = 1;
		SetNarrationPlayers(narData, _lastPlay.Defender, _lastPlay.Attacker);
		return narData;
	}
	
	private NarrationData GetCounterAttackNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.Text = "nar_CounterAttack_";
		narData.Variations = 1;
		SetNarrationPlayers(narData, _lastPlay.Defender, _lastPlay.Attacker);
		return narData;
	}
	
	private NarrationData GetKickOffNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.Text = "nar_KickOff_";
		narData.Variations = 1;
		return narData;
	}
	
	private NarrationData GetFirstHalfEndNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.Text = "nar_FirstHalfEnd_";
		narData.Variations = 1;
		return narData;
	}
	
	private NarrationData GetSecondHalfStartNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.Text = "nar_SecondHalfStart_";
		narData.Variations = 1;
		SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
		return narData;
	}
	
	private NarrationData GetFullTimeNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.Text = "nar_TimeUp_";
		narData.Variations = 1;
		return narData;
	}
	
	private NarrationData GetEmptyPlayNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.Text = "nar_SwitchPossession_";
		narData.Variations = 1;
		SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
		return narData;
	}
	
	private NarrationData GetLostBallNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.Text = "nar_LostPossession_";
		narData.Variations = 4;
		SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
		return narData;
	}
	
	
	private NarrationData GetShotOnGoalNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();		
		
		if(_lastPlay.OffensiveAction == PlayerAction.Shot) 
			narData.Text = "nar_Shot_";
		else if(_lastPlay.OffensiveAction == PlayerAction.Header)
			narData.Text = "nar_Header_";
		narData.Variations = 1;
		SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);	
		return narData;
	}
	
	private NarrationData GetGoalNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.Text = "nar_GoalScream_";
		narData.Type = NarrationType.GoalCelebration;
		SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
		return narData;
	}
	
	private NarrationData GetGoalAnnouncedNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();	
		
		switch(_lastPlay.OffenseExcitment)
		{
			case -1 : 
				narData.Text = "nar_WorstGoal_"; 
				narData.Variations = 5;
				break;
			case 0 : 
				narData.Text = "nar_Goal_"; 
				narData.Variations = 8;
				break;
			case 1 :
				narData.Text = "nar_BestGoal_";
				narData.Variations = 5;
				break;
		}
		
		narData.Type = NarrationType.GoalCelebration;
		SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);	
		return narData;
	}
	
	private NarrationData GetScorerAnnoucendNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.Text = "";
		narData.Type = NarrationType.GoalCelebration;
		return narData;
	}
	
	private NarrationData GetFaultNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.Text = "nar_Fault_";
		narData.Variations = 5;
		SetNarrationPlayers(narData, _lastPlay.Defender, _lastPlay.Attacker);
		return narData;
	}
	
	private NarrationData GetCornerKickNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.Text = "nar_CornerKick_";
		narData.Variations = 1;
		SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
		return narData;
	}
	
	private NarrationData GetPenaltytNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.PlayInfo = _lastPlay;
		narData.Text = "nar_Penalty_";
		narData.Variations = 1;
		SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
		return narData;
	}
	
	private NarrationData GetPenaltyShotNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();		
		
		narData.Text = "nar_PenaltyTake_";
		narData.Variations = 1;
		SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);	
		return narData;
	}
	
	private NarrationData GetShotMissedNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.Text = "nar_MissedShot_";
		narData.Variations = 2;
		SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
		return narData;
	}
	
	private NarrationData GetShotSavedNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		
		if(_lastPlay.DefenseExcitment == 0) narData.Text = "nar_SaveShot_";
		if (_lastPlay.DefenseExcitment == 1) narData.Text = "nar_BestSaveShot_";
		else if (_lastPlay.DefenseExcitment == -1) narData.Text = "nar_WorstSaveShot_";
		
		narData.Variations = 1;
		SetNarrationPlayers(narData, _lastPlay.Defender, _lastPlay.Attacker);
		return narData;
	}
	
	private NarrationData GetOffsideNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		narData.Text = "nar_Offside_";
		narData.Variations = 3;
		SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
		return narData;
	}
	
	private NarrationData GetGoalKickNarration(PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData();
		if(_lastPlay.IsActionSuccessful)
		{
			narData.Text = "nar_Goalkick_";		
		}
		else 
		{
			narData.Text = "nar_WrongGoalkick_";
		}
		
		narData.Variations = 1;
		SetNarrationPlayers(narData, _lastPlay.Attacker, _lastPlay.Defender);
		return narData;
	}
	
	private string ReplaceNames(string _text, NarrationData _data)
	{
		string txt = _text;
		txt = txt.Replace("{PLAYER_1}", "<color={COLOR_1}>" + _data.Player1 + "</color>");
		txt = txt.Replace("{PLAYER_2}", "<color={COLOR_2}>" + _data.Player2 + "</color>");
		txt = txt.Replace("{TEAM_1}", "<color={COLOR_1}>" + _data.Team1 + "</color>");
		txt = txt.Replace("{TEAM_2}", "<color={COLOR_2}>" + _data.Team2 + "</color>");
		txt = txt.Replace("{COLOR_1}", _data.Color1);
		txt = txt.Replace("{COLOR_2}", _data.Color2);
		txt = txt.Replace("{ZONE}", _data.Zone);
		
		return txt;
	}
    
	private void UpdateLayout()
	{
		Canvas.ForceUpdateCanvases();
		scroll.verticalNormalizedPosition = 0;
		Canvas.ForceUpdateCanvases();
	}

    public void Reset()
    {
        foreach(Transform go in content)
        {
            Destroy(go.gameObject);
        }
    }
}


public class NarrationData
{
	public string Text = "NARRATION MISSING";
	public int Variations = 1;
	public string Player1 = "MISSING PLAYER 1 NAME";
	public string Player2 = "MISSING PLAYER 2 NAME";
	public string Team1 = "MISSING TEAM 1 NAME";
	public string Team2 = "MISSING TEAM 2 NAME";
	public string Color1 = "MISSING COLOR 1";
	public string Color2 = "MISSING COLOR 2";
	public string Zone = "MISSING ZONE";
	public PlayInfo PlayInfo;
	public NarrationType Type;
}
