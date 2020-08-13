using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScreenType
{
    None,
    Match,
    Loading,
	Start,
	CreateTeam,
	Manager,
	Squad,
	Tournaments,
	Settings,
	Calendar,
	TournamentInfo,
	TournamentTable,
	TournamentLeaderboards,
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
