#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentCreationTeamList : MonoBehaviour
{
	[SerializeField] private  TournamentCreationTeam teamTemplate = null;
	[SerializeField] private Transform content = null;

	private List<GameObject> itemsList;

	private void Awake ()
    {
        itemsList = new List<GameObject>();
	}

    public void UpdateTeamList()
    {
        foreach(GameObject go in itemsList)
        {
            Destroy(go);
        }

        itemsList.Clear();
        foreach (TeamData team in TournamentCreation.Instance.AvailableTeams)
        {
            if (!team.IsUserControlled)
            {
                TournamentCreationTeam item = Instantiate(teamTemplate, content);
                item.Populate(team);
                itemsList.Add(item.gameObject);
            }
        }
    }
}
#endif