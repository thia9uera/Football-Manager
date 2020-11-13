using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchVisual : BaseScreen
{
    private TeamData homeTeam;
    private TeamData awayTeam;

    [SerializeField]
    private MatchPlayer playerTemplate;

    [SerializeField]
    private Transform playersContainer;
    private Vector2 homeTeamSpawn = new Vector2(-64f, 600f);
    private Vector2 awayTeamSpawn = new Vector2(64f, 600f);

    [SerializeField]
    private MatchZonesContainer zones, leftZones, rightZones;

    [SerializeField]
    private Transform kickOffPasser, kickOffReceiver;
       
    private List<MatchPlayer> homePlayers;
    private List<MatchPlayer> awayPlayers;

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void Populate(MatchData _data)
    {
	    homeTeam = _data.HomeTeam.TeamData;
	    awayTeam = _data.AwayTeam.TeamData;


        homeTeam.ResetMatchData();
        awayTeam.ResetMatchData();

        homePlayers = new List<MatchPlayer>();
        awayPlayers = new List<MatchPlayer>();
        SpawnPlayers(homeTeam, homePlayers, homeTeamSpawn, leftZones);
        SpawnPlayers(awayTeam, awayPlayers, awayTeamSpawn, rightZones);

        homePlayers[9].MoveTo(kickOffPasser.position, 1f, 2f);
        homePlayers[10].MoveTo(kickOffReceiver.position, 1f, 2f);
    }

    private void SpawnPlayers(TeamData _team, List<MatchPlayer> _list, Vector2 _spawnPoint, MatchZonesContainer _zones)
    {
        PlayerData[] players = _team.Squad;
        FormationData formation = _team.Formation;

        for(int i=0; i < players.Length; i++)
        {
            MatchPlayer player = Instantiate(playerTemplate, playersContainer);
            player.transform.localPosition = _spawnPoint;
            player.Populate(players[i], (i + 1).ToString());
            player.gameObject.SetActive(true);
            _list.Add(player);
        }

        for(int j=0; j< formation.Zones.Length; j++)
        {
            Zone zone = formation.Zones[j];
            _list[j].MoveTo(GetZone(zone, _zones).transform.position, 1f, 0.1f*j);
        }
    }

    private FieldZone GetZone(Zone _zone, MatchZonesContainer _container)
    {
        FieldZone zone = null;
        foreach (FieldZone z in _container.ZoneList)
        {
            if (z.Zone == _zone) zone = z;
        }
        return zone;
    }

    Zone GetTeamZone(TeamData _team, Zone _zone)
    {
        Zone zone = _zone;
        if (_team == awayTeam)
        {
	        zone = Field.Instance.GetAwayTeamZone(_zone);
        }
        return zone;
    }

}
