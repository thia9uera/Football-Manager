
using UnityEngine;
using UnityEngine.UI;

public class MatchNarration : MonoBehaviour
{
	[SerializeField] private MatchNarrationTextView narrationText = null;
	[SerializeField] private Transform content = null;
	[SerializeField] private ScrollRect scroll = null;
	[SerializeField] private Color grayFrame = Color.gray;
	[SerializeField] private Color grayText = Color.gray;

    public void UpdateNarration(string _text, Color _frameColor, string _zone)
    {
        MatchNarrationTextView text = Instantiate(narrationText, content);
        text.Populate(_text, _frameColor, Color.white, _zone);
        Canvas.ForceUpdateCanvases();
        scroll.verticalNormalizedPosition = 0;
        Canvas.ForceUpdateCanvases();
    }

    public void UpdateNarration(string _text, int _variations = 1, TeamData _team = null, Zone _zone = Zone.OwnGoal, PlayInfo _play = null)
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

        text.PlayInfo = _play;
        text.Populate(LocalizationController.Instance.Localize(_text), frameColor, textColor, zone);
        Canvas.ForceUpdateCanvases();
        scroll.verticalNormalizedPosition = 0;
        Canvas.ForceUpdateCanvases();
    }

    public void UpdateNarration(string _text, TeamData _team = null)
    {
        MatchNarrationTextView text = Instantiate(narrationText, content);

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

        string zone = "";

        text.Populate(_text, frameColor, textColor, zone);
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
