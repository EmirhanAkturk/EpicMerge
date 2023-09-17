using _Game.Scripts.Systems.TileNodeSystem;
using _Game.Scripts.Systems.TileObjectSystem;
using UnityEngine;

namespace _Game.Scripts.Systems.TileSystem
{
    public class TileObjectDetectionHandler : IObjectDetectionHandler
    {
        public bool IsDetectionActive { get; set; }

        private ITileNodeDetectionHandler currentTileNodeDetectionHandler;
        public void TileObjectEntered(TileObject tileObject, GameObject enteredObject)
        {
            if(!IsDetectionActive) return;
            if (!CheckHasTileNodeDetectionHandler(enteredObject, out var tileNodeDetectionHandler)) return;
            currentTileNodeDetectionHandler = tileNodeDetectionHandler;
            tileNodeDetectionHandler.ObjectEnterTileArea(tileObject);
            Debug.Log("###enteredObject : " + enteredObject.name);
        }

        public void TileObjectExited(TileObject tileObject, GameObject exitObject)
        {
            if(!IsDetectionActive) return;
            if (!CheckHasTileNodeDetectionHandler(exitObject, out var tileNodeObjectController)) return;
            if (currentTileNodeDetectionHandler == tileNodeObjectController)
            {
                currentTileNodeDetectionHandler = null;
            }
            tileNodeObjectController.ObjectExitTileArea(tileObject);
            Debug.Log("###exitObject : " + exitObject.name);
        }

        public void TileObjectPlaced(TileObject tileObject)
        {
            currentTileNodeDetectionHandler?.ObjectPlacedTileArea(tileObject);
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