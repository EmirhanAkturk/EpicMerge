using System.Collections.Generic;
using _Game.Scripts.Systems.ConfigurationSystem;
using _Game.Scripts.Systems.TileSystem.EventSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileSystem.TileObjectSystem;
using _Game.Scripts.Systems.TileSystem.TileObjectSystem.TileObjects;
using GameDepends;

namespace _Game.Scripts.Systems.TileSystem.TileMergeSystem
{
    public interface ITileObjectMergeHelper
    {
        void MergeCancel();
        bool CanMerge(TileNode tileObjectNode, TileNode movedNode, TileObjectValue targetValue, bool indicateMergeableObjects);
        bool CanMerge(TileNode tileObjectNode, TileNode movedNode, TileObjectValue targetValue, bool indicateMergeableObjects, out List<TileNode> wantedNodes);
        bool TryMerge(TileNode currentMovingObjectNode, TileNode movedNode, TileObjectValue targetValue);
    }

    public class TileObjectMergeHelper : ITileObjectMergeHelper
    {
        private int MergeRequiredObject => ConfigurationService.Configurations.mergeRequiredObject;

        private List<TileNode> _mergeableIndicatorShownNodes = new List<TileNode>();
        private readonly IEventService _eventService;

        public TileObjectMergeHelper(IEventService eventService)
        {
            _eventService = eventService;
            _eventService.OnTileObjectPlacedToTile += TileObjectPlacedToTile;
            _eventService.OnMergeCanceled += MergeCancel;
        }

        private void TileObjectPlacedToTile(TileNode tileNode, BaseTileObject baseTileObject)
        {
            MergeCancel();
        }

        public void MergeCancel()
        {
            UpdateMergeableObjectsIndicator(_mergeableIndicatorShownNodes, false);
            _eventService?.RaiseCanMergeStateChange(false);
        }

        public bool CanMerge(TileNode tileObjectNode, TileNode movedNode, TileObjectValue targetValue, bool indicateMergeableObjects)
        {
            return CanMerge(tileObjectNode, movedNode, targetValue, indicateMergeableObjects, out _);
        }

        public bool CanMerge(TileNode tileObjectNode, TileNode movedNode, TileObjectValue targetValue, bool indicateMergeableObjects, out List<TileNode> wantedNodes)
        {
            wantedNodes = null;

            if (targetValue.IsEmptyTileObjectValue() || movedNode.Value.IsEmptyTileObjectValue()) return false;

            wantedNodes = TileGraph.FindWantedNodesWithBfs(movedNode, targetValue, tileObjectNode);
            if (!wantedNodes.Contains(tileObjectNode)) wantedNodes.Add(tileObjectNode);

            bool canMerge = wantedNodes.Count >= MergeRequiredObject;
            if (indicateMergeableObjects)
            {
                UpdateMergeableObjectsIndicator(wantedNodes, canMerge);
            }
            _eventService?.RaiseCanMergeStateChange(canMerge);
            return canMerge;
        }

        public bool TryMerge(TileNode currentMovingObjectNode, TileNode movedNode, TileObjectValue targetValue)
        {
            bool canMerge = CanMerge(currentMovingObjectNode, movedNode, targetValue, false, out var wantedNodes);

            if (!canMerge)
            {
                return false;
            }

            Merge(wantedNodes, targetValue);
            return true;
        }

        private void Merge(List<TileNode> wantedNodes, TileObjectValue tileObjectValue)
        {
            var newTileObjectValues = GetMergedTileObjectValues(tileObjectValue, wantedNodes.Count);

            for (int i = 0; i < wantedNodes.Count; i++)
            {
                var node = wantedNodes[i];

                var value = i < newTileObjectValues.Count
                    ? newTileObjectValues[i]
                    : TileObjectValue.GetEmptyTileObjectValue();

                node.onTileObjectMerged?.Invoke(value);
            }

            UpdateMergeableObjectsIndicator(wantedNodes, false);
        }

        private List<TileObjectValue> GetMergedTileObjectValues(TileObjectValue tileObjectValue, int mergeObjectCount)
        {
            int upgradedObjectCount = mergeObjectCount / MergeRequiredObject;
            int reqObjectForOneMoreMerge = mergeObjectCount % MergeRequiredObject;
            int notUpgradedObjectCount = 0;

            if (reqObjectForOneMoreMerge > 1)
            {
                ++upgradedObjectCount;
            }
            else
            {
                notUpgradedObjectCount = reqObjectForOneMoreMerge;
            }

            return GetMergedTileObjectValues(tileObjectValue, upgradedObjectCount, notUpgradedObjectCount);
        }

        private List<TileObjectValue> GetMergedTileObjectValues(TileObjectValue tileObjectValue, int upgradedObjectCount, int notUpgradedObjectCount)
        {
            List<TileObjectValue> newTileObjectValues = new List<TileObjectValue>();

            for (int i = 0; i < upgradedObjectCount; i++)
            {
                var upgradedObjectValue = new TileObjectValue(tileObjectValue.objectId, tileObjectValue.objectLevel + 1);
                newTileObjectValues.Add(upgradedObjectValue);
            }

            for (int i = 0; i < notUpgradedObjectCount; i++)
            {
                var notUpgradedValue = new TileObjectValue(tileObjectValue.objectId, tileObjectValue.objectLevel);
                newTileObjectValues.Add(notUpgradedValue);
            }

            return newTileObjectValues;
        }

        private void UpdateMergeableObjectsIndicator(List<TileNode> tileNodes, bool isMergeable)
        {
            HideShowingMergeIndicators();

            if (tileNodes == null || tileNodes.Count == 0) return;

            foreach (var tileNode in tileNodes)
            {
                tileNode?.onUpdateMergeableIndicator.Invoke(isMergeable);
            }

            UpdateMergeableIndicatorShownList(tileNodes, isMergeable);
        }

        private void HideShowingMergeIndicators()
        {
            if (!HasIndicatorShownNode()) return;

            foreach (var tileNode in _mergeableIndicatorShownNodes)
            {
                tileNode?.onUpdateMergeableIndicator.Invoke(false);
            }
        }

        private bool HasIndicatorShownNode()
        {
            return _mergeableIndicatorShownNodes is { Count: > 0 };
        }

        private void UpdateMergeableIndicatorShownList(List<TileNode> tileNodes, bool isMergeable)
        {
            if (isMergeable)
            {
                _mergeableIndicatorShownNodes = tileNodes;
            }
            else
            {
                _mergeableIndicatorShownNodes?.Clear();
            }
        }
    }
}
