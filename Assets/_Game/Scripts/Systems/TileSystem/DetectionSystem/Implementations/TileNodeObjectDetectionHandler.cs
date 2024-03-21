using System;
using _Game.Scripts.Systems.TileSystem.TileObjectSystem;
using _Game.Scripts.Systems.TileSystem.TileObjectSystem.TileObjects;
using UnityEngine;
namespace _Game.Scripts.Systems.TileSystem.DetectionSystem
{
    public class TileNodeObjectDetectionHandler : MonoBehaviour, ITileNodeDetectionHandler
    {
        public Action<BaseTileObject> onTileObjectEntered;
        public Action<BaseTileObject> onTileObjectExited;
        public Action<BaseTileObject> onTileObjectPlaced;

        public void ObjectEnterTileArea(BaseTileObject baseTileObject)
        {
            onTileObjectEntered?.Invoke(baseTileObject);
        }

        public void ObjectExitTileArea(BaseTileObject baseTileObject)
        {
            onTileObjectExited?.Invoke(baseTileObject);
        }

        public void ObjectPlacedTileArea(BaseTileObject baseTileObject)
        {
            onTileObjectPlaced?.Invoke(baseTileObject);
        }
    }
}
