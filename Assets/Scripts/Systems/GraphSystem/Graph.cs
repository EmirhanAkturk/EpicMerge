using System.Collections.Generic;
using UnityEngine;

namespace Systems.GraphSystem
{
    /// <typeparam name="T"> Neighbor Node Type </typeparam>
    /// <typeparam name="TF"> Node Value Type </typeparam>
    public class Graph<T, TF> where T : Node<T, TF>
    {
        private readonly List<T> nodes;

        protected Graph()
        {
            nodes = new List<T>();
        }

        public void AddNode(T node)
        {
            nodes.Add(node);
        }

        public void AddEdge(T node1, T node2)
        {
            if(node1 == node2) return;
            
            node1.AddNeighbor(node2);
            node2.AddNeighbor(node1);
        }

        public List<T> GetNodes()
        {
            return nodes;
        }

        public void ClearNodes()
        {
            nodes.Clear();
        }

        #region Search Functions

        public static List<T> FindWantedNodesWithBfs(T start, TF targetValue, T except = null)
        {
            var visited = new HashSet<T>();
            var queue = new Queue<T>();
            List<T> results = new List<T>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                if (currentNode.Value.Equals(targetValue))
                {
                    results.Add(currentNode);
                }

                var neighbors = currentNode.GetNeighbors();
                foreach (var node in neighbors)
                {
                    var neighbor = node;
                    if(except != null && neighbor == except) continue;

                    if (!visited.Contains(neighbor) && neighbor.Value.Equals(start.Value))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }

            return results;
        }
            
        #endregion
    }
}