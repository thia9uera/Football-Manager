using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SquadSelectionView : MonoBehaviour
{
    public TeamData Team;

    public SquadMainView MainSquad;
    public SquadSubsView Subs;

    [SerializeField]
    private SquadSubView draggingObject;

    public bool IsDragging = false;
    public PlayerData selectedPlayer;
    public SquadSlotView selectedSlot;

    [SerializeField]
    private TextMeshProUGUI overallLabel;

    private List<PlayerData> playersIn;
    private List<PlayerData> playersOut;

    private List<PlayerData> oldSubs;

    [SerializeField]
    private TMP_Dropdown dropdown;

    private void Awake()
    {
        dropdown.ClearOptions();
        List<string> strategyList = new List<string>();
	    foreach (Team_Strategy strategy in GameData.Instance.TeamStrategies)
        {
            strategyList.Add(strategy.Name);
        }
        dropdown.AddOptions(strategyList);
    }

    public void Populate(TeamData _team)
    {
        Team = _team;
        oldSubs = new List<PlayerData>(Team.Substitutes);
        MainSquad.Populate(Team.Squad);
        Subs.Populate(Team.Substitutes);
        gameObject.SetActive(true);
        GetOverall();

        dropdown.value = (int)Team.Strategy;

        playersIn = new List<PlayerData>();
        playersOut = new List<PlayerData>();
    }

    public void SetStrategy()
    {
        Team.Strategy = (TeamStrategy)dropdown.value;
    }

    public void GetOverall()
    {
        int total = 0;
        foreach(PlayerData player in Team.Squad)
        {
            total += player.GetOverall();
        }

        overallLabel.text = "Overall: " + (total / Team.Squad.Length);
    }

    public void StartDragging(PlayerData _data)
    {
        selectedPlayer = _data;
        draggingObject.Populate(_data);
        draggingObject.gameObject.SetActive(true);
        IsDragging = true;
    }

    public void StopDragging()
    {
        draggingObject.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            StopDragging();
            if(selectedSlot != null)
            {
                Subs.SwapPlayers(selectedPlayer, selectedSlot.Player);
                MainSquad.SwapPlayers(selectedPlayer, selectedSlot.Player);
                selectedSlot.Populate(selectedPlayer);
            }
        }
          

        if (IsDragging)
        {
            draggingObject.transform.position = Input.mousePosition;
        }
    }

    public void Close()
    {
        List<PlayerData> updated = new List<PlayerData>(Team.Substitutes);

        foreach (PlayerData player in Team.Substitutes)
        {
            if (!updated.Contains(player))
            {
                playersIn.Add(player);
            }
        }
        foreach (PlayerData player in updated)
        {
            if (!oldSubs.Contains(player)) playersOut.Add(player);
        }

        MainController.Instance.FinishSquadEdit(playersIn, playersOut);   
    }
}
