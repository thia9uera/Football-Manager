using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textField;

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
