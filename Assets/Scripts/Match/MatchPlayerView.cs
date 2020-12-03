using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class MatchPlayerView : MonoBehaviour
{
	[SerializeField] private TMP_Text nameLabel = null;
	[SerializeField] private TMP_Text posLabel = null;
	[SerializeField] private TMP_Text ratingLabel = null;
	[SerializeField] private Image fatigueBar = null;

	public PlayerData Player;
	
	private PlayerData newPlayer;
	private CanvasGroup canvasGroup;
	
	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
	}

    public void Populate(PlayerData _player)
    {
        Player = _player;
	    nameLabel.text = _player.FullName;
        posLabel.text = LocalizationController.Instance.GetShortPositionString(_player.Position);
        UpdateFatigue();
    }
    
	public void SwitchPlayer(PlayerData _newPlayer)
	{
		newPlayer = _newPlayer;
		newPlayer.ApplyBonus();
		float originalX = transform.position.x;
		float offsetX = Player.Team.IsAwayTeam ? 50f : -50f;
		float moveX = transform.position.x + offsetX;	
		
		Sequence seq = DOTween.Sequence();
		seq.AppendInterval(0.5f);
		seq.Append(transform.DOMoveX(moveX, 1));
		seq.Join(canvasGroup.DOFade(0, 1));
		seq.AppendCallback(UpdateToNewPlayer);
		seq.Append(transform.DOMoveX(originalX, 1));
		seq.Join(canvasGroup.DOFade(1, 1));
	}
	
	private void UpdateToNewPlayer()
	{
		Populate(newPlayer);
	}

    public void UpdateFatigue()
    {
        float fill = (float)Player.Fatigue / 100;
        fatigueBar.fillAmount = fill;

	    if (fill < 0.25f) fatigueBar.color = GameData.Instance.Colors.Negative;
	    else if (fill < 0.5f) fatigueBar.color = GameData.Instance.Colors.Warning;
	    else fatigueBar.color = GameData.Instance.Colors.Positive;;

        ratingLabel.text = Player.MatchStats.MatchRating.ToString("F1");
    }
}
