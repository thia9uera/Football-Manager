using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MatchController : MonoBehaviour
{
	[SerializeField] private ActionChancePerZoneData actionChancePerZone = null;
	[SerializeField] private MatchScreen screen = null;

    private List<PlayInfo> playList;

    private TeamData homeTeam;
    private TeamData awayTeam;
    
    private bool keepAttacker;
	private bool keepDefender;
	private bool isGameOn;
	private bool isSimulatingMatch;
	private bool isSimulatingTournament;
	private bool isSecondHalf = false;
	private bool secondHalfStarted;
	
	private uint matchTime = 0;
	private byte turn = 0; 

	public uint MatchSpeed = 1;
	
	private MatchActionManager actionManager;
	private PlayDiceRolls playDiceRolls;
	private MatchEvents matchEvents;
	private MatchData currentMatch;
	private MatchData nextMatch;

    public void Populate(MatchData _data, bool _simulateTournament = false)
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
	    screen.Score.UpdateTime(matchTime, 0);
        screen.Score.UpdateScore(0, 0);
	    screen.Score.Populate(homeTeam.Name, 0, homeTeam.PrimaryColor, awayTeam.Name, 0, TeamDisplayColor(awayTeam));
	    
	    screen.Narration.Initialize(homeTeam.PrimaryColor, TeamDisplayColor(awayTeam), awayTeam);

        isSimulatingTournament = _simulateTournament;
	    if(_simulateTournament) StartSimulation();
    }

    public void UpdateTeams(List<PlayerData> _in, List<PlayerData> _out)
    {
        if (_in.Count > 0)
        {
            string playersIn = "In: ";
            string playersOut = "Out: ";
            PlayerData player;
            for (int i = 0; i < _in.Count; i++)
            {
                player = _in[i];
                if (i == 0) playersIn += player.FirstName + " " + player.LastName;
                else playersIn += ", " + player.FirstName + " " + player.LastName;
            }

            for (int i = 0; i < _out.Count; i++)
            {
                player = _out[i];
                if (i == 0) playersOut += player.FirstName + " " + player.LastName;
                else playersOut += ", " + player.FirstName + " " + player.LastName;
            }
            

	        screen.Narration.UpdateNarration(playersIn, TeamDisplayColor(UserTeam));
            screen.Narration.UpdateNarration(playersOut, TeamDisplayColor(UserTeam));
        }

        if(turn > 0 && turn < 90) PauseGame(false);

        screen.HomeTeamSquad.Populate(homeTeam);
        screen.AwayTeamSquad.Populate(awayTeam);
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
        KickOff();

        if (screen == null) screen = GetComponent<MatchScreen>();
	    screen.ShowMain(false);
    }

    public void KickOff()
    {
        screen.Field.UpdateFieldArea((int)Zone.CM);

	    isGameOn = true;
        if (!isSimulatingMatch)
        {
            //UpdateNarration("nar_KickOff_");
	        //StartCoroutine("GameLoop");
	        StartCoroutine("PlayNextTurn");
            StartCoroutine("Chronometer");
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
            StartTurn();
            yield return new WaitForSeconds(1f / MatchSpeed);
        }
    }
    
	private IEnumerator PlayNextTurn()
	{
		yield return new WaitForSeconds(1f / MatchSpeed);
		yield return StartCoroutine("StartTurn");
		yield return new WaitForEndOfFrame();
		yield return StartCoroutine("FinishTurn");		
	}

	private IEnumerator SimulateLoop()
	{
		Debug.Log("SIMULATE LOOP");
        while (isGameOn)
        {
	        yield return StartCoroutine("StartTurn");	        
	        yield return StartCoroutine("FinishTurn");  
	        yield return new WaitForEndOfFrame();
        }
    }

	private IEnumerator Chronometer()
    {
        while (isGameOn)
        {
            matchTime += MatchSpeed;
	        screen.Score.UpdateTime(matchTime, turn);
            yield return new WaitForSeconds((0.015f) / MatchSpeed);
        }
    }

	private IEnumerator StartTurn()
    {
	    bool evt = false;
	    PlayInfo currentPlay = new PlayInfo();
	    currentPlay.Turn = turn;
	    PlayInfo lastPlay = turn  > 0 ? playList[turn-1] : null;
	    if(lastPlay != null) currentPlay = CheckPossesion(lastPlay, currentPlay);
	    Debug.Log("START TURN " + turn);
	    //Kick off
        if (turn == 0)
        {
            evt = true;
            
	        currentPlay.Event = MatchEvent.KickOff;      
	        currentPlay.AttackingTeam = homeTeam;
	        currentPlay.DefendingTeam = awayTeam;
            
	        currentPlay = ResolveKickOff(currentPlay);
	        screen.Narration.UpdateNarration("nar_KickOff_", GameData.Instance.Colors.MediumGray);
	        yield return currentPlay;
        }
	    //Half time
        else if (!secondHalfStarted && turn >= 45 && lastPlay.Event == MatchEvent.None)
        {
	        lastPlay.Event = MatchEvent.HalfTime;
            secondHalfStarted = true;
	        currentPlay = ResolveEvents(currentPlay, lastPlay);
	        yield return currentPlay;
        }
	    //Time off
        else if (turn >= 90 && lastPlay.Event == MatchEvent.None)
        {
	        lastPlay.Event = MatchEvent.FullTime;
	        currentPlay = ResolveEvents(currentPlay, lastPlay);
	        evt = true;
        }						
        else
        {     		        
	        evt = lastPlay.Event != MatchEvent.None;
	        currentPlay = ResolveEvents(currentPlay, lastPlay);
	        yield return currentPlay;
        }	    

	    if (!evt) 
	    {
	    	currentPlay = DefineActions(currentPlay, lastPlay);
	    	yield return currentPlay;
	    }
	    
	    playList.Insert(turn, currentPlay);	   
	    
	    yield return null; 
    }
    
	public IEnumerator FinishTurn()
	{	    
		PlayInfo currentPlay = playList[turn];
		
	    #if (UNITY_EDITOR)
		DebugTextFile.Instance.DebugPlayInfo(currentPlay, homeTeam, awayTeam);
    	#endif
    	
		if(!isSimulatingMatch && isGameOn)
		{
			if(turn > 0)UpdateNarration(playList[turn-1]);
			StartCoroutine("PlayNextTurn");
		}
	    
		turn++;
		
		yield return null; 
	}
    
	private PlayInfo ResolveEvents(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{
		switch (_lastPlay.Event)
		{
			default :_currentPlay =  matchEvents.GetEventResults(_currentPlay, _lastPlay); break;
			case MatchEvent.KickOff: ResolveKickOff(_currentPlay); break;
			case MatchEvent.HalfTime: ResolveHalfTime(_currentPlay); break;
			case MatchEvent.FullTime: ResolveFullTime(); break;
		}
		return _currentPlay;
	}
	
	private PlayInfo ResolveKickOff(PlayInfo _playInfo)
	{
		_playInfo.Zone = Zone.CM;
		_playInfo.Attacker = _playInfo.AttackingTeam.GetAttackingPlayer(_playInfo.Zone, null, true);
		_playInfo.Defender = null;
		_playInfo.OffensiveAction = PlayerAction.Pass;
		_playInfo.Event = MatchEvent.None;
		_playInfo = actionManager.ResolveAction(_playInfo, null);
		return _playInfo;
	}

	private void ResolveHalfTime(PlayInfo _playInfo)
	{
		_playInfo.Event = MatchEvent.KickOff;
		_playInfo.AttackingTeam = awayTeam;
		_playInfo.DefendingTeam = homeTeam;
	}

	private void ResolveFullTime()
	{
		screen.Narration.UpdateNarration("nar_TimeUp_", GameData.Instance.Colors.MediumGray);
		isGameOn = false;
		EndGame();		
	}

	private PlayInfo DefineActions(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{
		screen.HomeTeamSquad.UpdateFatigue();
		screen.AwayTeamSquad.UpdateFatigue();

		keepAttacker = false;
		keepDefender = false;
		PlayerData playerToExclude = null;
		bool forcePlayer = false;
		
		_currentPlay.Zone = _currentPlay.TargetZone = _lastPlay.TargetZone;

		if (_lastPlay.OffensiveAction == PlayerAction.Pass || _lastPlay.OffensiveAction == PlayerAction.LongPass || _lastPlay.OffensiveAction == PlayerAction.Cross)
		{
			_currentPlay.Assister = playerToExclude = _lastPlay.Attacker;
			if (_lastPlay.IsActionSuccessful) forcePlayer = true;
		}
		else if (_lastPlay.OffensiveAction == PlayerAction.Dribble || _lastPlay.OffensiveAction == PlayerAction.Sprint) keepAttacker = true;

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
			}
			else
			{
				//Step 3: Get type of offensive currentPlay
				bool header = false;
				if (_lastPlay.IsActionSuccessful && _lastPlay.OffensiveAction == PlayerAction.Cross && _currentPlay.AttackingTeam.GetTeamZone(_currentPlay.Zone) == Zone.Box) header = true;
				_currentPlay.OffensiveAction = actionManager.GetOffensiveAction(_currentPlay.Marking, _currentPlay.Attacker, _currentPlay.Zone, header, _currentPlay.CounterAttack);

				//Step 4: Test action against defender (if there is one)
				_currentPlay = actionManager.ResolveAction(_currentPlay, _lastPlay);
			}
		}
		
		return _currentPlay;
	}	

    public void PauseGame(bool _isPaused)
    {
        isGameOn = !_isPaused;

        if(isGameOn)
        {
            StartCoroutine("GameLoop");
            StartCoroutine("Chronometer");
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

	    screen.Simulation.AddMatch(currentMatch);

	    //Save tournament match data
	    TournamentData currentTournament = MainController.Instance.GetTournamentById(currentMatch.TournamentId);
        if (currentTournament!= null)
        {
        	TeamStatistics homeTeamStats = homeTeam.GetTournamentStatistics(currentTournament.Id);
        	TeamStatistics awayTeamStats = awayTeam.GetTournamentStatistics(currentTournament.Id);
	        
	        nextMatch = currentTournament.GetNextMatch(isSimulatingTournament);
	        Sequence seq = DOTween.Sequence();
            if (nextMatch != null)
            {              
            	screen.ShowButtons(false);
	            seq.AppendInterval(1f).AppendCallback(StartNextMatch);  
            }
            else
            {	            
	            seq.AppendInterval(1f).AppendCallback(ExitSimulation);  	            
            }
        }
    }
    
	private void StartNextMatch()
	{
		Populate(nextMatch, isSimulatingTournament);
		StartSimulation();
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

	    Zone zone = Field.Instance.GetTeamZone(_zone, _defender.Team == awayTeam);

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
		if (_lastPlay.Event == MatchEvent.Goalkick) return SwitchPossesion(_lastPlay,_currentPlay);
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
		
		return _currentPlay;
    }

	private void UpdateNarration(PlayInfo _playInfo)
	{
		screen.Narration.UpdateNarration(_playInfo);		
		
        screen.Field.UpdateFieldArea((int)_playInfo.Zone);       
    }

	private void SetGlobalStrings(PlayInfo _play)
    {
        Zone attZone = _play.Zone;
        string attacker = "";
        string defender = "";
        string attTeam = "";
        string defTeam = "";
        string zone = LocalizationController.Instance.GetZoneString(attZone);
        if (_play.Attacker != null)
        {
	        attZone = Field.Instance.GetTeamZone(_play.Zone, _play.AttackingTeam == awayTeam);
            zone = LocalizationController.Instance.GetZoneString(attZone);
            attacker = _play.Attacker.FirstName;
            attTeam = _play.Attacker.Team.Name;
        }
        if(_play.Defender != null)
        {
            defender = _play.Defender.FirstName;
            defTeam = _play.Defender.Team.Name;
        }
        LocalizationController.Instance.SetGlobals(attacker, defender, attTeam, defTeam, zone);
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

    public void StartButtonClickHandler()
    {
        if (matchTime == 0) KickOff();
        else
        {
            isGameOn = !isGameOn;
            if (isGameOn)
            {
	            StartCoroutine("PlayNextTurn");
                StartCoroutine("Chronometer");
            }
        }
    }

	private void Reset()
	{
        turn = 0;
        playList = new List<PlayInfo>();
        keepAttacker = false;
        keepDefender = false;
        matchTime = 0;
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