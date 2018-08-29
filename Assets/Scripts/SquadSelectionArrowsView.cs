using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SquadSelectionArrowsView : MonoBehaviour
{
    private SquadSelectionArrowView[] arrows;


    public void UpdateStrategy(PlayerData.PlayerStrategy _strategy)
    {
        Transform slot = transform.parent;
        slot.GetComponent<SquadSlotView>().Player.Strategy = _strategy;

        arrows = GetComponentsInChildren<SquadSelectionArrowView>();
        foreach (SquadSelectionArrowView arrow in arrows)
        {
            arrow.Select(arrow.Strategy == _strategy);
        }
    }

    public void HideArrows(PlayerData.PlayerPosition _pos)
    {
        foreach (SquadSelectionArrowView arrow in arrows)
        {
            switch(_pos)
            {
                case PlayerData.PlayerPosition.LD:
                case PlayerData.PlayerPosition.LDM:
                case PlayerData.PlayerPosition.LM:
                case PlayerData.PlayerPosition.LAM:
                case PlayerData.PlayerPosition.LF:
                    if(arrow.Strategy == PlayerData.PlayerStrategy.Left || arrow.Strategy == PlayerData.PlayerStrategy.LeftDefensive || arrow.Strategy == PlayerData.PlayerStrategy.LeftOffensive)
                    {
                        arrow.gameObject.SetActive(false);
                    }
                    break;

                case PlayerData.PlayerPosition.RD:
                case PlayerData.PlayerPosition.RDM:
                case PlayerData.PlayerPosition.RM:
                case PlayerData.PlayerPosition.RAM:
                case PlayerData.PlayerPosition.RF:
                    if (arrow.Strategy == PlayerData.PlayerStrategy.Right || arrow.Strategy == PlayerData.PlayerStrategy.RightDefensive || arrow.Strategy == PlayerData.PlayerStrategy.RightOffensive)
                    {
                        arrow.gameObject.SetActive(false);
                    }
                    break;
            }
        }
    }
}
