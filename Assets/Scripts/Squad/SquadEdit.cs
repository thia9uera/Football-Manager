using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SquadEdit : MonoBehaviour
{
    public FormationsData Formations;
    private List<string> formationNameList;

    [SerializeField]
    private SquadEditField Field;

    [SerializeField]
    private TMP_Dropdown dropdown;

    [SerializeField]
    private TeamData team;

    private void Start()
    {
        UpdateDropdown();
        Show();
    }

    public void Show()
    {
        Field.PopulatePlayers(team.Squad);
        Field.UpdateFormation(team.Formation);
    }

    private void UpdateDropdown()
    {
        formationNameList = new List<string>();

        int selected = 0;
        for (int i = 0; i < Formations.List.Length; i++)
        {
            FormationData data = Formations.List[i];
            formationNameList.Add(data.Name);
            if (data.Name == team.Formation.Name) selected = i;
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(formationNameList);
        dropdown.value = selected;
    }

    public void OnDropDownSelect()
    {
        foreach (FormationData data in Formations.List)
        {
            if (data.Name == formationNameList[dropdown.value])
            {
                team.Formation = data;
                Field.UpdateFormation(data);
            }
        }
    }
}
