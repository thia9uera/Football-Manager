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

    [Space(10)]
    [Header("Players")]
    public PlayerData[] Squad;
}
