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

    [SerializeField]
    private Color grayFrame;

    [SerializeField]
    private Color grayText;

    public void UpdateNarration(string _text, Color _color, int _variations = 1)
    {
        MatchNarrationTextView text =  Instantiate(narrationText, content);

        int r = Random.Range(1, _variations + 1);
        _text += r;
        Color textColor = Color.white;

        if (_color == Color.gray)
        {
            _color = grayFrame;
            textColor = grayText;
        }

        string zone = MainController.Instance.Match.CurrentZone.ToString();

        text.Populate(MainController.Instance.Localization.Localize(_text), _color, textColor, zone);
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
