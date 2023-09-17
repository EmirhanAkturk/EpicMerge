using System;
using _Game.Scripts.Systems.TileNodeSystem;
using _Game.Scripts.Systems.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileObjectSystem;

namespace GameDepends
{
    public class EventService
    {
        public static Action<TileObject> onTileObjectDragStart;
        public static Action<TileObject> onTileObjectDragEnd;
        public static Action<TileObject> onAfterTileObjectDragEnd;
        
        public static Action<TileNode, TileObject> onTileObjectPlacedToTile;
        
        //Merge Events
        public static Action<bool> onCanMergeStateChange;
        public static Action onMergeCanceled;
    }
}
