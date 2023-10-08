using _Game.Scripts.Systems.TileObjectSystem;
using UnityEngine;

namespace _Game.Scripts.Systems.DetectionSystem
{
    public class TileObjectDetectionHandler : IObjectDetectionHandler
    {
        private ITileNodeDetectionHandler currentTileNodeDetectionHandler;
        public void TileObjectEntered(BaseTileObject baseTileObject, GameObject enteredObject)
        {
            if (!CheckHasTileNodeDetectionHandler(enteredObject, out var tileNodeDetectionHandler)) return;
            currentTileNodeDetectionHandler = tileNodeDetectionHandler;
            tileNodeDetectionHandler.ObjectEnterTileArea(baseTileObject);
            // Debug.Log("###enteredObject : " + enteredObject.name);
        }

        public void TileObjectExited(BaseTileObject baseTileObject, GameObject exitObject)
        {
            if (!CheckHasTileNodeDetectionHandler(exitObject, out var tileNodeObjectController)) return;
            if (currentTileNodeDetectionHandler == tileNodeObjectController)
            {
                currentTileNodeDetectionHandler = null;
            }
            tileNodeObjectController.ObjectExitTileArea(baseTileObject);
            // Debug.Log("###exitObject : " + exitObject.name);
        }

        public void TileObjectPlaced(BaseTileObject baseTileObject)
        {
            currentTileNodeDetectionHandler?.ObjectPlacedTileArea(baseTileObject);
        }

        // private const string TILE_NODE_TAG = "TileNode";

        public bool CheckHasTileNodeDetectionHandler(GameObject go, out ITileNodeDetectionHandler tileNodeObjectController)
        {
            // if (!go.CompareTag(TILE_NODE_TAG))
            // {
            //     tileNodeObjectController = null;
            //     return false;
            // }

            return go.TryGetComponent(out tileNodeObjectController);
        }
    }
}