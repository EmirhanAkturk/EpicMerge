using _Game.Scripts.Systems.TileSystem.TileObjectSystem;
using _Game.Scripts.Systems.TileSystem.TileObjectSystem.TileObjects;
namespace _Game.Scripts.Systems.TileSystem.DetectionSystem
{
    public interface ITileNodeDetectionHandler
    {
        public void ObjectEnterTileArea(BaseTileObject baseTileObject);
        public void ObjectExitTileArea(BaseTileObject baseTileObject);
        public void ObjectPlacedTileArea(BaseTileObject baseTileObject);
    }
}
