using System;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem;
using _Game.Scripts.Systems.TileSystem.TileObjectSystem;
namespace _Game.Scripts.Systems.TileSystem.EventSystem
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
