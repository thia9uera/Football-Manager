using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TournamentCreationMatchTeam : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nameLabel;

    [SerializeField]
	Image frameImage;
    
	TeamData data;

	public void Populate(TeamData _data)
    {
	    data = _data;
	    nameLabel.text = _data.Name;
	    frameImage.color = _data.PrimaryColor;
	    
	    if(_data.Id == "Placeholder") nameLabel.color = Color.red;
    }

    public void ClickHandler()
    {
        TournamentCreation.Instance.RemoveTeam(data.Id);
    }
}
