
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
		public string Text;
		public int Variations;
		public string Player1;
		public string Player2;
		public string Team1;
		public string Team2;
		public PlayInfo PlayInfo;
	}
	
	public void Initialize(Color _homeColor, Color _awayColor, TeamData _awayTeam)
	{
		homeColor = _homeColor;
		awayColor = _awayColor;
		awayTeam = _awayTeam;
		loc = LocalizationController.Instance;
	}
	
	public void UpdateNarration(PlayInfo _playInfo)
	{
		NarrationData narData = new NarrationData(); 
		narData.PlayInfo = _playInfo;
		if(_playInfo.IsActionSuccessful)
		{
			narData.Team1 = _playInfo.AttackingTeam.Name;
			narData.Team2 = _playInfo.DefendingTeam.Name;
			if(_playInfo.Attacker != null) narData.Player1 = _playInfo.Attacker.FullName;
			if(_playInfo.Defender != null) narData.Player2 = _playInfo.Defender.FullName;
		}
		else
		{
			narData.Team2 = _playInfo.AttackingTeam.Name;
			narData.Team1 = _playInfo.DefendingTeam.Name;
			if(_playInfo.Attacker != null) narData.Player2= _playInfo.Attacker.FullName;
			if(_playInfo.Defender != null) narData.Player1 = _playInfo.Defender.FullName;
		}
			
		string tag = "";
		int variations = 1;
		bool isNeutral = false;
		
		PlayInfo currentPlay = _playInfo;
		switch(currentPlay.OffensiveAction)
		{
		case PlayerAction.Pass:
			if (currentPlay.IsActionSuccessful)
			{
				flowPasses++;
				if (flowPasses == 3) tag = "nar_FlowPasses_";
				else tag = "nar_Pass_";
				if (currentPlay.CounterAttack > 0) tag = "nar_CounterAttack_";
			}
			else
			{
				flowPasses = 0;
	                
				if(_playInfo.IsActionDefended) 
				{
					tag = "nar_BlockPass_";
				}
				else tag = "nar_WrongPass_";
			}
			break;

		case PlayerAction.Dribble:
			if (currentPlay.IsActionSuccessful)
			{
				flowDribbles++;
				if (flowDribbles == 3) tag = "nar_FlowDribbles_";
				else tag = "nar_Dribble_";
			}
			else
			{
				flowDribbles = 0;
	                
				if(_playInfo.IsActionDefended) 
				{
					tag = "nar_BlockDribble_";
				}
				else tag = "nar_WrongDribble_";
			}
			break;

		case PlayerAction.Header:
			if (currentPlay.IsActionSuccessful && currentPlay.Event == MatchEvent.ShotOnGoal)
			{
				tag = "nar_Header_";
				variations = 1;
			}
			else
			{
				if (currentPlay.IsActionDefended)
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

		switch(currentPlay.Event)
		{
		case MatchEvent.KickOff:
			tag = "nar_KickOff_";
			variations = 1;
			/*
			if (isSecondHalf)
			{
				tag = "nar_SecondHalfStart_";
				isSecondHalf = false;
			}
			*/
			isNeutral = true;
			break;

		case MatchEvent.HalfTime:
			tag = "nar_FirstHalfEnd_";
			variations = 1;
			//isSecondHalf = true;
			isNeutral = true;
			break;

		case MatchEvent.FullTime:
			tag = "nar_TimeUp_";
			variations = 1;
			isNeutral = true;
			break;

		case MatchEvent.ShotOnGoal:
			if(_playInfo.OffensiveAction == PlayerAction.Shot) tag = "nar_Shot_";
			else if(_playInfo.OffensiveAction == PlayerAction.Header) tag = "nar_Header_";
			variations = 1;
			break;

		case MatchEvent.Goal:
			tag = "nar_GoalScream_";
			variations = 1;
			//screen.Score.UpdateScore(homeTeam.MatchStats.Goals, awayTeam.MatchStats.Goals);
			break;

		case MatchEvent.GoalAnnounced:
			tag = "nar_Goal_";
			variations = 8;
			if (currentPlay.Excitment == 1)
			{
				tag = "nar_BestGoal_";
				variations = 5;
			}
			else if (currentPlay.Excitment == -1)
			{
				tag = "nar_WorstGoal_";
				variations = 5;
			}
			break;

		case MatchEvent.Fault:
			//tag = "nar_Fault_";
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
			//loc.PLAYER_1 = _playInfo.Defender.FullName;
			//loc.PLAYER_2 = _playInfo.Attacker.FullName;
			if (currentPlay.Excitment == 1) tag = "nar_BestSaveShot_";
			else if (currentPlay.Excitment == -1) tag = "nar_WorstShot";
			variations = 1;
			break;

		case MatchEvent.ShotMissed:
			tag = "nar_MissedShot_";
			variations = 2;
			break;

		case MatchEvent.Offside:
			tag = "nar_Offside_";
			variations = 3;
			break;
		}
		
		if (tag != "")
		{
			narData.Text = tag;
			narData.Variations = variations;
			if (isNeutral) UpdateNarration(tag, GameData.Instance.Colors.MediumGray, variations);
			else 
			{
				if(_playInfo.IsActionSuccessful || _playInfo.IsActionDefended) UpdateNarration(narData);
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
			.Replace("{TEAM_2}", _data.Team2);
			
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
