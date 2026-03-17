using System.Collections.Generic;
using _Game.Scripts.Systems.TileSystem.EventSystem;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using UnityEngine;
using Zenject;
namespace _Game.Scripts.Systems.TileSystem.TileNodeSystem.Test
{
    public class MergeTestManager : MonoBehaviour
    {
        private readonly List<TileNode> canMergeNodes = new List<TileNode>();

        [Inject] private IEventService EventService { get; }

        private void Awake()
        {
            EventService.OnCanMergeStateChange += UpdateGizmo;
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
