using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MatchScreen : BaseScreen
{
    [SerializeField]
    GameObject main;

    [SerializeField]
    TextMeshProUGUI version;

    [SerializeField]
    MatchStartButton startButton;

    public MatchSimulationScreen Simulation;

    public MatchScoreView Score;
    public MatchFieldView Field;
    public MatchTeamView HomeTeamSquad;
    public MatchTeamView AwayTeamSquad;
    public MatchNarration Narration;
    public MatchSpeedSlider SpeedSlider;

    MatchController controller;

    private void Awake()
    {
        controller = GetComponent<MatchController>();
    }

    public override void Show()
    {
        base.Show();
        ShowMain(true);
    }

    public void ShowMain(bool _show)
    {
        main.SetActive(_show);
        Simulation.gameObject.SetActive(!_show);
    }

    private void Update()
    {
        version.text = "v." + Application.version + "  " + Mathf.FloorToInt(1.0f / Time.smoothDeltaTime);
    }

    public void StartButtonClickHandler()
    {
        controller.StartButtonClickHandler();
        startButton.Toggle();
    }

    public void EditSquad()
    {
        MainController.Instance.Screens.EditSQuad.Team = controller.GetUserTeam();
        MainController.Instance.Screens.ShowScreen(ScreenType.EditSquad);
    }

    public void Reset()
    {
        startButton.SetLabelStart();
        Field.ResetField();
    }
}
