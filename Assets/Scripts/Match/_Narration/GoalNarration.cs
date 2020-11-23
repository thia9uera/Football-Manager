using UnityEngine;
using TMPro;

public class GoalNarration : MonoBehaviour
{
	[SerializeField] private TMP_Text narrationLabel = null;
	
	public void Show(string _narrationText)
	{
		if(_narrationText != "") narrationLabel.text = _narrationText;
		gameObject.SetActive(true);
	}
	
	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
