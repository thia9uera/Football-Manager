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

    public void Populate(TeamData _team)
    {
        Team = _team;
        MainSquad.Populate(Team.Squad);
        Subs.Populate(Team.Substitutes);
        gameObject.SetActive(true);
        GetOverall();
    }

    public void GetOverall()
    {
        int total = 0;
        foreach(PlayerData player in Team.Squad)
        {
            total += player.GetOverall();
        }

        overallLabel.text = "Overall: " + (total / 11);
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
        MainController.Instance.FinishSquadEdit();   
    }
}
