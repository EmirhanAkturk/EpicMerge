using System;
using _Game.Scripts.Systems.TileNodeSystem;
using _Game.Scripts.Systems.TileObjectSystem;

namespace GameDepends
{
    public class EventService
    {
        public static Action<BaseTileObject> onTileObjectDragStart;
        public static Action<BaseTileObject> onTileObjectDragEnd;
        public static Action<BaseTileObject> onAfterTileObjectDragEnd;
        
        public static Action<TileNode, BaseTileObject> onTileObjectPlacedToTile;
        
        //Merge Events
        public static Action<bool> onCanMergeStateChange;
        public static Action onMergeCanceled;
    }
}
