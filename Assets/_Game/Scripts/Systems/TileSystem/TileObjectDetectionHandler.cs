using _Game.Scripts.Systems.TileObjectSystem;
using UnityEngine;

namespace _Game.Scripts.Systems.TileSystem
{
    public class TileObjectDetectionHandler : IObjectDetectionHandler
    {
        public void TileObjectEntered(TileObject tileObject, GameObject enteredObject)
        {
            if (!CheckHasTileNodeDetectionHandler(enteredObject, out var tileNodeObjectController)) return;
            tileNodeObjectController.ObjectEnterTileArea(tileObject);
            Debug.Log("###enteredObject : " + enteredObject.name);
        }

        public void TileObjectExited(TileObject tileObject, GameObject exitObject)
        {
            if (!CheckHasTileNodeDetectionHandler(exitObject, out var tileNodeObjectController)) return;
            tileNodeObjectController.ObjectExitTileArea(tileObject);
            Debug.Log("###exitObject : " + exitObject.name);
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