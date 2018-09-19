using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debug_FieldView : MonoBehaviour
{
    public PlayerData TestPlayer;

    public Dropdown dropDownPlayerPosition;

    public Dropdown dropDownPlayerStrategy;

    public Dropdown dropDownTeamStrategy;

    [SerializeField]
    private Text fatigueLabel;

    [SerializeField]
    private Slider slider;

    public void Start()
    {
        dropDownPlayerPosition.value = (int)TestPlayer.Position;

        List<string> strategyList = new List<string>();
        foreach (Player_Strategy strategy in MainController.Instance.PlayerStrategyData.player_Strategys)
        {
            strategyList.Add(strategy.Name);
        }
        dropDownPlayerStrategy.AddOptions(strategyList);
        dropDownPlayerStrategy.value = (int)TestPlayer.AssignedPosition;

        strategyList.Clear();
        foreach (Team_Strategy strategy in MainController.Instance.TeamStrategyData.team_Strategys)
        {
            strategyList.Add(strategy.Name);
        }
        dropDownTeamStrategy.AddOptions(strategyList);

        TestPlayer.Strategy = (PlayerData.PlayerStrategy)dropDownPlayerStrategy.value;
    }

    public void Test()
    {
        Debug_ZoneView zone;
        TestPlayer.ApplyBonus();
        foreach (Transform t in transform)
        {
            zone = t.GetComponent<Debug_ZoneView>();

            float chance = CalculatePresence(TestPlayer, zone.Zone);

            if (chance >= 1f)
            {
                zone.Populate(1f);
            }
            else
            {
                zone.Populate(chance);
            }
        }
    }

    public void ValueChange()
    {
        PlayerData.PlayerPosition pos = (PlayerData.PlayerPosition)dropDownPlayerPosition.value;
        TestPlayer.Position = TestPlayer.AssignedPosition = pos;
    }

    public void SetPlayerStrategy()
    {
        TestPlayer.Strategy = (PlayerData.PlayerStrategy)dropDownPlayerStrategy.value;
    }


    private float CalculatePresence(PlayerData _player, MatchController.FieldZone _zone)
    {
        float chance = _player.GetChancePerZone(_zone);

        if (chance < 1f && chance > 0f)
        {
            chance *= ((float)(_player.Speed + _player.Vision) / 200) * ((float)_player.Fatigue / 100);
        }
        return chance;
    }

    public void OnSliderChange()
    {
        TestPlayer.Fatigue = (int)slider.value;
        fatigueLabel.text = "Fatigue: " + TestPlayer.Fatigue;
    }
}
