#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TournamentCreationTeam : MonoBehaviour
{
    TextMeshProUGUI nameLabel;

    TeamData data;

	void Awake ()
    {
        nameLabel = GetComponentInChildren<TextMeshProUGUI>();
	}

    public void Populate(TeamData _team)
    {
        data = _team;

        nameLabel.text = data.Name;
    }

    public void OnClickHandler()
    {
        TournamentCreation.Instance.AddTeam(data);
        Destroy(gameObject, 0.1f);
    }
}
#endif