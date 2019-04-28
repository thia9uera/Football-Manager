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
    private GameObject details, icoSubs, icoWarning;

    private float clicked = 0;
    private float clicktime = 0;
    private float clickdelay = 0.25f;
    private bool isDoubleClick;

    private Field.Zone zone;

    private bool isSub;
    private int index;

    private bool isSelected;

    public void MoveTo(Vector3 _pos, Field.Zone _zone)
    {
        zone = _zone;
        if(Player) Player.Zone = _zone;
        RectTransform rect = GetComponent<RectTransform>();
        rect.DOMove(_pos, 0.5f).OnComplete(UpdateZone);
        if (group.alpha == 0f) group.DOFade(1f, 0.5f);
    }

    private void UpdateZone()
    {
        Player.Zone = Player.Team.Formation.Zones[index];
        icoWarning.SetActive(Player.IsWronglyAssigned());
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
            Player.Zone = Player.Team.Formation.Zones[index];
            nameLabel.text = Player.GetFullName();
            overallLabel.text = Player.GetOverall().ToString();
            portrait.sprite = Player.Portrait;
            details.SetActive(true);
            UpdateZone();
        }
        else
        {
            details.SetActive(false);
            icoWarning.SetActive(false);
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
        transform.DOScale(0.8f, 0.1f);
        clicked++;
        if (clicked == 1)
        {
            clicktime = Time.time;
            isDoubleClick = false;
        }

        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            isDoubleClick = true;
            clicked = 0;
            clicktime = 0;
            OnDoubleClick();
        }
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
        transform.DOScale(1f, 0.2f).OnComplete(CheckClickStatus);
    }

    private void CheckClickStatus()
    {
        if (Time.time - clicktime > clickdelay && !isDoubleClick)
        {
            clicked = 0;
            Select();
        }
    }
    private void Select()
    {
        isSelected = !isSelected;
        icoSubs.SetActive(isSelected);
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
