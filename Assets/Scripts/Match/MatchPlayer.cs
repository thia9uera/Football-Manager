using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MatchPlayer : MonoBehaviour
{
    [SerializeField]
    private Image bodyImg;

    [SerializeField]
    private TextMeshProUGUI numberLabel;

    [HideInInspector]
    public PlayerData Data;


    public void Populate(PlayerData p_data, string p_number)
    {
        Data = p_data;
        bodyImg.color = Data.Team.PrimaryColor;
        numberLabel.text = p_number;
    }

    public void MoveTo(Vector3 _pos, float _time, float _delay)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.DOMove(_pos, _time).SetDelay(_delay);
    }
}
