using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingScreen : BaseScreen
{
	[SerializeField] private TextMeshProUGUI label = null;

	private List<GameObject> buttons;
	private UserData[] saves;

	private float time;
	private bool isLoaded;
	
	private ScreenType screenToShowAfterLoad;

    public override void Show()
    {
	    base.Show();
	    ResetVariables();
	    AnimateLoadingText();
    }
    
	private void AnimateLoadingText()
	{
		DOTweenTMPAnimator animator = new DOTweenTMPAnimator(label);
		for (int i = 0; i < animator.textInfo.characterCount; ++i)
		{
			Vector3 currCharOffset = animator.GetCharOffset(i);
			animator.DOOffsetChar(i, currCharOffset + new Vector3(0, 20f, 0), 0.5f).SetDelay(0.1f*i).SetEase(Ease.InOutQuad).SetLoops(999, LoopType.Yoyo);
		}
	}
    
	public void LoadScreenDelay(float _delay, ScreenType _screenToShow)
	{
		screenToShowAfterLoad = _screenToShow;
		Invoke("ShowScreen", _delay);
	}
	
	public void ShowScreen()
	{
		ScreenController.Instance.ShowScreen(screenToShowAfterLoad);
	}

	private void ResetVariables()
	{
		time = Time.time;
		isLoaded = false;
	}

    private void Update()
    {
	    if(Time.time - time >= 1f)
	    {
            if (isLoaded) ScreenController.Instance.ShowScreen(screenToShowAfterLoad);
            else time = Time.time;
        }
    }
}
