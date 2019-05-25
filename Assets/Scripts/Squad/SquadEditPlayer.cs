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

    public float tapSpeed = 0.25f;
    private float countdown = 0;
    private int taps = 0;

    private Field.Zone zone;

    private bool isSub;
    public int Index;

    private bool isSelected;

    public void MoveTo(Vector3 _pos, Field.Zone _zone)
    {
        zone = _zone;
        if(Player) Player.Zone = _zone;
        RectTransform rect = GetComponent<RectTransform>();
        rect.DOMove(_pos, 0.5f).OnComplete(UpdateZone);
        if (group.alpha <= 0f) group.DOFade(1f, 0.5f);
    }

    private void UpdateZone()
    {
        Player.Zone = Player.Team.Formation.Zones[Index];
        icoWarning.SetActive(Player.IsWronglyAssigned());
    }

    public void FadeScaling(float _delay=0)
    {
        transform.DOScale(0.5f, 0.2f).From().SetDelay(_delay);
        group.DOFade(0f, 0.2f).From().SetDelay(_delay);
    }

    public void FadeOut()
    {
        transform.DOScale(0f, 0.3f).SetDelay(0.2f);
        group.DOFade(0f, 0.3f).OnComplete(Destroy);
    }

    public void PopulateSub(PlayerData _player, SquadEdit _controller, float _delay=0)
    {
        controller = _controller;
        Player = _player;
        isSub = true;

        nameLabel.text = Player.GetFullName();
        overallLabel.text = Player.GetOverall().ToString();
        portrait.sprite = Player.Portrait;
        details.SetActive(true);
        FadeScaling(_delay);
    }

    public void Populate(PlayerData _player, SquadEdit _controller, int _index)
    {
        controller = _controller;
        Player = _player;
        Index = _index;

        if (Player)
        {
            Player.Zone = Player.Team.Formation.Zones[Index];
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


    public void Select()
    {
        isSelected = !isSelected;
        icoSubs.SetActive(isSelected);
    }

    public void Empty()
    {
        Player = null;
        Populate(Player, controller, Index);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    public void OnPointerDown()
    {
        transform.DOScale(0.9f, 0.1f);
    }

    private void OnDoubleClick()
    {
        if (isSub)
        {
            controller.AddPlayer(Player, this);
        }
        else
        {
            controller.RemovePlayer(Player, Index);
            Empty();
        }
    }

    private void OnSingleClick()
    {
        if (isSub) controller.SelectSubPlayer(this);
        else controller.SelectSquadPlayer(this);
    }

    public void OnPointerUp()
    {
        transform.DOScale(1f, 0.2f);
    }

    public void OnPointerClick()
    {
        countdown = tapSpeed;
        taps++;
        if (taps == 2)
        {
            CheckClicks();
        }
    }

    public void OnPointerEnter()
    {
        transform.DOScale(1.1f, 0.1f);
    }

    public void OnPointerExit()
    {
        transform.DOScale(1f, 0.2f);
    }

    private void Update()
    {
        if (countdown > 0)
        {
            countdown -= Time.deltaTime;
        }
        else if (countdown < 0)
        {
            countdown = 0;
            CheckClicks();
        }
    }

    private void CheckClicks()
    {
        if (taps >= 2)
        {
            OnDoubleClick();
        }
        if (taps == 1)
        {
            OnSingleClick();
        }
        taps = 0; 
      }
}
