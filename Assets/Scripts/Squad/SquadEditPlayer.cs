using DG.Tweening;
using UnityEngine;

public class SquadEditPlayer : MonoBehaviour
{
    public PlayerData Player;

    public void MoveTo(Vector3 _pos, Field.Zone _zone)
    {
        Player.Zone = _zone;
        RectTransform rect = GetComponent<RectTransform>();
        rect.DOMove(_pos, 0.5f);
    }
}
