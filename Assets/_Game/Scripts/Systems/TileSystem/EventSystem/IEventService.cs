using System;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileSystem.TileObjectSystem.TileObjects;

namespace _Game.Scripts.Systems.TileSystem.EventSystem
{
    public interface IEventService
    {
        // ── Drag events ──────────────────────────────────────────────────────
        event Action<BaseTileObject> OnTileObjectDragStart;
        event Action<BaseTileObject> OnTileObjectDragEnd;
        event Action<BaseTileObject> OnAfterTileObjectDragEnd;

        // ── Placement events ─────────────────────────────────────────────────
        event Action<TileNode, BaseTileObject> OnTileObjectPlacedToTile;

        // ── Merge events ──────────────────────────────────────────────────────
        event Action<bool> OnCanMergeStateChange;
        event Action OnMergeCanceled;

        // ── Raise helpers ────────────────────────────────────────────────────
        void RaiseTileObjectDragStart(BaseTileObject obj);
        void RaiseTileObjectDragEnd(BaseTileObject obj);
        void RaiseAfterTileObjectDragEnd(BaseTileObject obj);
        void RaiseTileObjectPlacedToTile(TileNode node, BaseTileObject obj);
        void RaiseCanMergeStateChange(bool canMerge);
        void RaiseMergeCanceled();
    }
}
