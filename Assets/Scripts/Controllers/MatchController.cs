using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MatchController : MonoBehaviour
{
	public static MatchController Instance;
	
	public uint MatchSpeed = 1;
	
	[SerializeField] private MatchScreen screen = null;

    private List<PlayInfo> playList;
    private TeamData homeTeam;
    private TeamData awayTeam;   
	private bool isGameOn;
	private bool isSimulatingMatch;
	private bool secondHalfStarted;
	
	private MatchActionManager actionManager;
	private PlayDiceRolls playDiceRolls;
	private MatchEvents matchEvents;
	private MatchData currentMatch;
	private MatchData nextMatch;
	
	private const float TURN_DURATION = 2.0f;
	
	private void Awake()
	{
		if(!Instance) Instance = this;
	}

	public void Populate(MatchData _data, bool _isSimulating = false)
    {
	    Reset();
        
	    currentMatch = _data;

	    homeTeam = _data.HomeTeam.TeamData;
	    awayTeam = _data.AwayTeam.TeamData;
        
	    homeTeam.IsAwayTeam = false;
	    awayTeam.IsAwayTeam = true;

        homeTeam.ResetMatchData();
        awayTeam.ResetMatchData();

        screen.HomeTeamSquad.Populate(homeTeam, true);
        screen.AwayTeamSquad.Populate(awayTeam, true);
	    screen.Score.UpdateTime(0);
        screen.Score.UpdateScore(0, 0);
	    screen.Score.Populate(homeTeam.Name, 0, homeTeam.PrimaryColor, awayTeam.Name, 0, TeamDisplayColor(awayTeam));
	    
	    screen.Narration.Initialize(homeTeam.PrimaryColor, TeamDisplayColor(awayTeam), awayTeam);

	    isSimulatingMatch = _isSimulating;
	    if(_isSimulating) StartSimulation();
    }

	public void UpdateHomeTeam(TeamData _teamData, List<PlayerData> _in, List<PlayerData> _out)
    {
        if (_in.Count > 0)
        {
            string playersIn = "In: ";
            string playersOut = "Out: ";
            PlayerData player;
            for (int i = 0; i < _in.Count; i++)
            {
                player = _in[i];
	            if (i == 0) playersIn += player.FullName;
	            else playersIn += ", " + player.FullName;
            }

            for (int i = 0; i < _out.Count; i++)
            {
                player = _out[i];
	            if (i == 0) playersOut += player.FullName;
	            else playersOut += ", " + player.FullName;
            }
        }
		
	    homeTeam = _teamData;
	    screen.HomeTeamSquad.UpdatePlayers(_teamData);
	    screen.ShowMatchScreen(false);  
	    screen.ShowButtons(false);
	    
	    float delay = _in.Count > 0 ? 2.5f : 0.5f;
	    Sequence seq = DOTween.Sequence();
	    seq.AppendInterval(delay);
	    seq.AppendCallback(UnpauseGame);	    
    }

	private TeamData UserTeam
	{
		get
		{
	        if (homeTeam.IsUserControlled) return homeTeam;
	        else return awayTeam;
		}
	}
    
	private Color TeamDisplayColor(TeamData _team)
	{
		if(_team == awayTeam && awayTeam.PrimaryColor == homeTeam.PrimaryColor) return _team.SecondaryColor;
		return _team.PrimaryColor;
	}

	public void StartSimulation()
    {
        screen.Narration.Reset();

        isSimulatingMatch = true;

        Reset();
	    StartMatch();

        if (screen == null) screen = GetComponent<MatchScreen>();
	    screen.ShowSimulationScreen();
    }

	public void StartMatch()
    {
	    isGameOn = true;
        if (!isSimulatingMatch)
        {
	        StartCoroutine("GameLoop");
        }
        else
        {
	        StartCoroutine("SimulateLoop");
        }
    }

	private IEnumerator GameLoop()
    {
        while (isGameOn == true)
        {
        	yield return PlayTurn();
	        yield return new WaitForSeconds(TURN_DURATION / MatchSpeed);      
        }
    }
    
	private IEnumerator SimulateLoop()
	{
		while(isGameOn)
		{
			yield return PlayTurn();
			yield return new WaitForEndOfFrame();
		}		
	}

	private IEnumerator PlayTurn()
	{
		if(!isGameOn) return null;
	    bool evt = false;
	    PlayInfo currentPlay = new PlayInfo();
	    currentPlay.Turn = playList.Count;
	    PlayInfo lastPlay = currentPlay.Turn  > 0 ? playList[currentPlay.Turn-1] : null;
	    if(lastPlay != null) currentPlay = CheckPossesion(lastPlay, currentPlay);
	    //Kick off
        if (currentPlay.Turn == 0)
        {
	        currentPlay.Event = MatchEvent.KickOff;      
	        currentPlay.AttackingTeam = homeTeam;
	        currentPlay.DefendingTeam = awayTeam;
            
	        //currentPlay = matchEvents.GetEventResults(currentPlay, null);
	        screen.Narration.SetNarrationText("nar_PreMatch_1");
	        evt = true;
        }
	    //Half time
        else if (!secondHalfStarted && currentPlay.Turn >= 45 && lastPlay.Event == MatchEvent.None)
        {
	        lastPlay.Event = MatchEvent.HalfTime;
	        currentPlay.Zone = Zone.CM;
	        currentPlay.OffensiveAction = PlayerAction.None;
            secondHalfStarted = true;
	        currentPlay = ResolveEvents(currentPlay, lastPlay);
	        evt = true;
        }
	    //Time off
        else if (currentPlay.Turn >= 90 && lastPlay.Event == MatchEvent.None)
        {
	        lastPlay.Event = MatchEvent.FullTime;
	        currentPlay.Zone = Zone.CM;
	        currentPlay = ResolveEvents(currentPlay, lastPlay);
	        evt = true;
        }						
        else
        {     		        
	        evt = lastPlay.Event != MatchEvent.None;
	        currentPlay = ResolveEvents(currentPlay, lastPlay);
        }	    

	    if (!evt) currentPlay = DefineActions(currentPlay, lastPlay);
	    
	    playList.Add(currentPlay);
	    
	    //END TURN
	    #if (UNITY_EDITOR)
	    DebugTextFile.Instance.DebugPlayInfo(currentPlay, homeTeam, awayTeam);
    	#endif
    	
    	screen.Score.UpdateTime(currentPlay.Turn);
    	
	    if(!isSimulatingMatch)
	    {
		    if(currentPlay.Turn > 0) screen.Narration.UpdateNarration(currentPlay, lastPlay);
	    }
	    
	    return null;
    }
    
	private PlayInfo ResolveEvents(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{
		if(_lastPlay.Event == MatchEvent.GoalAnnounced) screen.Score.UpdateScore(homeTeam.MatchStats.Goals, awayTeam.MatchStats.Goals);
		
		switch (_lastPlay.Event)
		{
			default : return matchEvents.GetEventResults(_currentPlay, _lastPlay);
			case MatchEvent.HalfTime: return ResolveHalfTime(_currentPlay);
			case MatchEvent.FullTime: ResolveFullTime(_currentPlay); break;
		}		
		
		return _currentPlay;
	}

	private PlayInfo ResolveHalfTime(PlayInfo _playInfo)
	{
		_playInfo.Event = MatchEvent.SecondHalfKickOff;
		_playInfo.Zone = Zone.CM;
		_playInfo.AttackingTeam = awayTeam;
		_playInfo.DefendingTeam = homeTeam;
		return _playInfo;
	}

	private void ResolveFullTime(PlayInfo _currentPlay)
	{
		if(!isGameOn) return;
		isGameOn = false;
		_currentPlay.Zone = Zone.CM;
		EndGame();		
	}

	private PlayInfo DefineActions(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{
		screen.HomeTeamSquad.UpdateFatigue();
		screen.AwayTeamSquad.UpdateFatigue();

		bool keepAttacker = false;
		bool keepDefender = false;
		PlayerData playerToExclude = null;
		bool forcePlayer = false;
		
		_currentPlay.Zone = _lastPlay.TargetZone;
		_currentPlay.TargetZone = _lastPlay.TargetZone;
		if(_lastPlay.CounterAttack > 0) _currentPlay.CounterAttack = _lastPlay.CounterAttack -1;

		//If last action was an assistance, we exclude the assister in case of success and force a receiver
		if (_lastPlay.OffensiveAction == PlayerAction.Pass || _lastPlay.OffensiveAction == PlayerAction.LongPass || _lastPlay.OffensiveAction == PlayerAction.Cross)
		{
			_currentPlay.Assister = playerToExclude = _lastPlay.Attacker;
			if (_lastPlay.IsActionSuccessful) forcePlayer = true;
		}
		//If last action was a dribble or sprint we keep the attacker
		else if (_lastPlay.OffensiveAction == PlayerAction.Dribble || _lastPlay.OffensiveAction == PlayerAction.Sprint) keepAttacker = true;
		//If last actino was not successfull and a defender was present we keep the defender			
		if (!_lastPlay.IsActionSuccessful && _lastPlay.Defender != null) keepDefender = true;

		//Step 1: Get players involved in the dispute
		if (keepDefender) _currentPlay.Attacker = _lastPlay.Defender;
		else _currentPlay.Attacker = keepAttacker ? _lastPlay.Attacker : _currentPlay.AttackingTeam.GetAttackingPlayer(_currentPlay.Zone, playerToExclude, forcePlayer);

		_currentPlay.Defender = _currentPlay.DefendingTeam.GetDefendingPlayer(_currentPlay.Zone, playerToExclude, _currentPlay.CounterAttack);

		//No players from attacking team in the dispute
		if (_currentPlay.Attacker == null)  keepDefender = true;
		//Player from attacking team in the dispute
		else
		{
			//Step 2: Get type of marking
			if (_currentPlay.Defender == null || _currentPlay.Attacker.Zone == Zone.OwnGoal) _currentPlay.Marking = MarkingType.None;
			else _currentPlay.Marking = GetMarkingType(_currentPlay.Defender, _currentPlay.Zone);

			if (_currentPlay.Marking == MarkingType.Steal)
			{
				keepDefender = true;
				_currentPlay.DefendingTeam.MatchStats.Steals++;
				_currentPlay.CounterAttack = actionManager.CheckCounterAttack(_currentPlay.DefendingTeam, _currentPlay.Zone);
				_currentPlay.IsActionDefended = true;
			}
			else
			{
				//Step 3: Get type of offensive currentPlay
				bool header = false;
				if (_lastPlay.IsActionSuccessful && _lastPlay.OffensiveAction == PlayerAction.Cross && _currentPlay.AttackingTeam.GetTeamZone(_currentPlay.Zone) == Zone.Box) header = true;
				_currentPlay.OffensiveAction = actionManager.GetOffensiveAction(_currentPlay, header);

				//Step 4: Test action against defender (if there is one)
				_currentPlay = actionManager.ResolveAction(_currentPlay, _lastPlay);
			}
		}

		return _currentPlay;
	}	
	
	public void UnpauseGame()
	{
		PauseGame(false);
		screen.ShowMatchScreen(false);  
		screen.ShowButtons(true);
	}
	
    public void PauseGame(bool _isPaused)
    {
        isGameOn = !_isPaused;

        if(isGameOn)
        {
            StartCoroutine("GameLoop");
        }
        else 
        {
        	StopCoroutine("GameLoop");
        }
    }

	private void EndGame()
    {
	    StopAllCoroutines();
        if (homeTeam.MatchStats.Goals > awayTeam.MatchStats.Goals)
        {
            homeTeam.MatchStats.Wins++;
            awayTeam.MatchStats.Losts++;

            homeTeam.MatchStats.Points += 3;
        }
        else if (awayTeam.MatchStats.Goals > homeTeam.MatchStats.Goals)
        {
            awayTeam.MatchStats.Wins++;
            homeTeam.MatchStats.Losts++;

            awayTeam.MatchStats.Points += 3;
        }
        else
        {
            homeTeam.MatchStats.Draws++;
            awayTeam.MatchStats.Draws++;

            homeTeam.MatchStats.Points++;
            awayTeam.MatchStats.Points++;
        }

        homeTeam.MatchStats.GoalsAgainst += awayTeam.MatchStats.Goals;
        awayTeam.MatchStats.GoalsAgainst += homeTeam.MatchStats.Goals;
   	    
	    currentMatch.HomeTeam.Statistics = homeTeam.MatchStats;
	    currentMatch.AwayTeam.Statistics = awayTeam.MatchStats;
	    
	    currentMatch.isPlayed = true;
	    
	    homeTeam.UpdateLifeTimeStats(currentMatch.TournamentId);
	    awayTeam.UpdateLifeTimeStats(currentMatch.TournamentId);

	    screen.AddSimulatedMatch(currentMatch);

	    //Save tournament match data
	    TournamentData currentTournament = MainController.Instance.GetTournamentById(currentMatch.TournamentId);
        if (currentTournament!= null)
        {
        	TeamStatistics homeTeamStats = homeTeam.GetTournamentStatistics(currentTournament.Id);
        	TeamStatistics awayTeamStats = awayTeam.GetTournamentStatistics(currentTournament.Id);
	        
	        /*
	        nextMatch = currentTournament.GetNextMatchInCurrentRound();
	        Sequence seq = DOTween.Sequence();
            if (nextMatch != null)
            {              
            	screen.ShowButtons(false);
	            seq.AppendInterval(1f).AppendCallback(StartNextMatch);  
            }
            else
            {	            
	            seq.AppendInterval(1f).AppendCallback(ExitSimulation);  	            
	        }*/
        }
        
	    Sequence seq = DOTween.Sequence();
	    if(isSimulatingMatch) UpdateCalendar();
	    else seq.AppendInterval(1f).AppendCallback(UpdateCalendar); 
    }
    
	private void UpdateCalendar()
	{
		//Populate(nextMatch, isSimulatingMatch);
		//StartSimulation();
		screen.ShowSimulationScreen();
		CalendarController.Instance.UpdateCalendar();
	}

    private void ExitSimulation()
    {
	    isSimulatingMatch = false;
	    screen.ShowContinueButton();
	    //ScreenController.Instance.ShowScreen(ScreenType.TournamentHub);
    }

	private MarkingType GetMarkingType(PlayerData _defender, Zone _zone)
    {
	    MarkingType type = MarkingType.None;

        float totalChance = 0f;
        totalChance = _defender.Prob_Marking;

	    totalChance += (float)_defender.GetAttributeBonus(_defender.Speed) / 100;
	    totalChance += (float)_defender.GetAttributeBonus(_defender.Vision) / 100;

	    Zone zone = Field.Instance.GetTeamZone(_zone, _defender.Team.IsAwayTeam);

        if (_defender.Team.IsStrategyApplicable(zone))
        {
            totalChance *= _defender.Team.GetStrategy().MarkingChance;
        }

        float r = Dice.Roll(20, 1, (int)Dice.RollType.None, Mathf.FloorToInt(totalChance));

        if (r >= 20)
        {
            type = MarkingType.Steal;
	        _defender.Fatigue -= GameData.Instance.GameModifiers.FatigueHigh* (25 / (float)_defender.Stamina);
        }
        else if (r > 10)
        {
            type = MarkingType.Close;
        }
        else if (r > 3)
        {
            type = MarkingType.Distance;
        }

        return type;
    }

	private PlayInfo CheckPossesion(PlayInfo _lastPlay, PlayInfo _currentPlay)
	{
		if (_lastPlay.Event == MatchEvent.ShotMissed) return SwitchPossesion(_lastPlay,_currentPlay);
        else if (_lastPlay.Event == MatchEvent.ScorerAnnounced) return SwitchPossesion(_lastPlay,_currentPlay);
		else if (_lastPlay.Event == MatchEvent.Offside) return SwitchPossesion(_lastPlay,_currentPlay);
        else if (_lastPlay.Event == MatchEvent.ShotSaved) return SwitchPossesion(_lastPlay,_currentPlay);
        else if (_lastPlay.Attacker == null) return SwitchPossesion(_lastPlay,_currentPlay);
        else if (_lastPlay.Marking == MarkingType.Steal) return SwitchPossesion(_lastPlay,_currentPlay);
        else if (!_lastPlay.IsActionSuccessful && _lastPlay.Event == MatchEvent.None) return SwitchPossesion(_lastPlay,_currentPlay);
        
		//NO SWITCH
		_currentPlay.AttackingTeam = _lastPlay.AttackingTeam;
		_currentPlay.DefendingTeam = _lastPlay.DefendingTeam;
		return _currentPlay;
    }

	private PlayInfo SwitchPossesion(PlayInfo _lastPlay, PlayInfo _currentPlay)
	{
		_currentPlay.AttackingTeam = _lastPlay.DefendingTeam;
		_currentPlay.DefendingTeam = _lastPlay.AttackingTeam;
		_currentPlay.CounterAttack = 0;
		_currentPlay.AttackingBonus = 1;
		
		return _currentPlay;
    }

	private void UpdateRating(PlayInfo _playInfo)
    {
	    PlayInfo currentPlay = _playInfo;

        float attackerRating = 0f;
        float defenderRating = 0f;

        if(currentPlay.IsActionSuccessful)
        {
           attackerRating = 0.2f;
           defenderRating = -0.1f;
        }
        else
        {
            attackerRating = -0.12f;
        }

	    if(currentPlay.IsActionDefended) defenderRating = 0.2f;
        
        if (currentPlay.Event == MatchEvent.Fault) defenderRating = -0.2f;
	    else if (currentPlay.Event == MatchEvent.ShotSaved) defenderRating = 0.2f;
	    else if(currentPlay.Event == MatchEvent.PenaltySaved) defenderRating = 2f;
	    else if (currentPlay.Event == MatchEvent.Goal)
        {
            attackerRating = 2f;
            defenderRating = -1f;
        }

        if (currentPlay.Attacker != null)
        {
            currentPlay.Attacker.MatchStats.Presence++;
            currentPlay.Attacker.MatchStats.MatchRating += attackerRating;
            if (currentPlay.Attacker.MatchStats.MatchRating > 10) currentPlay.Attacker.MatchStats.MatchRating = 10f;
            else if (currentPlay.Attacker.MatchStats.MatchRating < 0) currentPlay.Attacker.MatchStats.MatchRating = 0f;
        }
        if (currentPlay.Defender != null)
        {
            currentPlay.Defender.MatchStats.Presence++;
            currentPlay.Defender.MatchStats.MatchRating += defenderRating;
            if (currentPlay.Defender.MatchStats.MatchRating > 10) currentPlay.Defender.MatchStats.MatchRating = 10f;
            else if (currentPlay.Defender.MatchStats.MatchRating < 0) currentPlay.Defender.MatchStats.MatchRating = 0f;
        }
    }

	private void Reset()
	{
		playList = new List<PlayInfo>(150);
		secondHalfStarted = false;

		screen.Reset(MatchSpeed);

        if (screen.HomeTeamSquad != null)
        {
            screen.HomeTeamSquad.ResetFatigue();
            screen.AwayTeamSquad.ResetFatigue();
        }

		if (!isSimulatingMatch) screen.Narration.Reset();
        
		actionManager = new MatchActionManager();
		playDiceRolls = new PlayDiceRolls();
		matchEvents = new MatchEvents();
    }
}