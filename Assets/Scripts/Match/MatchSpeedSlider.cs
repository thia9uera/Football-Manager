using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchSpeedSlider : MonoBehaviour
{
    [SerializeField] private MatchController controller = null;
    [SerializeField] private Slider slider = null;

	[SerializeField] private TMPro.TMP_Text label = null;
	
	private void Awake()
	{
		//UpdateSlider();
	}
	
    public void OnSliderUpdate()
    {
        label.text = slider.value + "x";
	    controller.MatchSpeed = (uint)slider.value;
	    slider.fillRect.gameObject.SetActive(slider.value > slider.minValue);
    }

	public void UpdateSlider(uint _speed)
    {
        label.text = _speed + "x";
	    slider.value = _speed;
	    OnSliderUpdate();
    }
}
