using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SquadEdit : BaseScreen
{
    public SquadEditSubstitutes Subs;

    public FormationsData Formations;
    private List<string> formationNameList;

    [SerializeField]
    private SquadEditField Field;

    [SerializeField]
    private TMP_Dropdown formationDropdown, strategyDropdown;

    public TeamData Team;

    [SerializeField]
    private Button btnConfirm;

    private bool isDragging;

    private List<PlayerData> squadList;
    private List<PlayerData> subsList;

    public override void Show()
    {
        base.Show();

        if (Team == null) Team = MainController.Instance.UserTeam;
        if (Team.Formation == null) Team.Formation = Formations.List[0];

        squadList = new List<PlayerData>(Team.Squad);
        subsList = new List<PlayerData>(Team.Substitutes);

        Field.PopulatePlayers(Team.Squad, Team.Formation, this);
        UpdateFormationDropdown();
        UpdateStrategyDropdown();
        Subs.Populate(Team.Substitutes, this);

        gameObject.SetActive(true);
    }

    private void UpdateFormationDropdown()
    {
        formationNameList = new List<string>();

        int selected = 0;
        for (int i = 0; i < Formations.List.Length; i++)
        {
            FormationData data = Formations.List[i];
            formationNameList.Add(data.Name);
            if (data.Name == Team.Formation.Name) selected = i;
        }

        formationDropdown.ClearOptions();
        formationDropdown.AddOptions(formationNameList);
        formationDropdown.value = selected;
    }

    private void UpdateStrategyDropdown()
    {
        List<Team_Strategy> list = MainController.Instance.TeamStrategyData.team_Strategys;
        List<string> strList = new List<string>();

        for (int i = 0; i < list.Count; i++)
        {
            Team_Strategy data = list[i];
            strList.Add(data.Name);
        }

        strategyDropdown.ClearOptions();
        strategyDropdown.AddOptions(strList);
        strategyDropdown.value = (int)Team.Strategy;
    }

    public void OnFormationDropDownSelect()
    {
        foreach (FormationData data in Formations.List)
        {
            if (data.Name == formationNameList[formationDropdown.value])
            {
                Team.Formation = data;
                Field.UpdateFormation(data);
            }
        }
    }

    public void OnStrategyDropdownSelect()
    {
        Team.Strategy = (TeamAttributes.TeamStrategy)strategyDropdown.value;
    }

    public void AddPlayer(PlayerData _player, SquadEditPlayer _obj)
    {
        if (!squadList.Contains(null)) return;
        for (int i = 0; i < squadList.Count; i++)
        {
            if (!squadList[i])
            {
                squadList[i] = _player;
                Field.AddPlayer(_player, i, this);
            }
        }
        subsList.Remove(_player);
        _obj.FadeOut();
        UpdateConfirmButton();
    }

    public void RemovePlayer(PlayerData _player, int _index)
    {
        squadList[_index] = null;
        subsList.Add(_player);
        Subs.AddPlayer(_player);
        UpdateConfirmButton();
    }

    private void UpdateConfirmButton()
    {
        btnConfirm.interactable = !squadList.Contains(null);
    }

    public void Confirm()
    {
        List<PlayerData> oldSubs = new List<PlayerData>(Team.Substitutes);
        List<PlayerData> playersIn = new List<PlayerData>();
        List<PlayerData> playersOut = new List<PlayerData>();

        foreach (PlayerData player in oldSubs)
        {
            if (!subsList.Contains(player)) playersIn.Add(player);
        }
        foreach (PlayerData player in subsList)
        {
            if (!oldSubs.Contains(player)) playersOut.Add(player);
        }

        Team.Squad = squadList.ToArray();
        Team.Substitutes = subsList.ToArray();

        Subs.Clear();
        MainController.Instance.FinishSquadEdit(playersIn, playersOut);
    }
}
