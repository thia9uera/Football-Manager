using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DebugController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textField;

    public void UpdateDebug()
    {
        textField.text = MainController.Instance.Match.DebugString;
       // GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
    }

    public void Toggle()
    {
        textField.text = MainController.Instance.Match.DebugString;
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
