using System;
using System.Collections.Generic;
using _Game.Scripts.Systems.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using Systems.ConfigurationSystem;
using UnityEngine;

namespace _Game.Scripts.Utility
{
    public static class TileObjectMergeHelper
    {
        public static Action<bool, List<TileNode>> onCanMergeStateChange;
        
        private static int MergeRequiredObject => ConfigurationService.Configurations.mergeRequiredObject;
    
        public static bool CanMerge(TileNode tileObjectNode, TileNode movedNode, TileObjectValue targetValue)
        {
            return CanMerge(tileObjectNode, movedNode, targetValue, out _);
        }

        public static bool CanMerge(TileNode tileObjectNode, TileNode movedNode, TileObjectValue targetValue, out List<TileNode> wantedNodes)
        {
            wantedNodes = null;
            
            if (targetValue.IsEmptyTileObjectValue() || movedNode.Value.IsEmptyTileObjectValue()) return false;
            
            wantedNodes = TileGraph.FindWantedNodesWithBfs(movedNode, targetValue, tileObjectNode);
            if(!wantedNodes.Contains(tileObjectNode)) wantedNodes.Add(tileObjectNode);
            
            bool canMerge = wantedNodes.Count >= MergeRequiredObject;
            // Debug.Log("canMerge : " + canMerge);
            onCanMergeStateChange?.Invoke(canMerge, wantedNodes);
            return canMerge;
        }        
        
        public static bool TryMerge(TileNode currentMovingObjectNode, TileNode movedNode, TileObjectValue targetValue)
        {
            bool canMerge = CanMerge(currentMovingObjectNode, movedNode, targetValue, out var wantedNodes);

            if (!canMerge)
            {
                return false;
            }

            Debug.Log("TryMerge : ");

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
            
            Debug.Log("Merged : ");
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
    }
}
