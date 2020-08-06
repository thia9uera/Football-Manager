using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchEvents
{
	private MatchActionManager actionManager;
	
	public MatchEvents()
	{
		actionManager = new MatchActionManager();
	}
	
	public PlayInfo GetEventResults(PlayInfo _currentPlay, PlayInfo _lastPlay)
	{		
		PlayInfo result = _currentPlay;
		
		switch (_lastPlay.Event)
		{
			case MatchEvent.None : return result;
			case MatchEvent.ShotOnGoal: result = ResolveShotOnGoal(_currentPlay, _lastPlay); break;
			case MatchEvent.Goal: result = ResolveGoal(_currentPlay, _lastPlay); break;
			case MatchEvent.GoalAnnounced: result = ResolveGoalAnnounced(_currentPlay, _lastPlay); break;
			case MatchEvent.ScorerAnnounced: result = ResolveScorerAnnounced(_currentPlay, _lastPlay); break;
			case MatchEvent.Offside:
			case MatchEvent.Fault: result = ResolveFreekick(_currentPlay, _lastPlay); break;
			case MatchEvent.Penalty: result = ResolvePenalty(_currentPlay, _lastPlay); break;
			case MatchEvent.Goalkick: result = ResolveGoalkick(_currentPlay, _lastPlay); break;
			case MatchEvent.ShotMissed: result = ResolveShotMissed(_currentPlay, _lastPlay); break;
			case MatchEvent.PenaltySaved :
			case MatchEvent.ShotSaved: result = ResolveShotSaved(_currentPlay, _lastPlay); break;
			case MatchEvent.CornerKick: result = ResolveCornerKick(_currentPlay, _lastPlay); break;
		}
		
		return result;
	}

	private PlayInfo ResolveShotOnGoal(PlayInfo currentPlay, PlayInfo lastPlay)
	{
		currentPlay = PlayInfo.CopyPlay(lastPlay);
		currentPlay.Defender = currentPlay.DefendingTeam.Squad[0];
		currentPlay.DefensiveAction = PlayerAction.Save;
		currentPlay.Event = MatchEvent.None;

		if(currentPlay.OffensiveAction == PlayerAction.Shot) currentPlay.Attacker.MatchStats.Shots++;
		else if (currentPlay.OffensiveAction == PlayerAction.Header) currentPlay.Attacker.MatchStats.Headers++;

		return actionManager.GetShotResults(currentPlay, lastPlay);
	}

	private PlayInfo ResolveShotSaved(PlayInfo currentPlay, PlayInfo lastPlay)
	{
		currentPlay.Attacker = currentPlay.AttackingTeam.Squad[0];
		currentPlay.Attacker.MatchStats.Saves++;
		currentPlay.Zone = currentPlay.AttackingTeam.GetTeamZone(Zone.OwnGoal);
		currentPlay.OffensiveAction = actionManager.GetOffensiveAction(MarkingType.None, currentPlay.Attacker, currentPlay.Zone, false, currentPlay.CounterAttack);
		return actionManager.GetActionSuccess(currentPlay, lastPlay);
	}

	private PlayInfo ResolveGoal(PlayInfo currentPlay, PlayInfo lastPlay)
	{
		currentPlay = PlayInfo.CopyPlay(lastPlay);

		currentPlay.Attacker.MatchStats.Goals++;
		currentPlay.AttackingTeam.MatchStats.Goals++;
		currentPlay.AttackingTeam.MatchData.Scorers.Add(currentPlay.Attacker);

		if (lastPlay.OffensiveAction == PlayerAction.Header)
		{
			currentPlay.Attacker.MatchStats.GoalsByHeader++;
			currentPlay.AttackingTeam.MatchStats.GoalsByHeader++;
		}

		if (currentPlay.Assister != null) currentPlay.Assister.MatchStats.Assists++;

		currentPlay.Event = MatchEvent.GoalAnnounced;
		return currentPlay;
	}

	private PlayInfo ResolveGoalAnnounced(PlayInfo currentPlay, PlayInfo lastPlay)
	{
		currentPlay = PlayInfo.CopyPlay(lastPlay);
		currentPlay.Event = MatchEvent.ScorerAnnounced;
		return currentPlay;
	}

	private PlayInfo ResolveScorerAnnounced(PlayInfo currentPlay, PlayInfo lastPlay)
	{
		currentPlay = PlayInfo.CopyPlay(lastPlay);
		currentPlay.Event = MatchEvent.KickOff;
		return currentPlay;
	}

	private PlayInfo ResolveFreekick(PlayInfo currentPlay, PlayInfo lastPlay)
	{
		currentPlay = PlayInfo.CopyPlay(lastPlay);
		currentPlay.Attacker = currentPlay.AttackingTeam.GetBestPlayerInArea(currentPlay.Zone, AttributeType.Freekick);
		currentPlay.Defender = null;
		currentPlay.Marking = MarkingType.None;
		currentPlay.DefensiveAction = PlayerAction.None;
		currentPlay.OffensiveAction = GetFreeKickAction(currentPlay);
		currentPlay.Event = MatchEvent.None;
		if (currentPlay.OffensiveAction == PlayerAction.Shot)
		{
			currentPlay.Event = MatchEvent.ShotOnGoal;
		}
		else
		{
			currentPlay = actionManager.ResolveAction(currentPlay, lastPlay);
		}
		
		return currentPlay;
	}

	private PlayInfo ResolvePenalty(PlayInfo currentPlay, PlayInfo lastPlay)
	{
		currentPlay = PlayInfo.CopyPlay(lastPlay);
		currentPlay.Attacker = currentPlay.AttackingTeam.GetTopPlayerByAttribute(AttributeType.Penalty, currentPlay.AttackingTeam.Squad);
		currentPlay.Defender = currentPlay.DefendingTeam.Squad[0];
		currentPlay.OffensiveAction = PlayerAction.Shot;
		currentPlay.Marking = MarkingType.None;
		currentPlay.Event = MatchEvent.ShotOnGoal;
		return currentPlay;
	}

	private PlayInfo ResolveGoalkick(PlayInfo currentPlay, PlayInfo lastPlay)
	{
		currentPlay.Attacker = currentPlay.AttackingTeam.Squad[0];
		currentPlay.Defender = null;
		currentPlay.Zone = currentPlay.AttackingTeam.GetTeamZone(Zone.OwnGoal);
		currentPlay.Marking = MarkingType.None;
		currentPlay.OffensiveAction = PlayerAction.Cross;
		return actionManager.ResolveAction(currentPlay, lastPlay);
	}

	private PlayInfo ResolveShotMissed(PlayInfo currentPlay, PlayInfo lastPlay)
	{
		currentPlay = PlayInfo.CopyPlay(lastPlay);
		currentPlay.Event = MatchEvent.Goalkick;
		return currentPlay;
	}

	private PlayInfo ResolveCornerKick(PlayInfo currentPlay, PlayInfo lastPlay)
	{
		currentPlay.Attacker = currentPlay.AttackingTeam.GetTopPlayerByAttribute(AttributeType.Crossing, currentPlay.AttackingTeam.Squad, false);
		currentPlay.Zone = currentPlay.AttackingTeam.GetTeamZone(Zone.ARF);
		currentPlay.OffensiveAction = PlayerAction.Cross;
		currentPlay.Event = MatchEvent.None;
		return actionManager.ResolveAction(currentPlay, lastPlay);
	}
	
	private PlayerAction GetFreeKickAction(PlayInfo _currentPlay)
	{
		PlayerData player = _currentPlay.Attacker;
		PlayerAction action = PlayerAction.Pass;
		MarkingType marking = MarkingType.None;
		Zone zone = _currentPlay.AttackingTeam.GetTeamZone(_currentPlay.Zone);
		ActionChancePerZone zoneChance = GameData.Instance.ActionChancePerZone[(int)zone];

		float pass = player.GetActionChance(PlayerAction.Pass, zoneChance, marking, zone);
		float longPass = player.GetActionChance(PlayerAction.LongPass, zoneChance, marking, zone);
		float cross = player.GetActionChance(PlayerAction.Cross, zoneChance, marking, zone);
		float shoot = player.GetActionChance(PlayerAction.Shot, zoneChance, marking, zone);

		if (player.Team.IsStrategyApplicable(zone))
		{
			Team_Strategy teamStrategy = GameData.Instance.TeamStrategies[(int)player.Team.Strategy];
			pass *= teamStrategy.PassingChance;
			cross *= teamStrategy.CrossingChance;
			shoot *= teamStrategy.ShootingChance;
			longPass *= teamStrategy.LongPassChance;
		}

		float total = pass + longPass + cross + shoot;
		pass = pass / total;
		longPass /= total;
		cross = cross / total;
		shoot = shoot / total;

		List<KeyValuePair<PlayerAction, float>> list = new List<KeyValuePair<PlayerAction, float>>
		{
			new KeyValuePair<PlayerAction, float>(PlayerAction.Pass, pass),
			new KeyValuePair<PlayerAction, float>(PlayerAction.LongPass, longPass),
			new KeyValuePair<PlayerAction, float>(PlayerAction.Cross, cross),
			new KeyValuePair<PlayerAction, float>(PlayerAction.Shot, shoot)
        };

		float random = Random.Range(0f, 1f);
		float cumulative = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			cumulative += list[i].Value;
			if (random < cumulative)
			{
				action = list[i].Key;
				break;
			}
		}

		return action;
	}
}