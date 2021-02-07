using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SettingsScreen : BaseScreen
{
	public enum SettingsType
	{
		Stepper,
		Slider
	}
	
	
	[SerializeField] private BaseStepper stepperPrefab = null;
	[SerializeField] private Transform settingsContent = null;
	[SerializeField] private SettingsData[] settings = null;
	
	private Dictionary<string, UnityAction<int>> settingsDictionary;
	
	private void Start()
	{
		settingsDictionary = new Dictionary<string, UnityAction<int>>();
		settingsDictionary.Add("language", OnLanguageChange);
		settingsDictionary.Add("name_format", OnNameFormatChange);
		
		UnityAction<int> action;
		foreach(SettingsData data in settings)
		{
			switch(data.Type)
			{
				case SettingsType.Stepper :
					BaseStepper stepper = Instantiate(stepperPrefab, settingsContent);
					action = settingsDictionary[data.ActionId];					
					stepper.Populate(data.Title, data.Options, data.SelectedOption);
					stepper.OnUpdate.AddListener(action);
					break;		
			}
		}
	}
	
	private void OnLanguageChange(int _value)
	{
		LocalizationController.Instance.CurrentLanguage = (LocalizationController.Language)_value;
		MainController.Instance.User.Language = _value;
		DataController.Instance.SaveUserOnly();
	}
	
	private void OnNameFormatChange(int _value)
	{
		MainController.Instance.User.PlayerNameFormat = _value;
		DataController.Instance.SaveUserOnly();
	}
}

[System.Serializable]
public struct SettingsData
{
	public SettingsScreen.SettingsType Type;
	public string ActionId;
	public string Title;
	public string[] Options;
	public int SelectedOption;
}
