using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class MatchScreen : BaseScreen
{
	[SerializeField] private GameObject matchScreen = null;
	[SerializeField] private MatchController controller = null;
	[SerializeField] private Button btnContinue = null;
	[SerializeField] private Button btnManageTeam = null;
	[SerializeField] private MatchSpeedSlider speedSlider = null;
	
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
	    ShowMatchScreen();
    }
    
	public void Populate(MatchData _data, bool _isSimulating = false)
	{
		controller.Populate(_data, _isSimulating);
	}

	public void ShowMatchScreen()
    {
	    matchScreen.SetActive(true);
	    Simulation.gameObject.SetActive(false);   
	    btnContinue.gameObject.SetActive(false);
	    IntroAnim();
    }
    
	public void ShowSimulationScreen()
	{
		matchScreen.SetActive(false);
		Simulation.gameObject.SetActive(true);   
		btnContinue.gameObject.SetActive(false);
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
		controller.StartMatch();
		//controller.StartSimulation();
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
		CalendarController.Instance.UpdateCalendar();
		ScreenController.Instance.ShowScreen(ScreenType.Manager);
	}

	public void Reset(uint _matchSpeed)
    {
	    Field.ResetField();
	    speedSlider.UpdateSlider(_matchSpeed);
    }
}
