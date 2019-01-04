using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Names", menuName = "PlayerNames", order = 1)]
public class PlayerNamesData : ScriptableObject
{
    public List<string> FirstNames;
    public List<string> LastNames;
}
