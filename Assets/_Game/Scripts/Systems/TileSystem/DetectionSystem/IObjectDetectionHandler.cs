using _Game.Scripts.Systems.TileSystem.TileObjectSystem;
using UnityEngine;

namespace _Game.Scripts.Systems.DetectionSystem
{
    public interface IObjectDetectionHandler
    {
        void TileObjectEntered(BaseTileObject baseTileObject, GameObject enteredObject);
        void TileObjectExited(BaseTileObject baseTileObject, GameObject exitObject);
        public void TileObjectPlaced(BaseTileObject baseTileObject);
    }
}
