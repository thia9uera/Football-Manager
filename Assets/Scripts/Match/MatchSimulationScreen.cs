using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchSimulationScreen : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI feedbackLabel;

    public void UpdateFeedback(string _str)
    {
        feedbackLabel.text = _str;
    }
}
