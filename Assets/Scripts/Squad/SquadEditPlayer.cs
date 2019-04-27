using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SquadEditPlayer : MonoBehaviour
{
    public PlayerData Player;

    [SerializeField]
    private TextMeshProUGUI nameLabel, overallLabel;

    [SerializeField]
    private Image portrait;

    [SerializeField]
    private CanvasGroup group;

    private SquadEdit controller;

    [SerializeField]
    private GameObject details;

    private float clicked = 0;
    private float clicktime = 0;
    private float clickdelay = 0.5f;

    private Field.Zone zone;

    private bool isSub;
    private int index;

    public void MoveTo(Vector3 _pos, Field.Zone _zone)
    {
        zone = _zone;
        if(Player) Player.Zone = _zone;
        RectTransform rect = GetComponent<RectTransform>();
        rect.DOMove(_pos, 0.5f);
        if (group.alpha == 0f) group.DOFade(1f, 0.5f);
    }

    public void FadeScaling()
    {
        transform.DOScale(0.5f, 0.5f).From();
        group.DOFade(0f, 0.5f).From();
    }

    public void FadeOut()
    {
        //transform.DOScale(0.5f, 0.5f);
        transform.DOScale(0f, 0.5f).SetDelay(0.3f);
        group.DOFade(0f, 0.5f).OnComplete(Destroy).SetDelay(0.3f);
    }

    public void PopulateSub(PlayerData _player, SquadEdit _controller)
    {
        controller = _controller;
        Player = _player;
        isSub = true;

        Player.Zone = zone;
        nameLabel.text = Player.GetFullName();
        overallLabel.text = Player.GetOverall().ToString();
        portrait.sprite = Player.Portrait;
        details.SetActive(true);
        FadeScaling();
    }

    public void Populate(PlayerData _player, SquadEdit _controller, int _index)
    {
        controller = _controller;
        Player = _player;
        index = _index;

        if (Player)
        {
            Player.Zone = zone;
            nameLabel.text = Player.GetFullName();
            overallLabel.text = Player.GetOverall().ToString();
            portrait.sprite = Player.Portrait;
            details.SetActive(true);
        }
        else
        {
            details.SetActive(false);
            portrait.sprite = MainController.Instance.Atlas.GetPortrait("Empty");
        }
    }

    public void Empty()
    {
        Player = null;
        Populate(Player, controller, index);
    }

    public void OnPointerDown()
    {
        clicked++;
        if (clicked == 1) clicktime = Time.time;

        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            clicked = 0;
            clicktime = 0;
            OnDoubleClick();
        }
        else if (Time.time - clicktime > 1)
        {
            clicked = 0;
        }

        transform.DOScale(0.8f, 0.1f);
    }

    private void OnDoubleClick()
    {
        if (isSub)
        {
            controller.AddPlayer(Player, this);
        }
        else
        {
            controller.RemovePlayer(Player, index);
            Empty();
        }
    }

    public void OnPointerUp()
    {
        transform.DOScale(1f, 0.2f);
    }

    private void Select()
    {

    }

    public void OnPointerEnter()
    {
        transform.DOScale(1.2f, 0.1f);
    }

    public void OnPointerExit()
    {
        transform.DOScale(1f, 0.2f);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
