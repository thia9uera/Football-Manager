using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputFieldRandom : MonoBehaviour
{
	[SerializeField] private TMP_InputField inputField;
	
	public RandomNameType Type;
	
	private void Start()
	{
		
	}
	
	public void SetText(string _value)
	{
		inputField.text = _value;
	}
	
	public string text 
	{
		set
		{
			inputField.text = value;
		}
		get 
		{
			return inputField.text;
		}
	}
	
	public void RandomButtonHandler()
	{
		string str = "";
		switch(Type)
		{
		case RandomNameType.FirstName : str = LocalizationController.Instance.GetRandomFirstName(); break;
			case RandomNameType.LastName : str = LocalizationController.Instance.GetRandomLastName(); break;
			case RandomNameType.FullName : str = LocalizationController.Instance.GetRandomFullName(); break;
			case RandomNameType.TeamName : str = LocalizationController.Instance.GetRandomTeamName(); break;
		}
		
		inputField.text = str;
	}
}
