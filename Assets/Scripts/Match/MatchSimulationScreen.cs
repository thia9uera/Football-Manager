using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchSimulationScreen : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI feedbackLabel;
	[SerializeField] TournamentFixturesItem itemTemplate;
	[SerializeField] Transform content;
	
	List<GameObject> itemList;

    public void UpdateFeedback(string _str)
    {
        feedbackLabel.text = _str;
    }

    public void UpdateFeedback(MatchData _data)
    {
        string home = _data.HomeTeam.TeamAttributes.Name;
        string homeScore = _data.HomeTeam.Statistics.Goals.ToString();
        string away = _data.AwayTeam.TeamAttributes.Name;
        string awayScore = _data.AwayTeam.Statistics.Goals.ToString();

        feedbackLabel.text = home + "  " + homeScore + "  X  " + awayScore + "  " + away;
    }
    
	public void AddMatch(MatchData _data)
	{
		TournamentFixturesItem match = Instantiate(itemTemplate, content);
		match.Populate(_data);
		itemList.Add(match.gameObject);
	}
	
	public void Clear()
	{
		Debug.Log("CLEAR MATCHES");
		if (itemList == null) itemList = new List<GameObject>();
		foreach (GameObject item in itemList)
		{
			Destroy(item.gameObject);
		}
		itemList.Clear();
	}
}
