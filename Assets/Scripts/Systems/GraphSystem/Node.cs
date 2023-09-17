using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Systems.TileSystem.TileNodeSystem.Graph;
using UnityEngine;
using Utils;

namespace Systems.GraphSystem
{
    /// <typeparam name="T"> Neighbor Node Type </typeparam>
    /// <typeparam name="TF"> Node Value Type </typeparam>
    public class Node<T, TF> where T : Node<T, TF>
    {
        public TF Value { get; private set; }
        
        protected List<T> Neighbors { get; private set;}

        public Node(TF value)
        {
            Value = value;
            Neighbors = new List<T>();
        }

        public void SetValue(TF newValue)
        {
            Value = newValue;
        }

        public bool IsNeighbor(T node)
        {
            return Neighbors.Contains(node);
        }

        public bool AddNeighbor(T neighbor)
        {
            if (CheckNeighborsFull() || IsNeighbor(neighbor)) return false;
            Neighbors.Add(neighbor);
            return true;
        }

        public List<T> GetNeighbors()
        {
            return Neighbors;
        }
        
        protected virtual bool CheckNeighborsFull()
        {
            return false;
        }

        #region Print Functions

        public virtual void PrintNeighborsValues()
        {
            PrintNodeValues(Neighbors);
        }        
        
        public virtual void PrintNodeValues(IEnumerable<T> nodes)
        {
            int nodesOrder = 0;
            foreach (var node in nodes)
            {
                ++nodesOrder;
                PrintNodeValue(node, nodesOrder);
            }
        }

        protected  virtual void PrintNodeValue(T node, int nodesOrder)
        {
            TF value = node.Value;
            LogUtility.PrintLog("Nodes order : " + nodesOrder + ", NodeValue : " + value);
        }

        #endregion
        #region Test Part

        /*[SerializeField] private GameObject vertexObjectPrefab;

        public void CreateVertexObject(T otherNode)
        {
            CreateVertexObject(transform.position, otherNode.transform.position);
        }
        
        public void CreateVertexObject(Vector3 node1Pos, Vector3 node2Pos)
        {
            var midPoint = (node1Pos + node2Pos) / 2;
            var yRotation = node1Pos.z.Equals(node2Pos.z) ? 0 : 90;

            var quaternion = Quaternion.Euler(Vector3.up * yRotation);
            var vertexObject = Instantiate(vertexObjectPrefab, midPoint, quaternion, transform);
        }*/
        #endregion        
        
    }
}