using System.Collections.Generic;
using UnityEngine;

namespace Systems.GraphSystem
{
    public class Node : MonoBehaviour
    {
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
    }
}