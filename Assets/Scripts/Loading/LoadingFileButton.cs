using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingFileButton : Button
{
    [SerializeField]
    TextMeshProUGUI label;

    public string Label
    {
        set
        {
            GetComponentInChildren<TextMeshProUGUI>().text = value;
        }
    }
}
