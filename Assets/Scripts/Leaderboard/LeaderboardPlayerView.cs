using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardPlayerView : MonoBehaviour
{
    [SerializeField]    public TextMeshProUGUI posLabel, nameLabel, goalsLabel, shotsLabel, shotsMissedLabel, headersLabel, headersMissedLabel,  passesLabel, crossesLabel, faultsLabel, tacklesLabel, dribblesLabel, presenceLabel, savesLabel;

    public PlayerData Player;

    [SerializeField]
    private Image frame; 


    public void Populate(PlayerData _player, int _index)
    {
        Player = _player;

        posLabel.text = MainController.Instance.Localization.GetShortPositionString(_player.Position);
        nameLabel.text = _player.FirstName + " " + _player.LastName;
        goalsLabel.text = _player.LifeTimeStats.TotalGoals.ToString();
        shotsLabel.text = _player.LifeTimeStats.TotalShots.ToString();
        shotsMissedLabel.text = _player.LifeTimeStats.TotalShotsMissed.ToString();
        headersLabel.text = _player.LifeTimeStats.TotalHeaders.ToString();
        headersMissedLabel.text = _player.LifeTimeStats.TotalHeadersMissed.ToString();
        passesLabel.text = _player.LifeTimeStats.TotalPasses.ToString();
        crossesLabel.text = _player.LifeTimeStats.TotalCrosses.ToString();
        faultsLabel.text = _player.LifeTimeStats.TotalFaults.ToString();
        tacklesLabel.text = _player.LifeTimeStats.TotalTackles.ToString();
        dribblesLabel.text = _player.LifeTimeStats.TotalDribbles.ToString();
        savesLabel.text = _player.LifeTimeStats.TotalSaves.ToString();
        presenceLabel.text = _player.LifeTimeStats.TotalPresence.ToString();

        if (_index % 2 != 0) frame.color = Color.gray;
    }
}
