using System;
using _Game.Scripts.Systems.TileObjectSystem;
using UnityEngine;

namespace _Game.Scripts.Systems.DetectionSystem
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
