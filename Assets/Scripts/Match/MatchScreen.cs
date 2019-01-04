using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchScreen : BaseScreen
{
    [SerializeField]
    GameObject main;

    [SerializeField]
    TextMeshProUGUI version;

    public MatchSimulationScreen Simulation;

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

    public void ShowMain(bool _show)
    {
        main.SetActive(_show);
        Simulation.gameObject.SetActive(!_show);
    }

    private void Update()
    {
        version.text = "v." + Application.version + "  " + Mathf.FloorToInt(1.0f / Time.smoothDeltaTime);
    }
}
