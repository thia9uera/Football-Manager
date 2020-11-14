using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TournamentInfoFixtures : MonoBehaviour
{
	[SerializeField] private TournamentFixturesItem itemTemplate = null;
	[SerializeField] private Transform content = null;
	[SerializeField] private Button navButtonPrev = null;
	[SerializeField] private Button navButtonNext = null;
	[SerializeField] private TextMeshProUGUI roundLabel = null;

	private List<GameObject> itemList;
	private List<MatchData> matchList;
	private int currentRound = 0;
	private int totalRounds = 0;
	
	public void Populate(TournamentData _data)
	{
		currentRound = _data.CurrentRound;
		totalRounds = _data.TotalRounds;
		matchList = _data.Matches;
		UpdateMatchList();
		UpdateNavButtons();
	}

	private void PopulateList(List<MatchData> _list)
	{
		Clear();
		uint idx = 0;
		foreach (MatchData data in _list)
		{
			TournamentFixturesItem match = Instantiate(itemTemplate, content);
			match.Populate(data, idx);
			itemList.Add(match.gameObject);
			idx++;
		}
	}
	
	private void UpdateMatchList()
	{
		List<MatchData> list = new List<MatchData>();
		
		foreach(MatchData match in matchList)
		{
			if(match.Round == currentRound) list.Add(match);
		}
		
		PopulateList(list);
	}

	private void Clear()
	{
		if (itemList == null) itemList = new List<GameObject>();
		foreach (GameObject item in itemList)
		{
			Destroy(item.gameObject);
		}
		itemList.Clear();
	}
	
	public void NavButtonClickHandler(int _value)
	{
		currentRound += _value;
		UpdateMatchList();
		UpdateNavButtons();
	}
	
	private void UpdateNavButtons()
	{
		navButtonNext.interactable = currentRound < (totalRounds -1);
		navButtonPrev.interactable = currentRound > 0;
		roundLabel.text = LocalizationController.Instance.Localize("tournament_round").Replace("{0}", (currentRound + 1).ToString()).Replace("{1}", totalRounds.ToString());
	}
}
