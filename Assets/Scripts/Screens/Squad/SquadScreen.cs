using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SquadScreen : BaseScreen
{
	[SerializeField] private SquadEditField field = null;	
	//[SerializeField] private Button btnConfirm = null;
	[SerializeField] private SquadEditSubstitutes substitutes = null;
	[SerializeField] private SquadEditPlayer dragPlayer = null;
	
	[Space(10)]
	[SerializeField] private TMP_Dropdown formationDropdown = null;
	[SerializeField] private TMP_Dropdown strategyDropdown = null;
	
	
	private TeamData teamData;
	private FormationsData formationData;
	private List<string> formationNameList;

	private List<PlayerData> squadList;
	private List<PlayerData> subsList;

	private SquadEditPlayer selectedSquadPlayer;
	private SquadEditPlayer selectedSubPlayer;

	[HideInInspector] public SquadEditPlayer HoveringPlayer;

	private bool isDragging;
	private bool hasChanged;
	
	override public void Show()
	{
		base.Show();		
		
		teamData = MainController.Instance.UserTeam;
		formationData = GameData.Instance.Formations;
		
		squadList = new List<PlayerData>(teamData.Squad);
		subsList = new List<PlayerData>(teamData.Substitutes);
		
		field.Reset();
		field.PopulatePlayers(squadList, teamData.Formation, this);
		substitutes.Clear();
		substitutes.Populate(subsList, this);
		
		UpdateFormationDropdown();
		UpdateStrategyDropdown();
		
		hasChanged = false;
	}
	
	override public void Hide()
	{
		base.Hide();
		if(hasChanged) DataController.Instance.QuickSave();
	}
	
	private void UpdateFormationDropdown()
	{
		formationNameList = new List<string>();

		int selected = 0;
		for (int i = 0; i < formationData.List.Length; i++)
		{
			FormationData data = formationData.List[i];
			formationNameList.Add(data.Name);
			if (data.Type == teamData.Formation.Type) selected = i;
		}

		formationDropdown.ClearOptions();
		formationDropdown.AddOptions(formationNameList);
		formationDropdown.value = selected;
	}
	
	public void OnFormationDropDownSelect()
	{
		foreach (FormationData data in formationData.List)
		{
			if (data.Name == formationNameList[formationDropdown.value])
			{
				teamData.Formation = data;
				field.UpdateFormation(data);
			}
		}
		hasChanged = true;
		formationDropdown.GetComponent<Animator>().SetTrigger("Normal");
	}
	
	private void UpdateStrategyDropdown()
	{
		List<Team_Strategy> list = GameData.Instance.TeamStrategies;
		List<string> strList = new List<string>();

		for (int i = 0; i < list.Count; i++)
		{
			Team_Strategy data = list[i];
			strList.Add(LocalizationController.Instance.Localize(data.Name));
		}

		strategyDropdown.ClearOptions();
		strategyDropdown.AddOptions(strList);
		strategyDropdown.value = (int)teamData.Strategy;
	}
	
	public void OnStrategyDropdownSelect()
	{
		teamData.Strategy = (TeamStrategy)strategyDropdown.value;
		hasChanged = true;
	}
	
	public void AddPlayer(PlayerData _player, SquadEditPlayer _obj, SquadEditPlayer _squadSlot=null)
	{
		if (!squadList.Contains(null)) return;
		for (int i = 0; i < squadList.Count; i++)
		{
			if (!squadList[i])
			{
				squadList[i] = _player;
				field.AddPlayer(_player, i, this);
			}
		}
		subsList.Remove(_player);
		_obj.Destroy();
		//UpdateConfirmButton();
		hasChanged = true;
	}

	public void RemovePlayer(PlayerData _player, int _index)
	{
		squadList[_index] = null;
		subsList.Add(_player);
		substitutes.AddPlayer(_player);
		//UpdateConfirmButton();
	}
	
	public void SelectSquadPlayer(SquadEditPlayer _playerSlot)
	{
		if (selectedSquadPlayer == null)
		{
			if (selectedSubPlayer != null)
			{
				SwapPlayer(selectedSubPlayer, _playerSlot);
				selectedSubPlayer.Select();
				selectedSubPlayer = null;
				return;
			}

			selectedSquadPlayer = _playerSlot;
			selectedSquadPlayer.Select();
		}
		else
		{
			if (selectedSquadPlayer == _playerSlot)
			{
				_playerSlot.Select();
				selectedSquadPlayer = null;
				return;
			}

			PlayerData player1 = selectedSquadPlayer.Player;
			PlayerData player2 = _playerSlot.Player;

			if (player1 != null) _playerSlot.Populate(player1, this, _playerSlot.Index);
			else _playerSlot.Empty();

			if (player2 != null)
			{
				selectedSquadPlayer.Populate(player2, this, selectedSquadPlayer.Index);
			}
			else selectedSquadPlayer.Empty();

			selectedSquadPlayer.Select();
			selectedSquadPlayer = null;
			return;
		}
	}

	public void SelectSubPlayer(SquadEditPlayer _playerSlot)
	{
		if (selectedSubPlayer == null)
		{
			if (selectedSquadPlayer != null)
			{
				SwapPlayer(_playerSlot, selectedSquadPlayer);
				selectedSquadPlayer.Select();
				selectedSquadPlayer = null;
				return;
			}

			selectedSubPlayer = _playerSlot;
			selectedSubPlayer.Select();
		}
		else
		{
			if (selectedSubPlayer == _playerSlot)
			{
				_playerSlot.Select();
				selectedSubPlayer = null;
			}
			else
			{
				selectedSubPlayer.Select();
				_playerSlot.Select();
				selectedSubPlayer = _playerSlot;
			}
		}
	}
	
	private void SwapPlayer(SquadEditPlayer _playerIn, SquadEditPlayer _playerOut)
	{
		if(_playerOut.Player != null) RemovePlayer(_playerOut.Player, _playerOut.Index);
		if(_playerIn.Player != null) AddPlayer(_playerIn.Player, _playerIn, _playerIn);
	}
	
	public void StartDragPlayer()
	{
		Debug.Log("START DRAG PLAYER");
		isDragging = true;
		
		if (!HoveringPlayer.IsSub) selectedSquadPlayer = HoveringPlayer;
		else selectedSubPlayer = HoveringPlayer;
		
		HoveringPlayer.SetOpacity(0.3f);
		
		dragPlayer.gameObject.SetActive(true);
		dragPlayer.PopulateDrag(HoveringPlayer.Player, HoveringPlayer.IsSub, HoveringPlayer.Index);
	}
	
	public void StopDragPlayer()
	{
		Debug.Log("STOP DRAG PLAYER");
		isDragging = false;
		if(selectedSquadPlayer) selectedSquadPlayer.SetOpacity(1);
		if(selectedSubPlayer) selectedSubPlayer.SetOpacity(1);
		
		if(HoveringPlayer != null)
		{
			if(dragPlayer.IsSub != HoveringPlayer.IsSub)
			{
				SwapPlayer(selectedSubPlayer, HoveringPlayer);
			}
			else
			{
				
			}
		}

		dragPlayer.gameObject.SetActive(false);
	}
	
	private void Update()
	{ 
		if(isDragging)
		{
			dragPlayer.transform.position = Input.mousePosition;
		}
	}
}
