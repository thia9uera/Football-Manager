using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonDefault : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI label;

    public string Label
    {
        get
        {
            return label.text;
        }

        set
        {
            label.text = value;
        }
    }
}
