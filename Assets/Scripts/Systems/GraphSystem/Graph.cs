using System.Collections.Generic;
using UnityEngine;

namespace Systems.GraphSystem
{
    public class Graph
    {
        private readonly List<Node> nodes;

        public Graph()
        {
            nodes = new List<Node>();
        }

        public void AddNode(Node node)
        {
            nodes.Add(node);
        }

        public void AddEdge(Node node1, Node node2)
        {
            if(node1 == node2) return;
            
            node1.AddNeighbor(node2);
            node2.AddNeighbor(node1);
        }

        public List<Node> GetNodes()
        {
            return nodes;
        }

        public void ClearNodes()
        {
            nodes.Clear();
        }

        #region Search Functions
        public static List<Node> FindWantedNodesWithBfs(Node start, int targetValue)
        {
            List<Node> result = new List<Node>();
            HashSet<Node> visited = new HashSet<Node>();
            Queue<Node> queue = new Queue<Node>();
            
            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Node currentNode = queue.Dequeue();
                if (currentNode.Value == targetValue)
                {
                    result.Add(currentNode);
                }

                var neighbors = currentNode.GetNeighbors(); 
                foreach (Node neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor) && neighbor.Value.Equals(start.Value))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }

            return result;
        }
            
        #endregion
    }
}