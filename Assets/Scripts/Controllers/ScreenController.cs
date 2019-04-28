using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    public MainMenu MainMenu;
    public TournamentHubScreen TournamentHub;
    public MatchScreen Match;
    public SquadEdit EditSQuad;

    public List<BaseScreen> ScreenList;
    public BaseScreen.ScreenType PrevScreen;
    public BaseScreen.ScreenType CurrentScreen;

    public void ShowScreen(BaseScreen.ScreenType _type)
    {
        PrevScreen = CurrentScreen;
        foreach (BaseScreen screen in ScreenList)
        {
            if (screen.Type == _type)
            {
                screen.Show();
                CurrentScreen = screen.Type;
            }
            else screen.Hide();
        }
        if (PrevScreen == BaseScreen.ScreenType.None) PrevScreen = CurrentScreen;
    }

    public void ShowPreviousScreen()
    {
        ShowScreen(PrevScreen);
    }
}
