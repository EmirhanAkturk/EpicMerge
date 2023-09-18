using _Game.Scripts.Systems.TileObjectSystem;
using UnityEngine;

namespace _Game.Scripts.Systems.TileSystem
{
    public interface ITileNodeDetectionHandler
    {
        public void ObjectEnterTileArea(BaseTileObject baseTileObject);
        public void ObjectExitTileArea(BaseTileObject baseTileObject);
        public void ObjectPlacedTileArea(BaseTileObject baseTileObject);
    }
}
