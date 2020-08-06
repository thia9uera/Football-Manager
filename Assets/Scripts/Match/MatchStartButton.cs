using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchStartButton : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI label;

    bool isPause;

    void Start()
    {
        label.text = LocalizationController.Instance.Localize("btn_Start");
    }

    public void Toggle()
    {
        isPause = !isPause;
        string str = "btn_Start";
        if (isPause) str = "btn_Pause";
        label.text = LocalizationController.Instance.Localize(str);
    }

    public void SetLabelStart()
    {
        label.text = LocalizationController.Instance.Localize("btn_Start");
    }

    public void SetLabelPause()
    {
        label.text = LocalizationController.Instance.Localize("btn_Pause");
    }
}
