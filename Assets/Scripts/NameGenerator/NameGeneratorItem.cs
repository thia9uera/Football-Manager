using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NameGeneratorItem : MonoBehaviour
{
    PlayerData data;

    [SerializeField]
    TextMeshProUGUI label;

    NameGenerator controller;

    public void Populate(PlayerData _data, NameGenerator _controller)
    {
        data = _data;
        controller = _controller;
        label.text = _data.FirstName + " " + _data.LastName;
    }

    public void ClickHandler()
    {
        controller.RemovePlayer(data);
    }
}
