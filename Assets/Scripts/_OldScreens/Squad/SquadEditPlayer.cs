﻿using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SquadEditPlayer : MonoBehaviour
{
	public PlayerData Player;
    
	[HideInInspector] public bool IsSub;
	[HideInInspector] public int Index;
	[HideInInspector] public bool isNewPlayer { set { isNewPlayerMark.SetActive(value);}}

	[SerializeField] private TMP_Text nameLabel = null;
    [SerializeField] private TMP_Text overallLabel = null;
    [SerializeField] private Image portrait = null;
    [SerializeField] private CanvasGroup group = null;
    [SerializeField] private GameObject details = null;
    [SerializeField] private GameObject icoSubs = null;
	[SerializeField] private GameObject icoWarning = null;
	[SerializeField] private GameObject isNewPlayerMark = null;
    
	private SquadScreen controller;

	private float tapSpeed = 0.25f;
    private float countdown = 0;
    private int taps = 0;

    private Zone zone;
	private bool isSelected;	

    public void MoveTo(Vector3 _pos, Zone _zone)
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
		group.alpha = 0;
        transform.DOScale(0.5f, 0.2f).From().SetDelay(_delay);
	    group.DOFade(1f, 0.2f).SetDelay(_delay);
    }

    public void FadeOut()
    {
        transform.DOScale(0f, 0.3f).SetDelay(0.2f);
        group.DOFade(0f, 0.3f).OnComplete(Destroy);
    }

	public void PopulateSub(PlayerData _player, SquadScreen _controller, float _delay=0)
    {
        controller = _controller;
        Player = _player;
        IsSub = true;

	    nameLabel.text = Player.FullName;
	    nameLabel.gameObject.SetActive(false);
        overallLabel.text = Player.GetOverall().ToString();
        portrait.sprite = Player.Portrait;
        details.SetActive(true);
        FadeScaling(_delay);
    }

    public void Populate(PlayerData _player, SquadScreen _controller, int _index)
    {
        controller = _controller;
        Player = _player;
        Index = _index;
        if (Player)
        {
            Player.Zone = Player.Team.Formation.Zones[Index];
	        nameLabel.text = Player.FullName;
            overallLabel.text = Player.GetOverall().ToString();
            portrait.sprite = Player.Portrait;
            details.SetActive(true);
	        UpdateZone();
        }
        else
        {
            details.SetActive(false);
            icoWarning.SetActive(false);
	        portrait.sprite = AtlasManager.Instance.GetPortrait("Empty");
        }
    }

    public void PopulateDrag(PlayerData _player, bool _isSub, int _index)
    {
        Player = _player;
        IsSub = _isSub;
        Index = _index;
        portrait.sprite = Player.Portrait;
        overallLabel.text = _player.GetOverall().ToString();
    }

    public void Select()
    {
        isSelected = !isSelected;
        icoSubs.SetActive(isSelected);
    }
    
	public void SetOpacity(float _value)
	{
		group.alpha = _value;
	}

    public void Empty()
    {
        Player = null;
        Populate(Player, controller, Index);
    }

	public void Destroy()
	{
		transform.DOKill();
		group.DOKill();
        Destroy(gameObject);
    }

    public void OnPointerDown()
    {
        transform.DOScale(0.9f, 0.1f);
    }

    private void OnDoubleClick()
	{
		/*
        if (IsSub)
        {
            controller.AddPlayer(Player, this);
        }
        else
        {
            controller.RemovePlayer(Player, Index);
            Empty();
		}
		*/
    }

    private void OnSingleClick()
    {
        if (IsSub) controller.SelectSubPlayer(this);
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

	    controller.HoveringPlayer = this;
	    if(IsSub) nameLabel.gameObject.SetActive(true);
    }

    public void OnPointerExit()
    {
        transform.DOScale(1f, 0.2f);

	    controller.HoveringPlayer = null;
	    if(IsSub) nameLabel.gameObject.SetActive(false);
    }
    
	public void OnBeginDrag()
	{
		controller.StartDragPlayer();
	}
	
	public void OnEndDrag()
	{
		controller.StopDragPlayer();
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
