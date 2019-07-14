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
    public TournamentCreation Controller; 

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

    public void TournamentSetup()
    {
        Controller.ChangeType(TournamentAttributes.TournamentType.Championship);
        TeamsAmount.gameObject.SetActive(false);
        HomeAwayToggle.gameObject.SetActive(true);
    }

    public void CupSetup()
    {
        Controller.ChangeType(TournamentAttributes.TournamentType.Cup);
        TeamsAmount.gameObject.SetActive(true);
        HomeAwayToggle.gameObject.SetActive(false);       
    }

    public void OnTypeDropdownChange()
    {
        switch (TypeDropDown.value)
        {
            case 0:
                TournamentSetup();
                break;
            case 1:
                CupSetup();
                break;
        }
    }
}
