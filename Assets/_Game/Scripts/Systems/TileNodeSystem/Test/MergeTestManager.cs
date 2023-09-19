using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Systems.IndicationSystem;
using _Game.Scripts.Systems.MergeSystem;
using _Game.Scripts.Systems.TileNodeSystem.Graph;
using _Game.Scripts.Systems.TileObjectSystem;
using _Game.Scripts.Utils;
using GameDepends;
using UnityEngine;
using Utils;

namespace _Game.Scripts.Systems.TileNodeSystem.Test
{
    public class MergeTestManager : Singleton<MergeTestManager>
    {
        private readonly List<TileNode> canMergeNodes = new List<TileNode>();
        
        private void Awake()
        {
            EventService.onCanMergeStateChange += UpdateGizmo;
        }

        private void UpdateGizmo(bool isMergeable)
        {
            // canMergeNodes = TileObjectMergeHelper._mergeableIndicatorShownNodes;
        }

        private void OnDrawGizmos()
        {
            if(canMergeNodes == null || canMergeNodes.Count == 0 ) return;

            Vector3 scale = new Vector3(.8f, .1f, .8f);
            foreach (var node in canMergeNodes)
            {
                Gizmos.DrawCube(node.Transform.position + Vector3.up, scale);
            }
        }
    }
}
