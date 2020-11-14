using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchSimulationScreen : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI feedbackLabel = null;
	[SerializeField] private TournamentFixturesItem itemTemplate = null;
	[SerializeField] private Transform content = null;
	
	List<GameObject> itemList;

    public void UpdateFeedback(string _str)
    {
        feedbackLabel.text = _str;
    }

    public void UpdateFeedback(MatchData _data)
    {
	    string home = _data.HomeTeam.TeamData.Name;
        string homeScore = _data.HomeTeam.Statistics.Goals.ToString();
	    string away = _data.AwayTeam.TeamData.Name;
        string awayScore = _data.AwayTeam.Statistics.Goals.ToString();

        feedbackLabel.text = home + "  " + homeScore + "  X  " + awayScore + "  " + away;
    }
    
	public void AddMatch(MatchData _data)
	{
		TournamentFixturesItem match = Instantiate(itemTemplate, content);
		match.Populate(_data);
		
		if(itemList == null) itemList = new List<GameObject>();
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
