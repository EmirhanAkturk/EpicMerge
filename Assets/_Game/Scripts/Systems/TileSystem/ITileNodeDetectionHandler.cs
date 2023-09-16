using _Game.Scripts.Systems.TileObjectSystem;
using UnityEngine;

namespace _Game.Scripts.Systems.TileSystem
{
    public interface ITileNodeDetectionHandler
    {
        public void ObjectEnterTileArea(TileObject tileObject);
        public void ObjectExitTileArea(TileObject tileObject);
    }
}
