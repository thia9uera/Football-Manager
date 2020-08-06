using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : BaseScreen
{
    [SerializeField]
    List<BaseScreen> SubmenuList;

    ScreenType prevScreen;
    ScreenType currentScreen;

    public override void Show()
    {
        base.Show();
        ShowSubmenu(ScreenType.TournamentSelection);
    }

    public void ShowSubmenu(ScreenType _type)
    {
        prevScreen = currentScreen;
        foreach (BaseScreen screen in SubmenuList)
        {
            if (screen.Type == _type)
            {
                screen.Show();
                currentScreen = screen.Type;
            }
            else screen.Hide();
        }
        if (prevScreen == ScreenType.None) prevScreen = currentScreen;
    }

    public void PrevSubmenu()
    {
        ShowSubmenu(prevScreen);
    }
}
