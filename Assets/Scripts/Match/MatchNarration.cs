
using UnityEngine;
using UnityEngine.UI;

public class MatchNarration : MonoBehaviour
{
	[SerializeField] private MatchNarrationTextView narrationText = null;
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
	
	public class NarrationData
	{
		public string Text = "NARRATION MISSING";
		public int Variations = 1;
		public string Player1 = "MISSING PLAYER 1 NAME";
		public string Player2 = "MISSING PLAYER 2 NAME";
		public string Team1 = "MISSING TEAM 1 NAME";
		public string Team2 = "MISSING TEAM 2 NAME";
		public string Zone = "MISSING ZONE";
		public PlayInfo PlayInfo;
	}
	
	public void Initialize(Color _homeColor, Color _awayColor, TeamData _awayTeam)
	{
		homeColor = _homeColor;
		awayColor = _awayColor;
		awayTeam = _awayTeam;
		loc = LocalizationController.Instance;
	}
	
	public void UpdateNarration(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{
		NarrationData narData = new NarrationData(); 
		narData.PlayInfo = _lastPlay;
		narData.Zone = loc.GetZoneString(_lastPlay.AttackingTeam.GetTeamZone(_lastPlay.Zone));
		
		narData.Team1 = _lastPlay.AttackingTeam.Name;
		narData.Team2 = _lastPlay.DefendingTeam.Name;
		if(_lastPlay.Attacker != null) narData.Player1 = _lastPlay.Attacker.FullName;
		if(_lastPlay.Defender != null) narData.Player2 = _lastPlay.Defender.FullName;

			
		string tag = "";
		int variations = 1;
		bool isNeutral = false;
		
		// NO EVENTS
		if(_lastPlay.Event == MatchEvent.None)
		{
			if(!_lastPlay.IsActionSuccessful)
			{
				narData.Team2 = _lastPlay.AttackingTeam.Name;
				narData.Team1 = _lastPlay.DefendingTeam.Name;
				if(_lastPlay.Attacker != null) narData.Player2= _lastPlay.Attacker.FullName;
				if(_lastPlay.Defender != null) narData.Player1 = _lastPlay.Defender.FullName;
				flowDribbles = 0;
				flowPasses = 0;
			}

			switch(_lastPlay.OffensiveAction) {

			case PlayerAction.Pass:
			
				if (_lastPlay.IsActionSuccessful)
				{
					flowPasses++;
					if (flowPasses == 3) tag = "nar_FlowPasses_";
					else 
					{
						switch(_lastPlay.OffenseExcitment) {
						case -1 : tag = "nar_WorstPass_"; break;
						case 0 : tag = "nar_Pass_"; break;
						case 1 : tag = "nar_BestPass_"; break;
						}
					}
					narData.Variations = 3;

					if(_currentPlay.Attacker != null) narData.Player2 = _currentPlay.Attacker.FullName;
				}
				else
				{
					if(_lastPlay.IsActionDefended) tag = "nar_BlockPass_";
					else tag = "nar_WrongPass_";
				}
				break;
				
			case PlayerAction.Cross:
			
				if (_lastPlay.IsActionSuccessful)
				{
					flowPasses++;
					tag = "nar_Cross_";

					narData.Player2 = _currentPlay.Attacker.FullName;
				}
				else
				{
					if(_lastPlay.IsActionDefended) tag = "nar_BlockPass_";
					else tag = "nar_WrongCross_";
				}
				break;
	
			case PlayerAction.Dribble:
				if (_lastPlay.IsActionSuccessful)
				{
					flowDribbles++;
					if (flowDribbles == 3) tag = "nar_FlowDribbles_";
					else tag = "nar_Dribble_";
				}
				else
				{
					if(_lastPlay.IsActionDefended) tag = "nar_BlockDribble_";
					else tag = "nar_WrongDribble_";
				}
				break;
	
			case PlayerAction.Header:
				if (_lastPlay.IsActionSuccessful && _lastPlay.Event == MatchEvent.ShotOnGoal)
				{
					tag = "nar_Header_";
					variations = 1;
				}
				else
				{
					if (_lastPlay.IsActionDefended)
					{
						tag = "nar_BlockHeader_";
					}
					else
					{
						tag = "nar_WrongHeader_";
					}                
				}
				break;
			}
			
			if(_lastPlay.Marking == MarkingType.Steal) 
			{
				narData.Player1 = _lastPlay.Defender.FullName;
				narData.Player2 = _lastPlay.Attacker.FullName;
				tag = "nar_Steal_";
			}
			if (_lastPlay.CounterAttack == 4) tag = "nar_CounterAttack_";
		}
		//EVENTS
		else
		{			
			switch(_lastPlay.Event) {
				
			case MatchEvent.KickOff:
				tag = "nar_KickOff_";
				variations = 1;
				isNeutral = true;
				break;
				
			case MatchEvent.SecondHalfKickOff:
				tag = "nar_SecondHalfStart_";
				variations = 1;
				isNeutral = true;
				break;
	
			case MatchEvent.HalfTime:
				tag = "nar_FirstHalfEnd_";
				variations = 1;
				isNeutral = true;
				break;
	
			case MatchEvent.FullTime:
				tag = "nar_TimeUp_";
				variations = 1;
				isNeutral = true;
				break;
	
			case MatchEvent.ShotOnGoal:
				if(_lastPlay.OffensiveAction == PlayerAction.Shot) tag = "nar_Shot_";
				else if(_lastPlay.OffensiveAction == PlayerAction.Header) tag = "nar_Header_";
				variations = 1;
				narData.Player1 = _lastPlay.Attacker.FullName;
				break;
	
			case MatchEvent.Goal:
				tag = "nar_GoalScream_";
				variations = 1;
				break;
	
			case MatchEvent.GoalAnnounced:
				tag = "nar_Goal_";
				variations = 8;
				if (_lastPlay.OffenseExcitment == 1)
				{
					tag = "nar_BestGoal_";
					variations = 5;
				}
				else if (_lastPlay.OffenseExcitment == -1)
				{
					tag = "nar_WorstGoal_";
					variations = 5;
				}
				break;
	
			case MatchEvent.Fault:
				tag = "nar_Fault_";
				narData.Player1 = _lastPlay.Defender.FullName;
				narData.Player2 = _lastPlay.Attacker.FullName;
				variations = 5;
				break;
	
			case MatchEvent.CornerKick:
				tag = "nar_CornerKick_";
				variations = 1;
				break;
	
			case MatchEvent.Penalty:
				tag = "nar_Penalty_";
				variations = 1;
				break;
	
			case MatchEvent.ShotSaved:
				tag = "nar_SaveShot_";
				narData.Player1 = _lastPlay.Defender.FullName;
				narData.Player2 = _lastPlay.Attacker.FullName;
				if (_lastPlay.DefenseExcitment == 1) tag = "nar_BestSaveShot_";
				else if (_lastPlay.DefenseExcitment == -1) tag = "nar_WorstSaveShot_";
				variations = 1;
				break;
	
			case MatchEvent.ShotMissed:
				tag = "nar_MissedShot_";
				variations = 2;
				narData.Player1 = _lastPlay.Attacker.FullName;
				break;
	
			case MatchEvent.Offside:
				tag = "nar_Offside_";
				variations = 3;
				narData.Player1 = _lastPlay.Attacker.FullName;
				break;
			}
		}
		
		if (tag != "")
		{
			narData.Text = tag;
			narData.Variations = variations;
			if (isNeutral) UpdateNarration(tag, GameData.Instance.Colors.MediumGray, variations);
			else 
			{
				if(_lastPlay.IsActionSuccessful || _lastPlay.IsActionDefended) UpdateNarration(narData);
				else UpdateNarration(tag, GameData.Instance.Colors.LightGray, 1, narData);
			}
		}
	}
	
	private void UpdateNarration(NarrationData _data)
	{		
		int r = Random.Range(1, _data.Variations);
		string text = _data.Text + r;
		string narText = loc.Localize(text);
		narText = ReplaceNames(narText, _data);
		
		bool isAway;
		if(_data.PlayInfo.IsActionSuccessful) isAway = _data.PlayInfo.AttackingTeam == awayTeam;
		else isAway = _data.PlayInfo.DefendingTeam == awayTeam;
		
		MatchNarrationTextView textObj =  Instantiate(narrationText, content);
		textObj.Populate(narText, isAway, _data.PlayInfo, homeColor, awayColor) ;
		UpdateLayout();
	}
	
	private string ReplaceNames(string _text, NarrationData _data)
	{
		string txt = _text.Replace("{PLAYER_1}", _data.Player1)
			.Replace("{PLAYER_2}", _data.Player2)
			.Replace("{TEAM_1}", _data.Team1)
			.Replace("{TEAM_2}", _data.Team2)
			.Replace("{ZONE}", _data.Zone);
			
		return txt;
	}

	public void UpdateNarration(string _text, Color _frameColor, int _variations = 1, NarrationData _data = null)
	{
		int r = Random.Range(1, _variations);
		_text += r;
		MatchNarrationTextView text = Instantiate(narrationText, content);
        
		string narText = loc.Localize(_text);
		if(_data != null) narText = ReplaceNames(narText, _data);
        
        text.Populate(narText, _frameColor);
	    UpdateLayout();
    }

	public void UpdateNarration(string _text, Color _teamColor, Color  _textColor)
    {
        MatchNarrationTextView text = Instantiate(narrationText, content);

	    text.Populate(loc.Localize(_text), _teamColor, _textColor);
	    UpdateLayout();
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

public class NarrationParamenter
{
	public string Tag;
	public int Variations;
}
