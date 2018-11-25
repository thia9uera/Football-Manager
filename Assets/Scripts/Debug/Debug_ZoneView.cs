﻿using System.Collections;
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

    private float chance;

    public void Start()
    {
        transform.gameObject.name = Zone.ToString();
    }

    public void Populate(int _type)
    {
        image.color = colors[_type];
        text.text = labels[_type];
    }

    public void Populate(float _chance)
    {
        chance = _chance;

        if (_chance > 0.5f) image.color = colors[1];
        else if (_chance > 0.25f) image.color = colors[2];
        else if (_chance > 0.1) image.color = colors[3];
        else if (_chance > 0) image.color = colors[4];
        else image.color = Color.gray;
        text.text = Mathf.RoundToInt(_chance*100) + "%"; 
        //text.text = _chance.ToString();
    }

    public void OnClickHandler()
    {
        Debug_FieldView.Instance.Popup.ShowPopup(chance, Zone);
    }
}
