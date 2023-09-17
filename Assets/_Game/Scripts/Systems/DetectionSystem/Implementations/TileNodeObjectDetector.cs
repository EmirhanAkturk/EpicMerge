using System;
using _Game.Scripts.Systems.TileObjectSystem;
using UnityEngine;
using Utils;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    [RequireComponent(typeof(Collider))]
    public class TileNodeObjectDetector : MonoBehaviour
    {
        public Action<TileObject> onTileObjectEntered;
        public Action<TileObject> onTileObjectExited;

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

        private static bool CheckIsTileObject(Collider other, out TileObject tileObject)
        {
            if (!other.gameObject.CompareTag(TILE_OBJECT_TAG))
            {
                tileObject = null;
                return false;
            }

            tileObject = other.GetComponent<TileObject>();
            
            return tileObject != null;
        }
    }
}
