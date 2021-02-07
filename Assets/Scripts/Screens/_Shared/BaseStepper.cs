using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;
using I2.Loc;

public class BaseStepper : MonoBehaviour
{
	public class OnValueChanged : UnityEvent<int>{}
	public OnValueChanged OnUpdate;
	
	[SerializeField] private TMP_Text titleLabel = null;
	[SerializeField] private TMP_Text optionLabel = null;
	[SerializeField] private Button prevButton = null;
	[SerializeField] private Button nextButton = null;
	
	private int selectedOption;
	private string[] options;
	
	public void Populate(string _title, string[] _options, int _selectedOption)
	{
		selectedOption = _selectedOption;
		options = _options;
		//titleLabel.text = _title;
		titleLabel.GetComponent<Localize>().Term = _title;
		UpdateSelection();
		if(OnUpdate == null) OnUpdate = new OnValueChanged();
	}
	
	public void OnNextClickHandler()
	{
		selectedOption++;
		UpdateSelection();
	}
	
	public void OnPrevClickHandler()
	{
		selectedOption--;
		UpdateSelection();
	}
	
	public void UpdateSelection()
	{
		prevButton.interactable = selectedOption > 0;
		nextButton.interactable = selectedOption < options.Length -1;
		//optionLabel.text = options[selectedOption];
		optionLabel.GetComponent<Localize>().Term = options[selectedOption];
		if(OnUpdate != null) OnUpdate.Invoke(selectedOption);
	}
}


