using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SquadSelectionArrowsView : MonoBehaviour
{
    private SquadSelectionArrowView[] arrows;


    public void UpdateStrategy(PlayerAttributes.PlayerStrategy _strategy)
    {
        Transform slot = transform.parent;
        slot.GetComponent<SquadSlotView>().Player.Strategy = _strategy;

        arrows = GetComponentsInChildren<SquadSelectionArrowView>();
        foreach (SquadSelectionArrowView arrow in arrows)
        {
            arrow.Select(arrow.Strategy == _strategy);
        }
    }

    public void HideArrows(Field.Zone _pos)
    {
        foreach (SquadSelectionArrowView arrow in arrows)
        {
            switch(_pos)
            {
                case Field.Zone.BLD:
                case Field.Zone.LD:
                case Field.Zone.LDM:
                case Field.Zone.LM:
                case Field.Zone.LAM:
                case Field.Zone.LF:
                case Field.Zone.ALF:
                    if(arrow.Strategy == PlayerAttributes.PlayerStrategy.Left || arrow.Strategy == PlayerAttributes.PlayerStrategy.LeftDefensive || arrow.Strategy == PlayerAttributes.PlayerStrategy.LeftOffensive)
                    {
                        arrow.gameObject.SetActive(false);
                    }
                    break;

                case Field.Zone.BRD:
                case Field.Zone.RD:
                case Field.Zone.RDM:
                case Field.Zone.RM:
                case Field.Zone.RAM:
                case Field.Zone.RF:
                case Field.Zone.ARF:
                    if (arrow.Strategy == PlayerAttributes.PlayerStrategy.Right || arrow.Strategy == PlayerAttributes.PlayerStrategy.RightDefensive || arrow.Strategy == PlayerAttributes.PlayerStrategy.RightOffensive)
                    {
                        arrow.gameObject.SetActive(false);
                    }
                    break;
            }
        }
    }
}
