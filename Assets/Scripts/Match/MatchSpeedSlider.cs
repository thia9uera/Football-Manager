using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchSpeedSlider : MonoBehaviour
{
    [SerializeField]
    private MatchController controller;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    TMPro.TextMeshProUGUI label;

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
    }
}
