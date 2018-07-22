using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Perk", menuName = "Perk Data", order = 1)]
public class PerkData : ScriptableObject
{
    public string Name;
    public string Description;

    [Space(20)]
    public Effect[] AtributesAffected;

    [System.Serializable]
    public class Effect
    {
        public PlayerData.PlayerAttributes Attribute;
        [Tooltip("Attribute X this value")]
        public float Modifier;
    }

    [Tooltip("Leave empty if there are no restrictions")]
    public MatchController.FieldZone[] ZoneRestriction;
}
