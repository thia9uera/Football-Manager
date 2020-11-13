using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class MatchScreen : BaseScreen
{
	[SerializeField] private GameObject main;
	[SerializeField] private MatchController controller;
	[SerializeField] private Button btnContinue;
	[SerializeField] private Button btnManageTeam;
	[SerializeField] private MatchSpeedSlider speedSlider;
	
	[Space(10)]
    public MatchSimulationScreen Simulation;

	[Space(10)]
    public MatchScoreView Score;
    public MatchFieldView Field;
    public MatchTeamView HomeTeamSquad;
    public MatchTeamView AwayTeamSquad;
    public MatchNarration Narration;
    

    public override void Show()
    {
        base.Show();
	    ShowMain(true);
    }
    
	public void Populate(MatchData _data, bool _isSimulating = false)
	{
		controller.Populate(_data, _isSimulating);
	}

    public void ShowMain(bool _show)
    {
        main.SetActive(_show);
	    Simulation.gameObject.SetActive(!_show);   
	    btnContinue.gameObject.SetActive(false);
	    if(_show) IntroAnim();
	    else StartMatch();
    }
    
	private void IntroAnim()
	{
		CanvasGroup scoreGroup = Score.GetComponent<CanvasGroup>();
		scoreGroup.alpha = 0;
		ShowButtons(true);
		scoreGroup.DOFade(1, 0.5f).OnComplete(StartMatch);
	}
	
	private void StartMatch()
	{
		controller.KickOff();
		//controller.StartSimulation();
		Debug.Log("START MATCH");
	}

    public void EditSquad()
    {
	    //ScreenController.Instance.EditSquad.Team = controller.GetUserTeam();
	    //ScreenController.Instance.ShowScreen(ScreenType.EditSquad);
    }
    
	public void ShowButtons(bool _value)
	{
		btnManageTeam.gameObject.SetActive(_value);
		speedSlider.gameObject.SetActive(_value);
	}
    
	public void ShowContinueButton()
	{
		btnContinue.gameObject.SetActive(true);
	}
	
	public void OnContinueButtonClicked()
	{
		ScreenController.Instance.ShowScreen(ScreenType.Manager);
	}

	public void Reset(uint _matchSpeed)
    {
	    Field.ResetField();
	    speedSlider.UpdateSlider(_matchSpeed);
    }
}
