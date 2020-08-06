using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfoRow : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI posLabel;
	[SerializeField] private TextMeshProUGUI nameLabel;
	[SerializeField] private TextMeshProUGUI attacklabel;
	[SerializeField] private TextMeshProUGUI defenseLabel;
	[SerializeField] private TextMeshProUGUI physicalLabel;
	[SerializeField] private TextMeshProUGUI tacticalLabel;
	[SerializeField] private TextMeshProUGUI mentalLabel;
	
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
