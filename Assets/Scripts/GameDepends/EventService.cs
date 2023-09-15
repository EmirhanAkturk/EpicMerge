using System;
using _Game.Scripts.Systems.TileNodeSystem;
using _Game.Scripts.Systems.TileObjectSystem;

namespace GameDepends
{
    public class EventService
    {
        public static Action<TileObject> onTileObjectDragStart;
        public static Action<TileObject> onTileObjectDragEnd;
        public static Action<TileObject> onAfterTileObjectDragEnd;
        
        // TODO Sent TileNode, not TileNodeController
        public static Action<TileNodeObjectController, TileObject> onTileObjectPlacedToTile;
        // public static Action<TileObject, TileNodeObjectController> onTileObjectEnteredToNode;
    }
}
