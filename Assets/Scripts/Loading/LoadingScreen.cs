using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class LoadingScreen : BaseScreen
{
    [SerializeField]
    TextMeshProUGUI label;

    float time;
    bool isLoaded;

    public override void Show()
    {
        base.Show();

        time = Time.time;
        List<TeamData> teams = new List<TeamData>(Resources.LoadAll<TeamData>("Teams"));
        int t = 0;
        int p = 0;
        foreach (TeamData team in teams)
        {
            MainController.Instance.AllTeams.Add(team);
            t++;
            foreach (PlayerData player in team.GetAllPlayers())
            {
                MainController.Instance.AllPlayers.Add(player);
                player.Team = team;
                EditorUtility.SetDirty(player);
                p++;
            }
            label.text = "LOADING... ( " + t + " TEAMS & " + p + " PLAYERS )";
            isLoaded = true;
        }
    }

    private void Update()
    {
        if(Time.time - time >= 1f)
        {
            if (isLoaded) MainController.Instance.Screens.ShowScreen(ScreenType.MainMenu);
            else time = Time.time;
        }
    }
}
