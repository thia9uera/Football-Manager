using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquadSelectionArrowView : MonoBehaviour
{
    public PlayerAttributes.PlayerStrategy Strategy;
    private Image image;
    public bool IsSelected = false;

    public void Select(bool _select = true)
    {
        IsSelected = _select;
        image = GetComponent<Image>();
        if (_select) image.color = Color.green;
        else image.color = Color.gray;
    }

    public void HandleClick()
    {
        if(IsSelected) GetComponentInParent<SquadSelectionArrowsView>().UpdateStrategy(PlayerAttributes.PlayerStrategy.Neutral);
        else GetComponentInParent<SquadSelectionArrowsView>().UpdateStrategy(Strategy);
    }
}
