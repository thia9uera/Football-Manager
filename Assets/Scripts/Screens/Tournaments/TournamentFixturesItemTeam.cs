using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TournamentFixturesItemTeam : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI nameLabel = null;
	[SerializeField] private TextMeshProUGUI scoreLabel = null;

	public void Populate(string _name, string _rating, string _score, Color _color, bool _isPlayed)
    {
	    nameLabel.text = _name + " (" +_rating + ")";
        scoreLabel.text = _score;
        nameLabel.color = _color;

	    scoreLabel.gameObject.SetActive(_isPlayed);
    }
}
