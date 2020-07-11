using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScreen : MonoBehaviour
{
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
        MatchVisual,
    }

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
