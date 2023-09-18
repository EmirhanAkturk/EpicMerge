using System;
using _Game.Scripts.Systems.TileObjectSystem;
using UnityEngine;
using Utils;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    [RequireComponent(typeof(Collider))]
    public class TileNodeObjectDetector : MonoBehaviour
    {
        public Action<BaseTileObject> onTileObjectEntered;
        public Action<BaseTileObject> onTileObjectExited;

        private const string TILE_OBJECT_TAG = "TileObject";

        public void Init()
        {
          
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!CheckIsTileObject(other, out var tileObject)) return;
        
            // LogUtility.PrintColoredLog("Tile Node Entered The Area of This Tile", LogColor.Blue);
            onTileObjectEntered?.Invoke(tileObject);
        }        
        
        private void OnTriggerExit(Collider other)
        {
            if (!CheckIsTileObject(other, out var tileObject)) return;

            // LogUtility.PrintColoredLog("Tile Node Exited The Area of This Tile", LogColor.Blue);
            onTileObjectExited?.Invoke(tileObject);
        }

        private static bool CheckIsTileObject(Collider other, out BaseTileObject baseTileObject)
        {
            if (!other.gameObject.CompareTag(TILE_OBJECT_TAG))
            {
                baseTileObject = null;
                return false;
            }

            baseTileObject = other.GetComponent<BaseTileObject>();
            
            return baseTileObject != null;
        }
    }
}
