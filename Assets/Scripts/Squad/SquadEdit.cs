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

    private List<PlayerData> squadList;
    private List<PlayerData> subsList;

    public SquadEditPlayer SelectedSquadPlayer;
    public SquadEditPlayer SelectedSubPlayer;

    public SquadEditPlayer HoveringPlayer;
    public SquadEditPlayer DragPlayer;
    private bool isDragging;
    private bool isHoveringSubs;

    [SerializeField]
    private GameObject hoveringSubsHit;

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

    public void AddPlayer(PlayerData _player, SquadEditPlayer _obj, SquadEditPlayer _squadSlot=null)
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

        Team.SetSquad(squadList.ToArray());
        Team.SetSubstitutes(subsList.ToArray());

        Subs.Clear();
        MainController.Instance.FinishSquadEdit(playersIn, playersOut);
    }

    public void SelectSquadPlayer(SquadEditPlayer _playerSlot)
    {
        if (SelectedSquadPlayer == null)
        {
            if (SelectedSubPlayer != null)
            {
                SwapPlayer(SelectedSubPlayer, _playerSlot);
                SelectedSubPlayer.Select();
                SelectedSubPlayer = null;
                return;
            }

            SelectedSquadPlayer = _playerSlot;
            SelectedSquadPlayer.Select();
        }
        else
        {
            if (SelectedSquadPlayer == _playerSlot)
            {
                _playerSlot.Select();
                SelectedSquadPlayer = null;
                return;
            }

            PlayerData player1 = SelectedSquadPlayer.Player;
            PlayerData player2 = _playerSlot.Player;

            if (player1 != null) _playerSlot.Populate(player1, this, _playerSlot.Index);
            else _playerSlot.Empty();

            if (player2 != null)
            {
                SelectedSquadPlayer.Populate(player2, this, SelectedSquadPlayer.Index);
            }
            else SelectedSquadPlayer.Empty();

            SelectedSquadPlayer.Select();
            SelectedSquadPlayer = null;
            return;
        }
    }

    public void SelectSubPlayer(SquadEditPlayer _playerSlot)
    {
        if (SelectedSubPlayer == null)
        {
            if (SelectedSquadPlayer != null)
            {
                SwapPlayer(_playerSlot, SelectedSquadPlayer);
                SelectedSquadPlayer.Select();
                SelectedSquadPlayer = null;
                return;
            }

            SelectedSubPlayer = _playerSlot;
            SelectedSubPlayer.Select();
        }
        else
        {
            if (SelectedSubPlayer == _playerSlot)
            {
                _playerSlot.Select();
                SelectedSubPlayer = null;
                return;
            }
            else
            {
                SelectedSubPlayer.Select();
                _playerSlot.Select();
                SelectedSubPlayer = _playerSlot;
                return;
            }

        }
    }

    private void SwapPlayer(SquadEditPlayer _playerIn, SquadEditPlayer _playerOut)
    {
        if(_playerOut.Player != null) RemovePlayer(_playerOut.Player, _playerOut.Index);
        if(_playerIn.Player != null) AddPlayer(_playerIn.Player, _playerIn, _playerIn);
    }

    public void HoveringSubs()
    {
        isHoveringSubs = true;
    }

    public void NotHoveringSubs()
    {
        isHoveringSubs = false;
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            if(!isDragging && HoveringPlayer != null && HoveringPlayer.Player != null)
            {
                isDragging = true;
                DragPlayer.PopulateDrag(HoveringPlayer.Player, HoveringPlayer.IsSub, HoveringPlayer.Index);
                if (!HoveringPlayer.IsSub) SelectedSquadPlayer = HoveringPlayer;
                else SelectedSubPlayer = HoveringPlayer;
                hoveringSubsHit.SetActive(true);
                DragPlayer.gameObject.SetActive(true);
            }
        }
        else
        {
            if(isDragging)
            {
                isDragging = false;
                hoveringSubsHit.SetActive(false);
                if (isHoveringSubs)
                {
                    if (!DragPlayer.IsSub)
                    {
                        RemovePlayer(SelectedSquadPlayer.Player, SelectedSquadPlayer.Index);
                        SelectedSquadPlayer.Empty();
                    }
                }
                else if(HoveringPlayer != null)
                {
                    if(DragPlayer.IsSub)
                    {
                        SwapPlayer(SelectedSubPlayer, HoveringPlayer);
                    }
                    else
                    {
                        PlayerData player1 = HoveringPlayer.Player;
                        PlayerData player2 = SelectedSquadPlayer.Player;

                        if(player2 != null) HoveringPlayer.Populate(player2, this, HoveringPlayer.Index);
                        if(player1 != null) SelectedSquadPlayer.Populate(player1, this, SelectedSquadPlayer.Index);
                    }
                }

                DragPlayer.gameObject.SetActive(false);
            }
        }

        if(isDragging)
        {
            DragPlayer.transform.position = Input.mousePosition;
        }
    }
}
