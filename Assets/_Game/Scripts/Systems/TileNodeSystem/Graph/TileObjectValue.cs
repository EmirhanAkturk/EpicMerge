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
        
        public TileObjectValue(TileObjectValue tileObjectValue)
        {
            objectId = tileObjectValue.objectId;
            objectLevel = tileObjectValue.objectLevel;
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

        private static readonly TileObjectValue EmptyTileObjectValue = new TileObjectValue(-1, 0);
        public static TileObjectValue GetEmptyTileObjectValue()
        {
            return new TileObjectValue(EmptyTileObjectValue);
        }

        // TODO Move the bottom part into extension class
        public static bool IsEmptyTileObjectValue(TileObjectValue tileObjectValue)
        {
            return tileObjectValue.Equals(EmptyTileObjectValue);
        }
    }
}
