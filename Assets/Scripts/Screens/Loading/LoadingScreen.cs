using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingScreen : BaseScreen
{
	[SerializeField] private TextMeshProUGUI label;

    List<GameObject> buttons;
	private UserData[] saves;

	private float time;
	private int loaded;
	private int total;
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
		Invoke("ShowScreen", 10f);
	}
	
	public void ShowScreen()
	{
		ScreenController.Instance.ShowScreen(screenToShowAfterLoad);
	}

	public void LoadBundles()
    {
	    ResetVariables();
	    screenToShowAfterLoad = ScreenType.Start;

        AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "data_bundle"));
        if (bundle == null)
        {
            label.text = "Failed to load AssetBundle =[";
            return;
        }

        Object[] list = bundle.LoadAllAssets();
        total = list.Length;
        foreach(Object obj in list)
        {
	        loaded++;
            if (obj is PlayerData) MainController.Instance.AllPlayers.Add((PlayerData)obj);
            else if (obj is TeamData) 
            {
            	TeamData teamData = (TeamData)obj;
            	MainController.Instance.AllTeams.Add(teamData);
            	if(teamData.IsUserControlled) MainController.Instance.UserTeam = teamData;
            }
            else if (obj is TournamentData) MainController.Instance.AllTournaments.Add((TournamentData)obj);
        }

        if (loaded == total) isLoaded = true;
    }

	private void ResetVariables()
	{
		time = Time.time;
		loaded = 0;
		total = 0;
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
