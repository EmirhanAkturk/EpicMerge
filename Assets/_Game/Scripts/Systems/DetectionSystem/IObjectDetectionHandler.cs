using _Game.Scripts.Systems.TileObjectSystem;
using UnityEngine;

namespace _Game.Scripts.Systems.DetectionSystem
{
    public interface IObjectDetectionHandler
    {
        void TileObjectEntered(TileObject tileObject, GameObject enteredObject);
        void TileObjectExited(TileObject tileObject, GameObject exitObject);
        public void TileObjectPlaced(TileObject tileObject);
    }
}