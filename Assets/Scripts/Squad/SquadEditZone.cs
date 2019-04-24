using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadEditZone : MonoBehaviour
{
    public Field.Zone Zone;

    public RectTransform Rect
    {
        get
        {
            return GetComponent<RectTransform>();
        }
    }
}
