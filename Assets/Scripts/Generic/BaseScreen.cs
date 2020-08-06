using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScreenType
{
    None,
    MainMenu,
    TournamentHub,
    Match,
    EditSquad,
    TournamentCreation,
    TournamentSelection,
    Leaderboards,
    Loading,
	Start,
	CreateTeam,
	Manager,
	Squad,
}
    
public class BaseScreen : MonoBehaviour
{
    public ScreenType Type;

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
