using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchScreen : BaseScreen
{
    [SerializeField]
    GameObject main, simulation;

    public override void Show()
    {
        base.Show();
        ShowMain(true);
    }

    public void ShowMain(bool _show)
    {
        main.SetActive(_show);
        simulation.SetActive(!_show);
    }
}
