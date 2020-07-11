using UnityEngine;

public class FieldZone : MonoBehaviour
{
    public Field.Zone Zone;

    private void Awake()
    {
        MatchZonesContainer container = GetComponentInParent<MatchZonesContainer>() as MatchZonesContainer;

        if (container != null) container.AddZone(this);
    }

    public RectTransform Rect
    {
        get
        {
            return GetComponent<RectTransform>();
        }
    }
}
