using System.Collections.Generic;
using _Game.Scripts.Systems.TileNodeSystem;
using _Game.Scripts.Systems.TileObjectSystem;
using GameDepends;
using Systems.ConfigurationSystem;

namespace _Game.Scripts.Systems.MergeSystem
{
    public static class TileObjectMergeHelper
    {
        private static int MergeRequiredObject => ConfigurationService.Configurations.mergeRequiredObject;
        
        private static List<TileNode> _mergeableIndicatorShownNodes;
        
        static TileObjectMergeHelper()
        {
            EventService.onTileObjectPlacedToTile += TileObjectPlacedToTile;
            EventService.onMergeCanceled += MergeCancel;
        }

        private static void TileObjectPlacedToTile(TileNode tileNode, BaseTileObject baseTileObject)
        {
            MergeCancel();
        }

        public static void MergeCancel()
        {
            UpdateMergeableObjectsIndicator( _mergeableIndicatorShownNodes, false);
            EventService.onCanMergeStateChange?.Invoke(false);
        }
        
        public static bool CanMerge(TileNode tileObjectNode, TileNode movedNode, TileObjectValue targetValue, bool indicateMergeableObjects)
        {
            return CanMerge(tileObjectNode, movedNode, targetValue, indicateMergeableObjects, out _);
        }

        public static bool CanMerge(TileNode tileObjectNode, TileNode movedNode, TileObjectValue targetValue, bool indicateMergeableObjects, out List<TileNode> wantedNodes)
        {
            wantedNodes = null;
            
            if (targetValue.IsEmptyTileObjectValue() || movedNode.Value.IsEmptyTileObjectValue()) return false;
            
            wantedNodes = TileGraph.FindWantedNodesWithBfs(movedNode, targetValue, tileObjectNode);
            if(!wantedNodes.Contains(tileObjectNode)) wantedNodes.Add(tileObjectNode);
            
            bool canMerge = wantedNodes.Count >= MergeRequiredObject;
            // Debug.Log("canMerge : " + canMerge);
            if (indicateMergeableObjects)
            {
                UpdateMergeableObjectsIndicator(wantedNodes, canMerge);
            }
            EventService.onCanMergeStateChange?.Invoke(canMerge);
            return canMerge;
        }

        public static bool TryMerge(TileNode currentMovingObjectNode, TileNode movedNode, TileObjectValue targetValue)
        {
            bool canMerge = CanMerge(currentMovingObjectNode, movedNode, targetValue, false, out var wantedNodes);

            if (!canMerge)
            {
                return false;
            }

            // Debug.Log("TryMerge : ");

            Merge(wantedNodes, targetValue);
            return true;
        }

        private static void Merge(List<TileNode> wantedNodes, TileObjectValue tileObjectValue)
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
            // Debug.Log("Merged : ");
        }

        private static List<TileObjectValue> GetMergedTileObjectValues(TileObjectValue tileObjectValue, int mergeObjectCount)
        {
            int upgradedObjectCount = mergeObjectCount / MergeRequiredObject;
            int reqObjectForOneMoreMerge = mergeObjectCount % MergeRequiredObject; 
            int notUpgradedObjectCount = 0;
            
            if ( reqObjectForOneMoreMerge > 1)
            {
                ++upgradedObjectCount;
            }
            else
            {
                notUpgradedObjectCount = reqObjectForOneMoreMerge;
            }

            return GetMergedTileObjectValues(tileObjectValue, upgradedObjectCount, notUpgradedObjectCount);
        }

        private static List<TileObjectValue> GetMergedTileObjectValues(TileObjectValue tileObjectValue, int upgradedObjectCount, int notUpgradedObjectCount)
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
        
        private static void UpdateMergeableObjectsIndicator(List<TileNode> tileNodes, bool isMergeable)
        {
            HideShowingMergeIndicators();
            
            if(tileNodes == null || tileNodes.Count == 0) return;

            foreach (var tileNode in tileNodes)
            {
                tileNode?.onUpdateMergeableIndicator.Invoke(isMergeable);
            }

            UpdateMergeableIndicatorShownList(tileNodes, isMergeable);
        }

        private static void HideShowingMergeIndicators()
        {
            if (!HasIndicatorShownNode()) return;
            
            foreach (var tileNode in _mergeableIndicatorShownNodes)
            {
                tileNode?.onUpdateMergeableIndicator.Invoke(false);
            }
        }

        private static bool HasIndicatorShownNode()
        {
            return _mergeableIndicatorShownNodes is { Count: > 0 };
        }

        private static void UpdateMergeableIndicatorShownList(List<TileNode> tileNodes, bool isMergeable)
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
