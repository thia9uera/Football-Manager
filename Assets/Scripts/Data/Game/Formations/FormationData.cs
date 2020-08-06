using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Formation", menuName = "Data/Formation Data", order = 1)]
public class FormationData : ScriptableObject
{
    public string Name = "0-0-0";
    public Zone[] Zones;

    [System.Serializable]
    public class Connection
    {
        public Zone ZoneA;
        public Zone ZoneB;

        public Connection(Zone _zoneA, Zone _zoneB)
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
