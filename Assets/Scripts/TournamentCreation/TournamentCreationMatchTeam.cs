using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TournamentCreationMatchTeam : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nameLabel;

    [SerializeField]
    Image frameImage;

    TournamentData.TeamMatchData data;

    public void Populate(TournamentData.TeamMatchData _data)
    {
        data = _data;
        nameLabel.text = data.Team.Name;
        frameImage.color = data.Team.PrimaryColor;
        if(_data.Team.IsPlaceholder)
        {
            nameLabel.color = data.Team.SecondaryColor;
        }
    }

    public void ClickHandler()
    {
        TournamentCreation.Instance.RemoveTeam(data.Team);
    }
}
