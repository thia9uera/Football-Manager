using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardPlayerView : MonoBehaviour
{
	[SerializeField] private TMP_Text posLabel= null;
	[SerializeField] private TMP_Text nameLabel = null;
	[SerializeField] private TMP_Text goalsLabel = null;
	[SerializeField] private TMP_Text assistsLabel = null;
	[SerializeField] private TMP_Text shotsLabel = null;
	[SerializeField] private TMP_Text shotsMissedLabel = null;
	[SerializeField] private TMP_Text headersLabel = null;
	[SerializeField] private TMP_Text headersMissedLabel = null;
	[SerializeField] private TMP_Text passesLabel = null;
	[SerializeField] private TMP_Text crossesLabel = null;
	[SerializeField] private TMP_Text faultsLabel = null;
	[SerializeField] private TMP_Text customLabel = null;

    
	[SerializeField] private Image frame = null; 
    
	public PlayerData Player;


    public void Populate(PlayerData _player, int _index, float _customStat)
    {
        Player = _player;

        posLabel.text = LocalizationController.Instance.GetShortPositionString(_player.Position);
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
