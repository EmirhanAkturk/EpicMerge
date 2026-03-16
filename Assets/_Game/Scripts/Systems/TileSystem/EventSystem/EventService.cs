using System;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileSystem.TileObjectSystem.TileObjects;

namespace _Game.Scripts.Systems.TileSystem.EventSystem
{
    public class EventService : IEventService
    {
        public event Action<BaseTileObject> OnTileObjectDragStart;
        public event Action<BaseTileObject> OnTileObjectDragEnd;
        public event Action<BaseTileObject> OnAfterTileObjectDragEnd;

        public event Action<TileNode, BaseTileObject> OnTileObjectPlacedToTile;

        public event Action<bool> OnCanMergeStateChange;
        public event Action OnMergeCanceled;

        public void RaiseTileObjectDragStart(BaseTileObject obj)       => OnTileObjectDragStart?.Invoke(obj);
        public void RaiseTileObjectDragEnd(BaseTileObject obj)         => OnTileObjectDragEnd?.Invoke(obj);
        public void RaiseAfterTileObjectDragEnd(BaseTileObject obj)    => OnAfterTileObjectDragEnd?.Invoke(obj);
        public void RaiseTileObjectPlacedToTile(TileNode node, BaseTileObject obj) => OnTileObjectPlacedToTile?.Invoke(node, obj);
        public void RaiseCanMergeStateChange(bool canMerge)            => OnCanMergeStateChange?.Invoke(canMerge);
        public void RaiseMergeCanceled()                               => OnMergeCanceled?.Invoke();
    }
}
