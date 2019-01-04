using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    public MainMenu MainMenu;
    public TournamentHubScreen TournamentHub;
    public MatchScreen Match;

    public List<BaseScreen> ScreenList;
    BaseScreen.ScreenType prevScreen;
    BaseScreen.ScreenType currentScreen;

    public void ShowScreen(BaseScreen.ScreenType _type)
    {
        prevScreen = currentScreen;
        foreach (BaseScreen screen in ScreenList)
        {
            if (screen.Type == _type)
            {
                screen.Show();
                currentScreen = screen.Type;
            }
            else screen.Hide();
        }
        if (prevScreen == BaseScreen.ScreenType.None) prevScreen = currentScreen;
    }

    public void ShowPreviousScreen()
    {
        ShowScreen(prevScreen);
    }
}
