using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchNarration : MonoBehaviour
{
    [SerializeField]
    private MatchNarrationTextView narrationText;

    [SerializeField]
    private Transform content;

    [SerializeField]
    private ScrollRect scroll;

    public void UpdateNarration(string _text, Color _color)
    {
        MatchNarrationTextView text =  Instantiate(narrationText, content);
        text.Populate(_text, _color);
        Canvas.ForceUpdateCanvases();
        scroll.verticalNormalizedPosition = 0;
        Canvas.ForceUpdateCanvases();
    }

    public void Reset()
    {
        foreach(Transform go in content)
        {
            Destroy(go.gameObject);
        }
    }
}
