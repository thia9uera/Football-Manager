using PrimitiveUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadEditField : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] zones;

    [SerializeField]
    private PrimitiveCanvas lines;

    private float fieldWidth;
    private float fieldHeight;

    private StrokeStyle lineStyle;
    private StrokeStyle lineStyle2;

    private void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();
        fieldWidth = rect.sizeDelta.x;
        fieldHeight = rect.sizeDelta.y;
        lineStyle = new StrokeStyle(Color.white, 2f, StrokeScaleMode.Absolute);
        lineStyle2 = new StrokeStyle(Color.red, 2f, StrokeScaleMode.Absolute);

        DrawLine(zones[0], zones[1]);
    }

    private void DrawLine(RectTransform _pointA, RectTransform _pointB)
    {
        float startX = _pointA.anchoredPosition.x / fieldWidth;
        float startY = _pointA.anchoredPosition.y / fieldHeight;

        float endX = _pointB.anchoredPosition.x / fieldWidth;
        float endY = _pointB.anchoredPosition.y / fieldHeight;

        lines.DrawLine(new Vector2(startX, startY), new Vector2(endX, endY), lineStyle);
    }
}
