using System.Collections.Generic;
using UnityEngine;

namespace Systems.GraphSystem
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private GameObject vertexObjectPrefab;

        public int Value { get; private set; }
        protected List<Node> Neighbors { get; set;}

        public virtual void Init(int value)
        {
            Value = value;
            Neighbors = new List<Node>();
        }

        public virtual void PrintNeighbors()
        {
            foreach (Node neighbor in Neighbors)
            {
                Debug.Log(neighbor.Value);
            }
        }

        public bool IsNeighbor(Node node)
        {
            return Neighbors.Contains(node);
        }

        public bool AddNeighbor(Node neighbor)
        {
            if (CheckNeighborsFull() || IsNeighbor(neighbor)) return false;
            Neighbors.Add(neighbor);
            return true;
        }

        public List<Node> GetNeighbors()
        {
            return Neighbors;
        }
        
        protected virtual bool CheckNeighborsFull()
        {
            return false;
        }
        
        public void CreateVertexObject(Node otherNode)
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