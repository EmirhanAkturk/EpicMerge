using System;
using _Game.Scripts.Systems.TileObjectSystem;
using Systems.GraphSystem;
using UnityEngine;
using Utils;

namespace _Game.Scripts.Systems.TileNodeSystem
{
    public class TileNode : Node<TileNode, TileObjectValue>
    {
        public Action<TileObjectValue> onTileObjectMerged; 
        public Action<BaseTileObject> onTileObjectChanged; 
        public Action<bool> onUpdateMergeableIndicator;

        public Transform Transform { get; private set; }
        public string Name { get; private set; } // For test
        private const int MAX_NEIGHBORS_COUNT = 4;

        #region Init Functions

        public TileNode(Transform transform, TileObjectValue tileObjectValue) : base(tileObjectValue)
        {
            Transform = transform;
            Name = Transform.gameObject.name;
        }

        #endregion

        #region Get Functions

        public TileNode GetEmptyNeighbor()
        {
            var neighbors = GetNeighbors();
            foreach (var tileNode in neighbors)
            {
                if (tileNode.Value.IsEmptyTileObjectValue())
                {
                    return tileNode;
                }
            }

            return null;
        }

        #endregion
    
        #region Other Functions

        protected override bool CheckNeighborsFull()
        {
            return Neighbors.Count >= MAX_NEIGHBORS_COUNT;
        }
    
        #endregion
        
        #region Test Functions
        
        [ContextMenu("Bfs")]
        public void Bfs()
        {
            var wantedNodes = TileGraph.FindWantedNodesWithBfs(this, Value );
        
            Debug.Log("################## ");
            Debug.Log("BFS Wanted Value : " + Value);
            PrintNodeValues(wantedNodes);
        }
    
        public void PrintNeighbors()
        {
            Debug.Log("#######################");
            Debug.Log($"Print {Name} Neighbors : ");
    
            PrintNodeValues(Neighbors);
        }
    
        protected override void PrintNodeValue(TileNode node, int nodesOrder)
        {
            TileObjectValue value = node.Value;
            LogUtility.PrintLog($"TileName : {node.Name}, Nodes order : " + nodesOrder + ", NodeValue : " + value);
        }
        
        #endregion

    }

    [Serializable]
    public class ValueMaterialPair
    {
        public int value;
        public Material material;
    }

    [Serializable]
    public class ValueModelPair
    {
        public int value;
        public Mesh model;
    }
}