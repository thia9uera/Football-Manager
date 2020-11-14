using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonDefault : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI label = null;
	[SerializeField] protected Button button = null;

    public string Label
    {
        set
        {
            label.text = value;
        }
    }
    
	public bool Enabled
	{
		set
		{
			button.interactable = value;
		}
	}
	
	public CanvasGroup CanvasGroup
	{
		get
		{
			return transform.GetComponent<CanvasGroup>();
		}
	}
}
