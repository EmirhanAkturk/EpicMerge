using _Game.Scripts.Systems.TileObjectSystem;

namespace _Game.Scripts.Systems.DetectionSystem
{
    public interface ITileNodeDetectionHandler
    {
        public void ObjectEnterTileArea(BaseTileObject baseTileObject);
        public void ObjectExitTileArea(BaseTileObject baseTileObject);
        public void ObjectPlacedTileArea(BaseTileObject baseTileObject);
    }
}
