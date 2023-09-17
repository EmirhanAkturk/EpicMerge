using _Game.Scripts.Systems.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;

namespace _Game.Scripts.Utils.Extensions
{
    public static class TileObjectExtensions 
    {
        // TODO Move the bottom part into extension class
        public static bool IsEmptyTileObjectValue(this TileObjectValue tileObjectValue)
        {
            return tileObjectValue.IsEmptyTileObjectValue();
        }
    }
}
