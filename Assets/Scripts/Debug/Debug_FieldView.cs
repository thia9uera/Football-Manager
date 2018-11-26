using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debug_FieldView : MonoBehaviour
{
    public static Debug_FieldView Instance;

    public PlayerData TestPlayer;

    public Dropdown DropDownPlayerPosition;

    public Dropdown DropDownPlayerStrategy;

    public Dropdown DropDownTeamStrategy;

    public Debug_Popup Popup;

    [SerializeField]
    private Text fatigueLabel;

    [SerializeField]
    private Slider slider;

    Team_Strategy teamStrategy;

    public void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void Start()
    {
        DropDownPlayerPosition.value = (int)TestPlayer.Zone;

        List<string> list = new List<string>();
        foreach (Player_Strategy strategy in MainController.Instance.PlayerStrategyData.player_Strategys)
        {
            list.Add(strategy.Name);
        }
        DropDownPlayerStrategy.AddOptions(list);
        DropDownPlayerStrategy.value = (int)TestPlayer.Strategy;

        list.Clear();
        foreach (Team_Strategy strategy in MainController.Instance.TeamStrategyData.team_Strategys)
        {
            list.Add(strategy.Name);
        }
        DropDownTeamStrategy.AddOptions(list);
        teamStrategy = MainController.Instance.TeamStrategyData.team_Strategys[DropDownTeamStrategy.value];

        list.Clear();
        DropDownPlayerPosition.ClearOptions();
        for(int i = 0; i < 31; i++)
        {
            list.Add(((MatchController.FieldZone)i).ToString());
        }
        DropDownPlayerPosition.AddOptions(list);

        TestPlayer.Strategy = (PlayerData.PlayerStrategy)DropDownPlayerStrategy.value;

        Test();
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
        MatchController.FieldZone pos = (MatchController.FieldZone)DropDownPlayerPosition.value;
        teamStrategy = MainController.Instance.TeamStrategyData.team_Strategys[DropDownTeamStrategy.value];
        SetPlayerStrategy();
        TestPlayer.Zone = pos;
        Test();
    }

    public void SetPlayerStrategy()
    {
        TestPlayer.Strategy = (PlayerData.PlayerStrategy)DropDownPlayerStrategy.value;
    }

    private float CalculatePresence(PlayerData _player, MatchController.FieldZone _zone)
    {
        float chance = _player.GetChancePerZone(_zone, true, teamStrategy);

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
        Test();
    }
}
