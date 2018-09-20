using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using I2.Loc;

public class MatchNarrationTextView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI label;

    [SerializeField]
    private Image frame;

    public void Populate(string _text, Color _color)
    {
        label.text = MainController.Instance.Localization.Localize("nar_Pass_01");
        frame.color = _color;
    }
}
