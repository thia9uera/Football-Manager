using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreateTeamScreen : BaseScreen
{
	[Space(10)]
	[SerializeField] private InputFieldRandom coachNameField;
	[SerializeField] private InputFieldRandom teamNameField;
    	
	[Space(10)]
	[SerializeField] private ColorPicker primaryColorPicker;
	[SerializeField] private ColorPicker secondaryColorPicker;
	
	[SerializeField] private PlayerInfoList playersList;
	
	private void Start()
	{				
		primaryColorPicker.Initiliaze(0, 1);
		secondaryColorPicker.Initiliaze(1, 0);
	}
	
	public override void Show()
	{
		base.Show();
		coachNameField.RandomButtonHandler();
		teamNameField.RandomButtonHandler();
		
		playersList.Populate(MainController.Instance.UserTeam.AllPlayers);
	}
	
	public void UpdateColorPickers()
	{
		int primaryId = primaryColorPicker.IdSelected;
		int secondaryId = secondaryColorPicker.IdSelected;
		
		primaryColorPicker.DisableColor(secondaryId);
		secondaryColorPicker.DisableColor(primaryId);
	}
	
	
	public void ReadyClickHandler()
	{
		MainController.Instance.UserTeam.Name = teamNameField.text;
		MainController.Instance.UserTeam.PrimaryColor = primaryColorPicker.ColorSelected;
		MainController.Instance.UserTeam.SecondaryColor = secondaryColorPicker.ColorSelected;
		
		DataController.Instance.CreateUserData(coachNameField.text, teamNameField.text);
		
		ScreenController.Instance.ShowScreen(ScreenType.Loading);
	}
	
	public void BackClickHandler()
	{
		ScreenController.Instance.ShowScreen(ScreenType.Start);
	}
}
