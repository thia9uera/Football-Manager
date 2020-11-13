using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{	
	[SerializeField] private ColorPick colorPickTemplate;
	[SerializeField] private Transform colorPickContainer;
	
	private List<Color> availableColors;
	private List<ColorPick> colorPickList;
	
	public int IdSelected = 0;
	
	public Color ColorSelected
	{
		get { return availableColors[IdSelected];}
	}
	
	public void Initiliaze(int _startingColor, int _blockedColor)
	{
		availableColors = GameData.Instance.Colors.TeamColors;
		IdSelected = _startingColor;
		colorPickList = new List<ColorPick>();
		int id = 0;
		foreach(Color color in availableColors)
		{
			ColorPick pick = Instantiate(colorPickTemplate, colorPickContainer);
			pick.SetColor(color);
			pick.Id = id;
			pick.SetSelected(id == _startingColor);
			pick.interactable = id != _blockedColor;
			id++;
			pick.onClick.AddListener(delegate 
			{
				SelectColor(pick.Id);               
			});
			colorPickList.Add(pick);
		}
	}
	
	public void SelectColor(int _id)
	{
		IdSelected = _id;
		foreach(ColorPick pick in colorPickList)
		{
			pick.SetSelected(pick.Id == _id);
		}
		ScreenController.Instance.CreateTeam.UpdateColorPickers();
	}
	
	public void DisableColor(int _id)
	{
		foreach(ColorPick pick in colorPickList)
		{
			pick.interactable = pick.Id != _id;
		}
	}
}
