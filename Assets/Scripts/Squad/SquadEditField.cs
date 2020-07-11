using PrimitiveUI;
using DG.Tweening;
using UnityEngine;

public class SquadEditField : MonoBehaviour
{
    [SerializeField]
    private FieldZone[] zones;

    [SerializeField]
    private PrimitiveCanvas lines;

    [SerializeField]
    private SquadEditPlayer[] players;

    private float fieldWidth;
    private float fieldHeight;

   // private StrokeStyle lineStyle;

    private FormationData formation;

    private void Awake()
    {
        RectTransform rect = GetComponent<RectTransform>();
        fieldWidth = rect.sizeDelta.x;
        fieldHeight = rect.sizeDelta.y;
        //lineStyle = new StrokeStyle(Color.white, 4f, StrokeScaleMode.Absolute);
    }

    public void PopulatePlayers(PlayerData[] _players, FormationData _formation, SquadEdit _controller)
    {
        for(int i = 0; i < _players.Length; i++)
        {
            players[i].Populate(_players[i], _controller, i);
        }

        UpdateFormation(_formation);
    }

    public void AddPlayer(PlayerData _player, int _index, SquadEdit _controller)
    {
        players[_index].Populate(_player, _controller, _index);
    }

    public void UpdateFormation(FormationData _data)
    {
        formation = _data;
        for(int i = 0; i < _data.Zones.Length; i++)
        {
            Field.Zone zone = _data.Zones[i];
            players[i].MoveTo(GetZone(zone).transform.position, zone);
        }

        UpdateConnections();
    }

    private FieldZone GetZone(Field.Zone _zone)
    {
        FieldZone zone = null;
        foreach(FieldZone z in zones)
        {
            if (z.Zone == _zone) zone = z;
        }
        return zone;
    }

    private void UpdateConnections()
    {
        lines.Clear();

        CanvasGroup group = lines.GetComponent<CanvasGroup>();
        group.alpha = 0;

        Color color = Color.white;

        foreach (FormationData.Connection connection in formation.Connections)
        {
            int synergy = GetZonePlayer(connection.ZoneA).GetSynergyBonus(GetZonePlayer(connection.ZoneB).Synergy);

            if (synergy == 1) color = Color.green;
            else if (synergy == -1) color = Color.red;

            DrawLine(GetZoneRect(connection.ZoneA), GetZoneRect(connection.ZoneB), color);
        }

        group.DOFade(1f, 1f).SetDelay(0.5f);
    }

    private RectTransform GetZoneRect(Field.Zone _zone)
    {
        RectTransform rect = null;

        foreach (FieldZone zone in zones)
        {
            if (zone.Zone == _zone) rect = zone.Rect;
        }
        return rect;
    }

    private PlayerData GetZonePlayer(Field.Zone _zone)
    {
        PlayerData player = null;

        foreach (SquadEditPlayer p in players)
        {
            if (p.Player.Zone == _zone) player = p.Player;
        }
        return player;
    }

    private void DrawLine(RectTransform _pointA, RectTransform _pointB, Color _color)
    {
        float startX = (_pointA.anchoredPosition.x) / fieldWidth;
        float startY = (_pointA.anchoredPosition.y) / fieldHeight;

        float endX = (_pointB.anchoredPosition.x) / fieldWidth;
        float endY = (_pointB.anchoredPosition.y) / fieldHeight;

        StrokeStyle lineStyle = new StrokeStyle(_color, 4f, StrokeScaleMode.Absolute);

        lines.DrawLine(new Vector2(startX, startY), new Vector2(endX, endY), lineStyle);
    }
}
