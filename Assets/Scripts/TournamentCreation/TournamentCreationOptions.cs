using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TournamentCreationOptions : MonoBehaviour
{
    public TMP_InputField InputName;
    public TMP_Dropdown TypeDropDown;
    public TournamentCreationTeamList TeamList;
    public TournamentCreationStars StarsRequired;
    public TournamentCreationTeamsAmount TeamsAmount;
    public Toggle HomeAwayToggle;

    void Start ()
    {
        TypeDropDown.ClearOptions();
        List<string> list = new List<string>();
        for(int i = 0; i < 2; i++)
        {
            list.Add(((TournamentAttributes.TournamentType)i).ToString());
        }
        TypeDropDown.AddOptions(list);
	}
}
