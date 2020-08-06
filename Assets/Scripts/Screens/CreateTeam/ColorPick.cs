using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPick : Button
{
	[HideInInspector] public int Id;
	
	[SerializeField] private GameObject selectedIndicator;
	[SerializeField] private Image colorImg;
	
	public void SetSelected(bool _value)
	{
		selectedIndicator.SetActive(_value);
	}
	
	public void SetColor(Color _color)
	{
		colorImg.color = _color;
	}
	
	public void ClickHandler()
	{
		GetComponentInParent<ColorPicker>().SelectColor(Id);
	}
}
