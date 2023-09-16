using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Utility;
using UnityEngine;
using Utils;

namespace _Game.Scripts.Systems.TileNodeSystem.Test
{
    public class MergeTestManager : Singleton<MergeTestManager>
    {
        private List<TileNode> canMergeNodes = new List<TileNode>();
        
        private void Awake()
        {
            TileObjectMergeHelper.onCanMergeStateChange += UpdateGizmoList;
        }

        private void UpdateGizmoList(bool canMerge, List<TileNode> nodes)
        {
            if (!canMerge)
            {
                canMergeNodes.Clear();
            }
            else
            {
                canMergeNodes = nodes.ToList();
            }
        }

        private void OnDrawGizmos()
        {
            if(canMergeNodes.Count == 0 ) return;

            Vector3 scale = new Vector3(.8f, .1f, .8f);
            foreach (var node in canMergeNodes)
            {
                Gizmos.DrawCube(node.transform.position + Vector3.up, scale);
            }
        }
    }
}
