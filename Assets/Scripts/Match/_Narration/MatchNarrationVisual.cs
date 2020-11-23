using TMPro;
using UnityEngine;

public class MatchNarrationVisual : MonoBehaviour
{
	[SerializeField] private TMP_Text narrationText = null;
	[SerializeField] private MatchFieldView fieldView = null;
	[SerializeField] private GoalNarration goalNarration = null;
	
	public void UpdateNarration(NarrationData _narData, Zone _zone = Zone.CM)
	{
		switch(_narData.Type)
		{
		case NarrationType.GoalCelebration : ShowGoalCelebration(true, _narData.Text); break;
			default :
				narrationText.text = _narData.Text;
				fieldView.UpdateFieldArea((int)_zone);
				ShowGoalCelebration(false);
				break;
		}
	}
	
	public void SetNarrationText(string _text, Zone _zone = Zone.CM)
	{
		narrationText.text = _text;
	}
	
	public void ShowGoalCelebration(bool _value = true, string _narrationText = "")
	{
		if(!_value)
		{
			goalNarration.Hide();
			return;
		}
		goalNarration.Show(_narrationText);
	}
}
