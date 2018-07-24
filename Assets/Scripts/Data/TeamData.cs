using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Team", menuName = "Team Data", order = 1)]
public class TeamData : ScriptableObject
{
    [Header("Details")]
    public string Name;
    [Tooltip("Max 3 characters")]
    public string Tag;

    [Space(10)]
    public Color PrimaryColor;
    public Color SecondaryColor;

    public enum TeamFormation
    {
        _3_4_3,
        _3_5_2,
        _4_3_3,
        _3_3_4,

    }

    [Space(10)]
    public TeamFormation Formation;

    [Space(10)]
    [Header("Players")]
    public PlayerData[] Squad;
    public PlayerData[] Substitutes;

    [Space(10)]
    public PlayerData Captain;
}
