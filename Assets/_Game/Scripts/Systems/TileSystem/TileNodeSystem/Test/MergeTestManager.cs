using System.Collections.Generic;
using _Game.Scripts.Systems.TileSystem.EventSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using UnityEngine;
using Utils;
namespace _Game.Scripts.Systems.TileSystem.TileNodeSystem.Test
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
