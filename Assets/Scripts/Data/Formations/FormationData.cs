using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Formation", menuName = "Formation Data", order = 1)]
public class FormationData : ScriptableObject
{
    public string Name = "0-0-0";
    public Field.Zone[] Zones;

    [System.Serializable]
    public class Connection
    {
        public Field.Zone ZoneA;
        public Field.Zone ZoneB;

        public Connection(Field.Zone _zoneA, Field.Zone _zoneB)
        {
            ZoneA = _zoneA;
            ZoneB = _zoneB;
        }

        public bool Equals(Connection _other)
        {
            bool equal = false;

            if (_other.ZoneA == ZoneA && _other.ZoneB == ZoneB) equal = true;
            else if (_other.ZoneA == ZoneB && _other.ZoneB == ZoneA) equal = true;

            return equal;
        }
    }

    public List<Connection> Connections;
}
