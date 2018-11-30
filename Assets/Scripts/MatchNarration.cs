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

    public void UpdateNarration(string _text, Color _frameColor)
    {
        MatchNarrationTextView text = Instantiate(narrationText, content);
        text.Populate(_text, _frameColor, Color.white);
        Canvas.ForceUpdateCanvases();
        scroll.verticalNormalizedPosition = 0;
        Canvas.ForceUpdateCanvases();
    }

    public void UpdateNarration(string _text, int _variations = 1, TeamData _team = null, MatchController.FieldZone _zone = MatchController.FieldZone.OwnGoal)
    {
        MatchNarrationTextView text =  Instantiate(narrationText, content);

        int r = Random.Range(1, _variations + 1);
        _text += r;
        Color textColor = Color.white;
        Color frameColor;

        if (_team == null)
        {
            frameColor = grayFrame;
            textColor = grayText;
        }
        else
        {
            frameColor = _team.PrimaryColor;
        }

        string zone = _zone.ToString();

        text.Populate(MainController.Instance.Localization.Localize(_text), frameColor, textColor, zone);
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
