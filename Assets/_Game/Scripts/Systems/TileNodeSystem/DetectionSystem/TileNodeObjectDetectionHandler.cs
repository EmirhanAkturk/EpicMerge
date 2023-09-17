using System;
using _Game.Scripts.Systems.TileObjectSystem;
using _Game.Scripts.Systems.TileSystem;
using UnityEngine;
using Utils;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    public class TileNodeObjectDetectionHandler : MonoBehaviour, ITileNodeDetectionHandler
    {
        public Action<TileObject> onTileObjectEntered;
        public Action<TileObject> onTileObjectExited;
        public Action<TileObject> onTileObjectPlaced;

        public void ObjectEnterTileArea(TileObject tileObject)
        {
            onTileObjectEntered?.Invoke(tileObject);
        }

        public void ObjectExitTileArea(TileObject tileObject)
        {
            onTileObjectExited?.Invoke(tileObject);
        }

        public void ObjectPlacedTileArea(TileObject tileObject)
        {
            onTileObjectPlaced?.Invoke(tileObject);
        }
    }
}
