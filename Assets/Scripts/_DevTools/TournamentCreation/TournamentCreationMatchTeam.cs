using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TournamentCreationMatchTeam : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameLabel = null;
	[SerializeField] private Image frameImage = null;
    
	private TeamData data;

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
