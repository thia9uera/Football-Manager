using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MatchNarrationTextView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI label;

    [SerializeField]
    private Image frame;

    public void Populate(string _text, Color _color)
    {
        label.text = _text;
        frame.color = _color;
    }
}
