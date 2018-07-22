using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Debug_ZoneView : MonoBehaviour
{
    [SerializeField]
    private Image image;

    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private Color[] colors;

    [SerializeField]
    private string[] labels;

    public MatchController.FieldZone Zone;

    public void Populate(int _type)
    {
        image.color = colors[_type];
        text.text = labels[_type];
    }
}
