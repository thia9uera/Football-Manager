using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfoRow : MonoBehaviour
{
	[SerializeField] private TMP_Text posLabel = null;
	[SerializeField] private TMP_Text nameLabel = null;
	[SerializeField] private TMP_Text attacklabel = null;
	[SerializeField] private TMP_Text defenseLabel = null;
	[SerializeField] private TMP_Text physicalLabel = null;
	[SerializeField] private TMP_Text tacticalLabel = null;
	
	public void Populate(PlayerData _data)
	{
		posLabel.text = LocalizationController.Instance.GetShortPositionString((PlayerPosition)_data.Position);
		nameLabel.text = _data.FullName;
		attacklabel.text = _data.GetSummaryAttribute(SummaryAttributeType.Attack).ToString();
		defenseLabel.text = _data.GetSummaryAttribute(SummaryAttributeType.Defense).ToString();
		physicalLabel.text = _data.GetSummaryAttribute(SummaryAttributeType.Physical).ToString();
		tacticalLabel.text = _data.GetSummaryAttribute(SummaryAttributeType.Tactical).ToString();
	}
}
