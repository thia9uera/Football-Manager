using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardTileView : MonoBehaviour
{
    private Image background;

    [SerializeField]
    private Color colorShade;

    public void Awake()
    {
        background = transform.GetComponent<Image>();
    }

    public void ChangeColor()
    {
        background.color = colorShade;
    }
}
