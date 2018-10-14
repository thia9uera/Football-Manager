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

    public void HideArrows(MatchController.FieldZone _pos)
    {
        foreach (SquadSelectionArrowView arrow in arrows)
        {
            switch(_pos)
            {
                case MatchController.FieldZone.BLD:
                case MatchController.FieldZone.LD:
                case MatchController.FieldZone.LDM:
                case MatchController.FieldZone.LM:
                case MatchController.FieldZone.LAM:
                case MatchController.FieldZone.LF:
                case MatchController.FieldZone.ALF:
                    if(arrow.Strategy == PlayerData.PlayerStrategy.Left || arrow.Strategy == PlayerData.PlayerStrategy.LeftDefensive || arrow.Strategy == PlayerData.PlayerStrategy.LeftOffensive)
                    {
                        arrow.gameObject.SetActive(false);
                    }
                    break;

                case MatchController.FieldZone.BRD:
                case MatchController.FieldZone.RD:
                case MatchController.FieldZone.RDM:
                case MatchController.FieldZone.RM:
                case MatchController.FieldZone.RAM:
                case MatchController.FieldZone.RF:
                case MatchController.FieldZone.ARF:
                    if (arrow.Strategy == PlayerData.PlayerStrategy.Right || arrow.Strategy == PlayerData.PlayerStrategy.RightDefensive || arrow.Strategy == PlayerData.PlayerStrategy.RightOffensive)
                    {
                        arrow.gameObject.SetActive(false);
                    }
                    break;
            }
        }
    }
}
