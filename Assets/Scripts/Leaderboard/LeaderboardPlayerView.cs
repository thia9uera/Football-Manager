using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardPlayerView : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI posLabel, nameLabel, goalsLabel, assistsLabel, shotsLabel, shotsMissedLabel, headersLabel, headersMissedLabel, passesLabel, crossesLabel, faultsLabel, customLabel;

    public PlayerData Player;

    [SerializeField]
    private Image frame; 


    public void Populate(PlayerData _player, int _index, float _customStat)
    {
        Player = _player;

        posLabel.text = MainController.Instance.Localization.GetShortPositionString(_player.Position);
        nameLabel.text = _player.FirstName + " " + _player.LastName;
        goalsLabel.text = _player.Attributes.LifeTimeStats.Goals.ToString();
        assistsLabel.text = _player.Attributes.LifeTimeStats.Assists.ToString();
        shotsLabel.text = _player.Attributes.LifeTimeStats.Shots.ToString();
        shotsMissedLabel.text = _player.Attributes.LifeTimeStats.ShotsMissed.ToString();
        headersLabel.text = _player.Attributes.LifeTimeStats.Headers.ToString();
        headersMissedLabel.text = _player.Attributes.LifeTimeStats.HeadersMissed.ToString();
        passesLabel.text = _player.Attributes.LifeTimeStats.Passes.ToString();
        crossesLabel.text = _player.Attributes.LifeTimeStats.Crosses.ToString();
        faultsLabel.text = _player.Attributes.LifeTimeStats.Faults.ToString();
        customLabel.text = _customStat.ToString("F2");

        if (_index % 2 != 0) frame.color = Color.white;
    }
}
