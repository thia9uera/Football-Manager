using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TournamentCreationMatchTeam : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nameLabel;

    [SerializeField]
    Image frameImage;

    TeamMatchData data;

    public void Populate(TeamMatchData _data)
    {
        data = _data;
        nameLabel.text = data.TeamAttributes.Name;
        frameImage.color = data.TeamAttributes.PrimaryColor;
        if(_data.TeamAttributes.IsPlaceholder)
        {
            nameLabel.color = data.TeamAttributes.SecondaryColor;
        }
    }

    public void ClickHandler()
    {
        TournamentCreation.Instance.RemoveTeam(data.TeamAttributes.Id);
    }
}
