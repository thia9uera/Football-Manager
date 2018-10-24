using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchSpeedSlider : MonoBehaviour
{
    MatchController controller;

    Slider slider;

    [SerializeField]
    TMPro.TextMeshProUGUI label;

    private void Start()
    {
        controller = MainController.Instance.Match;
        slider = GetComponent<Slider>();
        label.text = controller.MatchSpeed + "x";
    }

    public void OnSliderUpdate()
    {
        label.text = slider.value + "x";
        controller.MatchSpeed = (int)slider.value;
    }
}
