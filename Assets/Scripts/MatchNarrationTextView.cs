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
    private TextMeshProUGUI zone;

    [SerializeField]
    private Image frame;

    public void Populate(string _text, Color _color, Color _textColor, string _zone)
    {
        label.text = _text;
        label.color = _textColor;
        zone.text = _zone;

        frame.color = _color;
    }
}
