using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Perk", menuName = "Perk Data", order = 1)]
public class PerkData : ScriptableObject
{
    [Space(10)]
    public string Name;
    public string Description;

    [Space(20)]
    public Effect[] AttributesAffected;

    [System.Serializable]
    public class Effect
    {
        public PlayerData.AttributeType Attribute;
        [Tooltip("Attribute X this value")]
        public float Modifier;
    }

    [Tooltip("Leave empty if there are no restrictions")]
    public MatchController.FieldZone[] ZoneRestriction;
}
