using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadSelectionView : MonoBehaviour
{
    public TeamData Team;

    public SquadMainView MainSquad;
    public SquadSubsView Subs;

    void Start()
    {
        MainSquad.Populate(Team.Squad);
    }
}
