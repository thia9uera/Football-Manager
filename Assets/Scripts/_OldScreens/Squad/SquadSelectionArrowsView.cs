using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SquadSelectionArrowsView : MonoBehaviour
{
    private SquadSelectionArrowView[] arrows;


    public void UpdateStrategy(PlayerStrategy _strategy)
    {
        Transform slot = transform.parent;
        slot.GetComponent<SquadSlotView>().Player.Strategy = _strategy;

        arrows = GetComponentsInChildren<SquadSelectionArrowView>();
        foreach (SquadSelectionArrowView arrow in arrows)
        {
            arrow.Select(arrow.Strategy == _strategy);
        }
    }

    public void HideArrows(Zone _pos)
    {
        foreach (SquadSelectionArrowView arrow in arrows)
        {
            switch(_pos)
            {
                case Zone.BLD:
                case Zone.LD:
                case Zone.LDM:
                case Zone.LM:
                case Zone.LAM:
                case Zone.LF:
                case Zone.ALF:
                    if(arrow.Strategy == PlayerStrategy.Left || arrow.Strategy == PlayerStrategy.LeftDefensive || arrow.Strategy == PlayerStrategy.LeftOffensive)
                    {
                        arrow.gameObject.SetActive(false);
                    }
                    break;

                case Zone.BRD:
                case Zone.RD:
                case Zone.RDM:
                case Zone.RM:
                case Zone.RAM:
                case Zone.RF:
                case Zone.ARF:
                    if (arrow.Strategy == PlayerStrategy.Right || arrow.Strategy == PlayerStrategy.RightDefensive || arrow.Strategy == PlayerStrategy.RightOffensive)
                    {
                        arrow.gameObject.SetActive(false);
                    }
                    break;
            }
        }
    }
}
