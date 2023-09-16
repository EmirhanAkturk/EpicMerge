using System;
using System.Collections.Generic;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using Systems.ConfigurationSystem;
using UnityEngine;

namespace _Game.Scripts.Utility
{
    public static class TileObjectMergeHelper
    {
        public static Action<bool, List<TileNode>> onCanMergeStateChange;
        
        private static int MergeRequiredObject => ConfigurationService.Configurations.mergeRequiredObject;
    
        public static bool CanMerge(TileNode tileNode, TileObjectValue targetValue)
        {
            return CanMerge(tileNode, targetValue, out _);
        }

        public static bool CanMerge(TileNode tileNode, TileObjectValue targetValue, out List<TileNode> wantedNodes)
        {
            wantedNodes = null;
            
            if (TileObjectValue.IsEmptyTileObjectValue(targetValue)) return false;
            
            wantedNodes = TileGraph.FindWantedNodesWithBfs(tileNode, targetValue);
            bool canMerge = wantedNodes.Count >= MergeRequiredObject;
            Debug.Log("canMerge : " + canMerge);
            onCanMergeStateChange?.Invoke(canMerge, wantedNodes);
            return canMerge;
        }        
        
        public static bool TryMerge(TileNode tileNode, TileObjectValue targetValue)
        {
            bool canMerge = CanMerge(tileNode, targetValue, out var wantedNodes);

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
