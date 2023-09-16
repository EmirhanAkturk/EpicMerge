using System;

namespace _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph
{
    public struct TileNodeValue
    {
        public int objectId;
        public int objectLevel;

        public TileNodeValue(int id, int level)
        {
            objectId = id;
            objectLevel = level;
        }
        
        public bool Equals(TileNodeValue other)
        {
            return objectId == other.objectId && objectLevel == other.objectLevel;
        }

        public override bool Equals(object obj)
        {
            return obj is TileNodeValue other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(objectId, objectLevel);
        }
    }
}
