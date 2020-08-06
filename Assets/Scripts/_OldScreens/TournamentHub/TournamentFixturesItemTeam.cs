using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TournamentFixturesItemTeam : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nameLabel;

    [SerializeField]
    TextMeshProUGUI scoreLabel;

    [SerializeField]
    Image frame;

    public void Populate(string _name, string _rating, string _score, Color _color, bool _isPlayed)
    {
        nameLabel.text = _name + " (" +_rating + ")";
        scoreLabel.text = _score;
        frame.color = _color;

        scoreLabel.gameObject.SetActive(_isPlayed);
    }
}
