using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "PlayerTactics", menuName = "Player Tactics", order = 1)]
public class PlayerTacticsData : ScriptableObject
{
    [Space(10)]
    public string Name;
    public string Description;

    [System.Serializable]
    public class Effect
    {
        public PlayerData.PlayerAttributes Attribute;
        public float Bonus;
    }

    [Space(10)]
    public Effect[] AtributesAffected;

    [Space(10)]
    public MatchController.FieldZone[] ZoneRestriciton;
}
    
