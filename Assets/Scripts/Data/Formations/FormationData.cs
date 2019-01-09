using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Formation", menuName = "Formation Data", order = 1)]
public class FormationData : ScriptableObject
{
    public enum TeamFormation
    {
        _3_4_3,
        _3_5_2,
        _4_3_3,
        _3_3_4,
    }

    public TeamFormation Formation;
    public MatchController.FieldZone[] Zones;
}
