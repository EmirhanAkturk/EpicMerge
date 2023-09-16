using System;

namespace _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph
{
    [Serializable]
    public struct TileObjectValue
    {
        public int objectId;
        public int objectLevel;

        public TileObjectValue(int id, int level)
        {
            objectId = id;
            objectLevel = level;
        }
        
        public bool Equals(TileObjectValue other)
        {
            return objectId == other.objectId && objectLevel == other.objectLevel;
        }

        public override bool Equals(object obj)
        {
            return obj is TileObjectValue other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(objectId, objectLevel);
        }
    }
}
