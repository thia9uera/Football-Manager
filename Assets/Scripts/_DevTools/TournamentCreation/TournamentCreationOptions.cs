using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TournamentCreationOptions : MonoBehaviour
{
    public TMP_InputField InputName;
    public TMP_Dropdown TypeDropDown;
	public TMP_Dropdown WeekdayDropdown;
    public TournamentCreationTeamList TeamList;
    public TournamentCreationTeamsAmount TeamsAmount;
    public Toggle HomeAwayToggle;
    public TournamentCreation Controller; 

    void Start ()
	{
		TypeDropDown.ClearOptions();
        List<string> list = new List<string>();
        for(int i = 0; i < 2; i++)
        {
            list.Add(((TournamentType)i).ToString());
        }
	    TypeDropDown.AddOptions(list);
        
	    WeekdayDropdown.ClearOptions();
	    list.Clear();
	    for(int j = 0; j < 7; j++)
	    {
		    list.Add(((WeekDay)j).ToString());
	    }
	    WeekdayDropdown.AddOptions(list);
	}

	public void ChampionshipSetup()
    {
        Controller.ChangeType(TournamentType.Championship);
        TeamsAmount.gameObject.SetActive(false);
	    HomeAwayToggle.gameObject.SetActive(true);
	    WeekdayDropdown.gameObject.SetActive(false);
    }

    public void CupSetup()
    {
        Controller.ChangeType(TournamentType.Cup);
        TeamsAmount.gameObject.SetActive(true);
	    HomeAwayToggle.gameObject.SetActive(false);  
	    WeekdayDropdown.gameObject.SetActive(true);
    }

    public void OnTypeDropdownChange()
    {
        switch (TypeDropDown.value)
        {
            case 0:
                ChampionshipSetup();
                break;
            case 1:
                CupSetup();
                break;
        }
    }
}
