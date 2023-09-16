using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Systems.GraphSystem
{
    /// <typeparam name="T"> Neighbor Node Type </typeparam>
    /// <typeparam name="TF"> Node Value Type </typeparam>
    public class Node<T, TF> : MonoBehaviour where T : Node<T, TF>
    {
        [SerializeField] private GameObject vertexObjectPrefab;
        
        public TF Value { get; private set; }
        protected List<T> Neighbors { get; private set;}

        public virtual void Init(TF value)
        {
            Value = value;
            Neighbors = new List<T>();
        }

        public void SetValue(TF newValue)
        {
            Value = newValue;
        }

        public virtual void PrintNeighbors()
        {
            PrintNodes(Neighbors);
        }        
        
        public virtual void PrintNodes(IEnumerable<T> nodes)
        {
            int nodesOrder = 0;
            foreach (var node in nodes)
            {
                ++nodesOrder;
                GameObject nodeObject = node.gameObject;
                LogUtility.PrintLog("Nodes order : " + nodesOrder + ", NodeName : " + nodeObject.name, nodeObject);
            }
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
        }
    }
}